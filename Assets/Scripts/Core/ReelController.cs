using UnityEngine;
using System.Collections;

public class ReelController : MonoBehaviour
{
    public float spinDuration = 1.5f;
    private bool spinning = false;

    public IEnumerator Spin()
    {
        spinning = true;

        float timer = 0f;

        while (timer < spinDuration)
        {
            transform.Rotate(0f, 0f, -1000f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        spinning = false;
    }

    public bool IsSpinning()
    {
        return spinning;
    }
}