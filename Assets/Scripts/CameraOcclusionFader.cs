using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Attach to the player. Fades out any objects that block the camera's line of sight.
// Objects must have a Collider to be detected.
public class CameraOcclusionFader : MonoBehaviour
{
    public LayerMask occlusionLayers = ~0;
    [Range(0f, 1f)]
    public float targetAlpha = 0.15f;
    public float fadeSpeed = 8f;

    private class FadeState
    {
        public Renderer renderer;
        public Material[] originalMaterials;   // shared refs — not copies
        public Material[] fadeMaterials;        // transparent copies we own
        public float currentAlpha = 1f;
        public bool blocking;
    }

    private readonly Dictionary<Renderer, FadeState> tracked = new Dictionary<Renderer, FadeState>();
    private readonly List<Renderer> toRestore = new List<Renderer>();
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        foreach (var state in tracked.Values)
            state.blocking = false;

        Vector3 origin = cam.transform.position;
        Vector3 dir = transform.position - origin;
        float dist = dir.magnitude;

        var hits = Physics.RaycastAll(origin, dir / dist, dist, occlusionLayers);
        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

            var rend = hit.collider.GetComponent<Renderer>();
            if (rend == null)
                rend = hit.collider.GetComponentInParent<Renderer>();
            if (rend == null) continue;

            if (!tracked.TryGetValue(rend, out var state))
            {
                state = StartFading(rend);
                if (state == null) continue;
                tracked[rend] = state;
            }

            state.blocking = true;
        }

        toRestore.Clear();
        foreach (var kvp in tracked)
        {
            var state = kvp.Value;
            float goal = state.blocking ? targetAlpha : 1f;
            state.currentAlpha = Mathf.MoveTowards(state.currentAlpha, goal, fadeSpeed * Time.deltaTime);
            ApplyAlpha(state.fadeMaterials, state.currentAlpha);

            if (!state.blocking && Mathf.Approximately(state.currentAlpha, 1f))
                toRestore.Add(kvp.Key);
        }

        foreach (var rend in toRestore)
        {
            Restore(tracked[rend]);
            tracked.Remove(rend);
        }
    }

    FadeState StartFading(Renderer rend)
    {
        var originals = rend.sharedMaterials;
        var copies = new Material[originals.Length];

        for (int i = 0; i < originals.Length; i++)
        {
            if (originals[i] == null) { copies[i] = null; continue; }
            copies[i] = new Material(originals[i]);
            MakeTransparent(copies[i]);
        }

        rend.materials = copies;

        return new FadeState
        {
            renderer = rend,
            originalMaterials = originals,
            fadeMaterials = copies,
            currentAlpha = 1f,
            blocking = true
        };
    }

    void ApplyAlpha(Material[] mats, float alpha)
    {
        foreach (var mat in mats)
        {
            if (mat == null) continue;
            Color c = mat.GetColor("_BaseColor");
            c.a = alpha;
            mat.SetColor("_BaseColor", c);
        }
    }

    void Restore(FadeState state)
    {
        if (state.renderer != null)
            state.renderer.sharedMaterials = state.originalMaterials;

        foreach (var mat in state.fadeMaterials)
            if (mat != null) Destroy(mat);
    }

    void MakeTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1f);                                       // Transparent
        mat.SetFloat("_Blend", 0f);                                         // Alpha
        mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)RenderQueue.Transparent;
    }

    void OnDestroy()
    {
        foreach (var state in tracked.Values)
            Restore(state);
    }
}
