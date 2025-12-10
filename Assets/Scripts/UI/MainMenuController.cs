using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Enum pour les exercices
    public enum ExerciseType
    {
        Squat = 0,
        Lunge = 1,
        Pushup = 2
    }
    
    // Méthode générique pour lancer un exercice
    private void StartExercise(int exerciseType)
    {
        PlayerPrefs.SetInt("SelectedExercise", exerciseType);
        PlayerPrefs.Save(); // ⭐ IMPORTANT: Sauvegarder immédiatement
        
        string exerciseName = ((ExerciseType)exerciseType).ToString();
        Debug.Log($"🏋️ Lancement: {exerciseName} (ID: {exerciseType})");
        
        SceneManager.LoadScene("ExerciseScene");
    }
    
    // Appelé quand on clique sur le bouton Squat
    public void StartSquat()
    {
        StartExercise((int)ExerciseType.Squat);
    }
    
    // Appelé quand on clique sur le bouton Lunge
    public void StartLunge()
    {
        StartExercise((int)ExerciseType.Lunge);
    }
    
    // Appelé quand on clique sur le bouton Pushup
    public void StartPushup()
    {
        StartExercise((int)ExerciseType.Pushup);
    }
    
    // Ouvrir l'historique
    public void OpenHistory()
    {
        Debug.Log("📊 Ouverture de l'historique");
        SceneManager.LoadScene("HistoryScene");
    }
    
    // Debug: Afficher la valeur sauvegardée
    public void DebugSelectedExercise()
    {
        int selected = PlayerPrefs.GetInt("SelectedExercise", -1);
        Debug.Log($"DEBUG: SelectedExercise = {selected}");
    }
}