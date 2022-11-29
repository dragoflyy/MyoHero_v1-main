using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    SerialPort sp;
    void Start()
    {
        Debug.Log("Starting");
        sp = new SerialPort("COM4", 115200);
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            Debug.Log(sp.ReadLine());
        } catch(System.Exception)
        {

        }
    }
}
