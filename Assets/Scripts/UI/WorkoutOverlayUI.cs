using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WorkoutOverlayUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text repCounterText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Image qualityIndicator;
    
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button exitButton;
    
    [Header("Pause Menu")]
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    
    private int currentReps = 0;
    private float sessionTime = 0f;
    private bool isPaused = false;
    
    void Start()
    {
        // Auto-détection des boutons s'ils ne sont pas assignés
        if (pauseButton == null)
            pauseButton = FindButtonByName("PauseButton");
        
        if (exitButton == null)
            exitButton = FindButtonByName("ExitButton");
        
        if (resumeButton == null)
            resumeButton = FindButtonByName("ResumeButton");
        
        if (quitButton == null)
            quitButton = FindButtonByName("QuitButton");
        
        // Auto-détection du CanvasGroup du menu de pause
        if (pauseMenuCanvasGroup == null)
            pauseMenuCanvasGroup = FindObjectOfType<Canvas>()?.transform.Find("PauseMenu")?.GetComponent<CanvasGroup>();
        
        // Setup des boutons
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPausePressed);
            Debug.Log("✅ Pause button connecté");
        }
        else
        {
            Debug.LogWarning("⚠️ Pause button non trouvé!");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitPressed);
            Debug.Log("✅ Exit button connecté");
        }
        else
        {
            Debug.LogWarning("⚠️ Exit button non trouvé!");
        }
        
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumePressed);
            Debug.Log("✅ Resume button connecté");
        }
        else
        {
            Debug.LogWarning("⚠️ Resume button non trouvé!");
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitPressed);
            Debug.Log("✅ Quit button connecté");
        }
        else
        {
            Debug.LogWarning("⚠️ Quit button non trouvé!");
        }
        
        // Masquer le menu de pause au démarrage
        if (pauseMenuCanvasGroup != null)
        {
            pauseMenuCanvasGroup.alpha = 0;
            pauseMenuCanvasGroup.blocksRaycasts = false;
            Debug.Log("✅ Pause menu caché");
        }
        else
        {
            Debug.LogWarning("⚠️ Pause menu CanvasGroup non trouvé!");
        }
        
        Debug.Log("✅ WorkoutOverlayUI initialisée");
    }
    
    /// <summary>
    /// Trouve un bouton par son nom
    /// </summary>
    private Button FindButtonByName(string buttonName)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas non trouvé!");
            return null;
        }
        
        Transform buttonTransform = canvas.transform.Find(buttonName);
        if (buttonTransform == null)
        {
            // Chercher dans les enfants récursivement
            buttonTransform = FindInChildren(canvas.transform, buttonName);
        }
        
        if (buttonTransform == null)
        {
            Debug.LogWarning($"⚠️ {buttonName} non trouvé dans le Canvas!");
            return null;
        }
        
        Button button = buttonTransform.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"❌ {buttonName} n'a pas de composant Button!");
            return null;
        }
        
        return button;
    }
    
    /// <summary>
    /// Cherche un GameObject enfant récursivement
    /// </summary>
    private Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            Transform found = FindInChildren(child, name);
            if (found != null)
                return found;
        }
        
        return null;
    }
    
    void Update()
    {
        // Ne pas mettre à jour le timer si en pause
        if (!isPaused)
        {
            // Mettre à jour le timer chaque frame
            sessionTime += Time.deltaTime;
            
            int minutes = (int)(sessionTime / 60);
            int seconds = (int)(sessionTime % 60);
            
            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        // Touches de clavier pour tester (en plus des boutons)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🔑 Escape pressé → Pause");
            OnPausePressed();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("🔑 Q pressé → Quit");
            OnQuitPressed();
        }
        
        if (isPaused && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🔑 R pressé → Resume");
            OnResumePressed();
        }
    }
    
    /// <summary>
    /// Ajouter une répétition
    /// </summary>
    public void AddRep()
    {
        if (isPaused) return;
        
        currentReps++;
        
        if (repCounterText != null)
            repCounterText.text = "Reps: " + currentReps;
        
        Debug.Log("✅ Rep ajoutée ! Total : " + currentReps);
    }
    
    /// <summary>
    /// Afficher un message de feedback
    /// </summary>
    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            
            // Cacher le message après 2 secondes
            CancelInvoke("HideFeedback");
            Invoke("HideFeedback", 2f);
        }
    }
    
    void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
    
    /// <summary>
    /// Changer la couleur de l'indicateur de qualité
    /// </summary>
    public void UpdateQuality(Color color)
    {
        if (qualityIndicator != null)
            qualityIndicator.color = color;
    }
    
    /// <summary>
    /// Pause la séance
    /// </summary>
    private void OnPausePressed()
    {
        isPaused = !isPaused;
        
        Debug.Log($"🔄 Pause toggled: isPaused = {isPaused}");
        
        if (isPaused)
        {
            Debug.Log("⏸️ Séance EN PAUSE - Time.timeScale = 0");
            Time.timeScale = 0f; // Pause le temps du jeu
            
            // Afficher le menu de pause
            if (pauseMenuCanvasGroup != null)
            {
                pauseMenuCanvasGroup.alpha = 1;
                pauseMenuCanvasGroup.blocksRaycasts = true;
                Debug.Log("✅ Pause menu affiché");
            }
            else
            {
                Debug.LogError("❌ pauseMenuCanvasGroup est null!");
            }
            
            // Changer le texte du bouton Pause
            if (pauseButton != null)
            {
                TextMeshProUGUI pauseButtonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (pauseButtonText != null)
                    pauseButtonText.text = "▶ REPRENDRE";
            }
        }
        else
        {
            Debug.Log("▶️ Séance REPRISE - Time.timeScale = 1");
            Time.timeScale = 1f; // Reprendre
            
            // Masquer le menu de pause
            if (pauseMenuCanvasGroup != null)
            {
                pauseMenuCanvasGroup.alpha = 0;
                pauseMenuCanvasGroup.blocksRaycasts = false;
                Debug.Log("✅ Pause menu caché");
            }
            
            // Changer le texte du bouton Pause
            if (pauseButton != null)
            {
                TextMeshProUGUI pauseButtonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (pauseButtonText != null)
                    pauseButtonText.text = "⏸ PAUSE";
            }
        }
    }
    
    /// <summary>
    /// Reprendre depuis le menu de pause
    /// </summary>
    private void OnResumePressed()
    {
        OnPausePressed(); // Toggle pause
    }
    
    /// <summary>
    /// Bouton exit rapide (coin)
    /// </summary>
    private void OnExitPressed()
    {
        OnQuitPressed();
    }
    
    /// <summary>
    /// Quitter la séance et retourner au menu
    /// </summary>
    private void OnQuitPressed()
    {
        Debug.Log("🔙 Quitter ExerciseScene → Retour au MainMenu");
        
        // S'assurer que le temps est normal
        Time.timeScale = 1f;
        
        // Sauvegarder la session si elle est en cours
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.IsSessionActive())
            {
                Debug.Log("💾 Fermeture de la session active");
                DataManager.Instance.EndSession();
            }
            else
            {
                Debug.Log("ℹ️ Aucune session active à sauvegarder");
            }
        }
        else
        {
            Debug.LogError("❌ DataManager.Instance est null!");
        }
        
        // Retourner au menu principal
        Debug.Log("📍 Chargement de MainMenu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Finir la séance normalement
    /// </summary>
    public void FinishSession()
    {
        Debug.Log("✅ Séance terminée avec succès!");
        
        Time.timeScale = 1f;
        
        if (DataManager.Instance != null && DataManager.Instance.IsSessionActive())
        {
            DataManager.Instance.EndSession();
        }
        
        ShowFeedback("Séance terminée! 🎉", Color.green);
        
        // Attendre 2 secondes puis retourner au menu
        Invoke("ReturnToMenu", 2f);
    }
    
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Obtenir le temps écoulé
    /// </summary>
    public float GetSessionTime()
    {
        return sessionTime;
    }
    
    /// <summary>
    /// Obtenir le nombre de reps actuelles
    /// </summary>
    public int GetCurrentReps()
    {
        return currentReps;
    }
}