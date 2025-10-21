using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameUIManager gameUIManager; // Reference to the GameUIManager
    
    [Header("Zone Settings")]
    public float fullEntryThreshold = 0.8f; // How much of the ball must be in the zone (0-1)
    
    private Dictionary<string, float> zoneEntryProgress = new Dictionary<string, float>();
    private string currentZone = "";
    private Collider ballCollider;
    
    void Start()
    {
        ballCollider = GetComponent<Collider>();
        if (ballCollider == null)
        {
            Debug.LogWarning("Ball is missing a Collider component!");
        }

        // Check for initial zones the ball might be in at startup
        Zone[] allZones = FindObjectsOfType<Zone>();
        foreach (Zone zone in allZones)
        {
            if (zone.GetComponent<Collider>() != null && ballCollider != null)
            {
                if (zone.GetComponent<Collider>().bounds.Contains(ballCollider.bounds.center))
                {
                    EnterZone(zone.zoneName);
                }
            }
        }
    }
    
    public void EnterZone(string zoneName)
    {
        if (!zoneEntryProgress.ContainsKey(zoneName))
        {
            zoneEntryProgress[zoneName] = 0f;
        }
    }
    
    public void ExitZone(string zoneName)
    {
        if (zoneEntryProgress.ContainsKey(zoneName))
        {
            zoneEntryProgress.Remove(zoneName);
            
            // If we exited the current zone, clear it
            if (currentZone == zoneName)
            {
                currentZone = "";
                UpdateZoneDisplay();
            }
        }
    }
    
    void Update()
    {
        UpdateZoneProgress();
    }
    
    private void UpdateZoneProgress()
    {
        string mostEnteredZone = "";
        float highestProgress = 0f;

        // Use a temporary dictionary to avoid modifying while enumerating
        Dictionary<string, float> tempProgress = new Dictionary<string, float>();

        // Find the zone with the highest entry progress
        foreach (var zone in zoneEntryProgress)
        {
            float progress = CalculateZoneEntryProgress(zone.Key);
            tempProgress[zone.Key] = progress;

            if (progress > highestProgress && progress >= fullEntryThreshold)
            {
                highestProgress = progress;
                mostEnteredZone = zone.Key;
            }
        }

        // Update the main dictionary after enumeration
        foreach (var kvp in tempProgress)
        {
            zoneEntryProgress[kvp.Key] = kvp.Value;
        }

        // Update current zone only if we found a valid zone with sufficient entry
        if (mostEnteredZone != "" && mostEnteredZone != currentZone)
        {
            currentZone = mostEnteredZone;
            UpdateZoneDisplay();
        }
        else if (mostEnteredZone == "" && currentZone != "")
        {
            // No valid zones, clear current zone
            currentZone = "";
            UpdateZoneDisplay();
        }
    }
    
    private float CalculateZoneEntryProgress(string zoneName)
    {
        // Find the zone GameObject
        GameObject zoneObject = GameObject.Find(zoneName);
        if (zoneObject == null) return 0f;
        
        Collider zoneCollider = zoneObject.GetComponent<Collider>();
        if (zoneCollider == null || ballCollider == null) return 0f;
        
        // Calculate how much of the ball is inside the zone
        // This is a simplified calculation - you might want to make it more precise
        Vector3 ballCenter = ballCollider.bounds.center;
        Vector3 zoneCenter = zoneCollider.bounds.center;
        Vector3 zoneSize = zoneCollider.bounds.size;
        
        // Calculate distance from ball center to zone center, normalized by zone size
        Vector3 normalizedDistance = new Vector3(
            Mathf.Abs(ballCenter.x - zoneCenter.x) / (zoneSize.x / 2f),
            Mathf.Abs(ballCenter.y - zoneCenter.y) / (zoneSize.y / 2f),
            Mathf.Abs(ballCenter.z - zoneCenter.z) / (zoneSize.z / 2f)
        );
        
        // Progress is 1 when ball center is at zone center, 0 when at edge
        float progress = 1f - Mathf.Clamp01(normalizedDistance.magnitude / Mathf.Sqrt(3f));
        
        return progress;
    }
    
    private void UpdateZoneDisplay()
    {
        if (gameUIManager != null)
        {
            gameUIManager.UpdateZoneText(currentZone);
            if (!string.IsNullOrEmpty(currentZone))
            {
                Debug.Log("Player is in zone: " + currentZone);
            }
            else
            {
                Debug.Log("Player is not in any zone (null)");
            }
        }
    }
    
    // Debug information in editor
    void OnDrawGizmosSelected()
    {
        foreach (var zone in zoneEntryProgress)
        {
            GameObject zoneObject = GameObject.Find(zone.Key);
            if (zoneObject != null)
            {
                Collider zoneCollider = zoneObject.GetComponent<Collider>();
                if (zoneCollider != null)
                {
                    float progress = zoneEntryProgress.ContainsKey(zone.Key) ? zoneEntryProgress[zone.Key] : 0f;
                    Gizmos.color = progress >= fullEntryThreshold ? Color.green : Color.yellow;
                    Gizmos.DrawWireCube(zoneCollider.bounds.center, zoneCollider.bounds.size);
                    
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(zoneCollider.bounds.center, $"{zone.Key}\nProgress: {progress:P0}");
                    #endif
                }
            }
        }
    }
}