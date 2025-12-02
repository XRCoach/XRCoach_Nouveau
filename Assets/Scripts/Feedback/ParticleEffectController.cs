using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contrôle les effets de particules synchronisés avec les mouvements
/// Trail effects, burst effects, et effets de célébration
/// </summary>
public class ParticleEffectController : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem movementTrailParticles;
    [SerializeField] private ParticleSystem goodMovementBurstParticles;
    [SerializeField] private ParticleSystem perfectMovementBurstParticles;
    [SerializeField] private ParticleSystem errorBurstParticles;
    [SerializeField] private ParticleSystem achievementParticles;

    [Header("Trail Settings")]
    [SerializeField] private List<Transform> jointTransforms;
    [SerializeField] private bool enableTrail = true;
    [SerializeField] private float trailEmissionRate = 20f;
    [SerializeField] private Gradient trailColorGradient;

    [Header("Burst Settings")]
    [SerializeField] private int goodBurstCount = 10;
    [SerializeField] private int perfectBurstCount = 30;
    [SerializeField] private int errorBurstCount = 5;

    [Header("Achievement Effect")]
    [SerializeField] private float achievementDuration = 3f;
    [SerializeField] private int achievementBurstCount = 50;

    private Dictionary<Transform, ParticleSystem> jointTrailSystems;
    private float currentMovementQuality = 1f;

    private void Awake()
    {
        InitializeTrailSystems();
        ConfigureParticleSystems();
    }

    /// <summary>
    /// Initialise un système de trail pour chaque joint
    /// </summary>
    private void InitializeTrailSystems()
    {
        if (!enableTrail || jointTransforms == null) return;

        jointTrailSystems = new Dictionary<Transform, ParticleSystem>();

        foreach (Transform joint in jointTransforms)
        {
            if (joint == null) continue;

            // Créé un système de particules pour ce joint
            GameObject trailObj = new GameObject($"Trail_{joint.name}");
            trailObj.transform.SetParent(joint);
            trailObj.transform.localPosition = Vector3.zero;

            ParticleSystem ps = trailObj.AddComponent<ParticleSystem>();
            ConfigureTrailParticleSystem(ps);

            jointTrailSystems[joint] = ps;
        }
    }

    /// <summary>
    /// Configure un système de particules pour le trail
    /// </summary>
    private void ConfigureTrailParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 0f;
        main.startSize = 0.05f;
        main.startColor = new ParticleSystem.MinMaxGradient(trailColorGradient);
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = trailEmissionRate;

        var shape = ps.shape;
        shape.enabled = false;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(trailColorGradient);

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
    }

    /// <summary>
    /// Configure les systèmes de burst
    /// </summary>
    private void ConfigureParticleSystems()
    {
        if (goodMovementBurstParticles != null)
        {
            var main = goodMovementBurstParticles.main;
            main.playOnAwake = false;
        }

        if (perfectMovementBurstParticles != null)
        {
            var main = perfectMovementBurstParticles.main;
            main.playOnAwake = false;
        }

        if (errorBurstParticles != null)
        {
            var main = errorBurstParticles.main;
            main.playOnAwake = false;
        }

        if (achievementParticles != null)
        {
            var main = achievementParticles.main;
            main.playOnAwake = false;
        }
    }

    /// <summary>
    /// Met à jour la qualité du mouvement pour le trail
    /// </summary>
    public void UpdateMovementQuality(float quality)
    {
        currentMovementQuality = Mathf.Clamp01(quality);

        if (jointTrailSystems != null)
        {
            Color trailColor = trailColorGradient.Evaluate(currentMovementQuality);

            foreach (var kvp in jointTrailSystems)
            {
                var main = kvp.Value.main;
                main.startColor = trailColor;
            }
        }
    }

    /// <summary>
    /// Active/désactive le trail effect
    /// </summary>
    public void SetTrailEnabled(bool enabled)
    {
        enableTrail = enabled;

        if (jointTrailSystems != null)
        {
            foreach (var kvp in jointTrailSystems)
            {
                if (enabled)
                    kvp.Value.Play();
                else
                    kvp.Value.Stop();
            }
        }
    }

    /// <summary>
    /// Déclenche un burst de particules pour bon mouvement
    /// </summary>
    public void TriggerGoodMovementBurst(Vector3 position)
    {
        if (goodMovementBurstParticles != null)
        {
            goodMovementBurstParticles.transform.position = position;
            goodMovementBurstParticles.Emit(goodBurstCount);
        }
    }

    /// <summary>
    /// Déclenche un burst de particules pour mouvement parfait
    /// </summary>
    public void TriggerPerfectMovementBurst(Vector3 position)
    {
        if (perfectMovementBurstParticles != null)
        {
            perfectMovementBurstParticles.transform.position = position;
            perfectMovementBurstParticles.Emit(perfectBurstCount);
        }
    }

    /// <summary>
    /// Déclenche un burst de particules pour erreur
    /// </summary>
    public void TriggerErrorBurst(Vector3 position)
    {
        if (errorBurstParticles != null)
        {
            errorBurstParticles.transform.position = position;
            errorBurstParticles.Emit(errorBurstCount);
        }
    }

    /// <summary>
    /// Déclenche l'effet d'achievement complet
    /// </summary>
    public void TriggerAchievementEffect(Vector3 position)
    {
        if (achievementParticles != null)
        {
            achievementParticles.transform.position = position;
            achievementParticles.Play();
        }
    }

    /// <summary>
    /// Déclenche un burst synchronisé sur tous les joints trackés
    /// </summary>
    public void TriggerMultiJointBurst(int particlesPerJoint = 5)
    {
        if (jointTrailSystems == null) return;

        foreach (var kvp in jointTrailSystems)
        {
            kvp.Value.Emit(particlesPerJoint);
        }
    }

    /// <summary>
    /// Ajuste l'intensité d'émission du trail
    /// </summary>
    public void SetTrailIntensity(float intensity)
    {
        float rate = trailEmissionRate * Mathf.Clamp01(intensity);

        if (jointTrailSystems != null)
        {
            foreach (var kvp in jointTrailSystems)
            {
                var emission = kvp.Value.emission;
                emission.rateOverTime = rate;
            }
        }
    }

    /// <summary>
    /// Change la couleur du trail dynamiquement
    /// </summary>
    public void SetTrailColor(Color color)
    {
        if (jointTrailSystems != null)
        {
            foreach (var kvp in jointTrailSystems)
            {
                var main = kvp.Value.main;
                main.startColor = color;
            }
        }
    }

    /// <summary>
    /// Nettoie toutes les particules
    /// </summary>
    public void ClearAllParticles()
    {
        if (jointTrailSystems != null)
        {
            foreach (var kvp in jointTrailSystems)
            {
                kvp.Value.Clear();
            }
        }

        if (goodMovementBurstParticles != null)
            goodMovementBurstParticles.Clear();

        if (perfectMovementBurstParticles != null)
            perfectMovementBurstParticles.Clear();

        if (errorBurstParticles != null)
            errorBurstParticles.Clear();

        if (achievementParticles != null)
            achievementParticles.Clear();
    }

    private void OnDestroy()
    {
        // Nettoie les systèmes créés dynamiquement
        if (jointTrailSystems != null)
        {
            foreach (var kvp in jointTrailSystems)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
            }
        }
    }
}