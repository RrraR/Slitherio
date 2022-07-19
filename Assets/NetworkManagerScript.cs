
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkManagerScript : MonoBehaviour
{
    private RoomInfo[] roomsList;

    public GameObject ourSnakeHead;
    public bool isAlive;
    private string username;

    void Start()
    {
        isAlive = true;
        for (int i = 0; i < 3; i++)
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
            if (PhotonNetwork.Server == ServerConnection.MasterServer)
            {
                Debug.Log("Connection Successfull");
                PhotonNetwork.JoinLobby();
                isAlive = true;
                break;
            }
        }

        if (PhotonNetwork.Server != ServerConnection.MasterServer)
        {
            isAlive = false;
        }
    }

    private void Update()
    {
        if (!isAlive)
        {
            Destroy(ourSnakeHead);
            PhotonNetwork.LeaveRoom(true);
            Application.Quit();
        }
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Join"))
            {
                PhotonNetwork.JoinOrCreateRoom("Sliterio", new RoomOptions() {MaxPlayers = 15}, null);
            }


            // if (roomsList != null)
            // {
            //     if (GUI.Button(new Rect(100, 250, 250, 100), "Join this room"))
            //     { 
            //         PhotonNetwork.JoinRandomRoom();
            //     }
            //
            //    
            //     
            //     // for (int i = 0; i < roomsList.Length; i++)
            //     // {
            //     //     if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join this room"))
            //     //     {
            //     //         PhotonNetwork.JoinRoom(roomsList[i].Name);
            //     //     }
            //     // }
            // }
        }
    }

    void OnJoinedLobby()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to the room");
        // Vector3 headPos = new Vector3();
        // if (PhotonNetwork.room.PlayerCount == 1)
        // {
        //     headPos = new Vector3(
        //         Random.Range(
        //             Random.Range(transform.position.x - 50, transform.position.x - 40),
        //             Random.Range(transform.position.x + 40, transform.position.x + 50)
        //         ), Random.Range(
        //             Random.Range(transform.position.y - 50, transform.position.y - 40),
        //             Random.Range(transform.position.y + 40, transform.position.y + 50)
        //         ), transform.position.z);
        // }
        // else
        // {
        //     Debug.Log("searching for others");
        //     GameObject player0 = GameObject.Find("Enemy");
        //     
        //     if (player0 != null)
        //     {
        //         Debug.Log("found player0");
        //     }
        //     headPos = new Vector3(
        //         Random.Range(
        //             Random.Range(transform.position.x - 50, transform.position.x - 40),
        //             Random.Range(transform.position.x + 40, transform.position.x + 50)
        //         ), Random.Range(
        //             Random.Range(transform.position.y - 50, transform.position.y - 40),
        //             Random.Range(transform.position.y + 40, transform.position.y + 50)
        //         ), player0.transform.position.z);
        // }
        // //GameObject player0 = GameObject.FindGameObjectWithTag("Player");
        // PhotonNetwork.Instantiate(ourSnakeHead.transform.name, headPos, Quaternion.identity, 0);
        Vector3 headPos = new Vector3(
            Random.Range(
                Random.Range(transform.position.x - 50, transform.position.x - 40),
                Random.Range(transform.position.x + 40, transform.position.x + 50)
            ), Random.Range(
                Random.Range(transform.position.y - 50, transform.position.y - 40),
                Random.Range(transform.position.y + 40, transform.position.y + 50)
            ), transform.position.z);
        PhotonNetwork.Instantiate(ourSnakeHead.transform.name, headPos, Quaternion.identity, 0);
        isAlive = true;
    }
}