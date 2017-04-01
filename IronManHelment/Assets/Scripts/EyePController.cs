using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePController : MonoBehaviour
{
    public GameObject[] battery;
    public GameObject[] objects = new GameObject[5];
    public Material[] mats = null;
    public int index = 0;
    public int dis = 0;
    private bool activated = false;
    public GameObject distanceText = null;

    private int check = 4;

    // Use this for initialization
    void Update()
    {
        if (check-- > 0)
        {
            //return;
        }
        //for (int i = 0; i < battery.Length; i++)
        //{
        //    //Renderer rend = battery[i].gameObject.GetComponent<Renderer>();
        //    if (index < i)
        //    {
        //        battery[i].GetComponent<Renderer>().material = mats[1];
        //    }
        //    else
        //    {
        //        battery[i].GetComponent<Renderer>().material = mats[0];
        //    }
        //}

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;


        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram...
            print("hit");
            // Display the cursor mesh.

            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;
            // cursor.transform.position = hitInfo.point;
            // Rotate the cursor to hug the surface of the hologram.
            //cursor.transform.rotation =
            //Quaternion.FromToRotation(Vector3.up, hitInfo.normal);


            //var currentPos = objects[0].transform.position;
            //float dist = Vector3.Distance(new Vector3(0f, 0f, 0f), hitInfo.point) / 9;


            var currentPos = Camera.main.transform.position;
            //currentPos.z -= 90;
            float dist = (Vector3.Distance(currentPos, hitInfo.point) * 100) / objects.Length;
            var pos = objects[0].transform.localPosition;
            var value = Vector3.Distance(currentPos, hitInfo.point);
            Debug.Log(value);

            TextMesh t = (TextMesh)distanceText.GetComponent(typeof(TextMesh));
            t.text = value.ToString("F2");

            for (int i = 1; i < objects.Length; i++)
            {
                objects[i].transform.localPosition = Vector3.Lerp(pos, new Vector3(pos.x, pos.y, pos.z + dist), 0.8f);
                if (i > 1 && activated)
                {
                    objects[i].transform.Rotate(0, 0, i * dist * Time.deltaTime);
                }
            }
        }
        else
        {
            // If the raycast did not hit a hologram, hide the cursor mesh.

        }

        check = 4;
    }

    public void Rotate()
    {
        activated = true;
        for (int i = 2; i < objects.Length; i++)
        {
            objects[i].transform.Rotate(0, 0, -1 * i * 360 * Time.deltaTime);
        }
    }

    public void StopRotate()
    {
        activated = false;
    }

    public IEnumerator OnToggle()
    {
        var url = "10.0.1.6/toggle";
        WWW www = new WWW(url);
        yield return www;
    }


    // Update is called once per frame

}
