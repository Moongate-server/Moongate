using System.Collections.Concurrent;
using Moongate.Core.Interfaces.EventBus;
using Moongate.Core.Interfaces.Services.System;
using Orion.Core.Server.Events.TextTemplate;
using Orion.Core.Server.Interfaces.Services.System;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using Serilog;

namespace Moongate.Server.Services.System;

public class TextTemplateService
    : ITextTemplateService, IEventBusListener<AddVariableEvent>, IEventBusListener<AddVariableBuilderEvent>
{
    private readonly ILogger _logger = Log.ForContext<TextTemplateService>();
    private readonly ConcurrentDictionary<string, Func<object>> _variableBuilder = new();
    private readonly ConcurrentDictionary<string, object> _variables = new();
    private readonly IEventBusService _eventBusService;

    public TextTemplateService(IEventBusService eventBusService)
    {
        _eventBusService = eventBusService;

        AddDefaultVariables();

        eventBusService.Subscribe<AddVariableEvent>(this);
        eventBusService.Subscribe<AddVariableBuilderEvent>(this);
    }

    private void AddDefaultVariables()
    {
        AddVariable("cpu_count", Environment.ProcessorCount);
        AddVariable("os_name", Environment.OSVersion.VersionString);
        AddVariable("os_version", Environment.OSVersion.Version);
        AddVariable("os_platform", Environment.OSVersion.Platform);
    }

    public void AddVariableBuilder(string variableName, Func<object> builder)
    {
        _logger.Debug("Adding variable builder for {variableName}", variableName);
        _variableBuilder[variableName] = builder;
    }

    public void AddVariable(string variableName, object value)
    {
        _logger.Debug("Adding variable {variableName} with value {value}", variableName, value);
        _variables[variableName] = value;
    }

    public string TranslateText(string text, object? context = null)
    {
        try
        {
            var template = Template.Parse(text);


            var scriptContext = new TemplateContext();
            var scriptObject = new ScriptObject();


            foreach (var variable in _variables)
            {
                scriptObject.Add(variable.Key, variable.Value);
            }


            foreach (var builder in _variableBuilder)
            {
                scriptObject.Add(builder.Key, new DynamicVariable(builder.Value));
            }

            scriptContext.PushGlobal(scriptObject);

            if (context != null)
            {
                scriptObject.Add("context", context);
            }

            return template.Render(scriptContext);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error rendering template: {Template}", text);

            throw new Exception("Error rendering template", ex);
        }
    }

    public Dictionary<string, string> GetVariablesAndContent()
    {
        var list = new Dictionary<string, string>();

        foreach (var variable in _variables)
        {
            list[variable.Key] = variable.Value?.ToString() ?? string.Empty;
        }

        foreach (var builder in _variableBuilder)
        {
            list[builder.Key] = builder.Value.Invoke().ToString() ?? string.Empty;
        }

        return list;
    }

    public List<string> GetVariables()
    {
        var list = new List<string>();
        list.AddRange(_variables.Keys);
        list.AddRange(_variableBuilder.Keys);

        list = list.OrderByDescending(x => x).ToList();

        return list;
    }

    public void RebuildVariables()
    {
        foreach (var builder in _variableBuilder.AsParallel())
        {
            try
            {
                _variables[builder.Key] = builder.Value();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error building variable {VariableName}", builder.Key);
            }
        }
    }

    private class DynamicVariable : IScriptCustomFunction
    {
        private readonly Func<object> _valueFactory;

        public DynamicVariable(Func<object> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public object Invoke(
            TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement
        )
        {
            try
            {
                return _valueFactory();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ValueTask<object> InvokeAsync(
            TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement
        )
        {
            try
            {
                var result = _valueFactory();
                return new ValueTask<object>(result);
            }
            catch (Exception)
            {
                return new ValueTask<object>((object)null);
            }
        }

        public ScriptParameterInfo GetParameterInfo(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public int RequiredParameterCount => 0;
        public int ParameterCount => 0;
        public ScriptVarParamKind VarParamKind => ScriptVarParamKind.Direct;


        public Type ReturnType => typeof(object);
    }

    public Task HandleAsync(AddVariableEvent @event, CancellationToken cancellationToken = default)
    {
        AddVariable(@event.VariableName, @event.Value);

        return Task.CompletedTask;
    }

    public Task HandleAsync(AddVariableBuilderEvent @event, CancellationToken cancellationToken = default)
    {
        AddVariableBuilder(@event.VariableName, @event.Builder);

        return Task.CompletedTask;
    }
}
