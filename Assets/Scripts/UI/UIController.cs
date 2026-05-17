using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text balanceText;
    public TMP_Text resultText;

    public void UpdateBalance(int balance)
    {
        balanceText.text = "Balance: " + balance;
    }

    public void ShowResult(SpinResult result)
    {
        switch (result.type)
        {
            case SpinType.Loss:
                resultText.text = "LOSE";
                break;

            case SpinType.SmallWin:
                resultText.text = "SMALL WIN +" + result.payout;
                break;

            case SpinType.Jackpot:
                resultText.text = "JACKPOT +" + result.payout;
                break;
        }
    }
}