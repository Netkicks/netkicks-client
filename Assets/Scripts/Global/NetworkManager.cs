using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetManager gameClient;
    public  GameplayManager gameplayManager;

    void Start()
    {
        if (gameClient == null)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            listener.PeerDisconnectedEvent += onDisconnected;
            listener.NetworkReceiveEvent += onGotMessage;
            listener.PeerConnectedEvent += onConnected;
            gameClient = new NetManager(listener);
            gameClient.Start();
            SceneManager.sceneLoaded += onSceneWasLoaded;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Connect()
    {
        gameClient.Connect("localhost", 9090, JsonConvert.SerializeObject(DataManager.myPlayer));
    }

    private void onSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "gameplay")
            gameplayManager = GameObject.Find("_gameplayManager").GetComponent<GameplayManager>();
    }

    private void onGotMessage(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        byte type = reader.GetByte();

        if (type == NetworkMessageType.BALL_POSITION_UPDATE)
        {
            if (gameplayManager != null)
                gameplayManager.UpdateBall(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        else if (type == NetworkMessageType.REQUEST_MATCH_DATA)
        {
            DataManager.currentMatch = JsonConvert.DeserializeObject<Match>(reader.GetString());
            SceneManager.LoadScene("gameplay");
        }
    }

    private void onConnected(NetPeer peer)
    {
        NetDataWriter writer =
            Utils.GetNetDataWriter(NetworkMessageType.REQUEST_MATCH_DATA);
        writer.Put(JsonConvert.SerializeObject(DataManager.myPlayer));
        gameClient.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    private void onDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        string reason = disconnectInfo.Reason.ToString();
        string details;
        if (disconnectInfo.AdditionalData != null && disconnectInfo.AdditionalData.TryGetString(out details))
            reason += " - " + details;
        if (SceneManager.GetActiveScene().name == "gameplay")
            SceneManager.LoadScene("lobby");
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name == "login")
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "LOGIN"))
            {
                SceneManager.LoadScene("lobby");
            }
        }
        else if (SceneManager.GetActiveScene().name == "lobby")
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "CONNECT"))
            {
                Connect();
            }
        }
        else if (SceneManager.GetActiveScene().name == "gameplay")
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "DISCONNECT"))
            {
                gameClient.DisconnectAll();
            }
        }

    }

    private void Update()
    {
        gameClient.PollEvents();
    }

    private void OnApplicationQuit()
    {
        gameClient.DisconnectAll();
    }
}
