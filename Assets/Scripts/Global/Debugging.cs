using UnityEngine;
using Newtonsoft.Json;

public class Debugging : MonoBehaviour
{

    void Start()
    {
        if (DataManager.myPlayer == null) {
            DataManager.myPlayer = new Player() { id = "ID0", name = "Player0", };
        }
    }
}
