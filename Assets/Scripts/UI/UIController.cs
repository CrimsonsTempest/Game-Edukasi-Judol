using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public TMP_Text balanceText;
    public TMP_Text resultText;
    public TMP_Dropdown rulePreviewDropdown;

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

    public void UpdateRulePreview(List<string> rules)
    {
        rulePreviewDropdown.ClearOptions();
        rulePreviewDropdown.AddOptions(rules);
        rulePreviewDropdown.RefreshShownValue();
    }
}