using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The transform of the player or object to follow")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("How smoothly the camera follows the target")]
    public float smoothTime = 0.3f;

    [Tooltip("Distance behind the target")]
    public float distance = 5f;

    [Tooltip("Height offset from the target")]
    public float height = 2f;

    [Header("Mouse Control")]
    [Tooltip("Sensitivity for mouse look")]
    public float mouseSensitivity = 300f;

    [Tooltip("Minimum vertical angle")]
    public float minYAngle = -80f;

    [Tooltip("Maximum vertical angle")]
    public float maxYAngle = 80f;

    [Header("Collision Avoidance")]
    [Tooltip("Layer mask for walls or obstacles to avoid")]
    public LayerMask wallLayer;

    [Tooltip("Minimum distance to maintain from walls")]
    public float minDistanceFromWall = 0.5f;

    private Vector3 velocity = Vector3.zero;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Lock cursor for mouse look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle cursor visibility and lock state with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Handle mouse input for camera rotation
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        // Create rotation quaternion
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Calculate desired position based on rotation
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -distance) + Vector3.up * height;

        // Check for collisions with walls and adjust desired position
        Vector3 directionToDesired = desiredPosition - target.position;
        float distanceToDesired = directionToDesired.magnitude;

        if (Physics.Raycast(target.position, directionToDesired.normalized, out RaycastHit hit, distanceToDesired, wallLayer))
        {
            // If hit a wall, set desired position to just outside the wall
            desiredPosition = hit.point - directionToDesired.normalized * minDistanceFromWall;
        }

        // Smoothly interpolate to the adjusted desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Set camera position and rotation
        transform.position = smoothedPosition;
        transform.rotation = rotation;

        // Rotate the player to match the camera's horizontal rotation
        target.rotation = Quaternion.Euler(0, rotationX, 0);
    }
}
