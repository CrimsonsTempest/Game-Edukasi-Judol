using UnityEngine;
using System.Collections.Generic;

public class StateMachineEngine : IOutcomeEngine
{
    private GameState currentState;

    public StateMachineEngine(GameState initialState)
    {
        currentState = initialState;
    }

    public SpinResult GenerateResult()
    {
        Debug.Log(
            "[STATE MACHINE] Current State: " +
            currentState
        );

        switch (currentState)
        {
            case GameState.Cold:
                return new SpinResult(SpinType.Loss, 0);

            case GameState.Hot:
                return new SpinResult(SpinType.SmallWin, 30);

            case GameState.Bonus:
                return new SpinResult(SpinType.Jackpot, 200);

            default:
                if (Random.value > 0.5f)
                {
                    return new SpinResult(SpinType.SmallWin, 10);
                }

                return new SpinResult(SpinType.Loss, 0);
        }
    }

    public List<string> GetRules()
    {
        return new List<string>
        {
            "Cold Mode",
            "Normal Mode",
            "Hot Mode",
            "Bonus Mode"
        };
    }

    public string GetEngineName()
    {
        return "State Machine";
    }

    public void SetState(GameState state)
    {
        currentState = state;
    }
}