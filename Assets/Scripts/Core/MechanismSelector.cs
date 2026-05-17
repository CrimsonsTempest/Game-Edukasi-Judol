using TMPro;
using UnityEngine;

public class MechanismSelector : MonoBehaviour
{
    public TMP_Dropdown mechanismDropdown;
    public GameController gameController;

    void Start()
    {
        mechanismDropdown.onValueChanged.AddListener(OnMechanismChanged);

        OnMechanismChanged(mechanismDropdown.value);
    }

    private void OnMechanismChanged(int index)
    {
        switch (index)
        {
            case 0:
                gameController.SetEngine(new DatabaseOutcomeEngine());
                break;

            case 1:
                gameController.SetEngine(new PatternOutcomeEngine());
                break;
        }
    }
}