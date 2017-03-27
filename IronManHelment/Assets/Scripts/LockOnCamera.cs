using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    private Vector3 initialPosition;

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 newPos;
        newPos.x = cameraPos.x + initialPosition.x;
        newPos.y = cameraPos.y + initialPosition.y;
        newPos.z = cameraPos.z + initialPosition.z;
        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 newPos;
        newPos.x = cameraPos.x + initialPosition.x;
        newPos.y = cameraPos.y + initialPosition.y;
        newPos.z = cameraPos.z + initialPosition.z;
        transform.position = newPos;
    }
}
