using UnityEngine;

public class AlwaysShowZoneCollider : MonoBehaviour
{
    [Header("Zone Collider Display")]
    public Color zoneColor = new Color(0, 0.5f, 1f, 0.2f); // Blue for zones
    public Color triggerColor = new Color(1f, 0.5f, 0f, 0.2f); // Orange for triggers
    
    private Collider collider;
    
    void OnDrawGizmos()
    {
        if (collider == null)
            collider = GetComponent<Collider>();
            
        if (collider == null) return;
        
        // Choose color based on whether it's a trigger
        Gizmos.color = collider.isTrigger ? triggerColor : zoneColor;
        
        if (collider is BoxCollider boxCollider)
        {
            DrawBoxCollider(boxCollider);
        }
        // Add other collider types as needed
    }
    
    void DrawBoxCollider(BoxCollider boxCollider)
    {
        Vector3 size = Vector3.Scale(boxCollider.size, transform.lossyScale);
        Vector3 center = transform.TransformPoint(boxCollider.center);
        
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.DrawCube(Vector3.zero, size);
    }
}
