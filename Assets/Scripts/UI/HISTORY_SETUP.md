# ✅ Configuration HistoryScene

## 🚀 Méthode Rapide (Recommandée - 30 secondes)

### Option 1: Automatique (Idéal)
1. Ouvrir HistoryScene.unity
2. Menu: **XRCoach → Setup → Create HistoryScene UI**
3. ✅ UI créée automatiquement!
4. Sauvegarder la scène (Ctrl+S)

### Option 2: Manuelle (Si automatique ne marche pas)
Suivre les étapes ci-dessous

---

## 📋 Configuration Manuelle

### Step 1: Créer la hiérarchie Canvas

Dans HistoryScene.unity, créer:

```
Canvas (RenderMode: Screen Space - Overlay)
├── GraphicRaycaster
├── CanvasScaler
└── [Elements à créer ci-dessous]
```

### Step 2: Ajouter un Header

1. Créer Panel vide "Header"
   - Position: Top (0, 0, 1, 1) avec height 80px
   - Color: Bleu foncé (0.1, 0.1, 0.2)

2. Ajouter TextMeshPro "Title"
   - Text: "📊 HISTORIQUE DES SÉANCES"
   - Font Size: 36
   - Alignment: Center

### Step 3: Créer un Scroll View

1. Menu: GameObject → UI (TextMeshPro) → Scroll View
2. Renommer "HistoryScrollView"
3. Positionner: Top 80px, Bottom 80px
4. Content Panel:
   - Ajouter VerticalLayoutGroup
   - Child Force Expand Height: OFF
   - Child Force Expand Width: ON
   - Spacing: 10

### Step 4: Ajouter le bouton Retour

1. Créer Panel "BottomPanel"
   - Position: Bottom (0, 0, 1, 0) avec height 60px
   - Color: Bleu foncé

2. Ajouter Button "BackButton"
   - Size: 180 x 50
   - Position: Left side
   - Text: "← RETOUR"

### Step 5: Ajouter le texte "Pas de données"

1. Créer TextMeshPro "NoDataText"
   - Text: "📭 Aucune session enregistrée"
   - Font Size: 32
   - Color: Yellow
   - Disabled by default (SetActive false)

### Step 6: Assigner les références

1. Créer GameObject vide "HistoryController"
2. Ajouter le script HistoryController.cs
3. Dans l'Inspector, assigner:
   - contentPanel → Content (du ScrollView)
   - backButton → BackButton
   - noDataText → NoDataText

---

## 🔧 Script RequiredComponents

Assurer que ces GameObjects existent:

```
Scene Managers (DontDestroyOnLoad)
├── SaveManager (SaveManager.cs)
└── DataManager (DataManager.cs)
```

**Si absents, utiliser le menu:**
```
XRCoach → Setup → Add SaveManager to Scene
XRCoach → Setup → Add DataManager to Scene
```

---

## 🧪 Test

### Avant de lancer HistoryScene:

1. Aller dans **MainMenu**
2. Lancer une session d'exercice
3. Revenir au menu
4. Cliquer sur "Historique"
5. ✅ Devrait afficher la session

### Test avec données fictives:

Créer un script de test temporaire:

```csharp
void Start()
{
    if (DataManager.Instance.CurrentUser.history.Count == 0)
    {
        // Ajouter une session de test
        Session test = new Session();
        test.totalReps = 15;
        test.avgScore = 0.85f;
        DataManager.Instance.CurrentUser.history.Add(test);
    }
}
```

---

## 📝 Dépannage

### ❌ Rien ne s'affiche
**Vérifier:**
1. HistoryController est assigné à un GameObject
2. contentPanel est assigné (doit être le "Content" du ScrollView)
3. DataManager.Instance existe (check Console)

### ❌ "Pas de sessions enregistrées" toujours visible
**Solutions:**
1. Vérifier que CurrentUser n'est pas null
   ```csharp
   Debug.Log(DataManager.Instance.CurrentUser?.name);
   ```
2. Ajouter une session test depuis MainMenu
3. Vérifier la sauvegarde fonctionne

### ❌ Le bouton Retour ne marche pas
**Vérifier:**
1. Button component a OnClick listener
2. Listener appelle HistoryController.OnBackPressed()

### ❌ Erreur "Find of type HistoryController..."
**Solution:**
1. Assurer que HistoryController.cs est dans le bon dossier
2. Recompiler (Ctrl+R)

---

## 🎯 Utilisation en jeu

### Flux utilisateur:
```
MainMenu
  ↓ (Bouton "Historique")
HistoryScene
  ↓ (Affiche toutes les séances)
  ├─ 📅 10/12/2025 14:30 - 15 reps - Score: 85%
  ├─ 📅 09/12/2025 15:45 - 12 reps - Score: 78%
  └─ 📅 08/12/2025 10:20 - 18 reps - Score: 92%
  ↓ (Bouton "← RETOUR")
MainMenu
```

---

## 📊 Données affichées

Pour chaque session:
- **Date & Heure:** `dd/MM/yyyy HH:mm`
- **Répétitions:** Nombre total
- **Score:** Moyenne en pourcentage

**Format:**
```
📅 10/12/2025 14:30 - 15 reps - Score: 85.0%
Moyenne: 85.0%
```

---

## ✨ Personnalisation

### Changer les couleurs:

Dans HistorySceneSetup.cs:
```csharp
// Header
headerImage.color = new Color(0.2f, 0.3f, 0.5f); // Bleu plus clair

// Bottom Panel
bottomImage.color = new Color(0.1f, 0.2f, 0.3f);
```

### Ajouter plus d'infos par session:

Dans HistoryController.cs, method CreateSessionItem():
```csharp
text = string.Format(
    "📅 {0:dd/MM/yyyy} | Reps: {1} | Score: {2:F1}% | Durée: {3}min",
    session.date,
    session.totalReps,
    session.avgScore * 100,
    session.durationMinutes // Si propriété existe
);
```

---

**Status:** ✅ Prêt à utiliser!
