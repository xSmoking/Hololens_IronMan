using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSys : MonoBehaviour
{

    public Material[] mats;
    public float maxDistance;
    public float maxSpeed;
    public float smoothTime;

    private Vector3 initialOffset;
    private Vector3 velocity;

    private void Start()
    {
        initialOffset = transform.localPosition;
    }

    private void Update()
    {
        Transform cam = Camera.main.transform;

        Vector3 rotatedOffset = cam.right * initialOffset.x + cam.up * initialOffset.y + cam.forward * initialOffset.z;
        Vector3 targetPosition = cam.position + cam.forward + rotatedOffset;
        Vector3 currentPosition = transform.position;

        Vector3 toTarget = targetPosition - currentPosition;

        if (toTarget.magnitude > maxDistance)
        {
            transform.localPosition = targetPosition + toTarget * maxDistance;
        }
        else
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime, maxSpeed);
        }

        transform.LookAt(cam);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - cam.eulerAngles.z);
    }
    public void OnActivate()
    {
        GetComponent<Renderer>().material = mats[0];
    }

    public void OnDeActivate()
    {
        GetComponent<Renderer>().material = mats[1];
    }
    
    void hide()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    void activate()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

}
