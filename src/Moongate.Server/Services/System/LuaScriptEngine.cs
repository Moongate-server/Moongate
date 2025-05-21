using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using DryIoc;
using Moongate.Core.Attributes.Scripts;
using Moongate.Core.Data.Configs.Services;
using Moongate.Core.Data.Scripts;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Strings;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Json;
using Moongate.Core.Types;
using Moongate.Server.Utils;
using NLua;
using NLua.Exceptions;
using Serilog;

namespace Moongate.Server.Services.System;

public class ScriptEngineService : IScriptEngineService
{
    private readonly ILogger _logger = Log.ForContext<ScriptEngineService>();
    private readonly IContainer _serviceProvider;
    private readonly Lua _luaEngine;

    private readonly LuaTypeDefinitionsGenerator _typeGenerator = new();

    private readonly DirectoriesConfig _directoryConfig;

    private FileSystemWatcher _watcher;

    private readonly Subject<string> _fileChanges = new();

    private const string _prefixToIgnore = "__";

    private readonly ScriptEngineConfig _scriptEngineConfig;

    public List<ScriptFunctionDescriptor> Functions { get; } = new();
    public Dictionary<string, object> ContextVariables { get; } = new();

    public Dictionary<string, object> Constants { get; } = new();


    private IDisposable _fileWatcherSubscription;

    public ScriptEngineService(
        DirectoriesConfig directoryConfig, IContainer serviceProvider, ScriptEngineConfig scriptEngineConfig
    )
    {
        _directoryConfig = directoryConfig;
        _serviceProvider = serviceProvider;
        _scriptEngineConfig = scriptEngineConfig;

        _luaEngine = new Lua();

        _luaEngine.LoadCLRPackage();

        AddModulesDirectory();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var script in _scriptEngineConfig.InitScriptsFileNames)
        {
            var fileName = Path.Combine(_directoryConfig[DirectoryType.Scripts], script);

            if (!File.Exists(fileName))
            {
                continue;
            }

            if (!fileName.StartsWith(_prefixToIgnore))
            {
                await ExecuteFileAsync(fileName);
            }
        }

        _watcher = new FileSystemWatcher(_directoryConfig[DirectoryType.Scripts])
        {
            Filter = "*.lua",
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnScriptFileChanged;
        _watcher.Created += OnScriptFileChanged;

        _logger.Debug("Enabled file watcher for scripts directory: {Directory}", _directoryConfig[DirectoryType.Scripts]);

        _fileWatcherSubscription = _fileChanges
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(async file =>
                {
                    try
                    {
                        await ExecuteFileAsync(file);
                        _logger.Debug("Reloaded script: {File}", Path.GetFileName(file));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error reloading script: {File}", Path.GetFileName(file));
                    }
                }
            );


        await GenerateDefinitionFileAsync();
    }

    private async Task GenerateDefinitionFileAsync()
    {
        var index = await LuaTypeDefinitionsGenerator.GenerateTypeDefinitionsAsync(Functions, ContextVariables, Constants);

        var indexFile = Path.Combine(_directoryConfig[DirectoryType.Scripts], "__index.lua");

        if (File.Exists(indexFile))
        {
            File.Delete(indexFile);
        }

        await File.WriteAllTextAsync(indexFile, index);

        _logger.Debug("Generated definition index file: {File}", indexFile);
    }

    private void OnScriptFileChanged(object sender, FileSystemEventArgs e)
    {
        if (!e.Name.StartsWith(_prefixToIgnore))
        {
            _fileChanges.OnNext(e.FullPath);
        }
    }

    public void AddConstant<T>(T value)
    {
        if (typeof(T).IsEnum)
        {
            _logger.Debug("Adding enum {Enum}", value.GetType().Name);


            var prefix = value.GetType().Name.ToSnakeCase().ToUpper();


            foreach (var enumValue in Enum.GetValues(value.GetType()))
            {
                var enumName = Enum.GetName(value.GetType(), enumValue).ToSnakeCase().ToUpper();

                var constantName = $"{prefix}_{enumName}";

                _logger.Debug(
                    "Adding constant {ConstantName} with value {EnumValue} {ValueAsInt} ",
                    constantName,
                    enumValue,
                    Convert.ToInt32(enumValue)
                );

                _luaEngine[constantName] = Convert.ToInt32(enumValue);

                Constants[constantName] = Convert.ToInt32(enumValue);
            }
        }
    }


