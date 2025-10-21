using UnityEngine;
using UnityEngine.UIElements;

public class MissionUIManager : MonoBehaviour
{
    [Header("UI References")]
    public UIDocument missionUIDocument;

    void Start()
    {
        // Disable the MissionUIManager on start
        gameObject.SetActive(false);

        if (missionUIDocument == null)
        {
            missionUIDocument = GetComponent<UIDocument>();
        }

        // You can add more initialization here if needed
    }
}
