using UnityEngine;
using DigitalRuby.Threading;
using System.Threading;

public class PlayerController : MonoBehaviour
{
    public bool isMine = false;
    Camera mainCamera;
    NetworkManager networkManager;
    RaycastHit hit;
    bool leftMouseButtonDown, rightMouseButtonDown;
    public float lerpFactor = 0.09f, speed = 120f, distanceFromCursor;
    Vector3 newPosition;
    public Vector3 networkReceivedPosition, dest, dir;

    private void Start()
    {
        if (!isMine)
            return;
        networkManager = GameObject.Find("_networkManager").GetComponent<NetworkManager>();
        mainCamera = GameObject.Find("camera").GetComponent<Camera>();
        EZThread.BeginThread(BroadcastPosition);
    }

    private void Update()
    {
        if (Time.deltaTime <= lerpFactor)
        {
            transform.position =
                           Vector3.Lerp(transform.position, networkReceivedPosition, Time.deltaTime / lerpFactor);
        }
        else
        {
            print("oops!");
            transform.position = networkReceivedPosition;
        }
    
        if (!isMine)
            return;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (!leftMouseButtonDown && !rightMouseButtonDown)
            {
                distanceFromCursor = (hit.point - transform.position).magnitude;
                dir = (hit.point - transform.position) / distanceFromCursor;

                if (distanceFromCursor >= Constants.MAX_DIST_FROM_CURSOR_TO_BALL)
                    distanceFromCursor = Constants.MAX_DIST_FROM_CURSOR_TO_BALL;
                else if (distanceFromCursor <= Constants.MIN_DIST_FROM_CURSOR_TO_BALL)
                    distanceFromCursor = 0;
            }

            dest = (dir * speed * distanceFromCursor * Time.deltaTime);
            newPosition = transform.position + dest;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 140, 100, 20), "Lerp factor: " + lerpFactor.ToString());
        GUI.Label(new Rect(10, 160, 100, 20), "Speed: " + speed.ToString());

        if (Input.GetMouseButtonDown(0) && !leftMouseButtonDown)
        {
            leftMouseButtonDown = true;
        }

        if (Input.GetMouseButtonUp(0) && leftMouseButtonDown)
        {
            leftMouseButtonDown = false;
        }

        if (Input.GetMouseButtonDown(1) && !rightMouseButtonDown)
        {
            rightMouseButtonDown = true;
        }

        if (Input.GetMouseButtonUp(1) && rightMouseButtonDown)
        {
            rightMouseButtonDown = false;
        }
    }

    void BroadcastPosition()
    {
        Thread.Sleep(Constants.SEND_PLAYER_POSITION_FREQUENCY);
        networkManager.BroadcastPosition(newPosition.x, newPosition.z);
    }

    public void UpdatePosition(float x, float z)
    {
        networkReceivedPosition = new Vector3(x, 0, z);
        GameObject.Find("bogus").transform.position = networkReceivedPosition;
    }
}


