using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Noms des scènes")]
    [Tooltip("Nom exact de la scène du menu principal")]
    public string menuSceneName = "MainMenu";
    [Tooltip("Nom exact de la scène d'exercice")]
    public string exerciseSceneName = "ExerciseScene";

    private void Awake()
    {
        // Singleton + persistance entre scènes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Charge le menu au démarrage
        LoadMenu();
    }

    public void LoadMenu()
    {
        LoadSceneAdditive(menuSceneName);
    }

    public void LoadExercise()
    {
        LoadSceneAdditive(exerciseSceneName);
    }

    public void UnloadExercise()
    {
        if (SceneManager.GetSceneByName(exerciseSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(exerciseSceneName);
        }
    }

    private void LoadSceneAdditive(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Nom de la scène vide ! Vérifiez les champs dans l'Inspector.");
            return;
        }

        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    // Bonus : pour debug, affiche les scènes chargées
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Scènes chargées :");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Debug.Log($"- {SceneManager.GetSceneAt(i).name}");
            }
        }
    }
}