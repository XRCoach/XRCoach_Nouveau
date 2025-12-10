using UnityEngine;
using UnityEditor;
using SportCoachQA.Performance;

namespace SportCoachQA.Editor.Windows
{
    /// <summary>
    /// Fenêtre Editor pour monitorer la performance en temps réel
    /// </summary>
    public class PerformanceMonitorWindow : EditorWindow
    {
        #region Window Setup
        [MenuItem("SportCoachQA/Performance Monitor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PerformanceMonitorWindow>("Perf Monitor");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        #endregion

        #region UI State
        private bool isMonitoring = false;
        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle criticalStyle;
        private GUIStyle normalStyle;
        private bool stylesInitialized = false;
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        private void OnGUI()
        {
            InitializeStyles();

            EditorGUILayout.Space(10);
            DrawHeader();
            EditorGUILayout.Space(10);

            DrawControlButtons();
            EditorGUILayout.Space(10);

            if (isMonitoring && Application.isPlaying)
            {
                var monitor = PerformanceMonitor.Instance;
                if (monitor != null)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    DrawMetrics();
                    EditorGUILayout.Space(10);
                    DrawAlerts();
                    EditorGUILayout.Space(10);
                    DrawReport();
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    DrawInactiveState();
                }
            }
            else
            {
                DrawInactiveState();
            }
        }
        #endregion

        #region UI Styles
        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };

            criticalStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(1f, 0.3f, 0.3f) },
                fontStyle = FontStyle.Bold
            };

            normalStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(0.3f, 1f, 0.3f) }
            };

            stylesInitialized = true;
        }
        #endregion

        #region UI Drawing
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("SPORT COACH QA", headerStyle);
            EditorGUILayout.LabelField("Performance Monitor", EditorStyles.centeredGreyMiniLabel);
        }

        private void DrawControlButtons()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            // Start/Stop Monitoring
            GUI.backgroundColor = isMonitoring ? Color.red : Color.green;

            string buttonText = isMonitoring ? "⏹ Stop Monitoring" : "▶ Start Monitoring";

            if (GUILayout.Button(buttonText, GUILayout.Height(40), GUILayout.Width(200)))
            {
                ToggleMonitoring();
            }

            GUI.backgroundColor = Color.white;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            // Reset button (only when monitoring AND in play mode AND instance exists)
            if (isMonitoring && Application.isPlaying)
            {
                var monitor = PerformanceMonitor.Instance;
                if (monitor != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("🔄 Reset Stats", GUILayout.Width(150)))
                    {
                        monitor.ResetStats();
                        Debug.Log("[QA] Performance stats reset");
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawMetrics()
        {
            var monitor = PerformanceMonitor.Instance;
            if (monitor == null) return;

            // FPS Section
            DrawSection("📊 FPS METRICS");
            DrawMetricRow("Current FPS:", $"{monitor.CurrentFPS:F1}", monitor.CurrentFPS);
            DrawMetricRow("Average FPS:", $"{monitor.AverageFPS:F1}", monitor.AverageFPS);
            DrawMetricRow("Min FPS:", $"{monitor.MinFPS:F1}", monitor.MinFPS);
            DrawMetricRow("Max FPS:", $"{monitor.MaxFPS:F1}", monitor.MaxFPS);

            EditorGUILayout.Space(10);

            // Memory Section
            DrawSection("💾 MEMORY METRICS");
            DrawMemoryRow("Current Memory:", $"{monitor.CurrentMemoryMB:F2} MB", monitor.CurrentMemoryMB);
            DrawMemoryRow("Peak Memory:", $"{monitor.PeakMemoryMB:F2} MB", monitor.PeakMemoryMB);
        }

        private void DrawAlerts()
        {
            var monitor = PerformanceMonitor.Instance;
            if (monitor == null) return;

            DrawSection("⚠️ ALERTS");

            if (monitor.IsCriticalPerformance)
            {
                EditorGUILayout.HelpBox(
                    $"CRITICAL FPS: {monitor.CurrentFPS:F1} FPS (Target: 60 FPS)",
                    MessageType.Error
                );
            }

            if (monitor.IsCriticalMemory)
            {
                EditorGUILayout.HelpBox(
                    $"CRITICAL MEMORY: {monitor.CurrentMemoryMB:F2} MB (Threshold: 512 MB)",
                    MessageType.Warning
                );
            }

            if (!monitor.IsCriticalPerformance && !monitor.IsCriticalMemory)
            {
                EditorGUILayout.HelpBox("All systems nominal ✓", MessageType.Info);
            }
        }

        private void DrawReport()
        {
            var monitor = PerformanceMonitor.Instance;
            if (monitor == null) return;

            DrawSection("📋 FULL REPORT");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.TextArea(monitor.GenerateReport(), GUILayout.Height(100));
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("📋 Copy Report to Clipboard", GUILayout.Height(30)))
            {
                GUIUtility.systemCopyBuffer = monitor.GenerateReport();
                Debug.Log("[QA] Report copied to clipboard");
            }
        }

        private void DrawInactiveState()
        {
            EditorGUILayout.Space(50);

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("🎮 Press PLAY to start monitoring",
                    EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Performance data is only available in Play mode",
                    EditorStyles.centeredGreyMiniLabel);
            }
            else if (!isMonitoring)
            {
                EditorGUILayout.LabelField("Click 'Start Monitoring' to begin",
                    EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void DrawSection(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawMetricRow(string label, string value, float fps)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(150));

            GUIStyle style = fps < 30f ? criticalStyle : normalStyle;
            EditorGUILayout.LabelField(value, style);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMemoryRow(string label, string value, float memoryMB)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(150));

            GUIStyle style = memoryMB > 512f ? criticalStyle : normalStyle;
            EditorGUILayout.LabelField(value, style);

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Control Logic
        private void ToggleMonitoring()
        {
            isMonitoring = !isMonitoring;

            if (isMonitoring)
            {
                if (Application.isPlaying)
                {
                    // En Play mode, force la création de l'instance
                    var instance = PerformanceMonitor.Instance;
                    Debug.Log("[QA] Performance monitoring started (Play mode)");
                }
                else
                {
                    // En Edit mode, avertissement
                    Debug.LogWarning("[QA] Monitoring enabled. Press PLAY to see live data.");
                }
            }
            else
            {
                Debug.Log("[QA] Performance monitoring stopped");
            }
        }
        #endregion
    }
}