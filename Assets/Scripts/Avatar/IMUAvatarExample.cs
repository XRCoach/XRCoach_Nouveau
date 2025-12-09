using UnityEngine;

/// <summary>
/// Simple example demonstrating how to use the IMU avatar system
/// Add this to any GameObject to see a working example
/// </summary>
public class IMUAvatarExample : MonoBehaviour
{
    [Header("References (Auto-found if not assigned)")]
    [SerializeField] private IMUBodyIntegrator integrator;
    [SerializeField] private IMUProvider imuProvider;
    [SerializeField] private AvatarController avatarController;
    
    private void Start()
    {
        // Auto-find components
        if (integrator == null)
            integrator = FindObjectOfType<IMUBodyIntegrator>();
        
        if (imuProvider == null)
            imuProvider = FindObjectOfType<IMUProvider>();
        
        if (avatarController == null)
            avatarController = FindObjectOfType<AvatarController>();
        
        // Log status
        LogSystemStatus();
        
        // Optional: Auto-calibrate after 2 seconds
        Invoke(nameof(AutoCalibrate), 2f);
    }
    
    private void Update()
    {
        // Example: Toggle tracking with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (integrator != null)
            {
                // Toggle tracking (you'd need to add a getter for enableTracking)
                Debug.Log("Toggle tracking with integrator.SetTrackingEnabled()");
            }
        }
        
        // Example: Manual calibration with C key (already handled by IMUProvider)
        if (Input.GetKeyDown(KeyCode.C))
        {
            CalibrateSystem();
        }
        
        // Example: Reset with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSystem();
        }
        
        // Example: Display current rotation
        if (Input.GetKeyDown(KeyCode.D))
        {
            DisplayDebugInfo();
        }
    }
    
    private void LogSystemStatus()
    {
        Debug.Log("=== IMU Avatar System Status ===");
        Debug.Log($"IMUBodyIntegrator: {(integrator != null ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"IMUProvider: {(imuProvider != null ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"AvatarController: {(avatarController != null ? "✓ Found" : "✗ Missing")}");
        
        if (integrator == null)
        {
            Debug.LogWarning("Add IMUBodyIntegrator component to use the system!");
        }
    }
    
    private void AutoCalibrate()
    {
        if (imuProvider != null)
        {
            imuProvider.Calibrate();
            Debug.Log("✅ Auto-calibration completed!");
        }
    }
    
    private void CalibrateSystem()
    {
        if (integrator != null)
        {
            integrator.CalibrateAll();
            Debug.Log("✅ System calibrated via integrator!");
        }
        else if (imuProvider != null)
        {
            imuProvider.Calibrate();
            Debug.Log("✅ IMU calibrated!");
        }
    }
    
    private void ResetSystem()
    {
        if (integrator != null)
        {
            integrator.ResetAll();
            Debug.Log("✅ System reset via integrator!");
        }
    }
    
    private void DisplayDebugInfo()
    {
        if (imuProvider != null)
        {
            Debug.Log($"IMU Attitude: {imuProvider.Attitude.eulerAngles}");
            Debug.Log($"IMU Acceleration: {imuProvider.Acceleration}");
            Debug.Log($"IMU Gyroscope: {imuProvider.Gyroscope}");
        }
    }
    
    /// <summary>
    /// Example: Programmatically enable/disable tracking
    /// </summary>
    public void SetTrackingEnabled(bool enabled)
    {
        if (integrator != null)
        {
            integrator.SetTrackingEnabled(enabled);
        }
    }
    
    /// <summary>
    /// Example: Add a new body part to track
    /// </summary>
    public void AddHeadTracking()
    {
        if (integrator != null)
        {
            integrator.AddBodyPartMapping(
                HumanBodyBones.Head, 
                new Vector3(0, 0, 0), 
                enabled: true
            );
            Debug.Log("✅ Head tracking added!");
        }
    }
}
