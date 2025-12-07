using UnityEngine;

public class IMUProvider : MonoBehaviour
{
    public Vector3 Acceleration { get; private set; }
    public Vector3 Gyroscope { get; private set; }
    public Quaternion Attitude { get; private set; }

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
        // 1. Android/iOS Mode (Real Sensors)
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Acceleration = Input.gyro.userAcceleration;
            Gyroscope = Input.gyro.rotationRateUnbiased;
            Attitude = Input.gyro.attitude;
        }
        // 2. Windows/Editor Mode (Simulation)
        else
        {
            SimulateMotion();
        }
    }

    private void SimulateMotion()
    {
        // Default: Phone standing upright
        Quaternion targetRot = Quaternion.identity;

        // If SPACE is pressed, rotate 90 degrees (Simulate a deep squat or phone tilt)
        if (Input.GetKey(KeyCode.Space))
        {
            targetRot = Quaternion.Euler(90, 0, 0);
            Acceleration = new Vector3(0, -1, 0); // Fake gravity
        }
        else
        {
            Acceleration = Vector3.zero;
        }

        // Smoothly rotate towards target
        Attitude = Quaternion.Slerp(Attitude, targetRot, Time.deltaTime * 5f);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.red;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width, Screen.height));
        GUILayout.Label("--- IMU (SIMULATION) ---", style);
        GUILayout.Label($"Press SPACE to Squat", style);
        GUILayout.Label($"Attitude: {Attitude.eulerAngles:F1}", style);
        GUILayout.EndArea();
    }
}