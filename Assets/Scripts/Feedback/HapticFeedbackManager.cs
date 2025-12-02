using UnityEngine;
using System.Collections;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

/// <summary>
/// Gère le feedback haptique (vibrations) pour iOS et Android
/// Fournit des patterns de vibration pour différents types de feedback
/// </summary>
public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private bool enableHaptics = true;
    [SerializeField] private float hapticIntensity = 1f;

    [Header("Pattern Timings (Android)")]
    [SerializeField] private long lightVibrationDuration = 20;
    [SerializeField] private long mediumVibrationDuration = 50;
    [SerializeField] private long heavyVibrationDuration = 100;

    private bool isVibrating = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject vibrator;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeHaptics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialise le système de vibration selon la plateforme
    /// </summary>
    private void InitializeHaptics()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to initialize Android vibrator: " + e.Message);
        }
#endif
    }

    /// <summary>
    /// Vibration légère - pour feedback positif subtil
    /// </summary>
    public void LightVibration()
    {
        if (!enableHaptics) return;

#if UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
        // Sur iOS, on peut utiliser le Taptic Engine pour des vibrations plus précises
        TriggerIOSImpact(UnityEngine.iOS.NotificationFeedbackType.Success);
#elif UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(lightVibrationDuration);
#else
        Debug.Log("Light Vibration triggered");
#endif
    }

    /// <summary>
    /// Vibration moyenne - pour feedback important
    /// </summary>
    public void MediumVibration()
    {
        if (!enableHaptics) return;

#if UNITY_IOS && !UNITY_EDITOR
        TriggerIOSImpact(UnityEngine.iOS.NotificationFeedbackType.Warning);
#elif UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(mediumVibrationDuration);
#else
        Debug.Log("Medium Vibration triggered");
#endif
    }

    /// <summary>
    /// Vibration forte - pour erreurs ou achievements importants
    /// </summary>
    public void HeavyVibration()
    {
        if (!enableHaptics) return;

#if UNITY_IOS && !UNITY_EDITOR
        TriggerIOSImpact(UnityEngine.iOS.NotificationFeedbackType.Error);
#elif UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(heavyVibrationDuration);
#else
        Debug.Log("Heavy Vibration triggered");
#endif
    }

    /// <summary>
    /// Pattern de vibration personnalisé - pour répétition complétée
    /// </summary>
    public void RepCompletePattern()
    {
        if (!enableHaptics) return;

        StartCoroutine(RepCompletePatternCoroutine());
    }

    private IEnumerator RepCompletePatternCoroutine()
    {
        // Double tap: vibration courte, pause, vibration courte
        LightVibration();
        yield return new WaitForSeconds(0.1f);
        LightVibration();
    }

    /// <summary>
    /// Pattern de vibration pour série complétée
    /// </summary>
    public void SetCompletePattern()
    {
        if (!enableHaptics) return;

        StartCoroutine(SetCompletePatternCoroutine());
    }

    private IEnumerator SetCompletePatternCoroutine()
    {
        // Triple tap: 3 vibrations courtes
        for (int i = 0; i < 3; i++)
        {
            MediumVibration();
            yield return new WaitForSeconds(0.15f);
        }
    }

    /// <summary>
    /// Pattern de vibration pour erreur de posture
    /// </summary>
    public void ErrorPattern()
    {
        if (!enableHaptics) return;

        StartCoroutine(ErrorPatternCoroutine());
    }

    private IEnumerator ErrorPatternCoroutine()
    {
        // Pattern d'alerte: vibration longue
        HeavyVibration();
        yield return new WaitForSeconds(0.1f);
        MediumVibration();
    }

    /// <summary>
    /// Pattern de vibration pour achievement/milestone
    /// </summary>
    public void AchievementPattern()
    {
        if (!enableHaptics) return;

        StartCoroutine(AchievementPatternCoroutine());
    }

    private IEnumerator AchievementPatternCoroutine()
    {
        // Pattern de célébration: crescendo
        LightVibration();
        yield return new WaitForSeconds(0.1f);
        MediumVibration();
        yield return new WaitForSeconds(0.1f);
        HeavyVibration();
    }

    /// <summary>
    /// Feedback haptique continu selon l'intensité du mouvement
    /// </summary>
    public void ContinuousFeedback(float intensity)
    {
        if (!enableHaptics || isVibrating) return;

        StartCoroutine(ContinuousFeedbackCoroutine(intensity));
    }

    private IEnumerator ContinuousFeedbackCoroutine(float intensity)
    {
        isVibrating = true;

        // Adapte la fréquence de vibration selon l'intensité
        float interval = Mathf.Lerp(0.5f, 0.1f, intensity);

        LightVibration();
        yield return new WaitForSeconds(interval);

        isVibrating = false;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Déclenche une vibration sur Android
    /// </summary>
    private void VibrateAndroid(long milliseconds)
    {
        if (vibrator != null)
        {
            try
            {
                long adjustedDuration = (long)(milliseconds * hapticIntensity);
                vibrator.Call("vibrate", adjustedDuration);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Android vibration failed: " + e.Message);
            }
        }
    }
    
    /// <summary>
    /// Déclenche un pattern de vibration sur Android
    /// </summary>
    private void VibrateAndroidPattern(long[] pattern, int repeat)
    {
        if (vibrator != null)
        {
            try
            {
                vibrator.Call("vibrate", pattern, repeat);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Android pattern vibration failed: " + e.Message);
            }
        }
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// Déclenche un impact haptique sur iOS (Taptic Engine)
    /// </summary>
    private void TriggerIOSImpact(UnityEngine.iOS.NotificationFeedbackType type)
    {
        // Note: Pour utiliser pleinement le Taptic Engine, vous devrez peut-être
        // créer un plugin natif iOS. Ceci est une implémentation de base.
        UnityEngine.iOS.Device.SetNoBackupFlag("");
        Handheld.Vibrate();
    }
#endif

    /// <summary>
    /// Active/désactive le feedback haptique
    /// </summary>
    public void SetHapticsEnabled(bool enabled)
    {
        enableHaptics = enabled;
    }

    /// <summary>
    /// Ajuste l'intensité des vibrations (0-1)
    /// </summary>
    public void SetHapticIntensity(float intensity)
    {
        hapticIntensity = Mathf.Clamp01(intensity);
    }

    /// <summary>
    /// Test rapide de tous les patterns
    /// </summary>
    public void TestAllPatterns()
    {
        StartCoroutine(TestAllPatternsCoroutine());
    }

    private IEnumerator TestAllPatternsCoroutine()
    {
        Debug.Log("Testing Light Vibration");
        LightVibration();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Medium Vibration");
        MediumVibration();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Heavy Vibration");
        HeavyVibration();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Rep Complete Pattern");
        RepCompletePattern();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Set Complete Pattern");
        SetCompletePattern();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Error Pattern");
        ErrorPattern();
        yield return new WaitForSeconds(1f);

        Debug.Log("Testing Achievement Pattern");
        AchievementPattern();

        Debug.Log("All haptic patterns tested!");
    }
}