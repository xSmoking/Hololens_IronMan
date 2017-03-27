using UnityEngine;

public class SphereCommands : MonoBehaviour
{
    Vector3 originalPosition;
    private Renderer[] rs;

    // Use this for initialization
    void Start()
    {
        // Grab the original local position of the sphere when the app starts.
        originalPosition = transform.localPosition;
        rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
            r.enabled = false;
    }

    void OnWakeUp()
    {
        foreach (Renderer r in rs)
            r.enabled = true;
    }

    void OnClose()
    {
        foreach (Renderer r in rs)
            r.enabled = false;
    }

    // Called by SpeechManager when the user says the "Reset world" command
    void OnReset()
    {
        // If the sphere has a Rigidbody component, remove it to disable physics.
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
            DestroyImmediate(rigidbody);

        // Put the sphere back into its original local position.
        this.transform.localPosition = originalPosition;
    }
}
