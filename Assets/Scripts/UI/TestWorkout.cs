using UnityEngine;

public class TestWorkout : MonoBehaviour
{
    public WorkoutOverlayUI overlayUI;
    
    void Start()
    {
        // ⭐ IMPORTANT : Démarrer une session dès le début
        DataManager.Instance.StartSession();
        Debug.Log("🏋️ Session d'entraînement démarrée");
    }
    
    // Appelé par le bouton "Simuler Rep"
    public void SimulateRep()
    {
        overlayUI.AddRep();
        
        float randomScore = Random.Range(0f, 1f);
        
        // Enregistrer dans DataManager
        DataManager.Instance.AddRep(randomScore);
        
        Color color;
        string message;
        
        if (randomScore >= 0.8f)
        {
            color = Color.green;
            message = "Parfait ! ✅";
        }
        else if (randomScore >= 0.6f)
        {
            color = Color.yellow;
            message = "Bien, améliore la posture ⚠️";
        }
        else
        {
            color = Color.red;
            message = "Attention à la posture ! ❌";
        }
        
        overlayUI.ShowFeedback(message, color);
        overlayUI.UpdateQuality(color);
        
        Debug.Log("💯 Score : " + (randomScore * 100).ToString("F0") + "%");
    }
    
    // Appelé par le bouton "Terminer"
    public void EndWorkout()
    {
        DataManager.Instance.EndSession();
        Debug.Log("🏁 Séance terminée et sauvegardée !");
        
        // Retourner au menu principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}