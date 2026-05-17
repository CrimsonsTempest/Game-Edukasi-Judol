using System.Collections.Generic;

public interface IOutcomeEngine
{
    SpinResult GenerateResult();
    List<string> GetRules();
    string GetEngineName();
}