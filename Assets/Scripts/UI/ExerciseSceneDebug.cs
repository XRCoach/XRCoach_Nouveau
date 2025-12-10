using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Script de diagnostic pour ExerciseScene
/// Vérifie que tous les éléments UI sont correctement configurés
/// </summary>
public class ExerciseSceneDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========== EXERCISE SCENE DIAGNOSTIC ==========");
        
        // 1. Vérifier EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ EventSystem MANQUANT! Les boutons ne fonctionneront pas.");
            Debug.Log("   → Ajoute un objet vide 'EventSystem' avec le composant 'EventSystem'");
        }
        else
        {
            Debug.Log("✅ EventSystem trouvé");
        }
        
        // 2. Vérifier Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas MANQUANT!");
            return;
        }
        Debug.Log("✅ Canvas trouvé");
        
        // 3. Vérifier GraphicRaycaster
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ GraphicRaycaster MANQUANT sur le Canvas!");
            Debug.Log("   → Ajoute le composant 'GraphicRaycaster' au Canvas");
        }
        else
        {
            Debug.Log("✅ GraphicRaycaster trouvé");
        }
        
        // 4. Vérifier les boutons
        Button[] allButtons = FindObjectsOfType<Button>();
        Debug.Log($"✅ {allButtons.Length} boutons trouvés");
        
        foreach (Button btn in allButtons)
        {
            string buttonName = btn.name;
            bool isInteractable = btn.interactable;
            int listenerCount = btn.onClick.GetPersistentEventCount();
            
            Debug.Log($"   • {buttonName}: interactable={isInteractable}, listeners={listenerCount}");
            
            // Vérifier que le bouton a une Image
            Image img = btn.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogWarning($"      ⚠️ {buttonName} n'a pas d'Image component!");
            }
            
            // Vérifier CanvasGroup des parents
            CanvasGroup cg = btn.GetComponentInParent<CanvasGroup>();
            if (cg != null && !cg.blocksRaycasts)
            {
                Debug.LogWarning($"      ⚠️ {buttonName} parent CanvasGroup a blocksRaycasts=false!");
            }
        }
        
        // 5. Vérifier WorkoutOverlayUI
        WorkoutOverlayUI overlayUI = FindObjectOfType<WorkoutOverlayUI>();
        if (overlayUI == null)
        {
            Debug.LogError("❌ WorkoutOverlayUI MANQUANT!");
        }
        else
        {
            Debug.Log("✅ WorkoutOverlayUI trouvé");
        }
        
        // 6. Vérifier PauseMenu
        Canvas pauseMenuCanvas = FindObjectOfType<Canvas>()?.transform.Find("PauseMenu")?.GetComponent<Canvas>();
        CanvasGroup pauseMenuCG = FindObjectOfType<Canvas>()?.transform.Find("PauseMenu")?.GetComponent<CanvasGroup>();
        
        if (pauseMenuCG == null)
        {
            Debug.LogWarning("⚠️ PauseMenu CanvasGroup non trouvé - auto-détection va créer une référence");
        }
        else
        {
            Debug.Log("✅ PauseMenu CanvasGroup trouvé");
        }
        
        Debug.Log("============================================\n");
        Debug.Log("💡 Si des éléments sont manquants, utilise:");
        Debug.Log("   XRCoach → Setup → Create ExerciseScene UI\n");
    }
}
