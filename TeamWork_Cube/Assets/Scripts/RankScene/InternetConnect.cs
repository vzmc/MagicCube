using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnect : MonoBehaviour
{
    public int nStatus;
    public const int NotReachable = 0;                   // ネットなし
    public const int ReachableViaLocalAreaNetwork = 1;   // Wifi,ケーブル。
    public const int ReachableViaCarrierDataNetwork = 2; // 3G,4G。

    // Use this for initialization
    void Start()
    {
        // IPhone, Android
        nStatus = ConnectionStatus();

        if (Debug.isDebugBuild)
            Debug.Log("ConnectionStatus : " + nStatus);

        if (nStatus > 0)
            if (Debug.isDebugBuild)
                Debug.Log("ネット繋げる");
        else
            if (Debug.isDebugBuild)
                Debug.Log("ネット繋げない");

        this.StartCoroutine(PingConnect());
    }

    public static int ConnectionStatus()
    {
        int status;

        if (Application.internetReachability == NetworkReachability.NotReachable)
            status = NotReachable;
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            status = ReachableViaLocalAreaNetwork;
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            status = ReachableViaCarrierDataNetwork;
        else
            status = -1;

        return status;
    }

    IEnumerator PingConnect()
    {
        //Google IP
        string googleJP = "172.217.25.100";

        //Ping
        Ping ping = new Ping(googleJP);

        int nTime = 0;

        while (!ping.isDone)
        {
            yield return new WaitForSeconds(0.1f);

            if (nTime > 20) // time 2 sec, OverTime
            {
                nTime = 0;
                if (Debug.isDebugBuild)
                    Debug.Log("ネット繋げない : " + ping.time);
            }
            nTime++;
        }
        yield return ping.time;

        if (Debug.isDebugBuild)
            Debug.Log("ネット繋げる");
    }

}
