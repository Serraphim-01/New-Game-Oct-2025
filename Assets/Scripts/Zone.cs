using UnityEngine;

public class Zone : MonoBehaviour
{
    public string zoneName = "Zone";
    
    void Start()
    {
        // Set zoneName to match the GameObject name for consistency
        zoneName = gameObject.name;

        // Ensure the zone has a collider set as trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("Zone " + zoneName + " missing collider!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player/ball
        if (other.CompareTag("Player"))
        {
            ZoneManager zoneManager = other.GetComponent<ZoneManager>();
            if (zoneManager != null)
            {
                zoneManager.EnterZone(zoneName);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ZoneManager zoneManager = other.GetComponent<ZoneManager>();
            if (zoneManager != null)
            {
                zoneManager.ExitZone(zoneName);
            }
        }
    }
}