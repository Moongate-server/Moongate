using System.Diagnostics.CodeAnalysis;
using Moongate.Core.Data.Scripts;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface IScriptEngineService : IMoongateStartStopService
{
    Task ExecuteFileAsync(string file);

    ScriptEngineExecutionResult ExecuteCommand(string command);

    List<ScriptFunctionDescriptor> Functions { get; }

    Dictionary<string, object> ContextVariables { get; }
    void AddContextVariable(string name, object value);

    TVar? GetContextVariable<TVar>(string name, bool throwIfNotFound = true) where TVar : class;

    object? GetContextVariable(string name, Type type, bool throwIfNotFound = true);

    bool ExecuteContextVariable(string name, params object[] args);


    Task<bool> SeedAsync();

    void AddEnum<T>(T value) where T : struct, Enum;

    /// <summary>
    /// Adds a script module to the engine
    /// </summary>
    /// <param name="type">The module type with required members for AOT</param>
    void AddScriptModule(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors |
            DynamicallyAccessedMemberTypes.NonPublicConstructors |
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicProperties
        )]
        Type type
    );
}
