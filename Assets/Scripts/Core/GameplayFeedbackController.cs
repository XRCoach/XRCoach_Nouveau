using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contrôleur principal qui intègre tous les systèmes de feedback
/// Point d'entrée unique pour déclencher les feedbacks visuels, audio et haptiques
/// </summary>
public class GameplayFeedbackController : MonoBehaviour
{
    public static GameplayFeedbackController Instance { get; private set; }

    [Header("Component References")]
    [SerializeField] private AvatarController avatarController;
    [SerializeField] private VisualFeedbackManager visualFeedback;
    [SerializeField] private AudioFeedbackManager audioFeedback;
    [SerializeField] private ParticleEffectController particleEffects;

    [Header("Exercise Configuration")]
    [SerializeField] private ExerciseType currentExercise;
    [SerializeField] private int targetReps = 10;
    [SerializeField] private int targetSets = 3;

    [Header("Quality Thresholds")]
    [SerializeField] private float perfectThreshold = 0.95f;
    [SerializeField] private float goodThreshold = 0.75f;
    [SerializeField] private float acceptableThreshold = 0.5f;

    // État actuel
    private int currentRep = 0;
    private int currentSet = 1;
    private float currentPrecision = 1f;
    private bool isExercising = false;
    private RepPhase currentPhase = RepPhase.Ready;

    // Events
    public delegate void RepCompletedDelegate(int repNumber, float quality);
    public delegate void SetCompletedDelegate(int setNumber);
    public delegate void WorkoutCompletedDelegate();

