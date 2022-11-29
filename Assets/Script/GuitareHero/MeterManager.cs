
using UnityEngine;
using System.Collections;
using QTMRealTimeSDK;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEditor;




public class SimpleMovingAverage // classe filtre moyenne glissante.
{
    private readonly int _k;
    private readonly float[] _values;

    private int _index = 0;
    private float _sum = 0;

    public SimpleMovingAverage(int k)
    {

        _k = k;
        _values = new float[k];
    }

    public double Update(float nextInput)
    {
        _sum = _sum - _values[_index] + nextInput;

        _values[_index] = nextInput;

        _index = (_index + 1) % _k;

        return ((double)_sum) / _k;
    }
}




    namespace QualisysRealTime.Unity
{
    class MeterManager : MonoBehaviour
    {
        public string place;
        public string ChannelName = "";
        public int number;
        public bool isContracted = false;
        public float delay = 0.1f;
        protected RTClient rtClient;
        public float seuil_high = 50.0f;
        public float seuil_low = 10;
        public float offset = 0;
        public float max = 300;
        public float valeur;
        float x;
        int somme1;
        float somme2;
        bool  relax = false, on_tst=false;
        GameObject jauge, ok,text_val,seuil_cap;
        SimpleMovingAverage calculator;
        public bool bo_Aff_val = false;
        float maximum = 1;
        float val_extr=0, val_f_ext=0;

        void Start()
        {
            // try to load settings.
            string name = this.name;
            ChannelName = PlayerPrefs.GetString("AnalogSource" + name.Substring(name.Length - 1, 1));
            if (PlayerPrefs.GetFloat("sh" + place) != 0)
                seuil_high = PlayerPrefs.GetFloat("sh" + place);
            if (PlayerPrefs.GetFloat("sl" + place) != 0)
                seuil_low = PlayerPrefs.GetFloat("sl" + place);
            if (PlayerPrefs.GetFloat("offset" + place) != 0)
                offset = PlayerPrefs.GetFloat("offset" + place);
            if (PlayerPrefs.GetFloat("maxb" + place) != 0)
                max = PlayerPrefs.GetFloat("maxb" + place);
            if (float.IsNaN(max))
                max = 1;

            rtClient = RTClient.GetInstance();
            ok = GameObject.Find("Text"+number.ToString());
            jauge = transform.Find("Jauge").gameObject;
            text_val = GameObject.Find("Text_val" + number.ToString());
            seuil_cap = transform.Find("seuil_cap").gameObject;
            isContracted = false;
            somme1 = 0;
            somme2 = 0;
            calculator = new SimpleMovingAverage(k: 150);
        }

        void Update()
        {
            ChannelName = PlayerPrefs.GetString("AnalogSource" + name.Substring(name.Length - 1, 1));
            if (isContracted)
                ok.GetComponent<Text>().color = new Color(0, 243, 0);
            else 
                ok.GetComponent<Text>().color = new Color(243, 0, 0);
            if (rtClient == null) rtClient = RTClient.GetInstance();
            var channel = rtClient.GetAnalogChannel(ChannelName);
            if (channel != null)
            {
                foreach (var value in channel.Values)
                {
                    val_extr = value;
                    valeur = value;
                    //print(offset);
                    double val = calculator.Update(Mathf.Abs(value - offset));
                    //print("val : "+val.ToString());
                    if (double.IsNaN(val))
                        val = 0;
                    val_f_ext = (float)val;
                    if (!bo_Aff_val)
                        StartCoroutine(Aff_val(val));

                    x = 7*((float)val / max);
                    jauge.transform.localScale = new Vector3(0.97f, x, 1);
                    jauge.transform.localPosition = new Vector3(0, x / 2 - 7.3f / 2, 0);
                    somme1++;
                    if (relax)
                        somme2 += (float)value;
                    else
                        somme2 += (float)val;
                    if (val > maximum)
                    {
                        maximum = (float)val;
                    }
                    if (!relax && !on_tst && !isContracted && val > seuil_high)
                    {
                        isContracted = true;
                        StartCoroutine(Contraction());

                    }
                }
            }
        }

        void Calc_Offset()
        {
            offset = somme2 / somme1;
            if (double.IsNaN(offset))
                offset = 0;
            PlayerPrefs.SetFloat("offset" + place, offset);
        }

        public void Relax(bool start)
        {
            if (start)
            {
                relax = true;
                somme1 = 0;
                somme2 = 0;
            }
            else
            {
                relax = false;
                Calc_Offset();
            }
        }

        void Calc_contr()
        {
            seuil_high = 0.65f * maximum;
            seuil_low = 0.15f * maximum;
            max = maximum;
            seuil_cap.transform.localPosition = new Vector3(0, 7 * (seuil_high / max) - 3.65f, 1);
            PlayerPrefs.SetFloat("sh" + place, seuil_high);
            PlayerPrefs.SetFloat("sl" + place, seuil_low);
            PlayerPrefs.SetFloat("maxb" + place, max);
        }

        public void Seuils(bool start)
        {
            if (start)
            {
                on_tst = true;
                maximum = 1f;
            }
            else
            {
                Calc_contr();
                on_tst = false;
            }
        }

        public float Get_value()
        {
            return val_extr;
        }

        public float Get_value_filt()
        {
            return val_f_ext;
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
        IEnumerator Aff_val(double val)
        {
            bo_Aff_val = true;
            text_val.GetComponent<Text>().text = ((int)val).ToString();
            yield return new WaitForSeconds(0.1f);
            bo_Aff_val = false;

        }
    }
}