 
 =================================================================
SPORT COACH IMU - QA TOOLKIT MODULE
Version 1.0.0
Developed by: Membre 5 (QA, Tests & Validation)
=================================================================

DESCRIPTION
-----------
Module de QA professionnel pour le projet Sport Coach IMU.
Contient des outils de monitoring de performance et de validation
automatique pour tests mobile.

CONTENU DU PACKAGE
------------------
✓ Performance Monitor - Tracking FPS/Memory en temps réel
✓ QA Validator - Framework de tests automatisés
✓ Fenêtres Editor custom - Interfaces intuitives
✓ Assembly Definitions - Code modulaire et isolé

INSTALLATION
------------
1. Importer le package via Assets > Import Package > Custom Package
2. Sélectionner SportCoachIMU_QAToolkit.unitypackage
3. Cliquer sur "Import"
4. Vérifier qu'aucune erreur n'apparaît dans la Console

UTILISATION
-----------

Performance Monitor:
- Menu: SportCoachQA > Performance Monitor
- Appuyer sur Play
- Cliquer sur "Start Monitoring"
- Observer les métriques en temps réel

QA Validator:
- Menu: SportCoachQA > QA Validator
- Appuyer sur Play
- Cliquer sur un bouton de test
- Consulter les résultats

EXTENSION
---------
Pour ajouter vos propres tests:
1. Ouvrir QAValidatorWindow.cs
2. Créer une nouvelle méthode RunYourCustomTests()
3. Ajouter un bouton dans DrawTestButtons()

SUPPORT
-------
Namespace: SportCoachQA.Performance, SportCoachQA.Validation
Assemblies: SportCoachQA.Runtime, SportCoachQA.Editor
Compatibilité: Unity 2022.3 LTS+

NOTES IMPORTANTES
-----------------
- Les outils fonctionnent uniquement en Play mode
- Performance Monitor persiste entre les scènes
- Les tests s'exécutent de manière asynchrone
- Tous les rapports sont copiables dans le clipboard

=================================================================