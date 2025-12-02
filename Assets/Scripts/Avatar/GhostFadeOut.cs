using UnityEngine;

/// <summary>
/// Fait disparaître progressivement un ghost du trail
/// </summary>
public class GhostFadeOut : MonoBehaviour
{
    public float fadeDuration = 2f;

    private MeshRenderer meshRenderer;
    private Material materialInstance;
    private float startTime;
    private Color startColor;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Créé une instance du matériau pour ne pas affecter les autres
            materialInstance = new Material(meshRenderer.material);
            meshRenderer.material = materialInstance;
            startColor = materialInstance.color;
        }

        startTime = Time.time;
    }

    private void Update()
    {
        if (meshRenderer == null || materialInstance == null)
            return;

        float elapsed = Time.time - startTime;
        float alpha = 1f - (elapsed / fadeDuration);

        if (alpha <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Fade out l'alpha
        Color color = startColor;
        color.a = alpha;
        materialInstance.color = color;
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}