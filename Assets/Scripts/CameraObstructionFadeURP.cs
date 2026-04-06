using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionFadeURP : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;
    [Range(0f, 1f)] public float fadeAlpha = 0.3f;

    private Camera cam;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private HashSet<Renderer> currentlyFaded = new HashSet<Renderer>();

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        RestoreAll();

        Vector3 dir = player.position - cam.transform.position;
        float dist = dir.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, dir.normalized, dist, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                FadeObject(rend);
            }
        }
    }

    void FadeObject(Renderer rend)
    {
        if (!originalMaterials.ContainsKey(rend))
        {
            originalMaterials[rend] = rend.materials;
        }

        foreach (Material mat in rend.materials)
        {
            SetTransparent(mat, fadeAlpha);
        }

        currentlyFaded.Add(rend);
    }

    void RestoreAll()
    {
        foreach (Renderer rend in currentlyFaded)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                SetOpaque(mat);
            }
        }

        currentlyFaded.Clear();
    }

    void SetTransparent(Material mat, float alpha)
    {
        // URP uses _Surface instead of _Mode
        mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mat.SetFloat("_Blend", 0);   // Alpha blend

        mat.SetOverrideTag("RenderType", "Transparent");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        Color color = mat.GetColor("_BaseColor");
        color.a = alpha;
        mat.SetColor("_BaseColor", color);

        // Enable alpha clipping/blending keywords
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    void SetOpaque(Material mat)
    {
        mat.SetFloat("_Surface", 0);

        mat.SetOverrideTag("RenderType", "Opaque");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        Color color = mat.GetColor("_BaseColor");
        color.a = 1f;
        mat.SetColor("_BaseColor", color);

        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }
}
