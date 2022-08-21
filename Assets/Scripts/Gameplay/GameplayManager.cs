using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public Transform ball;
    public Transform[] playerPrefabs = new Transform[3];
    public Transform[] playerObjects = new Transform[Constants.MAX_PLAYERS_PER_MATCH];
    public PlayerController bogus;

    private void Start()
    {
        bogus = GameObject.Find("bogus").GetComponent<PlayerController>(); ;
        foreach (Player player in DataManager.currentMatch.players)
        {
            if (player == null)
                continue;
            Transform playerObject = Instantiate(playerPrefabs[0], new Vector3(player.x, 0, player.z), Quaternion.identity);
            playerObject.name = player.id;
            playerObject.GetComponent<PlayerController>().isMine = (player.id == DataManager.myPlayer.id);
            playerObjects[player.inGameIndex] = playerObject;
        }
    }

    public void UpdateBall(float x, float y, float z)
    {
        ball.transform.position = new Vector3(x, y, z);
    }

    public void UpdatePlayer(byte inGameIndex, float x, float z)
    {   
        bogus.networkReceivedPosition =
            new Vector3(x, playerObjects[inGameIndex].position.y, z);
    }
}



