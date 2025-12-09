using UnityEngine;

public class IMUProvider : MonoBehaviour
{
    // Public Data
    public Vector3 Acceleration { get; private set; }
    public Vector3 Gyroscope { get; private set; }

    // This is now the "Corrected" attitude (Zeroed)
    public Quaternion Attitude { get; private set; }

    // Internal Raw Data
    private Quaternion _rawAttitude;
    private Quaternion _calibrationOffset = Quaternion.identity; // The "Bias"

    private void Awake()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f;
        }
    }

    private void Update()
    {
        // 1. Get Raw Data (Android/iOS or Simulation)
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Acceleration = Input.gyro.userAcceleration;
            Gyroscope = Input.gyro.rotationRateUnbiased;
            _rawAttitude = Input.gyro.attitude;
        }
        else
        {
            SimulateMotion();
        }

        // 2. Apply Calibration (The Math Magic)
        // We multiply the Inverse of the Offset by the Raw Rotation to get "Zeroed" rotation.
        Attitude = Quaternion.Inverse(_calibrationOffset) * _rawAttitude;

        // 3. Input for Calibration (Press 'C' to reset zero)
        if (Input.GetKeyDown(KeyCode.C))
        {
            Calibrate();
        }
    }

    // Call this when the user is standing still at the start
    public void Calibrate()
    {
        _calibrationOffset = _rawAttitude;
        Debug.Log("✅ Sensors Calibrated! New Zero set.");
    }

    private void SimulateMotion()
    {
        // Simulation now includes a "Base Tilt" to prove calibration works
        // Let's pretend the phone is in a crooked pocket (15 degrees off)
        Quaternion pocketTilt = Quaternion.Euler(15, 0, 0);
        Quaternion movement = Quaternion.identity;

        if (Input.GetKey(KeyCode.Space))
        {
            movement = Quaternion.Euler(90, 0, 0);
        }

        Quaternion targetRot = pocketTilt * movement;
        _rawAttitude = Quaternion.Slerp(_rawAttitude, targetRot, Time.deltaTime * 5f);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.red;

        // Lower right corner
        GUILayout.BeginArea(new Rect(500, 250, Screen.width, Screen.height));
        GUILayout.Label("--- IMU PROVIDER ---", style);
        GUILayout.Label($"[C] to Calibrate", style);
        GUILayout.Label($"Raw: {_rawAttitude.eulerAngles:F1}", style);
        GUILayout.Label($"Corrected: {Attitude.eulerAngles:F1}", style);
        GUILayout.EndArea();
    }
}