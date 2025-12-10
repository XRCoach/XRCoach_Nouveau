using UnityEngine;
using System.Collections.Generic;

namespace SportCoachQA.Performance
{
    /// <summary>
    /// Moniteur de performance temps réel pour mobile (IMU optimisé)
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        #region Singleton Pattern
        private static PerformanceMonitor _instance;
        public static PerformanceMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    // En mode Editor, cherche d'abord une instance existante
                    _instance = FindObjectOfType<PerformanceMonitor>();

                    // Si aucune n'existe, crée-en une
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("[PerformanceMonitor]");
                        _instance = go.AddComponent<PerformanceMonitor>();

                        // DontDestroyOnLoad SEULEMENT en Play mode
                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Configuration
        [Header("Sampling Settings")]
        [SerializeField] private float updateInterval = 0.5f; // Update every 0.5s
        [SerializeField] private int sampleCount = 60; // Keep 60 samples (~30s history)

        [Header("Performance Thresholds")]
        [SerializeField] private float targetFPS = 60f;
        [SerializeField] private float criticalFPSThreshold = 30f;
        [SerializeField] private long criticalMemoryMB = 512; // 512 MB warning
        #endregion

        #region Performance Data
        private Queue<float> fpsHistory = new Queue<float>();
        private Queue<float> memoryHistory = new Queue<float>();

        private float currentFPS;
        private float averageFPS;
        private float minFPS = 999f;
        private float maxFPS = 0f;

        private float currentMemoryMB;
        private float peakMemoryMB;

        private float accumulatedTime;
        private int frameCount;
        #endregion

        #region Public Accessors
        public float CurrentFPS => currentFPS;
        public float AverageFPS => averageFPS;
        public float MinFPS => minFPS;
        public float MaxFPS => maxFPS;
        public float CurrentMemoryMB => currentMemoryMB;
        public float PeakMemoryMB => peakMemoryMB;
        public bool IsCriticalPerformance => currentFPS < criticalFPSThreshold;
        public bool IsCriticalMemory => currentMemoryMB > criticalMemoryMB;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            // DontDestroyOnLoad SEULEMENT en Play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            // Seulement actif en Play mode
            if (!Application.isPlaying) return;

            // Accumulate frame time
            accumulatedTime += Time.unscaledDeltaTime;
            frameCount++;

            // Update metrics at defined interval
            if (accumulatedTime >= updateInterval)
            {
                UpdateMetrics();
                accumulatedTime = 0f;
                frameCount = 0;
            }
        }
        #endregion

        #region Metrics Calculation
        private void UpdateMetrics()
        {
            // Calculate FPS
            currentFPS = frameCount / updateInterval;
            UpdateFPSStats();

            // Calculate Memory
            currentMemoryMB = (System.GC.GetTotalMemory(false) / 1024f / 1024f);
            UpdateMemoryStats();

            // Store history
            StoreHistory();
        }

        private void UpdateFPSStats()
        {
            // Update min/max seulement si FPS valide
            if (currentFPS > 0)
            {
                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;
            }

            // Calculate average from history
            float sum = 0f;
            foreach (float fps in fpsHistory)
                sum += fps;

            if (fpsHistory.Count > 0)
                averageFPS = sum / fpsHistory.Count;
        }

        private void UpdateMemoryStats()
        {
            if (currentMemoryMB > peakMemoryMB)
                peakMemoryMB = currentMemoryMB;
        }

        private void StoreHistory()
        {
            // Store FPS
            fpsHistory.Enqueue(currentFPS);
            if (fpsHistory.Count > sampleCount)
                fpsHistory.Dequeue();

            // Store Memory
            memoryHistory.Enqueue(currentMemoryMB);
            if (memoryHistory.Count > sampleCount)
                memoryHistory.Dequeue();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset all statistics
        /// </summary>
        public void ResetStats()
        {
            fpsHistory.Clear();
            memoryHistory.Clear();
            minFPS = 999f;
            maxFPS = 0f;
            peakMemoryMB = 0f;
            averageFPS = 0f;
            currentFPS = 0f;
            currentMemoryMB = 0f;
            Debug.Log("[PerformanceMonitor] Stats reset");
        }

        /// <summary>
        /// Generate performance report
        /// </summary>
        public string GenerateReport()
        {
            return $"=== PERFORMANCE REPORT ===\n" +
                   $"FPS - Current: {currentFPS:F1} | Avg: {averageFPS:F1} | Min: {minFPS:F1} | Max: {maxFPS:F1}\n" +
                   $"Memory - Current: {currentMemoryMB:F2} MB | Peak: {peakMemoryMB:F2} MB\n" +
                   $"Status: {(IsCriticalPerformance ? "⚠️ CRITICAL FPS" : "✓ FPS OK")} | " +
                   $"{(IsCriticalMemory ? "⚠️ CRITICAL MEMORY" : "✓ Memory OK")}";
        }
        #endregion
    }
}