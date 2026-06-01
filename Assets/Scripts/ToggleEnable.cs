using UnityEngine;

public class ToggleEnable : MonoBehaviour
{
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
