using QualisysRealTime.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DefiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject jaugeG, jaugeD, Text;

    bool GContracted, DContracted, SGContracted, SDContracted;
    string txt;
    Thread t;

    List<Action> Functions;

    void Start()
    {
        GContracted = false;
        DContracted = false;

        Functions = new List<Action>();
        Functions.Add(ContracG);
        Functions.Add(ContracD);
    }

    // Update is called once per frame
    void Update()
    {
        GContracted = jaugeG.GetComponent<MeterManager>().isContracted;
        DContracted = jaugeD.GetComponent<MeterManager>().isContracted;

        SGContracted = (SGContracted && GContracted);
        SDContracted = (SDContracted && DContracted);

        Text.GetComponent<Text>().text = txt;

        if (t == null)
        {
            int r = UnityEngine.Random.Range(0, Functions.Count);
            t = new Thread(() => { LaunchEvent(r); ; t = null; });
            t.Start();
        }
    }


    void LaunchEvent(int id)
    {
        txt = "Attention !";
        Thread.Sleep(10000);
        Functions[id].Invoke();
    }

    void ContracG()
    {
        txt = "Contracter la gauche !";
        int somme1 = 0;
        int somme2 = 0;

        DateTime N = DateTime.Now;

        while((DateTime.Now - N).Seconds < 5)
        {
            somme1++;
            if (GContracted)
                somme2++;
        }

        float Resultat = ((float)somme2 / somme1);
        Debug.Log("Resultat : " + Resultat);
        txt = "" + Resultat;
    }

    void ContracD()
    {
        txt = "Contracter la droite !";
        int somme1 = 0;
        int somme2 = 0;

        DateTime N = DateTime.Now;

        while ((DateTime.Now - N).Seconds < 5)
        {
            somme1++;
            if (DContracted)
                somme2++;
        }

        float Resultat = ((float)somme2 / somme1);
        Debug.Log("Resultat : " + Resultat);
        txt = "" + Resultat;
    }

    public void Quit()
    {

    }
}
