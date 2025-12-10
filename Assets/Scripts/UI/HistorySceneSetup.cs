using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Crée automatiquement la UI pour la HistoryScene
/// À utiliser lors de la première configuration
/// </summary>
public class HistorySceneSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XRCoach/Setup/Create HistoryScene UI")]
    public static void CreateHistoryUI()
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
        
        // === HEADER ===
        GameObject headerGO = new GameObject("Header");
        headerGO.transform.SetParent(canvasRect, false);
        RectTransform headerRect = headerGO.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.offsetMin = new Vector2(0, -80);
        headerRect.offsetMax = new Vector2(0, 0);
        
        Image headerImage = headerGO.AddComponent<Image>();
        headerImage.color = new Color(0.1f, 0.1f, 0.2f);
        
        // Titre
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(headerRect, false);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "📊 HISTORIQUE DES SÉANCES";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // === SCROLL VIEW ===
        GameObject scrollGO = new GameObject("ScrollView");
        scrollGO.transform.SetParent(canvasRect, false);
        RectTransform scrollRect = scrollGO.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.offsetMin = new Vector2(20, 100);
        scrollRect.offsetMax = new Vector2(-20, -20);
        
        ScrollRect scrollComponent = scrollGO.AddComponent<ScrollRect>();
        Image scrollImage = scrollGO.AddComponent<Image>();
        scrollImage.color = new Color(0.05f, 0.05f, 0.1f);
        
        // Content Panel
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(scrollRect.transform, false);
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = new Vector2(0, 0);
        contentRect.offsetMax = new Vector2(0, 0);
        
        VerticalLayoutGroup layoutGroup = contentGO.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        
        ContentSizeFitter fitter = contentGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollComponent.content = contentRect;
        
        // === BOTTOM PANEL ===
        GameObject bottomGO = new GameObject("BottomPanel");
        bottomGO.transform.SetParent(canvasRect, false);
        RectTransform bottomRect = bottomGO.AddComponent<RectTransform>();
        bottomRect.anchorMin = new Vector2(0, 0);
        bottomRect.anchorMax = new Vector2(1, 0);
        bottomRect.offsetMin = new Vector2(0, 0);
        bottomRect.offsetMax = new Vector2(0, 60);
        
        Image bottomImage = bottomGO.AddComponent<Image>();
        bottomImage.color = new Color(0.1f, 0.1f, 0.2f);
        
        // Back Button
        GameObject backButtonGO = new GameObject("BackButton");
        backButtonGO.transform.SetParent(bottomRect, false);
        RectTransform backRect = backButtonGO.AddComponent<RectTransform>();
        backRect.anchoredPosition = new Vector2(-100, 0);
        backRect.sizeDelta = new Vector2(180, 50);
        
        Image backButtonImage = backButtonGO.AddComponent<Image>();
        backButtonImage.color = new Color(0.2f, 0.4f, 0.8f);
        
        Button backButton = backButtonGO.AddComponent<Button>();
        backButton.targetGraphic = backButtonImage;
        
        GameObject backTextGO = new GameObject("Text");
        backTextGO.transform.SetParent(backRect, false);
        TextMeshProUGUI backText = backTextGO.AddComponent<TextMeshProUGUI>();
        backText.text = "← RETOUR";
        backText.fontSize = 24;
        backText.alignment = TextAlignmentOptions.Center;
        RectTransform backTextRect = backTextGO.GetComponent<RectTransform>();
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        
        // === NO DATA TEXT ===
        GameObject noDataGO = new GameObject("NoDataText");
        noDataGO.transform.SetParent(canvasRect, false);
        TextMeshProUGUI noDataText = noDataGO.AddComponent<TextMeshProUGUI>();
        noDataText.text = "📭 Aucune session enregistrée";
        noDataText.fontSize = 32;
        noDataText.alignment = TextAlignmentOptions.Center;
        noDataText.color = Color.yellow;
        RectTransform noDataRect = noDataGO.GetComponent<RectTransform>();
        noDataRect.anchorMin = Vector2.zero;
        noDataRect.anchorMax = Vector2.one;
        noDataGO.SetActive(false);
        
        // === ASSIGNER À HistoryController ===
        GameObject controllerGO = new GameObject("HistoryController");
        controllerGO.transform.SetParent(canvasRect, false);
        
        HistoryController controller = controllerGO.AddComponent<HistoryController>();
        
        // Assigner les références par réflexion (puisque les champs sont privés avec SerializeField)
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("contentPanel").objectReferenceValue = contentRect;
        so.FindProperty("backButton").objectReferenceValue = backButton;
        so.FindProperty("noDataText").objectReferenceValue = noDataText;
        so.ApplyModifiedProperties();
        
        Debug.Log("✅ HistoryScene UI créée avec succès!");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    [MenuItem("XRCoach/Setup/Add DataManager to Scene")]
    public static void AddDataManager()
    {
        if (FindObjectOfType<DataManager>() != null)
        {
            Debug.LogWarning("⚠️ DataManager existe déjà dans la scène");
            return;
        }
        
        GameObject dmGO = new GameObject("DataManager");
        dmGO.AddComponent<DataManager>();
        
        Debug.Log("✅ DataManager ajouté à la scène");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    [MenuItem("XRCoach/Setup/Add SaveManager to Scene")]
    public static void AddSaveManager()
    {
        if (FindObjectOfType<SaveManager>() != null)
        {
            Debug.LogWarning("⚠️ SaveManager existe déjà dans la scène");
            return;
        }
        
        GameObject smGO = new GameObject("SaveManager");
        smGO.AddComponent<SaveManager>();
        
        Debug.Log("✅ SaveManager ajouté à la scène");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
#endif
}
