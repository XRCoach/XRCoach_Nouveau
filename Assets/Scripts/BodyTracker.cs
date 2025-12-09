using UnityEngine;

/// <summary>
/// Tracks the user's body using IMU sensor data
/// Updates position and rotation based on IMU readings
/// </summary>
public class BodyTracker : MonoBehaviour
{
    [Header("IMU Integration")]
    [SerializeField] private IMUProvider imuProvider;
    [SerializeField] private bool useIMU = true;
    
    [Header("Tracking Settings")]
    [SerializeField] private bool trackRotation = true;
    [SerializeField] private bool trackPosition = false;
    [SerializeField] private float positionScale = 0.1f;
    
    [Header("Smoothing")]
    [SerializeField] private float rotationSmoothing = 5f;
    [SerializeField] private float positionSmoothing = 3f;
    
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private Vector3 velocity;
    
    private void Awake()
    {
        // Auto-find IMU provider if not assigned
        if (imuProvider == null)
        {
            imuProvider = FindObjectOfType<IMUProvider>();
        }
        
        targetRotation = transform.rotation;
        targetPosition = transform.position;
    }
    
    private void Update()
    {
        if (useIMU && imuProvider != null)
        {
            UpdateFromIMU(imuProvider.Attitude, imuProvider.Acceleration);
        }
        else
        {
            // Fallback simulation for testing in editor
            SimulateMovement();
        }
    }
    
    /// <summary>
    /// Updates body tracking from IMU sensor data
    /// Called automatically each frame or can be called externally
    /// </summary>
    public void UpdateFromIMU(Quaternion rotation, Vector3 acceleration)
    {
        // Update rotation
        if (trackRotation)
        {
            targetRotation = rotation;
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSmoothing
            );
        }
        
        // Update position (basic integration)
        if (trackPosition)
        {
            // Integrate acceleration to velocity
            velocity += acceleration * positionScale * Time.deltaTime;
            
            // Apply damping
            velocity *= 0.95f;
            
            // Integrate velocity to position
            targetPosition += velocity * Time.deltaTime;
            
            // Apply smoothing
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * positionSmoothing
            );
        }
    }
    
    /// <summary>
    /// Simulation for testing without physical device
    /// </summary>
    private void SimulateMovement()
    {
        if (Application.isEditor)
        {
            // Simulate rotation with keyboard
            float rotSpeed = 30f;
            float rotation = Mathf.Sin(Time.time * 0.5f) * rotSpeed;
            
            if (Input.GetKey(KeyCode.LeftArrow))
                rotation = -rotSpeed;
            if (Input.GetKey(KeyCode.RightArrow))
                rotation = rotSpeed;
            
            targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 2f
            );
        }
    }
    
    /// <summary>
    /// Reset position and velocity tracking
    /// </summary>
    public void ResetTracking()
    {
        targetPosition = transform.position;
        velocity = Vector3.zero;
        targetRotation = transform.rotation;
    }
}