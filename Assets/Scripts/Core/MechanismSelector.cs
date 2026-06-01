using TMPro;
using UnityEngine;

public class MechanismSelector : MonoBehaviour
{
    public TMP_Dropdown mechanismDropdown;
    public TMP_Dropdown rulePreviewDropdown;

    public GameController gameController;

    private RTPControlEngine rtpEngine;
    private StateMachineEngine stateEngine;

    void Start()
    {
        mechanismDropdown.onValueChanged.AddListener(OnMechanismChanged);
        rulePreviewDropdown.onValueChanged.AddListener(OnRuleChanged);

        OnMechanismChanged(mechanismDropdown.value);
    }

    private void OnMechanismChanged(int index)
    {
        switch (index)
        {
            case 0:
                gameController.SetEngine(new DatabaseOutcomeEngine());

                rulePreviewDropdown.interactable = false;
                Debug.Log("Mechanism Changed to DatabaseOutcome");

                break;

            case 1:
                gameController.SetEngine(new PatternOutcomeEngine());

                rulePreviewDropdown.interactable = false;
                Debug.Log("Mechanism Changed to PatternOutcome");

                break;

            case 2:
                rtpEngine = new RTPControlEngine(0.60f);

                gameController.SetEngine(rtpEngine);

                rulePreviewDropdown.interactable = true;
                Debug.Log("Mechanism Changed to RTPControlEngine");

                break;

            case 3:
                stateEngine = new StateMachineEngine(GameState.Cold);

                gameController.SetEngine(stateEngine);

                rulePreviewDropdown.interactable = true;
                Debug.Log("Mechanism Changed to  StateMachineEngine");

                break;
        }
    }

    private void OnRuleChanged(int index)
    {
        if (rtpEngine != null &&
            gameController.GetCurrentEngineName() == "RTP Control")
        {
            switch (index)
            {
                case 0:
                    rtpEngine.SetTargetRTP(0.60f);
                    break;

                case 1:
                    rtpEngine.SetTargetRTP(0.75f);
                    break;

                case 2:
                    rtpEngine.SetTargetRTP(0.90f);
                    break;
            }

            Debug.Log(
                "[RTP CONFIG] New RTP: " +
                rulePreviewDropdown.options[index].text
            );
        }

        if (stateEngine != null &&
            gameController.GetCurrentEngineName() == "State Machine")
        {
            switch (index)
            {
                case 0:
                    stateEngine.SetState(GameState.Cold);
                    break;

                case 1:
                    stateEngine.SetState(GameState.Normal);
                    break;

                case 2:
                    stateEngine.SetState(GameState.Hot);
                    break;

                case 3:
                    stateEngine.SetState(GameState.Bonus);
                    break;
            }

            Debug.Log(
                "[STATE CONFIG] New State: " +
                rulePreviewDropdown.options[index].text
            );
        }
    }
}