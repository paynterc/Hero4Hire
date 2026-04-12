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
            var follow = cam.GetComponent<CameraFollow>();
            if (follow != null)
                follow.target = player.transform;
        }
    }
}