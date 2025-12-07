using UnityEngine;

// This script simulates Member 3's job.
// It listens to Member 2's events and reacts.
public class MockFeedback : MonoBehaviour
{
    [Header("Dependencies")]
    public SquatAnalyzer Analyzer; // Link to your brain
    public Camera MainCamera;

    private Color _originalColor;

    private void Awake()
    {
        if (MainCamera == null) MainCamera = Camera.main;
        _originalColor = MainCamera.backgroundColor;
    }

    private void OnEnable()
    {
        // SUBSCRIBE to events (Start listening)
        if (Analyzer != null)
        {
            Analyzer.OnRepCount += HandleSuccess;
            Analyzer.OnBadForm += HandleError;
        }
    }

    private void OnDisable()
    {
        // UNSUBSCRIBE (Stop listening) - Important to prevent errors!
        if (Analyzer != null)
        {
            Analyzer.OnRepCount -= HandleSuccess;
            Analyzer.OnBadForm -= HandleError;
        }
    }

    // This runs when you do a Good Rep
    private void HandleSuccess()
    {
        Debug.Log("🔊 MOCK AUDIO: *DING* (Success)");
        StartCoroutine(FlashColor(Color.green));
    }

    // This runs when you do a Bad Rep
    private void HandleError(string errorMsg)
    {
        Debug.Log($"🔊 MOCK AUDIO: *BUZZ* ({errorMsg})");
        StartCoroutine(FlashColor(Color.red));
    }

    // Simple coroutine to flash the screen color for 0.2 seconds
    private System.Collections.IEnumerator FlashColor(Color targetColor)
    {
        MainCamera.backgroundColor = targetColor;
        yield return new WaitForSeconds(0.2f);
        MainCamera.backgroundColor = _originalColor;
    }
}