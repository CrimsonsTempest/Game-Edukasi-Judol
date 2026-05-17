using UnityEngine;

[System.Serializable]
public class SpinResult
{
    public SpinType type;
    public int payout;

    public SpinResult(SpinType type, int payout)
    {
        this.type = type;
        this.payout = payout;
    }
}