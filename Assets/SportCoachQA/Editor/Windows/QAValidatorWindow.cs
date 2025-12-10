using UnityEngine;
using UnityEditor;
using SportCoachQA.Validation;
using System.Collections.Generic;

namespace SportCoachQA.Editor.Windows
{
    /// <summary>
    /// Fenêtre Editor pour exécuter et visualiser les tests de validation
    /// </summary>
    public class QAValidatorWindow : EditorWindow
    {
        #region Window Setup
        [MenuItem("SportCoachQA/QA Validator")]
        public static void ShowWindow()
        {
            var window = GetWindow<QAValidatorWindow>("QA Validator");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }
        #endregion

        #region UI State
        private Vector2 scrollPosition;
        private QAValidator validator;
        private GUIStyle headerStyle;
        private GUIStyle passedStyle;
        private GUIStyle failedStyle;
        private bool stylesInitialized = false;
        private bool showDetails = true;
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            FindOrCreateValidator();
        }

        private void OnGUI()
        {
            InitializeStyles();

            EditorGUILayout.Space(10);
            DrawHeader();
            EditorGUILayout.Space(10);

            if (!Application.isPlaying)
            {
                DrawPlayModeWarning();
                return;
            }

            if (validator == null)
            {
                FindOrCreateValidator();
                if (validator == null)
                {
                    EditorGUILayout.HelpBox("QAValidator not found. Enter Play mode to create one.", MessageType.Warning);
                    return;
                }
            }

            DrawControlPanel();
            EditorGUILayout.Space(10);

            DrawStatistics();
            EditorGUILayout.Space(10);

            DrawTestButtons();
            EditorGUILayout.Space(10);

            DrawResults();
        }
        #endregion

