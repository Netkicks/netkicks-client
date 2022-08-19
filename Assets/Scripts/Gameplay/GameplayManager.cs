using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public Transform ball;
    public Transform[] playerPrefabs = new Transform[3];
    public Transform[] playerObjects = new Transform[Constants.MAX_PLAYERS_PER_MATCH];

    private void Start()
    {
        print("GameplayManager.Start()");
        foreach (Player player in DataManager.currentMatch.players)
        {
            if (player == null)
                continue;
            Transform playerObject = Instantiate(playerPrefabs[0]);
            playerObject.name = player.id;
            playerObject.GetComponent<PlayerController>().isMine = (player.id == DataManager.myPlayer.id);
            playerObjects[player.inGameIndex] = playerObject;
        }
    }

    public void UpdateBall(float x, float y, float z)
    {
        ball.transform.position = new Vector3(x, y, z);
    }
}

