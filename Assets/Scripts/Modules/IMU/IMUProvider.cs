using UnityEngine;

public class IMUProvider : MonoBehaviour
{
    public Vector3 Acceleration { get; private set; }
    public Vector3 Gyroscope { get; private set; }
    public Quaternion Attitude { get; private set; }

    private void Awake()
    {
        // Turn on the hardware sensors
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f; // 60 Hz
        }
    }

    private void Update()
    {
        // Read the sensors
        Acceleration = Input.gyro.userAcceleration;
        Gyroscope = Input.gyro.rotationRateUnbiased;
        Attitude = Input.gyro.attitude;
    }

    private void OnGUI()
    {
        // Debug display on phone screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.red;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width, Screen.height));
        GUILayout.Label("--- MOOTEZ IMU TEST ---", style);
        GUILayout.Label($"Acc: {Acceleration:F3}", style);
        GUILayout.Label($"Gyro: {Gyroscope:F3}", style);
        GUILayout.Label($"Att: {Attitude.eulerAngles:F1}", style);
        GUILayout.EndArea();
    }
}