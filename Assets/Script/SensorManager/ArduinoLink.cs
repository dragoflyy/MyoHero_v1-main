using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using JetBrains.Annotations;

public class ArduinoLink : MonoBehaviour
{
    SerialPort sp;
    float[] Values = new float[2], av;
    // Start is called before the first frame update
    void Start()
    {
        Values = new float[] { 0, 0 };
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
            Values = av; 
            //Debug.Log("error : " + ex); 
        }
        av = Values;
    }

    public float GetValueChannel(int channel)
    {
        if (Values != null)
            return Values[channel];
        return 0;
    }

    void OnApplicationQuit()
    {
        sp.Close();
        Debug.Log("deconnected");
    }
}
