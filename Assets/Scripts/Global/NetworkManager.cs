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

    void Start()
    {
        if (gameClient == null) {
            EventBasedNetListener listener = new EventBasedNetListener();
            listener.PeerDisconnectedEvent += onDisconnected;
            listener.NetworkReceiveEvent += onGotMessage;
            listener.PeerConnectedEvent += onConnected;
            gameClient = new NetManager(listener);
            gameClient.Start();
            gameClient.Connect("localhost", 9090, JsonConvert.SerializeObject(DataManager.myPlayer));
        }
    }

    private void onGotMessage(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        byte type = reader.GetByte();

        if (type == NetworkMessageType.REQUEST_MATCH_DATA)
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
        Debug.Log(disconnectInfo.Reason + ": " + disconnectInfo.AdditionalData.GetString());
    }

    private void Update()
    {
        gameClient.PollEvents();
    }
}
