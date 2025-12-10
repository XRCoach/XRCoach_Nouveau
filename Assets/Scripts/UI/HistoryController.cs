using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HistoryController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentPanel; // Le "Content" du Scroll View
    [SerializeField] private Button backButton; // Bouton retour
    [SerializeField] private TextMeshProUGUI noDataText; // Texte si pas d'historique
    [SerializeField] private GameObject sessionItemPrefab; // Prefab pour chaque session
    
    void Start()
    {
        Debug.Log("📊 HistoryScene chargée");
        
        // Setup du bouton retour
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackPressed);
        }
        
        // Afficher l'historique
        DisplayHistory();
    }
    
    void DisplayHistory()
    {
        // Vérifier que DataManager existe
        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager non trouvé!");
            ShowNoData("DataManager non trouvé");
            return;
        }
        
        User user = DataManager.Instance.CurrentUser;
        
        // Vérifier que l'utilisateur existe
        if (user == null)
        {
            Debug.Log("❌ Pas d'utilisateur chargé");
            ShowNoData("Pas d'utilisateur chargé");
            return;
        }
        
        // Vérifier que l'historique existe
        if (user.history == null || user.history.Count == 0)
        {
            Debug.Log("📭 Pas de sessions enregistrées");
            ShowNoData("Aucune session enregistrée");
            return;
        }
        
        // Nettoyer les anciens éléments
        if (contentPanel != null)
        {
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("❌ contentPanel n'est pas assigné!");
            ShowNoData("UI non configurée");
            return;
        }
        
        // Masquer le texte "pas de données"
        if (noDataText != null)
        {
            noDataText.gameObject.SetActive(false);
        }
        
        // Afficher chaque session
        int sessionCount = 0;
        foreach (Session session in user.history)
        {
            CreateSessionItem(session);
            sessionCount++;
        }
        
        Debug.Log($"📊 Historique affiché : {sessionCount} séances");
    }
    
    void CreateSessionItem(Session session)
    {
        GameObject sessionObj;
        
        // Utiliser le prefab si disponible, sinon créer dynamiquement
        if (sessionItemPrefab != null)
        {
            sessionObj = Instantiate(sessionItemPrefab, contentPanel);
        }
        else
        {
            sessionObj = new GameObject("SessionItem");
            sessionObj.transform.SetParent(contentPanel, false);
        }
        
        // Créer le texte
        TextMeshProUGUI textComponent = sessionObj.GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            textComponent = sessionObj.AddComponent<TextMeshProUGUI>();
        }
        
        // Formater le texte
        string text = string.Format(
            "📅 {0:dd/MM/yyyy HH:mm} - {1} reps - Score: {2:F1}%\n<size=80%>Moyenne: {3:F1}%</size>",
            session.date,
            session.totalReps,
            session.avgScore * 100,
            session.avgScore * 100
        );
        
        textComponent.text = text;
        textComponent.fontSize = 28;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Left;
        
        // Ajouter un layout si c'est dynamique
        if (sessionItemPrefab == null)
        {
            LayoutElement layout = sessionObj.AddComponent<LayoutElement>();
            layout.preferredHeight = 80;
        }
        
        Debug.Log($"  ✅ Session {session.date:dd/MM} - {session.totalReps} reps - {session.avgScore * 100:F1}%");
    }
    
    void ShowNoData(string message)
    {
        if (noDataText != null)
        {
            noDataText.gameObject.SetActive(true);
            noDataText.text = message;
            noDataText.color = Color.yellow;
        }
        
        Debug.LogWarning("⚠️ " + message);
    }
    
    void OnBackPressed()
    {
        Debug.Log("🔙 Retour au menu principal");
        SceneManager.LoadScene("MainMenu");
    }
}
