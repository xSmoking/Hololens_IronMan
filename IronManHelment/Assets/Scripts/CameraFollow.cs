using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float maxSpeed = 0;
    public float smoothTime = 0;

    private Vector3 initialOffset = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        initialOffset = transform.position;
    }

    private void Update()
    {
        Transform cam = Camera.main.transform;

        Vector3 rotatedOffset = cam.right * initialOffset.x + cam.up * initialOffset.y + cam.forward * initialOffset.z;
        Vector3 targetPosition = cam.position + cam.forward + rotatedOffset;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime, maxSpeed);

        transform.LookAt(cam);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - cam.eulerAngles.z);
    }
}