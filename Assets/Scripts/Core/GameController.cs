using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public ReelController reel1;
    public ReelController reel2;
    public ReelController reel3;

    public UIController uiController;

    private IOutcomeEngine activeEngine;

    private int playerBalance = 100;
    private bool isSpinning = false;
    
    // Profit tracking variables
    private int totalBetsPlaced = 0;
    private int totalPayoutsGiven = 0;

    void Start()
    {
        uiController.UpdateBalance(playerBalance);
    }

    public void SetEngine(IOutcomeEngine engine)
    {
        activeEngine = engine;
        uiController.UpdateRulePreview(activeEngine.GetRules());
    }

    public void OnSpinButtonPressed()
    {
        if (isSpinning)
            return;

        if (playerBalance < 10)
            return;

        if (activeEngine == null)
            return;

        StartCoroutine(SpinRoutine());
    }

    public string GetCurrentEngineName()
{
    if (activeEngine == null)
        return "None";

    return activeEngine.GetEngineName();
}




    private IEnumerator SpinRoutine()
{
    isSpinning = true;

    int balanceBeforeBet = playerBalance;

    playerBalance -= 10;
    totalBetsPlaced += 10;
    uiController.UpdateBalance(playerBalance);

    Debug.Log(
        "[SPIN START] Engine: " + activeEngine.GetEngineName() +
        " | Balance Before Bet: " + balanceBeforeBet +
        " | Balance After Bet: " + playerBalance
    );

    SpinResult result = activeEngine.GenerateResult();

    Debug.Log(
        "[OUTCOME GENERATED] Engine: " + activeEngine.GetEngineName() +
        " | Result: " + result.type +
        " | Payout: " + result.payout
    );

    yield return StartCoroutine(reel1.Spin());
    yield return StartCoroutine(reel2.Spin());
    yield return StartCoroutine(reel3.Spin());

    int balanceBeforePayout = playerBalance;

    playerBalance += result.payout;
    totalPayoutsGiven += result.payout;

    Debug.Log(
        "[PAYOUT APPLIED] Engine: " + activeEngine.GetEngineName() +
        " | Balance Before Payout: " + balanceBeforePayout +
        " | Balance After Payout: " + playerBalance
    );

    uiController.UpdateBalance(playerBalance);
    uiController.ShowResult(result);

    isSpinning = false;

    Debug.Log(
        "[SPIN END] Engine: " + activeEngine.GetEngineName()
    );

    int houseProfit = totalBetsPlaced - totalPayoutsGiven;
    int playerProfit = totalPayoutsGiven - totalBetsPlaced;

    Debug.Log(
        "[PROFIT TRACKING] Engine: " + activeEngine.GetEngineName() +
        " | Total House Profit: " + houseProfit +
        " | Total Player Profit: " + playerProfit +
        " (Total Bets: " + totalBetsPlaced + ", Total Payouts: " + totalPayoutsGiven + ")"
    );
}
}