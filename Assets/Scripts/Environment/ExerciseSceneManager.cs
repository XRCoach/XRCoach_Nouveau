using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Gère le chargement, la configuration et le cycle de vie des scènes d'exercice
/// </summary>
public class ExerciseSceneManager : MonoBehaviour
{
    public static ExerciseSceneManager Instance { get; private set; }

    [Header("Scene Configuration")]
    [SerializeField] private List<ExerciseSceneData> exerciseScenes;
    [SerializeField] private string currentExerciseID;

    [Header("Environment Settings")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Color ambientColorGood = new Color(0.2f, 0.8f, 0.4f);
    [SerializeField] private Color ambientColorBad = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private float ambientTransitionSpeed = 2f;

    private ExerciseSceneData currentScene;
    private Color targetAmbientColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        targetAmbientColor = RenderSettings.ambientLight;
    }

    private void Update()
    {
        // Transition douce de l'éclairage ambiant
        RenderSettings.ambientLight = Color.Lerp(
            RenderSettings.ambientLight,
            targetAmbientColor,
            Time.deltaTime * ambientTransitionSpeed
        );
    }

    /// <summary>
    /// Charge une scène d'exercice spécifique
    /// </summary>
    public void LoadExerciseScene(string exerciseID)
    {
        ExerciseSceneData sceneData = exerciseScenes.Find(s => s.exerciseID == exerciseID);

        if (sceneData == null)
        {
            Debug.LogError($"Exercise scene with ID '{exerciseID}' not found!");
            return;
        }

        currentExerciseID = exerciseID;
        currentScene = sceneData;

        // Chargement additif de la scène
        SceneManager.LoadScene(sceneData.sceneName, LoadSceneMode.Additive);

        // Configuration de l'environnement
        ConfigureEnvironment(sceneData);
    }

    /// <summary>
    /// Configure l'environnement 3D selon les paramètres de l'exercice
    /// </summary>
    private void ConfigureEnvironment(ExerciseSceneData sceneData)
    {
        // Configuration de l'éclairage
        if (mainLight != null)
        {
            mainLight.intensity = sceneData.lightIntensity;
            mainLight.color = sceneData.lightColor;
        }

        // Configuration de l'ambiance
        RenderSettings.ambientLight = sceneData.ambientColor;
        RenderSettings.fog = sceneData.enableFog;
        if (sceneData.enableFog)
        {
            RenderSettings.fogColor = sceneData.fogColor;
            RenderSettings.fogDensity = sceneData.fogDensity;
        }

        Debug.Log($"Exercise scene '{sceneData.exerciseName}' loaded and configured.");
    }

    /// <summary>
    /// Change la couleur ambiante selon la qualité du mouvement
    /// </summary>
    public void SetAmbientFeedback(bool isGoodPosture)
    {
        targetAmbientColor = isGoodPosture ? ambientColorGood : ambientColorBad;
    }

    /// <summary>
    /// Décharge la scène d'exercice actuelle
    /// </summary>
    public void UnloadCurrentExercise()
    {
        if (currentScene != null)
        {
            SceneManager.UnloadSceneAsync(currentScene.sceneName);
            currentScene = null;
            currentExerciseID = null;
        }
    }

    /// <summary>
    /// Récupère les données de la scène actuelle
    /// </summary>
    public ExerciseSceneData GetCurrentSceneData()
    {
        return currentScene;
    }
}

/// <summary>
/// Données de configuration pour une scène d'exercice
/// </summary>
[System.Serializable]
public class ExerciseSceneData
{
    public string exerciseID;
    public string exerciseName;
    public string sceneName;

    [Header("Lighting")]
    public float lightIntensity = 1.0f;
    public Color lightColor = Color.white;
    public Color ambientColor = new Color(0.4f, 0.4f, 0.4f);

    [Header("Atmosphere")]
    public bool enableFog = false;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.01f;

    [Header("Exercise Specific")]
    public Vector3 avatarStartPosition;
    public Vector3 cameraOffset;
}