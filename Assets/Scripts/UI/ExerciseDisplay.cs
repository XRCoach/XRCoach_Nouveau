using UnityEngine;
using TMPro;

public class ExerciseDisplay : MonoBehaviour
{
    public TMP_Text exerciseText;
    
    void Start()
    {
        // Récupérer l'exercice choisi
        int exerciseType = PlayerPrefs.GetInt("SelectedExercise", 0);
        
        string exerciseName = "";
        switch(exerciseType)
        {
            case 0: 
                exerciseName = "SQUATS"; 
                break;
            case 1: 
                exerciseName = "FENTES"; 
                break;
            case 2: 
                exerciseName = "POMPES"; 
                break;
            default:
                exerciseName = "UNKNOWN";
                Debug.LogWarning("⚠️ Type d'exercice inconnu: " + exerciseType);
                break;
        }
        
        if (exerciseText != null)
        {
            exerciseText.text = "Exercice : " + exerciseName;
        }
        else
        {
            Debug.LogError("❌ ExerciseDisplay: exerciseText n'est pas assigné!");
        }
        
        Debug.Log($"📋 Exercice sélectionné : {exerciseName} (ID: {exerciseType})");
        
        // Démarrer une session
        if (DataManager.Instance != null)
        {
            DataManager.Instance.StartSession();
            Debug.Log("✅ Nouvelle session démarrée");
        }
        else
        {
            Debug.LogError("❌ DataManager non trouvé!");
        }
    }
}