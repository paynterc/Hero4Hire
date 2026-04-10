using UnityEngine;
using System.IO;

public class PortraitCapture : MonoBehaviour
{
    public Camera portraitCamera;
    public int width  = 256;
    public int height = 256;

    void Awake()
    {
        if (portraitCamera == null)
            portraitCamera = GetComponent<Camera>();

        portraitCamera.enabled = false;
    }

    public Texture2D Capture()
    {
        var rt = new RenderTexture(width, height, 24);
        portraitCamera.targetTexture = rt;
        portraitCamera.Render(); // manually render one frame — camera stays disabled

        RenderTexture.active = rt;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        portraitCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return tex;
    }

    public void SavePortrait(string path)
    {
        var tex = Capture();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Destroy(tex);
    }
}
