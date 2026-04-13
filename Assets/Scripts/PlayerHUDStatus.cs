using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class PlayerHUDStatus : MonoBehaviour
{

    public UIDocument uiDocument;


	[Header("Player")]
    public Health health;
    public Energy energy;


    
    

	private VisualElement eBox;
	private VisualElement hBox;
	private VisualElement portraitImage;
    // public VisualElement characterNameText;

    void Start()
    {

    
        var root = uiDocument.rootVisualElement;
        eBox = root.Q<VisualElement>("EnergyBar"); // name from UI Builder
        hBox = root.Q<VisualElement>("HealthBar"); // name from UI Builder
        portraitImage = root.Q<VisualElement>("PlayerPortrait"); // name from UI Builder

        
        var manager = CharacterManager.Instance;
        if (manager != null)
        {
            
            var config = manager.config;

			// if (characterNameText != null)
				// characterNameText.text = config.characterName;

			if (portraitImage != null && File.Exists(config.portraitPath))
			{
				var bytes = File.ReadAllBytes(config.portraitPath);
				var tex   = new Texture2D(2, 2);
				tex.LoadImage(bytes);
				portraitImage.style.backgroundImage = new StyleBackground(tex);

			}
			else if (portraitImage != null)
			{
				Debug.LogWarning("[CharacterPortraitDisplay] Portrait file not found: " + config.portraitPath);
			}
            
        }else{
        	Debug.LogWarning("[PlayerHUDStatus] No CharacterManager found.");

        }
        
        
    }


    void Update()
    {

        if(energy != null){
        	SetEnergy(energy.currentEnergy, energy.maxEnergy);    
        }
        if(health != null){
        	SetHealth(health.currentHealth, health.maxHealth);    
        }        
        
    }


	public void SetEnergy(float current, float max)
    {
        float percent = current / max;
        // Debug.Log($"Setting energy to {percent}");
        eBox.style.width = Length.Percent(percent * 100f);
    }

	public void SetHealth(float current, float max)
    {
        float percent = current / max;
        // Debug.Log($"Setting energy to {percent}");
        hBox.style.width = Length.Percent(percent * 100f);
    }


}