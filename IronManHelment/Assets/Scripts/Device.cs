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
    public int remainingBattery = 0;
    public int fullchargeBattery = 1;
    public int batteryPercentage = 0;

    //PerformanceCounter cpuCounter;
    //PerformanceCounter ramCounter;

    // Use this for initialization
    void Start()
    {
        //cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        //ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    }

    // Update is called once per frame
    void Update()
    {
#if WINDOWS_UWP
        var aggBattery = Battery.AggregateBattery; 
        var report = aggBattery.GetReport();

        string rb = report.RemainingCapacityInMilliwattHours.ToString();
        string fb = report.FullChargeCapacityInMilliwattHours.ToString();
        remainingBattery = Int32.Parse(rb);
        fullchargeBattery = Int32.Parse(fb);
        batteryPercentage = remainingBattery / fullchargeBattery * 100;
        //batteryInfo.text = batteryPercentage.ToString();
#endif
        //cpuInfo.text = getCurrentCpuUsage();
        //ramInfo.text = getAvailableRAM();
    }

    public string getCurrentCpuUsage()
    {
        //return cpuCounter.NextValue() + "% CPU";
        return "";
    }

    public string getAvailableRAM()
    {
        //return ramCounter.NextValue() + "MB RAM Free";
        return "";
    }
}
