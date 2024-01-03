using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OSCSender : MonoBehaviour
{
    [SerializeField] string[] ip = {"192.168.0.102", "192.168.0.106", "192.168.0.109"};
    [SerializeField] int port = 12000;
    OscClient[] client = new OscClient[3];
    void Start()
    {
        client[0] = new OscClient(ip[0], port);
        client[1] = new OscClient(ip[1], port);
        client[2] = new OscClient(ip[2], port);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha0)) Send(0);
        // else if (Input.GetKeyDown(KeyCode.Alpha1)) Send(1);
    }

    public void Send(int num, int val)
    {
        if(num == 1) client[0].Send("/servo", val);
        else if(num == 2) client[1].Send("/servo", val);
        else if(num == 3) client[2].Send("/servo", val);
        Debug.Log("Client"+num+" send: " + val);
    }

    private void OnDestroy()
    {
        client[0].Dispose();
        client[1].Dispose();
        client[2].Dispose();
    }
}
