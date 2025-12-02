using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gère tous les feedbacks visuels synchronisés avec les mouvements
/// Outline, couleurs de précision, effets de particules
/// </summary>
public class VisualFeedbackManager : MonoBehaviour
{
    [Header("Target Renderers")]
    [SerializeField] private List<Renderer> targetRenderers;

    [Header("Outline Settings")]
    [SerializeField] private bool enableOutline = true;
    [SerializeField] private Color goodOutlineColor = Color.green;
    [SerializeField] private Color badOutlineColor = Color.red;
    [SerializeField] private float outlineWidth = 0.03f;
    [SerializeField] private float outlineIntensity = 1.5f;

    [Header("Precision Color Settings")]
    [SerializeField] private Gradient precisionGradient;
    [SerializeField] private float colorTransitionSpeed = 5f;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem goodMovementParticles;
    [SerializeField] private ParticleSystem perfectMovementParticles;
    [SerializeField] private ParticleSystem badMovementParticles;

    [Header("Flash Effect")]
    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float flashDuration = 0.2f;

    private Dictionary<Renderer, Material[]> originalMaterials;
    private Dictionary<Renderer, Material[]> outlineMaterials;
    private float currentPrecision = 1f;
    private float targetPrecision = 1f;
    private bool isFlashing = false;
    private float flashStartTime;

    private void Awake()
    {
        InitializeMaterials();
        InitializeGradient();
    }

    /// <summary>
    /// Initialise les matériaux et crée des instances
    /// </summary>
    private void InitializeMaterials()
    {
        originalMaterials = new Dictionary<Renderer, Material[]>();
        outlineMaterials = new Dictionary<Renderer, Material[]>();

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer == null) continue;

            // Sauvegarde les matériaux originaux
            originalMaterials[renderer] = renderer.materials;

            // Crée des instances pour éviter de modifier les assets
            Material[] instances = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                instances[i] = new Material(renderer.materials[i]);
            }
            renderer.materials = instances;
            outlineMaterials[renderer] = instances;
        }
    }

    /// <summary>
    /// Initialise le gradient de précision par défaut si non défini
    /// </summary>
    private void InitializeGradient()
    {
        if (precisionGradient == null || precisionGradient.colorKeys.Length == 0)
        {
            precisionGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);

            precisionGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private void Update()
    {
        // Transition douce de la précision
        currentPrecision = Mathf.Lerp(currentPrecision, targetPrecision, Time.deltaTime * colorTransitionSpeed);

        // Applique les couleurs
        ApplyPrecisionColors();

        // Gère le flash effect
        if (isFlashing)
        {
            UpdateFlashEffect();
        }
    }

    /// <summary>
    /// Met à jour la précision du mouvement (0 = mauvais, 1 = parfait)
    /// </summary>
    public void UpdatePrecision(float precision)
    {
        targetPrecision = Mathf.Clamp01(precision);

        // Active les particules appropriées
        UpdateParticleEffects(precision);
    }

    /// <summary>
    /// Applique les couleurs de précision aux matériaux
    /// </summary>
    private void ApplyPrecisionColors()
    {
        Color precisionColor = precisionGradient.Evaluate(currentPrecision);

        foreach (var kvp in outlineMaterials)
        {
            foreach (Material mat in kvp.Value)
            {
                if (mat.HasProperty("_OutlineColor"))
                {
                    mat.SetColor("_OutlineColor", precisionColor);
                    mat.SetFloat("_OutlineWidth", outlineWidth);
                    mat.SetFloat("_OutlineIntensity", outlineIntensity);
                }

                if (mat.HasProperty("_AccuracyColor"))
                {
                    mat.SetColor("_AccuracyColor", precisionColor);
                    mat.SetFloat("_AccuracyBlend", 0.3f);
                }
            }
        }
    }

    /// <summary>
    /// Met à jour les systèmes de particules selon la précision
    /// </summary>
    private void UpdateParticleEffects(float precision)
    {
        // Désactive tous les systèmes
        if (goodMovementParticles != null && !goodMovementParticles.isPlaying)
            goodMovementParticles.Stop();
        if (perfectMovementParticles != null && !perfectMovementParticles.isPlaying)
            perfectMovementParticles.Stop();
        if (badMovementParticles != null && !badMovementParticles.isPlaying)
            badMovementParticles.Stop();

        // Active le système approprié
        if (precision >= 0.9f && perfectMovementParticles != null)
        {
            if (!perfectMovementParticles.isPlaying)
                perfectMovementParticles.Play();
        }
        else if (precision >= 0.6f && goodMovementParticles != null)
        {
            if (!goodMovementParticles.isPlaying)
                goodMovementParticles.Play();
        }
        else if (precision < 0.4f && badMovementParticles != null)
        {
            if (!badMovementParticles.isPlaying)
                badMovementParticles.Play();
        }
    }

    /// <summary>
    /// Déclenche un effet de flash pour un feedback instantané
    /// </summary>
    public void TriggerFlash(FlashType flashType)
    {
        isFlashing = true;
        flashStartTime = Time.time;

        Color flashColor = flashType switch
        {
            FlashType.Good => Color.green,
            FlashType.Perfect => Color.cyan,
            FlashType.Bad => Color.red,
            _ => Color.white
        };

        foreach (var kvp in outlineMaterials)
        {
            foreach (Material mat in kvp.Value)
            {
                if (mat.HasProperty("_OutlineIntensity"))
                {
                    mat.SetFloat("_OutlineIntensity", flashIntensity);
                }
            }
        }
    }

    /// <summary>
    /// Met à jour l'effet de flash
    /// </summary>
    private void UpdateFlashEffect()
    {
        float elapsed = Time.time - flashStartTime;

        if (elapsed >= flashDuration)
        {
            isFlashing = false;
            // Restore normal intensity
            foreach (var kvp in outlineMaterials)
            {
                foreach (Material mat in kvp.Value)
                {
                    if (mat.HasProperty("_OutlineIntensity"))
                    {
                        mat.SetFloat("_OutlineIntensity", outlineIntensity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Active/désactive le système d'outline
    /// </summary>
    public void SetOutlineEnabled(bool enabled)
    {
        enableOutline = enabled;

        if (!enabled)
        {
            foreach (var kvp in outlineMaterials)
            {
                foreach (Material mat in kvp.Value)
                {
                    if (mat.HasProperty("_OutlineWidth"))
                    {
                        mat.SetFloat("_OutlineWidth", 0f);
                    }
                }
            }
        }
        else
        {
            ApplyPrecisionColors();
        }
    }

    private void OnDestroy()
    {
        // Nettoie les matériaux instanciés
        foreach (var kvp in outlineMaterials)
        {
            foreach (Material mat in kvp.Value)
            {
                if (mat != null)
                    Destroy(mat);
            }
        }
    }
}

public enum FlashType
{
    Good,
    Perfect,
    Bad
}