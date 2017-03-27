using UnityEngine;

public class SerialTest : MonoBehaviour
{
    private bool on = false;

    void Start()
    {
    }

    void Update()
    {
        SerialIO.Write(on ? "1" : "0");
        on = !on;

        string rString = SerialIO.Read();
        Debug.Log(rString);
    }
}