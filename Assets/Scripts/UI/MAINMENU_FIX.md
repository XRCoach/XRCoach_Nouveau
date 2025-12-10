# 🔧 Diagnostic - MainMenu Buttons Issue

## Le Problème
Tous les boutons mènent au Squat au lieu du bon exercice.

## Causes Possibles

### 1️⃣ **Les boutons ne sont pas assignés au bon script**
**Vérification:**
- Ouvrir MainMenu.unity
- Cliquer sur le bouton Squat
- Vérifier l'Inspector: 
  - Button component → OnClick ()
  - Doit avoir une fonction "StartSquat()" assignée

**Solution:**
Si ce n'est pas assigné:
1. Glisser le GameObject avec MainMenuController sur le slot
2. Sélectionner MainMenuController → StartSquat()

---

### 2️⃣ **Tous les boutons appellent StartSquat()**
**Vérification dans OnClick():**
```
Button Squat: MainMenuController.StartSquat()  ✅
Button Lunge: MainMenuController.StartLunge()  (doit être différent!)
Button Pushup: MainMenuController.StartPushup()
```

**Solution:**
1. Cliquer sur chaque bouton
2. Vérifier sa fonction OnClick
3. Corriger si tous appellent StartSquat()

---

### 3️⃣ **PlayerPrefs ne sauvegarde pas correctement**
**Vérification:**
1. Ajouter MainMenuDebug.cs à un GameObject vide dans MainMenu
2. Play et vérifier les logs
3. Appuyer sur Q (Squat), L (Lunge), P (Pushup) pour tester
4. Vérifier le Debug: "SelectedExercise = X"

---

### 4️⃣ **ExerciseScene ne lit pas la valeur**
**Vérification:**
1. Play, appuyer sur bouton Lunge
2. Vérifier console: "Exercice sélectionné : FENTES (ID: 1)"
3. Si c'est "SQUATS (ID: 0)" → Valeur non sauvegardée

---

## ✅ Checklist de Correction

### Step 1: Inspecter les Boutons
```
MainMenu.unity
├── Canvas
│   └── [Buttons Panel]
│       ├── SquatButton
│       │   └── Button Component
│       │       └── OnClick() → Doit appeler "StartSquat()"
│       ├── LungeButton
│       │   └── Button Component
│       │       └── OnClick() → Doit appeler "StartLunge()"
│       └── PushupButton
│           └── Button Component
│               └── OnClick() → Doit appeler "StartPushup()"
```

### Step 2: Vérifier MainMenuController
```
GameObject avec MainMenuController
├── Doit être dans la scène
└── Publiquement accessible pour les boutons
```

### Step 3: Test rapide
Ajouter ce script à un GameObject vide:
```csharp
void Start()
{
    PlayerPrefs.SetInt("SelectedExercise", 1);
    PlayerPrefs.Save();
}
```
Puis charger ExerciseScene → Devrait afficher "FENTES"

### Step 4: Vérifier les Logs
Play → Ouvrir Console
- ✅ "Lancement: Lunge (ID: 1)" quand on clique Lunge
- ✅ "Exercice sélectionné : FENTES (ID: 1)" en ExerciseScene

---

## 🎯 Solution Rapide (Garantie!)

1. **Télécharger le nouveau MainMenuController**
   - Le code a été mis à jour avec `PlayerPrefs.Save()`
   - Ajoute des logs pour déboguer

2. **Vérifier les Boutons:**
   ```
   Squat Button   → OnClick: StartSquat()
   Lunge Button   → OnClick: StartLunge()
   Pushup Button  → OnClick: StartPushup()
   ```

3. **Ajouter MainMenuDebug.cs:**
   - Créer GameObject vide "Debug"
   - Ajouter MainMenuDebug.cs
   - Assigner les boutons dans l'Inspector
   - Play et vérifier les logs

4. **Tester avec clavier:**
   ```
   Q = Squat
   L = Lunge
   P = Pushup
   H = History
   ```

---

## 🔍 FAQ

**Q: Je clique Lunge mais ça charge Squat?**
A: Les boutons ne sont probablement pas assignés correctement. Vérifier Step 1.

**Q: Comment je sais si c'est sauvegardé?**
A: Ajouter ce log dans ExerciseDisplay:
```csharp
Debug.Log("SelectedExercise value: " + PlayerPrefs.GetInt("SelectedExercise", -1));
```

**Q: Les logs disent "Lancement: Lunge" mais ExerciseScene affiche Squat?**
A: Le problème vient d'ExerciseDisplay ou PlayerPrefs. Vérifier que PlayerPrefs.Save() est appelé.

---

## 📝 Files Modifiés

1. **MainMenuController.cs** ✅
   - Ajout de `PlayerPrefs.Save()`
   - Amélioration des logs
   - Enum pour les exercices

2. **ExerciseDisplay.cs** ✅
   - Meilleure gestion des erreurs
   - Vérification si exerciseText est assigné
   - Logs améliorés

3. **MainMenuDebug.cs** ✨ (NOUVEAU)
   - Aide pour déboguer les boutons
   - Tests au clavier (Q, L, P, H)

---

**Besoin d'aide?** Vérifie la console (Ctrl+Shift+C) pour les messages d'erreur!
