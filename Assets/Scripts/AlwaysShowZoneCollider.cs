using UnityEngine;

public class AlwaysShowZoneCollider : MonoBehaviour
{
    private Collider zoneCollider;

    void Start()
    {
        zoneCollider = GetComponent<Collider>();
    }

    void OnDrawGizmos()
    {
        if (zoneCollider == null)
        {
            zoneCollider = GetComponent<Collider>();
        }

        if (zoneCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zoneCollider.bounds.center, zoneCollider.bounds.size);
        }
    }
}
