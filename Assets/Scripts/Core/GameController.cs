using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public ReelController reel1;
    public ReelController reel2;
    public ReelController reel3;

    public UIController uiController;

    private DatabaseOutcomeEngine outcomeEngine;
    private int playerBalance = 100;
    private bool isSpinning = false;

    void Start()
    {
        outcomeEngine = new DatabaseOutcomeEngine();
        uiController.UpdateBalance(playerBalance);
    }

    public void OnSpinButtonPressed()
    {
        if (isSpinning)
            return;

        if (playerBalance <= 0)
            return;

        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        playerBalance -= 10;
        uiController.UpdateBalance(playerBalance);

        SpinResult result = outcomeEngine.GenerateResult();

        yield return StartCoroutine(reel1.Spin());
        yield return StartCoroutine(reel2.Spin());
        yield return StartCoroutine(reel3.Spin());

        playerBalance += result.payout;

        uiController.UpdateBalance(playerBalance);
        uiController.ShowResult(result);

        isSpinning = false;
    }
}