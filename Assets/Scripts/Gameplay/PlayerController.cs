using UnityEngine;
using DigitalRuby.Threading;
using System.Threading;

public class PlayerController : MonoBehaviour
{
    public bool isMine = false;
    public Vector3 networkReceivedPosition;
    Camera mainCamera;
    NetworkManager networkManager;
    Ray ray;
    RaycastHit hit;
    bool leftMouseButtonDown, rightMouseButtonDown;
    Vector3 dest;
    public float speed = 1.4f;
    Vector3 dir;
    float distanceFromCursor;
    public float lerpFactor = 1.5f;
    Vector3 previousPosition;
    Vector3 newPosition;

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
        if (!isMine)
        {
            transform.position = networkReceivedPosition;
            return;
        }

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

            previousPosition = transform.position;
            newPosition = transform.position
                + (dir * speed * distanceFromCursor * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    private void OnGUI()
    {
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
        if (dest != previousPosition)
        {
            networkManager.BroadcastPosition(newPosition.x, newPosition.z);
            previousPosition = dest;
        }
    }
}

