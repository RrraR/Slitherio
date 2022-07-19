using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNetworkScript : Photon.MonoBehaviour
{
    private PhotonView view;
    void Awake()
    {
        view = PhotonView.Get(this);
        if (view.isMine)
            view.RPC("ChangeName", PhotonTargets.AllBuffered, PhotonNetwork.playerList.Length.ToString());
    }

    [PunRPC]
    void ChangeName(string myNewName)
    {
        gameObject.transform.name = myNewName;
    }

    public Transform bodyObject;

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Orb"))
        {
            if (view.isMine)
            {
                view.RPC("AddThisSnakeNewBodyPart", PhotonTargets.AllBuffered, gameObject.transform.name);
                view.RPC("DeleteOrbForOthers", PhotonTargets.AllBuffered, other.gameObject.name);
            }
        }

        if (other.transform.CompareTag("Enemy"))
        {
            if (view.isMine)
            {
                KillBodies();
                GetComponent<NetworkManagerScript>().isAlive = false;
            }
        }
    }

    public GameObject orbPrefabFromRes;

    void KillBodies()
    {
        SnakeMovement sM = gameObject.GetComponent<SnakeMovement>();
        for (int i = 0; i < sM.bodyParts.Count; i++)
        {
            PhotonNetwork.Instantiate(orbPrefabFromRes.name, sM.bodyParts[i].gameObject.transform.position,
                Quaternion.identity, 0);
            Destroy(sM.bodyParts[i].gameObject);
        }

        //PhotonNetwork.DestroyPlayerObjects();
        Destroy(gameObject);
        sM.bodyParts.Clear();
        Application.Quit();
    }

    [PunRPC]
    void DeleteOrbForOthers(string go)
    {
        Destroy(GameObject.Find(go).gameObject);
    }

    [PunRPC]
    void AddThisSnakeNewBodyPart(string gO)
    {
        Transform wantedPlayer = GameObject.Find(gO).transform;

        if (wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Count == 0)
        {
            Vector3 currentPos = wantedPlayer.position;
            Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;

            newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
            if (!view.isMine)
            {
                newBodyPart.tag = "Enemy";
                Debug.Log("Enemy found");
            }

            wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
        }
        else
        {
            Vector3 currentPos = wantedPlayer.GetComponent<SnakeMovement>()
                .bodyParts[^1].position;
            Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;

            newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
            if (!view.isMine)
            {
                newBodyPart.tag = "Enemy";
                Debug.Log("Enemy found");
            }

            wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
        }
    }
}