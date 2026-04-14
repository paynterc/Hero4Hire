using UnityEngine;
																																																		 
public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        var player = Instantiate(playerPrefab, transform.position, transform.rotation);

        var config = CharacterManager.Instance?.config;
        if (config != null)
            player.GetComponent<CharacterPreview>().ApplyConfig(config);

        var cam = Camera.main;
        if (cam != null)
        {
            var follow = cam.GetComponent<CameraFollowSweep>();
            if (follow != null)
                follow.target = player.transform;
        }
        
        
        var health = player.GetComponent<Health>();
        var energy = player.GetComponent<Energy>();
        
        
    	var hud2 = FindFirstObjectByType<PlayerHUDStatus>();
        if (hud2 != null)
        {
        	if(health)
        	{
        		hud2.health = health;
        	}
        	if(energy)
        	{
        		hud2.energy = energy;
        	}        	
        }else{
        	Debug.Log("PlayerHUDStatus not found");
        }
        
    }
}