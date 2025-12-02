using UnityEngine;
using System;

/// <summary>
/// Zone cible 3D qui détecte si l'utilisateur atteint une position/angle spécifique
/// Utilisé pour valider les phases d'exercice (ex: profondeur du squat)
/// </summary>
public class TargetZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    [SerializeField] private ZoneType zoneType = ZoneType.Position;
    [SerializeField] private HumanBodyBones trackedBone = HumanBodyBones.Hips;

    [Header("Position Zone Settings")]
    [SerializeField] private float toleranceRadius = 0.2f;
    [SerializeField] private bool requireContinuousStay = false;
    [SerializeField] private float requiredStayDuration = 1f;

    [Header("Angle Zone Settings")]
    [SerializeField] private float targetAngle = 90f;
    [SerializeField] private float angleTolerance = 10f;
    [SerializeField] private Vector3 angleAxis = Vector3.right;

    [Header("Visual Feedback")]
    [SerializeField] private MeshRenderer zoneRenderer;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material achievedMaterial;
    [SerializeField] private bool showZoneInGame = true;

    [Header("Audio")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip achieveSound;

    // Events
    public event Action<TargetZone> OnZoneEntered;
    public event Action<TargetZone> OnZoneExited;
    public event Action<TargetZone> OnZoneAchieved;

    private Transform trackedTransform;
    private bool isInZone = false;
    private bool isAchieved = false;
    private float zoneEntryTime;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (zoneRenderer != null)
        {
            zoneRenderer.enabled = showZoneInGame;
            zoneRenderer.material = inactiveMaterial;
        }
    }

    /// <summary>
    /// Définit le transform à tracker (généralement un os de l'avatar)
    /// </summary>
    public void SetTrackedTransform(Transform target)
    {
        trackedTransform = target;
    }

    private void Update()
    {
        if (trackedTransform == null || isAchieved)
            return;

        bool currentlyInZone = CheckIfInZone();

        // Gère l'entrée dans la zone
        if (currentlyInZone && !isInZone)
        {
            EnterZone();
        }
        // Gère la sortie de la zone
        else if (!currentlyInZone && isInZone)
        {
            ExitZone();
        }

        // Check achievement si en mode continuous stay
        if (isInZone && requireContinuousStay)
        {
            float timeInZone = Time.time - zoneEntryTime;
            if (timeInZone >= requiredStayDuration && !isAchieved)
            {
                AchieveZone();
            }
        }
        else if (isInZone && !requireContinuousStay && !isAchieved)
        {
            // Achievement immédiat si pas de durée requise
            AchieveZone();
        }
    }

    /// <summary>
    /// Vérifie si le transform tracké est dans la zone
    /// </summary>
    private bool CheckIfInZone()
    {
        switch (zoneType)
        {
            case ZoneType.Position:
                return CheckPositionZone();
            case ZoneType.Angle:
                return CheckAngleZone();
            default:
                return false;
        }
    }

    /// <summary>
    /// Vérifie la zone de position
    /// </summary>
    private bool CheckPositionZone()
    {
        float distance = Vector3.Distance(trackedTransform.position, transform.position);
        return distance <= toleranceRadius;
    }

    /// <summary>
    /// Vérifie la zone d'angle
    /// </summary>
    private bool CheckAngleZone()
    {
        Vector3 direction = trackedTransform.forward;
        float angle = Vector3.Angle(direction, angleAxis);
        float angleDifference = Mathf.Abs(angle - targetAngle);
        return angleDifference <= angleTolerance;
    }

    /// <summary>
    /// Appelé lors de l'entrée dans la zone
    /// </summary>
    private void EnterZone()
    {
        isInZone = true;
        zoneEntryTime = Time.time;

        if (zoneRenderer != null)
        {
            zoneRenderer.material = activeMaterial;
        }

        if (enterSound != null)
        {
            audioSource.PlayOneShot(enterSound);
        }

        OnZoneEntered?.Invoke(this);
    }

    /// <summary>
    /// Appelé lors de la sortie de la zone
    /// </summary>
    private void ExitZone()
    {
        isInZone = false;

        if (zoneRenderer != null && !isAchieved)
        {
            zoneRenderer.material = inactiveMaterial;
        }

        OnZoneExited?.Invoke(this);
    }

    /// <summary>
    /// Appelé quand la zone est réussie
    /// </summary>
    private void AchieveZone()
    {
        isAchieved = true;

        if (zoneRenderer != null)
        {
            zoneRenderer.material = achievedMaterial;
        }

        if (achieveSound != null)
        {
            audioSource.PlayOneShot(achieveSound);
        }

        OnZoneAchieved?.Invoke(this);
    }

    /// <summary>
    /// Reset la zone pour une nouvelle utilisation
    /// </summary>
    public void ResetZone()
    {
        isInZone = false;
        isAchieved = false;

        if (zoneRenderer != null)
        {
            zoneRenderer.material = inactiveMaterial;
        }
    }

    /// <summary>
    /// Obtient le pourcentage de progression dans la zone (pour continuous stay)
    /// </summary>
    public float GetProgressPercentage()
    {
        if (!requireContinuousStay || !isInZone)
            return isAchieved ? 1f : 0f;

        float timeInZone = Time.time - zoneEntryTime;
        return Mathf.Clamp01(timeInZone / requiredStayDuration);
    }

    private void OnDrawGizmos()
    {
        // Dessine la zone dans l'éditeur
        Gizmos.color = isAchieved ? Color.green : (isInZone ? Color.yellow : Color.red);

        if (zoneType == ZoneType.Position)
        {
            Gizmos.DrawWireSphere(transform.position, toleranceRadius);
        }
        else if (zoneType == ZoneType.Angle)
        {
            Gizmos.DrawRay(transform.position, angleAxis * 0.5f);
        }
    }
}

public enum ZoneType
{
    Position,   // Zone basée sur la position dans l'espace
    Angle       // Zone basée sur un angle articulaire
}