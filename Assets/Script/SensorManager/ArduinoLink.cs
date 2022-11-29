using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using JetBrains.Annotations;

public class ArduinoLink : MonoBehaviour
{
    SerialPort sp;
    float[] Values = new float[2];
    // Start is called before the first frame update
    void Start()
    {
        sp = new SerialPort("COM7", 115200);
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            string v = null;
            v = sp.ReadLine();
            int ii = 0;
            if (v.Split(';').Length == 2)
            {
                foreach (string word in v.Split(';'))
                {
                    Values[ii] = float.Parse(word);
                    ii++;
                }
            }

            
        }
        catch (System.Exception ex) 
        {
            for (int k = 0; k<2; k++)
                Values[k] = 0; 
            Debug.Log("error : " + ex); 
        }
    }

    public float GetValueChannel(int channel)
    {
        return Values[channel];
    }

    void OnApplicationQuit()
    {
        sp.Close();
        Debug.Log("deconnected");
    }
}
