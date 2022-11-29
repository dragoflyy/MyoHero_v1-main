// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections;
using QTMRealTimeSDK;
using System.Collections.Generic;

namespace QualisysRealTime.Unity
{
    class RTAnalog : MonoBehaviour
    {
        public string ChannelName = "Put QTM Analog channel name here";
        public bool isContracted = false;
        public float delay = 0.3f;
        protected RTClient rtClient;
        public float seuil_high = 50.0f;
        public float seuil_low = 10;
        public float offset = 0;
        float val;
        int somme1;
        float somme2;
        bool an_relax,relax = false;


        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();
            isContracted = false;
            somme1 = 0;
            somme2 = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //if (somme1 == 50)
            if (rtClient == null) rtClient = RTClient.GetInstance();

            var channel = rtClient.GetAnalogChannel(ChannelName);
            if (channel != null)
            {
                foreach (var value in channel.Values)
                {
                    // TODO::: What do we do with the analog values for this channel...?
                    val = Mathf.Abs(value-offset);
                    somme1++;
                    somme2 += value;
                    if (!relax && an_relax)
                    {
                        Calc_Offset();
                        Debug.Log("offset "+ offset.ToString());
                    }
                    if (!isContracted && val > seuil_high)
                    {
                        print("is contracted sur true !");
                        isContracted = true;
                        StartCoroutine(Contraction());

                    }
                }
            }
            an_relax = relax;
        }

        void Calc_Offset()
        {
            offset = somme2 / somme1;
        }

        public void Relax(bool start)
        {
            if (start) {
                relax = true;
                somme1 = 0;
                somme2 = 0;
            }
            else
                relax = false;
        }
       
        IEnumerator Contraction()
        {
            while (true)
            {
                somme1 = 0;
                somme2 = 0;
                yield return new WaitForSeconds(delay);
                if (somme2 / somme1 < seuil_low)
                {
                    isContracted = false;
                    break;
                }
            }
            
        }
    }
}