using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbGrowthScript : MonoBehaviour
{
    public Vector3 startinSize = Vector3.zero;
    public Vector3 currentSize;
    

    private float rotationSensitvity;
    private float wantedRotation;
    private float currentRotation;
    void Awake(){
        transform.localScale = startinSize;
        rotationSensitvity = Random.Range(-120,120);
    }

    void Update(){
        wantedRotation += Time.deltaTime * rotationSensitvity;
        currentRotation = Mathf.Lerp(currentRotation, wantedRotation, Time.deltaTime * 5);

        currentSize = Vector3.Slerp(currentSize, new Vector3(1.1f,1.2f,0.5f), Time.deltaTime * 5);

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation)); 
        transform.localScale = currentSize;
    }
}
