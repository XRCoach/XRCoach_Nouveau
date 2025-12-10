using UnityEngine;
using System;
using System.Collections.Generic;

namespace SportCoachQA.Validation
{
    /// <summary>
    /// Système de validation automatique pour les tests QA
    /// </summary>
    public class QAValidator : MonoBehaviour
    {
        #region Validation Result
        [System.Serializable]
        public class ValidationResult
        {
            public string testName;
            public bool passed;
            public string message;
            public float executionTime;
            public DateTime timestamp;

            public ValidationResult(string name, bool success, string msg, float time)
            {
                testName = name;
                passed = success;
                message = msg;
                executionTime = time;
                timestamp = DateTime.Now;
            }
        }
        #endregion

        #region Validation Categories
        public enum ValidationCategory
        {
            Performance,
            Sensors,
            UI,
            Data,
            Network,
            Custom
        }
        #endregion

        #region State
        private List<ValidationResult> validationHistory = new List<ValidationResult>();
        private bool isRunning = false;
        #endregion

        #region Public Properties
        public List<ValidationResult> ValidationHistory => validationHistory;
        public bool IsRunning => isRunning;
        public int TotalTests => validationHistory.Count;
        public int PassedTests => validationHistory.FindAll(r => r.passed).Count;
        public int FailedTests => validationHistory.FindAll(r => !r.passed).Count;
        public float SuccessRate => TotalTests > 0 ? (PassedTests / (float)TotalTests) * 100f : 0f;
        #endregion

        #region Core Validation Methods
        /// <summary>
        /// Exécute un test de validation
        /// </summary>
        public ValidationResult RunTest(string testName, Func<bool> testLogic, string successMsg = "", string failMsg = "")
        {
            float startTime = Time.realtimeSinceStartup;

            try
            {
                bool result = testLogic.Invoke();
                float execTime = Time.realtimeSinceStartup - startTime;

                string message = result
                    ? (string.IsNullOrEmpty(successMsg) ? "Test passed" : successMsg)
                    : (string.IsNullOrEmpty(failMsg) ? "Test failed" : failMsg);

                var validationResult = new ValidationResult(testName, result, message, execTime);
                validationHistory.Add(validationResult);

                LogResult(validationResult);

                return validationResult;
            }
            catch (Exception e)
            {
                float execTime = Time.realtimeSinceStartup - startTime;
                var errorResult = new ValidationResult(testName, false, $"Exception: {e.Message}", execTime);
                validationHistory.Add(errorResult);

                Debug.LogError($"[QA Validator] Test '{testName}' threw exception: {e.Message}");

                return errorResult;
            }
        }

        /// <summary>
        /// Valide une condition avec message personnalisé
        /// </summary>
        public ValidationResult Validate(string testName, bool condition, string message = "")
        {
            float startTime = Time.realtimeSinceStartup;
            float execTime = Time.realtimeSinceStartup - startTime;

            string resultMessage = string.IsNullOrEmpty(message)
                ? (condition ? "Validation passed" : "Validation failed")
                : message;

            var result = new ValidationResult(testName, condition, resultMessage, execTime);
            validationHistory.Add(result);

            LogResult(result);

            return result;
        }

        /// <summary>
        /// Valide une valeur numérique dans une plage
        /// </summary>
        public ValidationResult ValidateRange(string testName, float value, float min, float max)
        {
            bool inRange = value >= min && value <= max;
            string message = inRange
                ? $"Value {value:F2} is within range [{min:F2}, {max:F2}]"
                : $"Value {value:F2} is OUT OF RANGE [{min:F2}, {max:F2}]";

            return Validate(testName, inRange, message);
        }

        /// <summary>
        /// Valide qu'un objet n'est pas null
        /// </summary>
        public ValidationResult ValidateNotNull(string testName, object obj, string objectName = "")
        {
            bool isValid = obj != null;
            string name = string.IsNullOrEmpty(objectName) ? "Object" : objectName;
            string message = isValid
                ? $"{name} is valid (not null)"
                : $"{name} is NULL";

            return Validate(testName, isValid, message);
        }
        #endregion

        #region Batch Testing
        /// <summary>
        /// Lance une série de tests
        /// </summary>
        public void RunTestBatch(string batchName, Action testBatch)
        {
            if (isRunning)
            {
                Debug.LogWarning("[QA Validator] Test batch already running. Wait for completion.");
                return;
            }

            isRunning = true;
            Debug.Log($"[QA Validator] Starting test batch: {batchName}");

            float startTime = Time.realtimeSinceStartup;

            try
            {
                testBatch.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[QA Validator] Batch '{batchName}' failed with exception: {e.Message}");
            }

            float totalTime = Time.realtimeSinceStartup - startTime;
            isRunning = false;

            Debug.Log($"[QA Validator] Batch '{batchName}' completed in {totalTime:F3}s - {PassedTests}/{TotalTests} passed");
        }
        #endregion

        #region Reporting
        /// <summary>
        /// Génère un rapport complet
        /// </summary>
        public string GenerateReport()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();

            report.AppendLine("=== QA VALIDATION REPORT ===");
            report.AppendLine($"Total Tests: {TotalTests}");
            report.AppendLine($"Passed: {PassedTests} ✓");
            report.AppendLine($"Failed: {FailedTests} ✗");
            report.AppendLine($"Success Rate: {SuccessRate:F1}%");
            report.AppendLine();

            report.AppendLine("--- Test Results ---");
            foreach (var result in validationHistory)
            {
                string status = result.passed ? "✓" : "✗";
                report.AppendLine($"{status} {result.testName} ({result.executionTime * 1000f:F2}ms)");
                report.AppendLine($"  └─ {result.message}");
            }

            return report.ToString();
        }

        /// <summary>
        /// Efface l'historique des tests
        /// </summary>
        public void ClearHistory()
        {
            validationHistory.Clear();
            Debug.Log("[QA Validator] Validation history cleared");
        }
        #endregion

        #region Logging
        private void LogResult(ValidationResult result)
        {
            string icon = result.passed ? "✓" : "✗";
            string color = result.passed ? "green" : "red";

            Debug.Log($"<color={color}>[QA] {icon} {result.testName}</color> - {result.message} ({result.executionTime * 1000f:F2}ms)");
        }
        #endregion
    }
}