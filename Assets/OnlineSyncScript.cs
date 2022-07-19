using UnityEngine;

public class OnlineSyncScript : Photon.MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 20;
    }

    void FixedUpdate()
    {
        SmoothMovement();
    }

    void SmoothMovement()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * 5);
        }
    }

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3) stream.ReceiveNext();
            realRotation = (Quaternion) stream.ReceiveNext();
        }
    }
}