using UnityEngine;
using System.IO;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public CharacterConfig config = new CharacterConfig();

    public string SavePath    => Path.Combine(Application.persistentDataPath, "character.json");
    public string PortraitPath => Path.Combine(Application.persistentDataPath, "portrait.png");

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void Save()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(config, true));
    }

    public void Load()
    {
        if (File.Exists(SavePath))
            JsonUtility.FromJsonOverwrite(File.ReadAllText(SavePath), config);
    }
}
