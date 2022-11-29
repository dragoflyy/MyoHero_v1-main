using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QualisysRealTime.Unity;
using QTMRealTimeSDK;
using UnityEngine.UI;
using System.IO;

public class DropDownAnalog : MonoBehaviour
{
    List<Dropdown> Barres;
    RTClient rt;
    List<string> names;
    bool on_check = false;
    // Start is called before the first frame update
    void Start()
    {
        names = new List<string>();
        rt = RTClient.GetInstance();
        List<DiscoveryResponse> discoveryResponses = rt.GetServers();
        DiscoveryResponse server = discoveryResponses[0];
        print(server.IpAddress);
        rt.StartConnecting(server.IpAddress, 1024, false, false, false, false, true, false);
        Debut();



    }

    public int[] Load_datas()
    {
        int[] inds = { 0, 0, 0, 0 };
        for (int k=0;k<names.Count;k++)
        {
            for (int _=1;_<5;_++)
            {
                if (PlayerPrefs.GetString("AnalogSource" + _.ToString()) == names[k].Substring(3,names[k].Length-3))
                {
                    inds[_-1] = k;
                }
            }
        }
        return inds;
    }

    public void OnSubmit(Dropdown Barre)
    {
        change_analog_source(Barre);
    }

    public void change_analog_source(Dropdown Barre)
    {
        string nb = Barre.name;
        nb = nb.Substring(nb.Length - 1, 1);
        string ana_choice = names[Barre.value]; // ( on doit enlever l'index )
        PlayerPrefs.SetString("AnalogSource" + nb, ana_choice.Substring(3,ana_choice.Length-3));
        foreach (Transform Meter in GameObject.Find("Jauges").transform)
        {
            Meter.gameObject.GetComponent<MeterManager>().ChannelName = names[Barre.value];
        }
    }


    public void Update_Gui()
    {
        int[] inds = Load_datas();
        int index = 0;
        foreach(Dropdown Barre in Barres)
        {
            Barre.ClearOptions();
            Barre.AddOptions(names);
            Barre.value = inds[index];
            index++;
        }
    }

    public void Debut()
    {
        Barres = new List<Dropdown>();
        foreach (Transform Barre in transform)
        {
            Barres.Add(Barre.gameObject.GetComponent<Dropdown>());
        }
        foreach (Dropdown Barre in Barres)
        {
            Barre.onValueChanged.AddListener(delegate { OnSubmit(Barre); });
            Barre.ClearOptions();
        }
    }

    void Update()
    {
        rt = RTClient.GetInstance();
        var analogChannels = rt.AnalogChannels;
        List<string> test = new List<string>();
        int index = 1;
        foreach (var channel in analogChannels)
        {
            test.Add(index.ToString()+". "+channel.Name);
            index++;
        }
        bool test_eg = true;
        for (int k= 0;k < test.Count;k++) {
            if (test.Count != names.Count)
            {
                test_eg = false;
                break;
            }
            if (test[k]!=names[k])
            {
                test_eg = false;
                break;
            }
        }
        if (!test_eg)
        {
            print("Sources modified");
            names = test;
            Update_Gui();
        }
        StartCoroutine(Check());

    }

    IEnumerator Check()
    {
        if (!on_check)
        {
            on_check = true;
            yield return new WaitForSeconds(1);
            foreach (Dropdown Barre in Barres)
                change_analog_source(Barre);
            
            on_check = false;
        }
    }
}