        #region Styles
        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };

            passedStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(0.3f, 1f, 0.3f) },
                fontStyle = FontStyle.Bold
            };

            failedStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(1f, 0.3f, 0.3f) },
                fontStyle = FontStyle.Bold
            };

            stylesInitialized = true;
        }
        #endregion

        #region UI Drawing
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("SPORT COACH QA", headerStyle);
            EditorGUILayout.LabelField("Validation & Testing Framework", EditorStyles.centeredGreyMiniLabel);
        }

        private void DrawPlayModeWarning()
        {
            EditorGUILayout.Space(50);
            EditorGUILayout.HelpBox("Press PLAY to start testing", MessageType.Info);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Tests can only run in Play mode", EditorStyles.centeredGreyMiniLabel);
        }

        private void DrawControlPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("🔄 Clear History", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                validator.ClearHistory();
            }

            if (GUILayout.Button("📋 Copy Report", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                GUIUtility.systemCopyBuffer = validator.GenerateReport();
                Debug.Log("[QA] Report copied to clipboard");
            }

            GUILayout.FlexibleSpace();

            showDetails = GUILayout.Toggle(showDetails, "Show Details", EditorStyles.toolbarButton, GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawStatistics()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("📊 TEST STATISTICS", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Total Tests:", GUILayout.Width(120));
            EditorGUILayout.LabelField(validator.TotalTests.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Passed:", GUILayout.Width(120));
            EditorGUILayout.LabelField(validator.PassedTests.ToString(), passedStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Failed:", GUILayout.Width(120));
            EditorGUILayout.LabelField(validator.FailedTests.ToString(), failedStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Success Rate:", GUILayout.Width(120));
            Color rateColor = validator.SuccessRate >= 80f ? Color.green : (validator.SuccessRate >= 50f ? Color.yellow : Color.red);
            GUI.color = rateColor;
            EditorGUILayout.LabelField($"{validator.SuccessRate:F1}%", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawTestButtons()
        {
            EditorGUILayout.LabelField("🧪 QUICK TESTS", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Performance Tests
            if (GUILayout.Button("▶ Run Performance Tests", GUILayout.Height(35)))
            {
                RunPerformanceTests();
            }

            EditorGUILayout.Space(5);

            // System Tests
            if (GUILayout.Button("▶ Run System Validation Tests", GUILayout.Height(35)))
            {
                RunSystemTests();
            }

            EditorGUILayout.Space(5);

            // Custom Test
            if (GUILayout.Button("▶ Run Custom Test Example", GUILayout.Height(35)))
            {
                RunCustomTestExample();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawResults()
        {
            EditorGUILayout.LabelField("📋 TEST RESULTS", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (validator.ValidationHistory.Count == 0)
            {
                EditorGUILayout.HelpBox("No tests run yet. Click a test button above to start.", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);

            for (int i = validator.ValidationHistory.Count - 1; i >= 0; i--)
            {
                var result = validator.ValidationHistory[i];
                DrawTestResult(result);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTestResult(QAValidator.ValidationResult result)
        {
            Color bgColor = result.passed ? new Color(0.2f, 0.6f, 0.2f, 0.3f) : new Color(0.6f, 0.2f, 0.2f, 0.3f);

            GUI.backgroundColor = bgColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            // Header
            EditorGUILayout.BeginHorizontal();

            string icon = result.passed ? "✓" : "✗";
            GUIStyle style = result.passed ? passedStyle : failedStyle;

            EditorGUILayout.LabelField(icon, style, GUILayout.Width(20));
            EditorGUILayout.LabelField(result.testName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{result.executionTime * 1000f:F2}ms", EditorStyles.miniLabel, GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();

            // Details
            if (showDetails)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField(result.message, EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField($"Time: {result.timestamp:HH:mm:ss}", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(3);
        }
        #endregion

        #region Test Implementations
        private void RunPerformanceTests()
        {
            validator.RunTestBatch("Performance Tests", () =>
            {
                // Test FPS
                var perfMonitor = FindObjectOfType<SportCoachQA.Performance.PerformanceMonitor>();
                if (perfMonitor != null)
                {
                    validator.ValidateRange("FPS Check", perfMonitor.CurrentFPS, 30f, 300f);
                    validator.ValidateRange("Memory Check", perfMonitor.CurrentMemoryMB, 0f, 512f);
                    validator.Validate("Performance Monitor Active", perfMonitor.CurrentFPS > 0, "FPS tracking is active");
                }
                else
                {
                    validator.Validate("Performance Monitor", false, "PerformanceMonitor not found in scene");
                }

                // Test Frame Time
                float targetFrameTime = 1f / 60f; // 60 FPS = 16.67ms
                validator.ValidateRange("Frame Time", Time.deltaTime, 0f, targetFrameTime * 2f);
            });
        }

        private void RunSystemTests()
        {
            validator.RunTestBatch("System Validation", () =>
            {
                // Test Unity Systems
                validator.ValidateNotNull("Main Camera", Camera.main, "Main Camera");
                validator.ValidateNotNull("Event System", FindObjectOfType<UnityEngine.EventSystems.EventSystem>(), "Event System");

                // Test Application State
                validator.Validate("Application Running", Application.isPlaying, "Application is in Play mode");
                validator.Validate("Target Frame Rate", Application.targetFrameRate == -1 || Application.targetFrameRate >= 30,
                    $"Target FPS: {Application.targetFrameRate}");

                // Test Platform
                validator.Validate("Platform Check", true, $"Running on {Application.platform}");
            });
        }

        private void RunCustomTestExample()
        {
            validator.RunTestBatch("Custom Test Example", () =>
            {
                // Example: Test GameObject count
                int objectCount = FindObjectsOfType<GameObject>().Length;
                validator.ValidateRange("Scene Object Count", objectCount, 1, 1000);

                // Example: Test Time scale
                validator.Validate("Time Scale Normal", Mathf.Approximately(Time.timeScale, 1f),
                    $"Time scale is {Time.timeScale}");

                // Example: Custom logic test
                validator.RunTest("Custom Logic Test", () =>
                {
                    // Your custom test logic here
                    bool result = (1 + 1 == 2); // Example
                    return result;
                }, "Math works correctly ✓", "Math is broken ✗");
            });
        }
        #endregion

        #region Helper Methods
        private void FindOrCreateValidator()
        {
            validator = FindObjectOfType<QAValidator>();

            if (validator == null && Application.isPlaying)
            {
                GameObject go = new GameObject("[QAValidator]");
                validator = go.AddComponent<QAValidator>();
                Debug.Log("[QA] QAValidator instance created");
            }
        }
        #endregion
    }
}