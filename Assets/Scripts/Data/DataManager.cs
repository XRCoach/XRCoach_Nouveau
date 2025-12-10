using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public User CurrentUser { get; private set; }
    public Session CurrentSession { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ DataManager initialisé");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadOrCreateUser();
    }
    
    void LoadOrCreateUser()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("❌ SaveManager n'existe pas !");
            return;
        }
        
        CurrentUser = SaveManager.Instance.LoadUser();
        
        if (CurrentUser == null)
        {
            Debug.Log("🆕 Création d'un nouvel utilisateur");
            CurrentUser = new User("Player1");
            SaveManager.Instance.SaveUser(CurrentUser);
        }
        else
        {
            // ⭐ FIX IMPORTANT : Vérifier que history existe après le chargement
            if (CurrentUser.history == null)
            {
                Debug.LogWarning("⚠️ History était null après chargement, création d'une nouvelle liste");
                CurrentUser.history = new List<Session>();
            }
            
            Debug.Log("👋 Bienvenue " + CurrentUser.name + " - Historique : " + CurrentUser.history.Count + " séances");
        }
    }
    
    // Démarrer une nouvelle session
    public void StartSession()
    {
        if (CurrentSession != null)
        {
            Debug.LogWarning("⚠️ Une session est déjà en cours !");
            return;
        }
        
        CurrentSession = new Session();
        Debug.Log("▶️ Nouvelle session démarrée à " + CurrentSession.date.ToString("HH:mm:ss"));
    }
    
    // Ajouter une rep à la session
    public void AddRep(float score)
    {
        if (CurrentSession == null)
        {
            Debug.LogError("❌ ERREUR : Aucune session active ! Appelle StartSession() d'abord.");
            return;
        }
        
        CurrentSession.totalReps++;
        
        // Calculer la moyenne progressive
        if (CurrentSession.totalReps == 1)
        {
            CurrentSession.avgScore = score;
        }
        else
        {
            CurrentSession.avgScore = ((CurrentSession.avgScore * (CurrentSession.totalReps - 1)) + score) / CurrentSession.totalReps;
        }
        
        Debug.Log($"✅ Rep #{CurrentSession.totalReps} enregistrée, Score : {(score * 100):F0}%");
    }
    
    // Terminer la session
    public void EndSession()
    {
        // ⭐ VÉRIFICATIONS COMPLÈTES
        if (CurrentSession == null)
        {
            Debug.LogError("❌ ERREUR : Aucune session active à terminer !");
            return;
        }
        
        if (CurrentUser == null)
        {
            Debug.LogError("❌ ERREUR : CurrentUser est null !");
            return;
        }
        
        if (CurrentUser.history == null)
        {
            Debug.LogWarning("⚠️ History était null, création d'une nouvelle liste");
            CurrentUser.history = new List<Session>();
        }
        
        if (SaveManager.Instance == null)
        {
            Debug.LogError("❌ ERREUR : SaveManager.Instance est null !");
            return;
        }
        
        // Maintenant on peut sauvegarder en toute sécurité
        CurrentUser.history.Add(CurrentSession);
        SaveManager.Instance.SaveUser(CurrentUser);
        
        Debug.Log($"💾 Session sauvegardée ! {CurrentSession.totalReps} reps, Score moyen : {(CurrentSession.avgScore * 100):F0}%");
        
        CurrentSession = null;
    }
    
    // Vérifier si une session est active
    public bool IsSessionActive()
    {
        return CurrentSession != null;
    }
}
