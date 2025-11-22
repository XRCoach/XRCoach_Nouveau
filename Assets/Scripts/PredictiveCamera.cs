using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class PredictiveCamera : MonoBehaviour
{
    [Tooltip("Le [TrackingRoot] contrôlé par l'IMU")]
    public Transform target;

    [Range(0f, 0.3f)] public float predictionTime = 0.1f;

    private CinemachineCamera vcam;
    private Transform dummyFollow;
    private Vector3 lastValidPosition;
    private bool firstFrame = true;   // ← la ligne qui manquait !

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();

        // Crée le dummy qui sera le vrai Follow de Cinemachine
        GameObject dummy = new GameObject("PredictiveFollow_Dummy");
        dummy.transform.SetParent(transform);
        dummy.transform.localPosition = Vector3.zero;
        dummyFollow = dummy.transform;

        vcam.Follow = dummyFollow;   // Cinemachine suit le dummy
        // vcam.LookAt reste sur target ou null selon ton choix
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("PredictiveCamera : target non assigné !");
            return;
        }

        Vector3 currentPos = target.position;

        // ==== Protection complète ====
        if (float.IsNaN(currentPos.x) || float.IsNaN(currentPos.y) || float.IsNaN(currentPos.z))
            currentPos = lastValidPosition;

        // Première frame : pas de vitesse, on initialise juste
        if (firstFrame)
        {
            lastValidPosition = currentPos;
            dummyFollow.position = currentPos;
            firstFrame = false;
            return;
        }

        // Calcul de la vitesse (sécurisé)
        Vector3 velocity = (currentPos - lastValidPosition) / Time.deltaTime;

        // Mise à jour de la dernière position valide
        lastValidPosition = currentPos;

        // Prédiction et application sur le dummy
        Vector3 predictedPos = currentPos + velocity * predictionTime;
        dummyFollow.position = predictedPos;
    }
}