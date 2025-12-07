using UnityEngine;
using UnityEngine.Events; // Required for events

public class SquatAnalyzer : MonoBehaviour
{
    [Header("Dependencies")]
    public IMUProvider imuProvider; // Link to your sensor script

    [Header("Settings")]
    public float SquatThreshold = 75f;   // Angle to consider a "Squat"
    public float StandingThreshold = 20f; // Angle to consider "Standing"

    [Header("Debug Info")]
    public string CurrentState = "Idle";
    public float CurrentAngle = 0f;
    public int RepCount = 0;

    // States for our machine
    private enum ExerciseState { Standing, Descending, Squatting, Ascending }
    private ExerciseState _state = ExerciseState.Standing;

    private void Update()
    {
        if (imuProvider == null) return;

        // 1. Calculate the Angle
        // We compare the Phone's "Up" vector vs the World's "Up" vector.
        // When standing, they are aligned (0 degrees).
        // When squatting (thighs horizontal), the phone rotates ~90 degrees.
        Vector3 phoneUp = imuProvider.Attitude * Vector3.up;
        CurrentAngle = Vector3.Angle(Vector3.up, phoneUp);

        // 2. State Machine Logic
        switch (_state)
        {
            case ExerciseState.Standing:
                // If angle increases past 30, we are going down
                if (CurrentAngle > 30f) ChangeState(ExerciseState.Descending);
                break;

            case ExerciseState.Descending:
                // If we pass the threshold (75), we are in a deep squat
                if (CurrentAngle > SquatThreshold) ChangeState(ExerciseState.Squatting);
                // If they go back up too early, reset
                else if (CurrentAngle < StandingThreshold) ChangeState(ExerciseState.Standing);
                break;

            case ExerciseState.Squatting:
                // We are at the bottom. Wait for them to go up.
                if (CurrentAngle < SquatThreshold - 10f) ChangeState(ExerciseState.Ascending);
                break;

            case ExerciseState.Ascending:
                // If they reach the top, count the rep!
                if (CurrentAngle < StandingThreshold)
                {
                    RepCount++;
                    Debug.Log($"Repetition {RepCount} Completed!");
                    ChangeState(ExerciseState.Standing);
                }
                break;
        }
    }

    private void ChangeState(ExerciseState newState)
    {
        _state = newState;
        CurrentState = _state.ToString();
    }

    // Display Debug Info on Screen
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.yellow; // Yellow text for Analysis

        GUILayout.BeginArea(new Rect(500, 50, Screen.width, Screen.height));
        GUILayout.Label($"State: {CurrentState}", style);
        GUILayout.Label($"Angle: {CurrentAngle:F1}°", style);
        GUILayout.Label($"REPS: {RepCount}", style);
        GUILayout.EndArea();
    }
}