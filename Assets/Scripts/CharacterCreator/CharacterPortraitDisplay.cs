using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CharacterPortraitDisplay : MonoBehaviour
{
    public RawImage portraitImage;
    public TMP_Text characterNameText;

    void Start()
    {
        var manager = CharacterManager.Instance;
        if (manager == null)
        {
            Debug.LogWarning("[CharacterPortraitDisplay] No CharacterManager found.");
            return;
        }

        var config = manager.config;

        if (characterNameText != null)
            characterNameText.text = config.characterName;

        if (portraitImage != null && File.Exists(config.portraitPath))
        {
            var bytes = File.ReadAllBytes(config.portraitPath);
            var tex   = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            portraitImage.texture = tex;
        }
        else if (portraitImage != null)
        {
            Debug.LogWarning("[CharacterPortraitDisplay] Portrait file not found: " + config.portraitPath);
        }
    }
}
