using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    public UIDocument gameUIDocument;
    public GameObject missionUIManagerObject;

    private Label zoneTextLabel;

    void Start()
    {
        Debug.Log("GameUIManager Start called");

        if (gameUIDocument == null)
        {
            gameUIDocument = GetComponent<UIDocument>();
        }

        if (gameUIDocument != null)
        {
            var root = gameUIDocument.rootVisualElement;
            var missionButton = root.Q<Button>("missionButton");

            if (missionButton != null)
            {
                missionButton.clicked += OnMissionButtonClicked;
                Debug.Log("Mission button click event registered");
            }

            zoneTextLabel = root.Q<Label>("zoneText");
        }
    }

    private void OnMissionButtonClicked()
    {
        Debug.Log("Mission Button Clicked - THIS SHOULD APPEAR");

        // Add immediate visual feedback
        if (gameUIDocument != null)
        {
            var root = gameUIDocument.rootVisualElement;
            var missionButton = root.Q<Button>("missionButton");
            if (missionButton != null)
            {
                missionButton.style.backgroundColor = Color.red; // Immediate visual feedback
            }
        }

        if (missionUIManagerObject != null)
        {
            gameObject.SetActive(false);
            missionUIManagerObject.SetActive(true);
        }
    }

    public void UpdateZoneText(string zoneName)
    {
        if (zoneTextLabel != null)
        {
            if (!string.IsNullOrEmpty(zoneName))
            {
                zoneTextLabel.text = "Current Zone: " + zoneName;
            }
            else
            {
                zoneTextLabel.text = "No Zone";
            }
        }
    }
}
