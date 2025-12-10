using UnityEngine;

public class TestSave : MonoBehaviour
{
    public void OnSaveButtonClick()
    {
        // Créer un utilisateur
        User newUser = new User("TestPlayer");
        newUser.level = 5;
        
        // Sauvegarder
        SaveManager.Instance.SaveUser(newUser);
        Debug.Log("👤 Utilisateur sauvegardé : " + newUser.name);
    }
    
    public void OnLoadButtonClick()
    {
        // Charger l'utilisateur
        User loadedUser = SaveManager.Instance.LoadUser();
        
        if (loadedUser != null)
        {
            Debug.Log("👤 Utilisateur chargé : " + loadedUser.name);
            Debug.Log("🎯 Niveau : " + loadedUser.level);
        }
    }
}