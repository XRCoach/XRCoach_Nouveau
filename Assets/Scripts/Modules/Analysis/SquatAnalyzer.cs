using UnityEngine;
using System; // Required for Events

public class SquatAnalyzer : MonoBehaviour
{
    [Header("Dependencies")]
    public IMUProvider imuProvider;

    [Header("Settings")]
    public float SquatThreshold = 75f;
    public float StandingThreshold = 20f;

    [Header("Coach Settings")]
    public float MinDescentTime = 1.0f; // Seconds. Any faster is "Too Fast"

    [Header("Debug Info")]
    public string CurrentState = "Idle";
    public float CurrentAngle = 0f;
    public int RepCount = 0;
    public string LastError = ""; // Stores the last coaching feedback

    // Events for Member 3 later
    public event Action OnRepCount;
    public event Action<string> OnBadForm;

    private enum ExerciseState { Standing, Descending, Squatting, Ascending }
    private ExerciseState _state = ExerciseState.Standing;

    private float _stateStartTime = 0f; // To track duration

    private void Update()
    {
        if (imuProvider == null) return;

        // 1. Calculate Angle
        Vector3 phoneUp = imuProvider.Attitude * Vector3.up;
        CurrentAngle = Vector3.Angle(Vector3.up, phoneUp);

        // 2. State Machine
        switch (_state)
        {
            case ExerciseState.Standing:
                if (CurrentAngle > 30f) ChangeState(ExerciseState.Descending);
                break;

            case ExerciseState.Descending:
                // Check if they reached the bottom
                if (CurrentAngle > SquatThreshold)
                {
                    // --- SPEED CHECK ---
                    float descentDuration = Time.time - _stateStartTime;

                    if (descentDuration < MinDescentTime)
                    {
                        // Too Fast!
                        LastError = $"TOO FAST! ({descentDuration:F1}s)";
                        OnBadForm?.Invoke("Too Fast");
                        Debug.LogWarning(LastError);
                    }
                    else
                    {
                        // Good Speed
                        LastError = "";
                    }
                    // -------------------

                    ChangeState(ExerciseState.Squatting);
                }
                // Reset if they go back up without squatting
                else if (CurrentAngle < StandingThreshold) ChangeState(ExerciseState.Standing);
                break;

            case ExerciseState.Squatting:
                if (CurrentAngle < SquatThreshold - 10f) ChangeState(ExerciseState.Ascending);
                break;

            case ExerciseState.Ascending:
                if (CurrentAngle < StandingThreshold)
                {
                    RepCount++;
                    OnRepCount?.Invoke();
                    LastError = "Good Rep!"; // Positive feedback
                    ChangeState(ExerciseState.Standing);
                }
                break;
        }
    }

    private void ChangeState(ExerciseState newState)
    {
        _state = newState;
        _stateStartTime = Time.time; // Reset timer on state change
        CurrentState = _state.ToString();
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.yellow;

        // Position on the RIGHT side
        GUILayout.BeginArea(new Rect(Screen.width - 400, 50, 400, Screen.height));

        GUILayout.Label($"State: {CurrentState}", style);
        GUILayout.Label($"Angle: {CurrentAngle:F1}°", style);
        GUILayout.Label($"REPS: {RepCount}", style);

        // Show Errors in RED
        style.normal.textColor = Color.red;
        style.fontSize = 30;
        GUILayout.Space(20); // Add gap
        GUILayout.Label(LastError, style);

        GUILayout.EndArea();
    }
}