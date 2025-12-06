using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contr�le l'avatar 3D qui repr�sente l'utilisateur en temps r�el
/// G�re le rigging, l'animation proc�durale et le feedback miroir
/// </summary>
[RequireComponent(typeof(Animator))]
public class AvatarController : MonoBehaviour
{
    [Header("Avatar Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Header("Joint Tracking")]
    [SerializeField] private List<JointMapping> jointMappings;

    [Header("Mirror Mode")]
    [SerializeField] private bool enableMirrorMode = true;
    [SerializeField] private float smoothingFactor = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material goodPostureMaterial;
    [SerializeField] private Material badPostureMaterial;

    [Header("Ghost Trail")]
    [SerializeField] private bool enableGhostTrail = true;
    [SerializeField] private float ghostTrailInterval = 0.5f;
    [SerializeField] private int maxGhosts = 5;
    [SerializeField] private Material ghostMaterial;

    private Dictionary<HumanBodyBones, Transform> boneTransforms;
    private Dictionary<HumanBodyBones, Quaternion> targetRotations;
    private List<GameObject> ghostTrail;
    private float lastGhostTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        InitializeBoneMapping();
        InitializeGhostTrail();
    }

    /// <summary>
    /// Initialise le mapping des os humanoid
    /// </summary>
    private void InitializeBoneMapping()
    {
        if (animator.avatar == null)
        {
            Debug.LogError("AvatarController: Animator has no Avatar assigned. Please ensure the avatar prefab has a Humanoid Avatar configured in the Animator component.", gameObject);
            return;
        }

        boneTransforms = new Dictionary<HumanBodyBones, Transform>();
        targetRotations = new Dictionary<HumanBodyBones, Quaternion>();

        foreach (JointMapping mapping in jointMappings)
        {
            Transform bone = animator.GetBoneTransform(mapping.bone);
            if (bone != null)
            {
                boneTransforms[mapping.bone] = bone;
                targetRotations[mapping.bone] = bone.localRotation;
            }
        }
    }

    /// <summary>
    /// Initialise le syst�me de ghost trail
    /// </summary>
    private void InitializeGhostTrail()
    {
        ghostTrail = new List<GameObject>();
    }

    /// <summary>
    /// Met � jour la rotation d'un joint sp�cifique depuis les donn�es IMU
    /// </summary>
    public void UpdateJointRotation(HumanBodyBones bone, Quaternion rotation)
    {
        if (targetRotations.ContainsKey(bone))
        {
            // Applique le mode miroir si activ�
            if (enableMirrorMode)
            {
                rotation = MirrorRotation(rotation);
            }

            targetRotations[bone] = rotation;
        }
    }

    private void LateUpdate()
    {
        // Applique les rotations avec smoothing
        ApplySmoothRotations();

        // G�re le ghost trail
        if (enableGhostTrail)
        {
            UpdateGhostTrail();
        }
    }

    /// <summary>
    /// Applique les rotations avec smoothing pour un mouvement fluide
    /// </summary>
    private void ApplySmoothRotations()
    {
        foreach (var kvp in targetRotations)
        {
            if (boneTransforms.ContainsKey(kvp.Key))
            {
                Transform bone = boneTransforms[kvp.Key];
                bone.localRotation = Quaternion.Slerp(
                    bone.localRotation,
                    kvp.Value,
                    smoothingFactor
                );
            }
        }
    }

    /// <summary>
    /// Cr�� un effet miroir pour la rotation
    /// </summary>
    private Quaternion MirrorRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.y = -euler.y;
        euler.z = -euler.z;
        return Quaternion.Euler(euler);
    }

    /// <summary>
    /// Change le mat�riau de l'avatar selon la qualit� de posture
    /// </summary>
    public void SetPostureFeedback(PostureQuality quality)
    {
        Material targetMaterial = quality switch
        {
            PostureQuality.Good => goodPostureMaterial,
            PostureQuality.Bad => badPostureMaterial,
            _ => normalMaterial
        };

        if (meshRenderer != null && targetMaterial != null)
        {
            meshRenderer.material = targetMaterial;
        }
    }

    /// <summary>
    /// G�re la cr�ation et destruction des ghosts
    /// </summary>
    private void UpdateGhostTrail()
    {
        if (Time.time - lastGhostTime >= ghostTrailInterval)
        {
            CreateGhost();
            lastGhostTime = Time.time;
        }
    }

    /// <summary>
    /// Cr�� un ghost de l'avatar � sa position actuelle
    /// </summary>
    private void CreateGhost()
    {
        // Cr�� un nouveau ghost
        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;

        // Copie le mesh
        SkinnedMeshRenderer ghostRenderer = ghost.AddComponent<SkinnedMeshRenderer>();
        ghostRenderer.sharedMesh = meshRenderer.sharedMesh;
        ghostRenderer.material = ghostMaterial;

        // Bake le mesh � la pose actuelle
        Mesh bakedMesh = new Mesh();
        meshRenderer.BakeMesh(bakedMesh);

        MeshFilter filter = ghost.AddComponent<MeshFilter>();
        filter.mesh = bakedMesh;

        MeshRenderer renderer = ghost.AddComponent<MeshRenderer>();
        renderer.material = ghostMaterial;

        // Supprime le SkinnedMeshRenderer car on utilise maintenant un MeshRenderer statique
        Destroy(ghostRenderer);

        // Ajoute � la liste
        ghostTrail.Add(ghost);

        // Fade out component
        GhostFadeOut fadeOut = ghost.AddComponent<GhostFadeOut>();
        fadeOut.fadeDuration = ghostTrailInterval * maxGhosts;

        // Limite le nombre de ghosts
        if (ghostTrail.Count > maxGhosts)
        {
            GameObject oldGhost = ghostTrail[0];
            ghostTrail.RemoveAt(0);
            Destroy(oldGhost);
        }
    }

    /// <summary>
    /// Active/d�sactive le ghost trail
    /// </summary>
    public void SetGhostTrailEnabled(bool enabled)
    {
        enableGhostTrail = enabled;

        if (!enabled)
        {
            ClearGhostTrail();
        }
    }

    /// <summary>
    /// Nettoie tous les ghosts
    /// </summary>
    private void ClearGhostTrail()
    {
        foreach (GameObject ghost in ghostTrail)
        {
            if (ghost != null)
                Destroy(ghost);
        }
        ghostTrail.Clear();
    }

    private void OnDestroy()
    {
        ClearGhostTrail();
    }
}

/// <summary>
/// Mapping entre un os humanoid et son nom dans le rig
/// </summary>
[System.Serializable]
public class JointMapping
{
    public HumanBodyBones bone;
    public string jointName;
}

/// <summary>
/// Qualit� de la posture pour le feedback visuel
/// </summary>
public enum PostureQuality
{
    Normal,
    Good,
    Bad
}