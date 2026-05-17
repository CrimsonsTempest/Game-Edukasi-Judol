using System.Collections.Generic;

public class DatabaseOutcomeEngine
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
            new SpinResult(SpinType.Jackpot, 200),
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
}