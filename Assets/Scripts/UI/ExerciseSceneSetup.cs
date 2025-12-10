using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Crée automatiquement les boutons Pause/Exit pour ExerciseScene
/// </summary>
public class ExerciseSceneSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XRCoach/Setup/Create ExerciseScene UI")]
    public static void CreateExerciseUI()
    {
        // Trouver ou créer le Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            
            GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // === TOP RIGHT BUTTONS ===
        GameObject buttonPanelGO = new GameObject("ButtonPanel");
        buttonPanelGO.transform.SetParent(canvasRect, false);
        RectTransform buttonPanelRect = buttonPanelGO.AddComponent<RectTransform>();
        buttonPanelRect.anchorMin = new Vector2(1, 1);
        buttonPanelRect.anchorMax = new Vector2(1, 1);
        buttonPanelRect.pivot = new Vector2(1, 1);
        buttonPanelRect.anchoredPosition = new Vector2(-20, -20);
        buttonPanelRect.sizeDelta = new Vector2(200, 120);
        
        // Pause Button
        GameObject pauseButtonGO = new GameObject("PauseButton");
        pauseButtonGO.transform.SetParent(buttonPanelRect, false);
        RectTransform pauseRect = pauseButtonGO.AddComponent<RectTransform>();
        pauseRect.anchorMin = Vector2.zero;
        pauseRect.anchorMax = new Vector2(1, 0.5f);
        pauseRect.offsetMin = Vector2.zero;
        pauseRect.offsetMax = new Vector2(0, -5);
        
        Image pauseButtonImage = pauseButtonGO.AddComponent<Image>();
        pauseButtonImage.color = new Color(1f, 0.8f, 0f); // Orange
        
        Button pauseButton = pauseButtonGO.AddComponent<Button>();
        pauseButton.targetGraphic = pauseButtonImage;
        
        GameObject pauseTextGO = new GameObject("Text");
        pauseTextGO.transform.SetParent(pauseRect, false);
        TextMeshProUGUI pauseText = pauseTextGO.AddComponent<TextMeshProUGUI>();
        pauseText.text = "⏸ PAUSE";
        pauseText.fontSize = 20;
        pauseText.alignment = TextAlignmentOptions.Center;
        RectTransform pauseTextRect = pauseTextGO.GetComponent<RectTransform>();
        pauseTextRect.anchorMin = Vector2.zero;
        pauseTextRect.anchorMax = Vector2.one;
        pauseTextRect.offsetMin = Vector2.zero;
        pauseTextRect.offsetMax = Vector2.zero;
        
        // Exit Button
        GameObject exitButtonGO = new GameObject("ExitButton");
        exitButtonGO.transform.SetParent(buttonPanelRect, false);
        RectTransform exitRect = exitButtonGO.AddComponent<RectTransform>();
        exitRect.anchorMin = new Vector2(0, 0.5f);
        exitRect.anchorMax = Vector2.one;
        exitRect.offsetMin = new Vector2(0, 5);
        exitRect.offsetMax = Vector2.zero;
        
        Image exitButtonImage = exitButtonGO.AddComponent<Image>();
        exitButtonImage.color = new Color(1f, 0.2f, 0.2f); // Rouge
        
        Button exitButton = exitButtonGO.AddComponent<Button>();
        exitButton.targetGraphic = exitButtonImage;
        
        GameObject exitTextGO = new GameObject("Text");
        exitTextGO.transform.SetParent(exitRect, false);
        TextMeshProUGUI exitText = exitTextGO.AddComponent<TextMeshProUGUI>();
        exitText.text = "🔙 EXIT";
        exitText.fontSize = 20;
        exitText.alignment = TextAlignmentOptions.Center;
        RectTransform exitTextRect = exitTextGO.GetComponent<RectTransform>();
        exitTextRect.anchorMin = Vector2.zero;
        exitTextRect.anchorMax = Vector2.one;
        exitTextRect.offsetMin = Vector2.zero;
        exitTextRect.offsetMax = Vector2.zero;
        
        // === PAUSE MENU (Center) ===
        GameObject pauseMenuGO = new GameObject("PauseMenu");
        pauseMenuGO.transform.SetParent(canvasRect, false);
        
        Image pauseMenuImage = pauseMenuGO.AddComponent<Image>();
        pauseMenuImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        CanvasGroup pauseMenuCanvasGroup = pauseMenuGO.AddComponent<CanvasGroup>();
        pauseMenuCanvasGroup.alpha = 0;
        pauseMenuCanvasGroup.blocksRaycasts = false;
        
        RectTransform pauseMenuRect = pauseMenuGO.GetComponent<RectTransform>();
        pauseMenuRect.anchorMin = Vector2.zero;
        pauseMenuRect.anchorMax = Vector2.one;
        pauseMenuRect.offsetMin = Vector2.zero;
        pauseMenuRect.offsetMax = Vector2.zero;
        
        // Panel intérieur du menu de pause
        GameObject pauseContentGO = new GameObject("Content");
        pauseContentGO.transform.SetParent(pauseMenuRect, false);
        
        Image pauseContentImage = pauseContentGO.AddComponent<Image>();
        pauseContentImage.color = new Color(0.1f, 0.1f, 0.2f); // Bleu foncé
        
        RectTransform pauseContentRect = pauseContentGO.GetComponent<RectTransform>();
        pauseContentRect.sizeDelta = new Vector2(400, 300);
        pauseContentRect.anchoredPosition = Vector2.zero;
        
        // Titre du menu
        GameObject pauseTitleGO = new GameObject("Title");
        pauseTitleGO.transform.SetParent(pauseContentRect, false);
        TextMeshProUGUI pauseTitleText = pauseTitleGO.AddComponent<TextMeshProUGUI>();
        pauseTitleText.text = "⏸ EN PAUSE";
        pauseTitleText.fontSize = 40;
        pauseTitleText.alignment = TextAlignmentOptions.Center;
        RectTransform pauseTitleRect = pauseTitleGO.GetComponent<RectTransform>();
        pauseTitleRect.anchorMin = new Vector2(0, 0.7f);
        pauseTitleRect.anchorMax = new Vector2(1, 1);
        pauseTitleRect.offsetMin = Vector2.zero;
        pauseTitleRect.offsetMax = Vector2.zero;
        
        // Resume Button (dans le menu)
        GameObject resumeButtonGO = new GameObject("ResumeButton");
        resumeButtonGO.transform.SetParent(pauseContentRect, false);
        RectTransform resumeRect = resumeButtonGO.AddComponent<RectTransform>();
        resumeRect.anchorMin = new Vector2(0.1f, 0.4f);
        resumeRect.anchorMax = new Vector2(0.9f, 0.6f);
        resumeRect.offsetMin = Vector2.zero;
        resumeRect.offsetMax = Vector2.zero;
        
        Image resumeButtonImage = resumeButtonGO.AddComponent<Image>();
        resumeButtonImage.color = new Color(0.2f, 0.8f, 0.2f); // Vert
        
        Button resumeButton = resumeButtonGO.AddComponent<Button>();
        resumeButton.targetGraphic = resumeButtonImage;
        
        GameObject resumeTextGO = new GameObject("Text");
        resumeTextGO.transform.SetParent(resumeRect, false);
        TextMeshProUGUI resumeText = resumeTextGO.AddComponent<TextMeshProUGUI>();
        resumeText.text = "▶ REPRENDRE";
        resumeText.fontSize = 28;
        resumeText.alignment = TextAlignmentOptions.Center;
        RectTransform resumeTextRect = resumeTextGO.GetComponent<RectTransform>();
        resumeTextRect.anchorMin = Vector2.zero;
        resumeTextRect.anchorMax = Vector2.one;
        resumeTextRect.offsetMin = Vector2.zero;
        resumeTextRect.offsetMax = Vector2.zero;
        
        // Quit Button (dans le menu)
        GameObject quitMenuButtonGO = new GameObject("QuitButton");
        quitMenuButtonGO.transform.SetParent(pauseContentRect, false);
        RectTransform quitMenuRect = quitMenuButtonGO.AddComponent<RectTransform>();
        quitMenuRect.anchorMin = new Vector2(0.1f, 0.1f);
        quitMenuRect.anchorMax = new Vector2(0.9f, 0.3f);
        quitMenuRect.offsetMin = Vector2.zero;
        quitMenuRect.offsetMax = Vector2.zero;
        
        Image quitMenuButtonImage = quitMenuButtonGO.AddComponent<Image>();
        quitMenuButtonImage.color = new Color(1f, 0.2f, 0.2f); // Rouge
        
        Button quitMenuButton = quitMenuButtonGO.AddComponent<Button>();
        quitMenuButton.targetGraphic = quitMenuButtonImage;
        
        GameObject quitMenuTextGO = new GameObject("Text");
        quitMenuTextGO.transform.SetParent(quitMenuRect, false);
        TextMeshProUGUI quitMenuText = quitMenuTextGO.AddComponent<TextMeshProUGUI>();
        quitMenuText.text = "🔙 QUITTER";
        quitMenuText.fontSize = 28;
        quitMenuText.alignment = TextAlignmentOptions.Center;
        RectTransform quitMenuTextRect = quitMenuTextGO.GetComponent<RectTransform>();
        quitMenuTextRect.anchorMin = Vector2.zero;
        quitMenuTextRect.anchorMax = Vector2.one;
        quitMenuTextRect.offsetMin = Vector2.zero;
        quitMenuTextRect.offsetMax = Vector2.zero;
        
        // === ASSIGNER À WorkoutOverlayUI ===
        GameObject overlayGO = new GameObject("WorkoutOverlayUI");
        overlayGO.transform.SetParent(canvasRect, false);
        
        WorkoutOverlayUI overlay = overlayGO.AddComponent<WorkoutOverlayUI>();
        
        // Assigner les références
        SerializedObject so = new SerializedObject(overlay);
        so.FindProperty("pauseButton").objectReferenceValue = pauseButton;
        so.FindProperty("exitButton").objectReferenceValue = exitButton;
        so.FindProperty("pauseMenuCanvasGroup").objectReferenceValue = pauseMenuCanvasGroup;
        so.FindProperty("resumeButton").objectReferenceValue = resumeButton;
        so.FindProperty("quitButton").objectReferenceValue = quitMenuButton;
        so.ApplyModifiedProperties();
        
        Debug.Log("✅ ExerciseScene UI créée avec succès!");
        Debug.Log("   - Bouton Pause (haut-droit, orange)");
        Debug.Log("   - Bouton Exit (bas-droit, rouge)");
        Debug.Log("   - Menu Pause (centre)");
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
#endif
}
