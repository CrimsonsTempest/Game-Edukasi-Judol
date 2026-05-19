using UnityEngine;
using System.Collections.Generic;

public class RTPControlEngine : IOutcomeEngine
{
    private float targetRTP;
    private float totalBet = 0;
    private float totalPayout = 0;

    public RTPControlEngine(float targetRTP)
    {
        this.targetRTP = targetRTP;
    }

    public SpinResult GenerateResult()
    {
        totalBet += 10;

        float currentRTP = 0;

        if (totalBet > 0)
        {
            currentRTP = totalPayout / totalBet;
        }

        Debug.Log(
            "[RTP ENGINE] Current RTP: " +
            currentRTP +
            " | Target RTP: " +
            targetRTP
        );

        SpinResult result;

        if (currentRTP < targetRTP)
        {
            result = new SpinResult(SpinType.SmallWin, 20);
        }
        else
        {
            result = new SpinResult(SpinType.Loss, 0);
        }

        totalPayout += result.payout;

        return result;
    }

    public List<string> GetRules()
    {
        return new List<string>
        {
            "60% RTP",
            "75% RTP",
            "90% RTP"
        };
    }

    public string GetEngineName()
    {
        return "RTP Control";
    }

    public void SetTargetRTP(float value)
    {
        targetRTP = value;
    }
}