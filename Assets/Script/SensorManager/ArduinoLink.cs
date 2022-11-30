using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using JetBrains.Annotations;
using System.Threading;

public class ArduinoLink : MonoBehaviour
{
    SerialPort sp;
    float[] Values = new float[2], av;
    // Start is called before the first frame update
    void Start()
    {
        Values = new float[] { 0, 0 };
        string[] pn = SerialPort.GetPortNames();
        foreach(string pns in pn)
        {
            try
            {
                System.DateTime N = System.DateTime.Now;
                sp = null;
                bool timeout = true;

                Thread t = new Thread(()=>{ sp = new SerialPort(pns, 115200); timeout = false; });
                t.Start();

                while ((System.DateTime.Now - N).Seconds < 2) { }
                t.Interrupt();

                if (!timeout)
                {
                    sp.Open();
                    sp.ReadTimeout = 1;
                    Debug.Log("connected to " + pns);

                    bool coo = false;

                    for (int iii = 0; iii < 5; iii++)
                    {
                        try
                        {
                            sp.ReadLine();
                            Debug.Log("connected ! ");
                            coo = true;
                            break;
                        }
                        catch { }
                        Thread.Sleep(10);
                    }
                    if (!coo)
                    { sp.Close(); }
                    else { break; }
                }
            } catch (System.Exception ex) { Debug.Log("error : "+ex); }
        }
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
