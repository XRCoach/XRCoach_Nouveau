using UnityEngine;

/// <summary>
/// Main integration hub for IMU-based avatar control
/// Connects IMUProvider, BodyTracker, and AvatarController
/// Provides a unified interface for body tracking
/// </summary>
public class IMUBodyIntegrator : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private IMUProvider imuProvider;
    [SerializeField] private AvatarController avatarController;
    [SerializeField] private BodyTracker bodyTracker;
    
    [Header("Body Bone Mapping")]
    [Tooltip("Map IMU data to specific body parts")]
    [SerializeField] private BodyPartMapping[] bodyPartMappings;
    
    [Header("Global Settings")]
    [SerializeField] private bool enableTracking = true;
    [SerializeField] private float globalSmoothingFactor = 0.15f;
    
    [Header("Calibration")]
    [SerializeField] private KeyCode calibrationKey = KeyCode.C;
    [SerializeField] private KeyCode resetKey = KeyCode.R;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private bool isInitialized = false;
    
    private void Awake()
    {
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        // Auto-find components if not assigned
        if (imuProvider == null)
        {
            imuProvider = FindObjectOfType<IMUProvider>();
            if (imuProvider == null)
            {
                Debug.LogWarning("IMUBodyIntegrator: No IMUProvider found. Creating one...");
                GameObject imuObj = new GameObject("IMUProvider");
                imuProvider = imuObj.AddComponent<IMUProvider>();
            }
        }
        
        if (avatarController == null)
        {
            avatarController = FindObjectOfType<AvatarController>();
            if (avatarController == null)
            {
                Debug.LogWarning("IMUBodyIntegrator: No AvatarController found in scene.");
            }
        }
        
        if (bodyTracker == null)
        {
            bodyTracker = FindObjectOfType<BodyTracker>();
        }
        
        // Initialize default body part mappings if none set
        if (bodyPartMappings == null || bodyPartMappings.Length == 0)
        {
            SetupDefaultMappings();
        }
        
        isInitialized = true;
        Debug.Log("✅ IMUBodyIntegrator initialized successfully!");
    }
    
    private void Update()
    {
        if (!isInitialized || !enableTracking || imuProvider == null) return;
        
        // Handle calibration input
        if (Input.GetKeyDown(calibrationKey))
        {
            CalibrateAll();
        }
        
        if (Input.GetKeyDown(resetKey))
        {
            ResetAll();
        }
        
        // Update avatar from IMU data
        UpdateAvatarFromIMU();
    }
    
    private void UpdateAvatarFromIMU()
    {
        if (avatarController == null) return;
        
        // Get current IMU attitude
        Quaternion imuAttitude = imuProvider.Attitude;
        Vector3 imuAcceleration = imuProvider.Acceleration;
        
        // Apply to each mapped body part
        foreach (var mapping in bodyPartMappings)
        {
            if (!mapping.enabled) continue;
            
            // Calculate rotation with offset
            Quaternion rotation = imuAttitude * Quaternion.Euler(mapping.rotationOffset);
            
            // Apply axis inversions
            if (mapping.invertX || mapping.invertY || mapping.invertZ)
            {
                Vector3 euler = rotation.eulerAngles;
                if (mapping.invertX) euler.x = 360f - euler.x;
                if (mapping.invertY) euler.y = 360f - euler.y;
                if (mapping.invertZ) euler.z = 360f - euler.z;
                rotation = Quaternion.Euler(euler);
            }
            
            // Update avatar joint
            avatarController.UpdateJointRotation(mapping.targetBone, rotation);
        }
        
        // Update body tracker if available
        if (bodyTracker != null)
        {
            bodyTracker.UpdateFromIMU(imuAttitude, imuAcceleration);
        }
    }
    
    private void SetupDefaultMappings()
    {
        bodyPartMappings = new BodyPartMapping[]
        {
            new BodyPartMapping
            {
                enabled = true,
                targetBone = HumanBodyBones.Hips,
                rotationOffset = Vector3.zero,
                description = "Main body/torso tracking"
            },
            new BodyPartMapping
            {
                enabled = true,
                targetBone = HumanBodyBones.Spine,
                rotationOffset = new Vector3(0, 0, 0),
                description = "Spine rotation"
            }
        };
    }
    
    /// <summary>
    /// Calibrate all tracking systems
    /// </summary>
    public void CalibrateAll()
    {
        if (imuProvider != null)
        {
            imuProvider.Calibrate();
        }
        
        if (bodyTracker != null)
        {
            bodyTracker.ResetTracking();
        }
        
        Debug.Log("✅ All systems calibrated!");
    }
    
    /// <summary>
    /// Reset all tracking data
    /// </summary>
    public void ResetAll()
    {
        if (bodyTracker != null)
        {
            bodyTracker.ResetTracking();
        }
        
        Debug.Log("✅ All tracking reset!");
    }
    
    /// <summary>
    /// Enable or disable tracking
    /// </summary>
    public void SetTrackingEnabled(bool enabled)
    {
        enableTracking = enabled;
        Debug.Log($"Tracking {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Add a new body part mapping at runtime
    /// </summary>
    public void AddBodyPartMapping(HumanBodyBones bone, Vector3 offset, bool enabled = true)
    {
        var newMapping = new BodyPartMapping
        {
            enabled = enabled,
            targetBone = bone,
            rotationOffset = offset
        };
        
        var list = new System.Collections.Generic.List<BodyPartMapping>(bodyPartMappings);
        list.Add(newMapping);
        bodyPartMappings = list.ToArray();
    }
    
    private void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.green;
        
        GUILayout.BeginArea(new Rect(10, 450, 500, 200));
        GUILayout.Label("=== IMU BODY INTEGRATOR ===", style);
        GUILayout.Label($"Tracking: {(enableTracking ? "ENABLED" : "DISABLED")}", style);
        GUILayout.Label($"IMU Connected: {(imuProvider != null ? "YES" : "NO")}", style);
        GUILayout.Label($"Avatar Connected: {(avatarController != null ? "YES" : "NO")}", style);
        GUILayout.Label($"Active Mappings: {bodyPartMappings?.Length ?? 0}", style);
        GUILayout.Label($"[{calibrationKey}] Calibrate  [{resetKey}] Reset", style);
        GUILayout.EndArea();
    }
}

/// <summary>
/// Defines how IMU data maps to a specific body part
/// </summary>
[System.Serializable]
public class BodyPartMapping
{
    [Tooltip("Enable this mapping")]
    public bool enabled = true;
    
    [Tooltip("Which bone to apply the IMU rotation to")]
    public HumanBodyBones targetBone;
    
    [Tooltip("Rotation offset to apply to the IMU data")]
    public Vector3 rotationOffset = Vector3.zero;
    
    [Tooltip("Invert X axis rotation")]
    public bool invertX = false;
    
    [Tooltip("Invert Y axis rotation")]
    public bool invertY = false;
    
    [Tooltip("Invert Z axis rotation")]
    public bool invertZ = false;
    
    [Tooltip("Optional description of this mapping")]
    public string description = "";
}