    public void AddScriptModule(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors
        )]
        Type type
    )
    {
        try
        {
            if (!_serviceProvider.IsRegistered(type))
            {
                _logger.Debug("Type {Type} not registered in service provider, registering", type.Name);

                _serviceProvider.Register(type);
            }

            var obj = _serviceProvider.GetService(type);

            var sClassAttr = type.GetCustomAttribute<ScriptModuleAttribute>();

            if (sClassAttr == null)
            {
                throw new InvalidOperationException($"ScriptModuleAttribute not found in {type}");
            }

            _luaEngine.NewTable(sClassAttr.TableName);

            var tName = _luaEngine.GetTable(sClassAttr.TableName);

            foreach (var scriptMethod in type.GetMethods())
            {
                var sMethodAttr = scriptMethod.GetCustomAttribute<ScriptFunctionAttribute>();

                if (sMethodAttr == null)
                {
                    continue;
                }

                ExtractFunctionDescriptor(sClassAttr.TableName, sMethodAttr, scriptMethod);

                _logger.Debug(
                    "Adding script method {TableName}.{M}",
                    sClassAttr.TableName,
                    sMethodAttr.Alias ?? scriptMethod.Name
                );


                tName[sMethodAttr.Alias ?? scriptMethod.Name] = CreateLuaEngineDelegate(obj, scriptMethod);
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error during initialize script module {Alias}: {Ex}", type, ex);
        }
    }


    public async Task ExecuteFileAsync(string file)
    {
        _logger.Information("Executing script: {File}", Path.GetFileName(file));
        try
        {
            var script = await File.ReadAllTextAsync(file);
            _luaEngine.DoString(script);
        }
        catch (LuaException ex)
        {
            _logger.Error(
                ex,
                "Error executing script: {File}: {Formatted}",
                Path.GetFileName(file),
                FormatException(ex)
            );
        }
    }

    private void ExtractFunctionDescriptor(string tableName, ScriptFunctionAttribute attribute, MethodInfo methodInfo)
    {
        var descriptor = new ScriptFunctionDescriptor
        {
            FunctionName = tableName + "." + attribute.Alias ?? methodInfo.Name,
            Help = attribute.Help,
            Parameters = new(),
            ReturnType = methodInfo.ReturnType.Name,
            RawReturnType = methodInfo.ReturnType
        };

        foreach (var parameter in methodInfo.GetParameters())
        {
            var paramString = parameter.Name;

            if (paramString == "function")
            {
                paramString = "func";
            }

            var description = new ScriptFunctionParameterDescriptor(
                parameter.Name,
                parameter.ParameterType.Name,
                parameter.ParameterType,
                paramString
            );
            descriptor.Parameters.Add(description);
        }

        Functions.Add(descriptor);
    }

    public ScriptEngineExecutionResult ExecuteCommand(string command)
    {
        try
        {
            var result = new ScriptEngineExecutionResult
            {
                Result = _luaEngine.DoString(command)
            };

            return result;
        }
        catch (LuaException ex)
        {
            return new ScriptEngineExecutionResult { Exception = ex };
        }
    }

    public void AddContextVariable(string name, object value)
    {
        _logger.Information("Adding context variable {Name} with value {Value}", name, value);
        _luaEngine[name] = value;
        ContextVariables[name] = value;
    }

    public TVar? GetContextVariable<TVar>(string name, bool throwIfNotFound = true) where TVar : class
    {
        return GetContextVariable(name, typeof(TVar), throwIfNotFound) as TVar;
    }

    public object? GetContextVariable(string name, Type type, bool throwIfNotFound = true)
    {
        if (!ContextVariables.TryGetValue(name, out var ctxVar))
        {
            if (throwIfNotFound)
            {
                _logger.Error("Variable {Name} not found", name);
                throw new KeyNotFoundException($"Variable {name} not found");
            }

            return default;
        }


        if (ctxVar is LuaFunction luaFunction)
        {
            return (object)luaFunction;
        }

        if (ctxVar is LuaTable luaTable)
        {
            var json = JsonUtils.Serialize(luaTable);
            return JsonUtils.Deserialize(json, type);
        }

        return ctxVar;
    }

    public bool ExecuteContextVariable(string name, params object[] args)
    {
        if (ContextVariables.TryGetValue(name, out var ctxVar) && ctxVar is LuaFunction luaFunction)
        {
            luaFunction.Call(args);
            return true;
        }

        _logger.Error("Variable {Name} not found", name);
        return false;
    }

    public Task<bool> SeedAsync()
    {
        if (ExecuteContextVariable("seed"))
        {
            return Task.FromResult(true);
        }

        _logger.Warning(
            "Seed function not found, you should define a function callback 'on_seed' in your scripts"
        );

        return Task.FromResult(false);
    }


    public Task<bool> BootstrapAsync()
    {
        if (ExecuteContextVariable("bootstrap"))
        {
            return Task.FromResult(true);
        }

        _logger.Warning(
            "Bootstrap function not found, you should define a function callback 'on_bootstrap' in your scripts"
        );

        return Task.FromResult(false);
    }

    private static Delegate CreateLuaEngineDelegate(object obj, MethodInfo method)
    {
        var parameterTypes =
            method.GetParameters().Select(p => p.ParameterType).Concat(new[] { method.ReturnType }).ToArray();
        return method.CreateDelegate(Expression.GetDelegateType(parameterTypes), obj);
    }


    private void AddModulesDirectory()
    {
        var modulesPath = Path.Combine(_directoryConfig[DirectoryType.Scripts]) + Path.DirectorySeparatorChar;
        var scriptModulePath = Path.Combine(_directoryConfig[DirectoryType.ScriptModules]) + Path.DirectorySeparatorChar;


        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            modulesPath = modulesPath.Replace(@"\", @"\\");
            scriptModulePath = scriptModulePath.Replace(@"\", @"\\");
        }

        _luaEngine.DoString(
            $"""
             -- Update the search path
             local module_folder = '{modulesPath}'
             local module_script_folder = '{scriptModulePath}'
             package.path = module_folder .. '?.lua;' .. package.path
             package.path = module_script_folder .. '?.lua;' .. package.path
             """
        );
    }

    private static string FormatException(LuaException e)
    {
        var source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source[..^2];
        return string.Format("{0}\nLua (at {2})", e.Message, "", source);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _luaEngine.Dispose();
        _fileWatcherSubscription?.Dispose();
        _watcher?.Dispose();

        GC.SuppressFinalize(this);
    }
}
