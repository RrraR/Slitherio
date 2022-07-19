using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeMovement : Photon.MonoBehaviour
{
    public List<Transform> bodyParts = new List<Transform>();

    private Vector3 pointInWorld;
    private Vector3 mousePosition;
    private Vector3 direction;

    private float radius = 3.0f;

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            MousRotationSnake();
            //InputRotation();
            SpawnOrbManager();
            Running();
            Scaling();
        }

        ColorSnake();
        if (velocity > 0.08f)
        {
            makeSnakeGlow(true);
        }
        else
        {
            makeSnakeGlow(false);
        }
    }

    private Vector3 lastFramePosition;
    private float velocity;

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            MoveForward();
            //Rotation();
            CameraFollow();
            ApplyingStuffForBody();
        }

        velocity = Vector3.Magnitude(transform.position - lastFramePosition);
        lastFramePosition = transform.position;
    }

    public float spawnOrbEveryXSeconds = 1;
    public GameObject orbPrefarb;

    void SpawnOrbManager()
    {
        StartCoroutine("CallEveryFewSeconds", spawnOrbEveryXSeconds);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator CallEveryFewSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        PhotonView photonView = PhotonView.Get(this);
        float radiusSpawn = 5;
        Vector3 randomNewOrbPoition = new Vector3(
            Random.Range(
                Random.Range(transform.position.x - 10, transform.position.x - 5),
                Random.Range(transform.position.x + 5, transform.position.x + 10)
            ), Random.Range(
                Random.Range(transform.position.y - 10, transform.position.y - 5),
                Random.Range(transform.position.y + 5, transform.position.y + 10)
            ), transform.position.z);
        Vector3 direction = randomNewOrbPoition - transform.position;
        Vector3 finalPosition = transform.position + direction.normalized * radiusSpawn;
        int randomNum = Random.Range(1, 10000);
        photonView.RPC("SpawnForEveryBody", PhotonTargets.AllBufferedViaServer, finalPosition, randomNum);
        //photonView.RpcSecure(nameof()SpawnForEveryBody, PhotonTargets.AllBuffered, finalPosition, randomNum);
        StopCoroutine("CallEveryFewSeconds");
    }

    [PunRPC]
    void SpawnForEveryBody(Vector3 _finelPos, int _name)
    {
        GameObject NewOrb = Instantiate(orbPrefarb, _finelPos, Quaternion.identity) as GameObject;
        NewOrb.transform.name = _name.ToString();
        GameObject orbParent = GameObject.Find("Orbs");
        NewOrb.transform.parent = orbParent.transform;
        //Debug.Log("new orb spawned");
    }

    public Material purple, orange, yellow, red, darkBlue, lightBlue, mint, pink, green;

    void ColorSnake()
    {
        for(int i = 0; i < bodyParts.Count; i++)
        {
            if (i % 9 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = green;
            }
            else if (i % 8 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = pink;
            }
            else if (i % 7 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = mint;
            }else if (i % 6 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = lightBlue;
            }
            else if (i % 5 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = darkBlue;
            }
            else if (i % 4 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = red;
            }
            else if (i % 3 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = yellow;
            }
            else if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = orange;
            }
            else
            {
                bodyParts[i].GetComponent<Renderer>().material = purple;
            }
            // int randint = Random.Range(1, 10);
            // bodyParts[i].GetComponent<Renderer>().material = randint switch
            // {
            //     1 => purple,
            //     2 => orange,
            //     3 => yellow,
            //     4 => red,
            //     5 => darkBlue,
            //     6 => lightBlue,
            //     7 => mint,
            //     8 => pink,
            //     9 => green
            // };
        }
    }

    void MousRotationSnake()
    {
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>()
            .ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000.0f);
        mousePosition = new Vector3(hit.point.x, hit.point.y, 0);
        direction = Vector3.Slerp(direction, mousePosition - transform.position, Time.deltaTime * 1);
        direction.z = 0;
        pointInWorld = direction.normalized * radius + transform.position;
        transform.LookAt(pointInWorld);
    }

    public float speed = 3.5f;
    public float currentRotation;
    public float rotationSensitivity = 50.0f;

    // void InputRotation()
    // {
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         currentRotation += rotationSensitivity * Time.deltaTime;
    //     }
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         currentRotation -= rotationSensitivity * Time.deltaTime;
    //     }
    // }

    void MoveForward()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    // void Rotation()
    // {
    //     transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    // }
    [Range(0.0f, 1.0f)] public float smoothTime = 0.15f;

    // ReSharper disable Unity.PerformanceAnalysis
    void CameraFollow()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 cameraVelocity = Vector3.zero;
        camera.position =
            Vector3.SmoothDamp(camera.position,
                new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity,
                smoothTime);
    }

    private int OrbCounter;
    private int currentOrb;
    public int[] growOnThisOrb;
    private Vector3 currentSize = Vector3.one;
    public float growthRate = 0.1f;
    public float bodyPartOverTimeFollow = 0.19f;

    bool SizeUp(int x)
    {
        try
        {
            if (x == growOnThisOrb[currentOrb])
            {
                currentOrb++;
                return false;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            print("No more grow from this point(add more rows). + " + e.StackTrace.ToString());
        }

        return false;
    }

    // public Transform bodyObject;
    //
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.transform.CompareTag("Orb"))
    //     {
    //         Destroy(collision.gameObject);
    //
    //         if (!SizeUp(OrbCounter))
    //         {
    //             OrbCounter++;
    //             if (bodyParts.Count == 0)
    //             {
    //                 Vector3 currentPos = transform.position;
    //                 Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
    //                 //newBodyPart.localScale = currentSize;
    //                 //newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
    //                 bodyParts.Add(newBodyPart);
    //             }
    //             else
    //             {
    //                 Vector3 currentPos = bodyParts[^1].position;
    //                 Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
    //                 // newBodyPart.localScale = currentSize;
    //                 // newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
    //                 bodyParts.Add(newBodyPart);
    //             }
    //         }
    //     }
    // }

    public bool running;
    public float speedWhileRunning = 6.5f;
    public float speedWhileWalking = 3.5f;
    public float bodypartFollowTimeWalking = 0.19f;
    public float bodypartFollowTimeRunning = 0.1f;

    void Running()
    {
        //makeSnakeGlow(running);

        if (bodyParts.Count > 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                speed = speedWhileRunning;
                running = true;
                bodyPartOverTimeFollow = bodypartFollowTimeRunning;
            }

            if (Input.GetMouseButtonUp(0))
            {
                speed = speedWhileWalking;
                running = false;
                bodyPartOverTimeFollow = bodypartFollowTimeWalking;
            }
        }
        else
        {
            speed = speedWhileWalking;
            running = false;
            bodyPartOverTimeFollow = bodypartFollowTimeWalking;
        }

        if (running)
        {
            if (photonView.isMine)
            {
                StartCoroutine("LoseBodyParts");
            }
        }
        else
        {
            bodyPartOverTimeFollow = bodypartFollowTimeWalking;
        }
    }

    void makeSnakeGlow(bool run)
    {
        foreach (Transform bodypart in bodyParts)
        {
            bodypart.Find("glow_orb").gameObject.SetActive(run);
        }
    }

    IEnumerator LoseBodyParts()
    {
        yield return new WaitForSeconds(0.5f);
        if (photonView.isMine)
        {
            photonView.RPC("LoseBodyPartsOnline", PhotonTargets.AllBuffered);
        }

        StopCoroutine("LoseBodyParts");
    }

    [PunRPC]
    void LoseBodyPartsOnline()
    {
        int lastIndex = bodyParts.Count - 1;
        Transform lastBodyPart = bodyParts[lastIndex].transform;
        Instantiate(orbPrefarb, lastBodyPart.position, Quaternion.identity);
        bodyParts.RemoveAt(lastIndex);
        Destroy(lastBodyPart.gameObject);
        OrbCounter--;
    }

    private Vector3 headV;

    void ApplyingStuffForBody()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, currentSize, ref headV, 0.5f);
        foreach (Transform bodyPart_x in bodyParts)
        {
            bodyPart_x.localScale = transform.localScale;
            bodyPart_x.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
        }
    }

    public List<bool> scalingTrack;
    private int size;
    public float followTimeSensitivity;
    public float scaleSensitivity;

    void Scaling()
    {
        scalingTrack = new List<bool>(new bool[growOnThisOrb.Length]);

        size = 0;

        for (int i = 0; i < growOnThisOrb.Length; i++)
        {
            if (OrbCounter >= growOnThisOrb[i])
            {
                scalingTrack[i] = !scalingTrack[i];
                size++;
            }
        }

        currentSize = new Vector3(
            1 + (size * scaleSensitivity),
            1 + (size * scaleSensitivity),
            1 + (size * scaleSensitivity)
        );
        bodypartFollowTimeWalking = (size / 100.0f) + followTimeSensitivity;
        bodypartFollowTimeRunning = bodypartFollowTimeWalking / 2;
    }
}