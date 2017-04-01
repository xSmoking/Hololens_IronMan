using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

#if WINDOWS_UWP
using Windows.System;
using Windows.Devices.Power;
#endif

public class Device : MonoBehaviour
{
    public float remainingBattery = 0;
    public float fullchargeBattery = 1;
    public int batteryPercentage = 0;
    public GameObject[] batterySlots = new GameObject[15];

    // Update is called once per frame
    void Update()
    {
#if WINDOWS_UWP
        var aggBattery = Battery.AggregateBattery; 
        var report = aggBattery.GetReport();

        string rb = report.RemainingCapacityInMilliwattHours.ToString();
        string fb = report.FullChargeCapacityInMilliwattHours.ToString();
        remainingBattery = float.Parse(rb);
        fullchargeBattery = float.Parse(fb);
#endif
        float ratio = (remainingBattery / fullchargeBattery) * 15.0f;
        batteryPercentage = Convert.ToInt32(ratio);

        for (int i = 0; i < 15; ++i)
        {
            Material material = new Material(Shader.Find("Diffuse"));

            if (i < batteryPercentage)
                material.color = Color.green;
            else
                material.color = Color.red;

            batterySlots[i].GetComponent<MeshRenderer>().material = material;
        }
    }
}
