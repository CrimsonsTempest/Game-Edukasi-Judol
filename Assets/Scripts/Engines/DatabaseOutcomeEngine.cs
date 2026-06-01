using System.Collections.Generic;

public class DatabaseOutcomeEngine : IOutcomeEngine
{
    private List<SpinResult> scriptedResults;
    private int currentIndex = 0;

    public DatabaseOutcomeEngine()
    {
        scriptedResults = new List<SpinResult>
        {
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.SmallWin, 20),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Jackpot, 200),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.SmallWin, 20),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0),
            new SpinResult(SpinType.Loss, 0)
        };
    }

    public SpinResult GenerateResult()
    {
        if (currentIndex >= scriptedResults.Count)
        {
            currentIndex = 0;
        }

        SpinResult result = scriptedResults[currentIndex];
        currentIndex++;

        return result;
    }

    public List<string> GetRules()
    {
        return new List<string>
        {
            "Scripted sequence, Loop when finished,No randomness"
        };
    }

    public string GetEngineName()
    {
        return "Database Outcome";
    }
}