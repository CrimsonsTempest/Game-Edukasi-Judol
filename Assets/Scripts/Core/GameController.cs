using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    public void OnResetButtonPressed()
    {
        if (isSpinning)
            return;

        // Mereset semua state dengan cara memuat ulang scene saat ini.
        // Ini akan membersihkan stat RTP, reset balance ke 100, dll.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public string GetCurrentEngineName()
{
    if (activeEngine == null)
        return "None";

    return activeEngine.GetEngineName();
}

    private int[] GenerateTargetSymbols(SpinType type, int totalSymbols)
    {
        int[] targets = new int[3];
        if (totalSymbols == 0) return targets;

        switch (type)
        {
            case SpinType.Jackpot:
                int jpSymbol = Random.Range(0, totalSymbols);
                targets[0] = jpSymbol;
                targets[1] = jpSymbol;
                targets[2] = jpSymbol;
                break;

            case SpinType.SmallWin:
                // Standard casino style: Reel 1 & 2 match, Reel 3 differs
                int swSymbol = Random.Range(0, totalSymbols);
                int diffSymbol = Random.Range(0, totalSymbols);
                while (diffSymbol == swSymbol && totalSymbols > 1)
                {
                    diffSymbol = Random.Range(0, totalSymbols);
                }
                targets[0] = swSymbol;
                targets[1] = swSymbol;
                targets[2] = diffSymbol;
                break;

            case SpinType.Loss:
            default:
                // Ensure no win pattern (all 3 different if possible)
                targets[0] = Random.Range(0, totalSymbols);
                
                targets[1] = Random.Range(0, totalSymbols);
                while (targets[1] == targets[0] && totalSymbols > 1)
                {
                    targets[1] = Random.Range(0, totalSymbols);
                }
                
                targets[2] = Random.Range(0, totalSymbols);
                while ((targets[2] == targets[0] || targets[2] == targets[1]) && totalSymbols > 2)
                {
                    targets[2] = Random.Range(0, totalSymbols);
                }
                break;
        }
        return targets;
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

    // Generate target symbols based on the outcome
    int totalSymbols = (reel1.symbols != null) ? reel1.symbols.Length : 0;
    int[] targetSymbols = GenerateTargetSymbols(result.type, totalSymbols);

    // // PLAY SOUND: PlaySpinSound(); (Placeholder)

    // Start all reels spinning simultaneously
    reel1.StartSpin();
    reel2.StartSpin();
    reel3.StartSpin();

    // Spin duration 1 second
    yield return new WaitForSeconds(1.0f);

    // Stop Reel 1
    yield return StartCoroutine(reel1.StopSpin(targetSymbols[0]));
    // // PLAY SOUND: PlayReelStopSound(); (Placeholder)
    
    // Delay 300ms
    yield return new WaitForSeconds(0.3f);
    
    // Stop Reel 2
    yield return StartCoroutine(reel2.StopSpin(targetSymbols[1]));
    // // PLAY SOUND: PlayReelStopSound(); (Placeholder)
    
    // Delay 300ms
    yield return new WaitForSeconds(0.3f);
    
    // Stop Reel 3
    yield return StartCoroutine(reel3.StopSpin(targetSymbols[2]));
    // // PLAY SOUND: PlayReelStopSound(); (Placeholder)

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