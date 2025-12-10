using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script de débogage pour la MainMenu
/// À ajouter à un GameObject vide dans la MainMenu pour tester
/// </summary>
public class MainMenuDebug : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button squatBtn;
    [SerializeField] private Button lungeBtn;
    [SerializeField] private Button pushupBtn;
    [SerializeField] private Button historyBtn;
    
    void Start()
    {
        Debug.Log("=== MAIN MENU DEBUG ===");
        
        // Vérifier que les boutons sont assignés
        if (squatBtn == null) Debug.LogError("❌ squatBtn non assigné!");
        else Debug.Log("✅ squatBtn trouvé");
        
        if (lungeBtn == null) Debug.LogError("❌ lungeBtn non assigné!");
        else Debug.Log("✅ lungeBtn trouvé");
        
        if (pushupBtn == null) Debug.LogError("❌ pushupBtn non assigné!");
        else Debug.Log("✅ pushupBtn trouvé");
        
        if (historyBtn == null) Debug.LogError("❌ historyBtn non assigné!");
        else Debug.Log("✅ historyBtn trouvé");
        
        // Vérifier que MainMenuController existe
        MainMenuController controller = GetComponent<MainMenuController>();
        if (controller == null)
        {
            controller = FindObjectOfType<MainMenuController>();
        }
        
        if (controller == null)
        {
            Debug.LogError("❌ MainMenuController non trouvé!");
        }
        else
        {
            Debug.Log("✅ MainMenuController trouvé");
        }
        
        // Afficher les listeners des boutons
        Debug.Log("\n=== BUTTON LISTENERS ===");
        if (squatBtn != null)
            Debug.Log($"squatBtn listeners: {squatBtn.onClick.GetPersistentEventCount()}");
        if (lungeBtn != null)
            Debug.Log($"lungeBtn listeners: {lungeBtn.onClick.GetPersistentEventCount()}");
        if (pushupBtn != null)
            Debug.Log($"pushupBtn listeners: {pushupBtn.onClick.GetPersistentEventCount()}");
    }
    
    void Update()
    {
        // Debug avec touches clavier
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("🏋️ TEST: Squat");
            MainMenuController controller = FindObjectOfType<MainMenuController>();
            if (controller != null) controller.StartSquat();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("🏃 TEST: Lunge");
            MainMenuController controller = FindObjectOfType<MainMenuController>();
            if (controller != null) controller.StartLunge();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("💪 TEST: Pushup");
            MainMenuController controller = FindObjectOfType<MainMenuController>();
            if (controller != null) controller.StartPushup();
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("📊 TEST: History");
            MainMenuController controller = FindObjectOfType<MainMenuController>();
            if (controller != null) controller.OpenHistory();
        }
    }
}
