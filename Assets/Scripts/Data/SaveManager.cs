using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath;
    
    void Awake()
    {
        // Singleton (une seule copie du manager)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Chemin du fichier de sauvegarde
        saveFilePath = Path.Combine(Application.persistentDataPath, "userdata.json");
        Debug.Log("Fichier de sauvegarde : " + saveFilePath);
    }
    
    // Sauvegarder l'utilisateur
    public void SaveUser(User user)
    {
        string json = JsonUtility.ToJson(user, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("✅ Utilisateur sauvegardé !");
    }
    
    // Charger l'utilisateur
    public User LoadUser()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            User user = JsonUtility.FromJson<User>(json);
            Debug.Log("✅ Utilisateur chargé : " + user.name);
            return user;
        }
        else
        {
            Debug.Log("❌ Pas de sauvegarde trouvée");
            return null;
        }
    }
}