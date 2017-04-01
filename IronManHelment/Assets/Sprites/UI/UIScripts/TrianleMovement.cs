using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrianleMovement : MonoBehaviour {

    Vector3 initial;
    float totalTime;
    // Use this for initialization
    void Start()
    {
        initial = transform.position;
        totalTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        transform.localPosition = Vector3.Lerp(initial, new Vector3(-0.36f, -0.40f, 4f), totalTime);
    }
}