    public event RepCompletedDelegate OnRepCompleted;
    public event SetCompletedDelegate OnSetCompleted;
    public event WorkoutCompletedDelegate OnWorkoutCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ValidateComponents();
    }

    /// <summary>
    /// Valide que tous les composants nécessaires sont présents
    /// </summary>
    private void ValidateComponents()
    {
        if (avatarController == null)
            Debug.LogWarning("AvatarController not assigned!");
        if (visualFeedback == null)
            Debug.LogWarning("VisualFeedbackManager not assigned!");
        if (audioFeedback == null)
            Debug.LogWarning("AudioFeedbackManager not assigned!");
        if (particleEffects == null)
            Debug.LogWarning("ParticleEffectController not assigned!");
    }

    /// <summary>
    /// Démarre une session d'exercice
    /// </summary>
    public void StartExercise(ExerciseType exerciseType, int reps = 10, int sets = 3)
    {
        currentExercise = exerciseType;
        targetReps = reps;
        targetSets = sets;
        currentRep = 0;
        currentSet = 1;
        isExercising = true;
        currentPhase = RepPhase.Ready;

        Debug.Log($"Starting exercise: {exerciseType}, {reps} reps x {sets} sets");

        // Joue un message de bienvenue
        audioFeedback?.PlayEncouragement();
    }

    /// <summary>
    /// Met à jour les données de mouvement en temps réel
    /// Appelé par le système IMU/capteurs
    /// </summary>
    public void UpdateMovementData(MovementData data)
    {
        if (!isExercising) return;

        // Met à jour la précision actuelle
        currentPrecision = data.precision;

        // Met à jour l'avatar
        if (avatarController != null)
        {
            foreach (var jointData in data.joints)
            {
                avatarController.UpdateJointRotation(jointData.bone, jointData.rotation);
            }
        }

        // Met à jour les feedbacks visuels
        if (visualFeedback != null)
        {
            visualFeedback.UpdatePrecision(currentPrecision);
        }

        // Met à jour les particules
        if (particleEffects != null)
        {
            particleEffects.UpdateMovementQuality(currentPrecision);
        }

        // Détecte les changements de phase
        DetectPhaseChange(data);

        // Feedback continu selon la qualité
        ProvideContinuousFeedback(currentPrecision);
    }

    /// <summary>
    /// Détecte les changements de phase de mouvement (concentrique/excentrique)
    /// </summary>
    private void DetectPhaseChange(MovementData data)
    {
        RepPhase newPhase = data.phase;

        // Détection de répétition complète
        if (currentPhase == RepPhase.Eccentric && newPhase == RepPhase.Concentric)
        {
            OnRepetitionComplete();
        }

        currentPhase = newPhase;
    }

    /// <summary>
    /// Fournit un feedback continu basé sur la précision
    /// </summary>
    private void ProvideContinuousFeedback(float precision)
    {
        // Feedback visuel continu déjà géré par UpdatePrecision

        // Feedback haptique léger si mauvaise posture
        if (precision < acceptableThreshold)
        {
            HapticFeedbackManager.Instance?.ContinuousFeedback(1f - precision);
        }
    }

    /// <summary>
    /// Appelé quand une répétition est complétée
    /// </summary>
    private void OnRepetitionComplete()
    {
        currentRep++;

        // Détermine la qualité de la répétition
        MovementQuality quality = GetMovementQuality(currentPrecision);

        Debug.Log($"Rep {currentRep}/{targetReps} completed with {quality} quality ({currentPrecision:F2})");

        // Feedbacks selon la qualité
        switch (quality)
        {
            case MovementQuality.Perfect:
                TriggerPerfectRepFeedback();
                break;
            case MovementQuality.Good:
                TriggerGoodRepFeedback();
                break;
            default:
                TriggerAcceptableRepFeedback();
                break;
        }

        // Audio: compte la répétition
        audioFeedback?.PlayCount(currentRep);

        // Événement
        OnRepCompleted?.Invoke(currentRep, currentPrecision);

        // Check si la série est complète
        if (currentRep >= targetReps)
        {
            OnSetComplete();
        }
    }

    /// <summary>
    /// Feedback pour répétition parfaite
    /// </summary>
    private void TriggerPerfectRepFeedback()
    {
        // Visuel
        visualFeedback?.TriggerFlash(FlashType.Perfect);
        avatarController?.SetPostureFeedback(PostureQuality.Good);

        // Particules
        Vector3 avatarPos = avatarController != null ? avatarController.transform.position : transform.position;
        particleEffects?.TriggerPerfectMovementBurst(avatarPos);

        // Audio
        audioFeedback?.PlayMovementFeedback(MovementQuality.Perfect);
        audioFeedback?.PlayEncouragement();

        // Haptique
        HapticFeedbackManager.Instance?.RepCompletePattern();
    }

    /// <summary>
    /// Feedback pour bonne répétition
    /// </summary>
    private void TriggerGoodRepFeedback()
    {
        // Visuel
        visualFeedback?.TriggerFlash(FlashType.Good);
        avatarController?.SetPostureFeedback(PostureQuality.Good);

        // Particules
        Vector3 avatarPos = avatarController != null ? avatarController.transform.position : transform.position;
        particleEffects?.TriggerGoodMovementBurst(avatarPos);

        // Audio
        audioFeedback?.PlayMovementFeedback(MovementQuality.Good);

        // Haptique
        HapticFeedbackManager.Instance?.LightVibration();
    }

    /// <summary>
    /// Feedback pour répétition acceptable
    /// </summary>
    private void TriggerAcceptableRepFeedback()
    {
        // Visuel minimal
        avatarController?.SetPostureFeedback(PostureQuality.Normal);

        // Audio: correction si qualité faible
        if (currentPrecision < goodThreshold)
        {
            audioFeedback?.PlayCorrection();
        }
    }

    /// <summary>
    /// Appelé quand une série est complétée
    /// </summary>
    private void OnSetComplete()
    {
        Debug.Log($"Set {currentSet}/{targetSets} completed!");

        // Feedbacks de série complète
        visualFeedback?.TriggerFlash(FlashType.Perfect);

        Vector3 avatarPos = avatarController != null ? avatarController.transform.position : transform.position;
        particleEffects?.TriggerAchievementEffect(avatarPos);

        audioFeedback?.PlaySetComplete();
        HapticFeedbackManager.Instance?.SetCompletePattern();

        // Événement
        OnSetCompleted?.Invoke(currentSet);

        // Check si le workout est complet
        if (currentSet >= targetSets)
        {
            OnWorkoutComplete();
        }
        else
        {
            // Prépare la prochaine série
            currentSet++;
            currentRep = 0;
            currentPhase = RepPhase.Ready;
        }
    }

    /// <summary>
    /// Appelé quand le workout complet est terminé
    /// </summary>
    private void OnWorkoutComplete()
    {
        Debug.Log("Workout completed! Congratulations!");

        isExercising = false;

        // Feedbacks de workout complet
        audioFeedback?.PlayWorkoutComplete();
        HapticFeedbackManager.Instance?.AchievementPattern();

        Vector3 avatarPos = avatarController != null ? avatarController.transform.position : transform.position;
        particleEffects?.TriggerAchievementEffect(avatarPos);

        // Événement
        OnWorkoutCompleted?.Invoke();
    }

    /// <summary>
    /// Détermine la qualité du mouvement selon la précision
    /// </summary>
    private MovementQuality GetMovementQuality(float precision)
    {
        if (precision >= perfectThreshold)
            return MovementQuality.Perfect;
        else if (precision >= goodThreshold)
            return MovementQuality.Good;
        else
            return MovementQuality.Bad;
    }

    /// <summary>
    /// Déclenche un feedback d'erreur
    /// </summary>
    public void TriggerErrorFeedback(string errorMessage)
    {
        Debug.LogWarning($"Movement error: {errorMessage}");

        // Visuel
        visualFeedback?.TriggerFlash(FlashType.Bad);
        avatarController?.SetPostureFeedback(PostureQuality.Bad);

        // Particules
        Vector3 avatarPos = avatarController != null ? avatarController.transform.position : transform.position;
        particleEffects?.TriggerErrorBurst(avatarPos);

        // Audio
        audioFeedback?.PlayError();
        audioFeedback?.PlayCorrection();

        // Haptique
        HapticFeedbackManager.Instance?.ErrorPattern();
    }

    /// <summary>
    /// Arrête l'exercice en cours
    /// </summary>
    public void StopExercise()
    {
        isExercising = false;
        currentRep = 0;
        currentSet = 1;
        currentPhase = RepPhase.Ready;

        // Nettoie les particules
        particleEffects?.ClearAllParticles();
    }

    /// <summary>
    /// Obtient les statistiques actuelles
    /// </summary>
    public ExerciseStats GetCurrentStats()
    {
        return new ExerciseStats
        {
            currentRep = currentRep,
            targetReps = targetReps,
            currentSet = currentSet,
            targetSets = targetSets,
            currentPrecision = currentPrecision,
            isExercising = isExercising
        };
    }
}

/// <summary>
/// Données de mouvement reçues des capteurs IMU
/// </summary>
[System.Serializable]
public class MovementData
{
    public float precision;
    public RepPhase phase;
    public List<JointData> joints;
}

[System.Serializable]
public class JointData
{
    public HumanBodyBones bone;
    public Quaternion rotation;
    public float angle;
}

public enum ExerciseType
{
    Squat,
    BenchPress,
    Deadlift,
    ShoulderPress,
    Biceps,
    Triceps
}

public enum RepPhase
{
    Ready,
    Concentric,    // Phase de contraction
    Eccentric      // Phase d'étirement
}

[System.Serializable]
public class ExerciseStats
{
    public int currentRep;
    public int targetReps;
    public int currentSet;
    public int targetSets;
    public float currentPrecision;
    public bool isExercising;
}