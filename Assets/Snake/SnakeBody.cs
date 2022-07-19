using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : Photon.MonoBehaviour
{
    private int myOrder;

    public Transform head;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (head.GetComponent<SnakeMovement>().bodyParts.Count != 0)
        {
            //head = GameObject.FindGameObjectWithTag("Player").transform;
            for (int i = 0; i < head.GetComponent<SnakeMovement>().bodyParts.Count; i++)
            {
                if (gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject && gameObject != null)
                {
                    myOrder = i;
                }
            }
        }
    }

    private Vector3 movementVelocity;
    [Range(0.0f, 1.0f)] public float overTime = 0.5f;

    private void FixedUpdate()
    {
        if (myOrder == 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, overTime);
            transform.LookAt(head.transform.position);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position, ref movementVelocity, overTime);
            transform.LookAt(head.transform.position);
        }
    }
}