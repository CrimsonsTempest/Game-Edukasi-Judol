using System.Collections.Generic;

public class PatternOutcomeEngine : IOutcomeEngine
{
    private int spinCount = 0;
    private int lossStreak = 0;

    public SpinResult GenerateResult()
    {
        spinCount++;

        if (spinCount % 25 == 0)
        {
            lossStreak = 0;
            return new SpinResult(SpinType.Jackpot, 200);
        }

        if (lossStreak >= 5)
        {
            lossStreak = 0;
            return new SpinResult(SpinType.SmallWin, 50);
        }

        if (spinCount % 10 == 0)
        {
            lossStreak = 0;
            return new SpinResult(SpinType.SmallWin, 20);
        }

        lossStreak++;
        return new SpinResult(SpinType.Loss, 0);
    }

    public List<string> GetRules()
    {
        return new List<string>
        {
            "Every 10 spins = Small Win",
            "5 losses = Recovery Win",
            "Every 25 spins = Jackpot"
        };
    }

    public string GetEngineName()
    {
        return "Pattern Engine";
    }
}