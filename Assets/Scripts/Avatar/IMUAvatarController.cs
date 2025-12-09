using UnityEngine;

/// <summary>
/// Bridges the IMU sensor data to the avatar movement
/// Applies IMU rotation data to move the avatar's body parts
/// </summary>
public class IMUAvatarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IMUProvider imuProvider;
    [SerializeField] private AvatarController avatarController;
    
    [Header("Body Tracking")]
    [SerializeField] private Transform bodyRoot; // The main body transform (hips/spine)
    [SerializeField] private bool trackFullBody = false;
    
    [Header("IMU Mapping")]
    [Tooltip("Which body bone should follow the IMU rotation")]
    [SerializeField] private HumanBodyBones primaryBone = HumanBodyBones.Hips;
    
    [Header("Movement Settings")]
    [SerializeField] private bool enablePosition = true;
    [SerializeField] private float positionSensitivity = 1.0f;
    [SerializeField] private float heightOffset = 0f;
    
    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool invertZ = false;
    
    [Header("Smoothing")]
    [SerializeField] private bool enableSmoothing = true;
    [SerializeField] private float smoothingSpeed = 10f;
    
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 velocityEstimate;
    
    private void Awake()
    {
        // Auto-find components if not assigned
        if (imuProvider == null)
        {
            imuProvider = FindObjectOfType<IMUProvider>();
            if (imuProvider == null)
            {
                Debug.LogError("IMUAvatarController: No IMUProvider found in scene! Please add an IMUProvider component.");
            }
        }
        
        if (avatarController == null)
        {
            avatarController = GetComponent<AvatarController>();
            if (avatarController == null)
            {
                Debug.LogError("IMUAvatarController: No AvatarController found! Please assign or add an AvatarController component.");
            }
        }
        
        // Initialize position
        currentPosition = transform.position;
        currentRotation = transform.rotation;
    }
    
    private void Update()
    {
        if (imuProvider == null) return;
        
        // Update rotation based on IMU attitude
        if (enableRotation)
        {
            UpdateRotation();
        }
        
        // Update position based on IMU acceleration (optional)
        if (enablePosition)
        {
            UpdatePosition();
        }
    }
    
    private void UpdateRotation()
    {
        // Get the corrected attitude from IMU
        Quaternion imuRotation = imuProvider.Attitude;
        
        // Apply rotation offset
        imuRotation *= Quaternion.Euler(rotationOffset);
        
        // Apply inversions if needed
        Vector3 euler = imuRotation.eulerAngles;
        if (invertX) euler.x = -euler.x;
        if (invertY) euler.y = -euler.y;
        if (invertZ) euler.z = -euler.z;
        imuRotation = Quaternion.Euler(euler);
        
        // Apply smoothing
        if (enableSmoothing)
        {
            currentRotation = Quaternion.Slerp(currentRotation, imuRotation, Time.deltaTime * smoothingSpeed);
        }
        else
        {
            currentRotation = imuRotation;
        }
        
        // Update the avatar
        if (avatarController != null)
        {
            avatarController.UpdateJointRotation(primaryBone, currentRotation);
        }
        
        // Also update body root if assigned
        if (bodyRoot != null)
        {
            bodyRoot.rotation = currentRotation;
        }
    }
    
    private void UpdatePosition()
    {
        // Get acceleration from IMU
        Vector3 acceleration = imuProvider.Acceleration;
        
        // Integrate acceleration to get velocity (simplified physics)
        // Note: This is a basic integration and may drift over time
        // For better results, use sensor fusion or external tracking
        velocityEstimate += acceleration * Time.deltaTime * positionSensitivity;
        
        // Apply damping to prevent infinite drift
        velocityEstimate *= 0.95f;
        
        // Integrate velocity to get position
        Vector3 targetPosition = currentPosition + velocityEstimate * Time.deltaTime;
        targetPosition.y += heightOffset;
        
        // Apply smoothing
        if (enableSmoothing)
        {
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * smoothingSpeed);
        }
        else
        {
            currentPosition = targetPosition;
        }
        
        // Update transform
        transform.position = currentPosition;
    }
    
    /// <summary>
    /// Reset the position tracking (useful when recalibrating)
    /// </summary>
    public void ResetPosition()
    {
        currentPosition = transform.position;
        velocityEstimate = Vector3.zero;
    }
    
    /// <summary>
    /// Reset the rotation tracking
    /// </summary>
    public void ResetRotation()
    {
        currentRotation = transform.rotation;
    }
    
    /// <summary>
    /// Full reset of tracking
    /// </summary>
    public void ResetTracking()
    {
        ResetPosition();
        ResetRotation();
        
        if (imuProvider != null)
        {
            imuProvider.Calibrate();
        }
    }
    
    private void OnGUI()
    {
        if (imuProvider == null) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.cyan;
        
        GUILayout.BeginArea(new Rect(10, 150, 400, 300));
        GUILayout.Label("--- IMU AVATAR CONTROLLER ---", style);
        GUILayout.Label($"Position: {currentPosition:F2}", style);
        GUILayout.Label($"Rotation: {currentRotation.eulerAngles:F1}", style);
        GUILayout.Label($"Velocity: {velocityEstimate:F2}", style);
        GUILayout.Label($"[R] Reset Tracking", style);
        GUILayout.EndArea();
        
        // Handle reset input
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
        {
            ResetTracking();
            Debug.Log("✅ Tracking Reset!");
        }
    }
}
