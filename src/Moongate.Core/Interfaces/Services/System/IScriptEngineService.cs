using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface IScriptEngineService : IMoongateStartStopService
{
    void AddInitScript(string script);
    void ExecuteScript(string script);
    void ExecuteScriptFile(string scriptFile);
    void AddCallback(string name, Action<object[]> callback);
    void AddConstant(string name, object value);
    void ExecuteCallback(string name, params object[] args);
}
