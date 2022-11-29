using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QualisysRealTime.Unity;
using System.Text;
using System.IO;
using System;
using System.Xml;




public class Record
{
    DateTime Start_time;
    TextWriter writer;
    public string FilePath;
    public string Name;
    List<string> Txt_fin;
    XmlTextWriter Xml_record;

    public Record(string N)
    {
        Txt_fin = new List<string>();
        FilePath = Application.dataPath + "/Datas/temp.csv";
        Name = N;
        Start_time = DateTime.Now;
        File.WriteAllText(FilePath, "Time;EMG1;EMG2;EMG3;EMG4;EMG1_F;EMG2_F;EMG3_F;EMG4_F;Note_R;Note_V;\n");
        Txt_fin.Add("Time;EMG1;EMG2;EMG3;EMG4;EMG1_F;EMG2_F;EMG3_F;EMG4_F;Note_R;Note_V;");
        Debug.Log("Fichier Data temp crée");


    }

    public void Write(string text)
    {
        TimeSpan D_rel = DateTime.Now - Start_time;
        writer = File.AppendText(FilePath);
        int str_time = int.Parse((D_rel.ToString()).Substring(4, 1)) * 600000 +int.Parse((D_rel.ToString()).Substring(6,2))*1000 +int.Parse((D_rel.ToString()).Substring(9,3));
        writer.Write(str_time.ToString()+";"+text+"\n");

        Txt_fin.Add(str_time.ToString() + ";" + text);
        Save();
    }

    public void Save()
    {
        
        writer.Close();
    }

    string Date_trait()
    {
        string res = Start_time.ToString();
        res = res.Replace("/", "_");
        res = res.Replace(" ", "_");
        res = res.Replace(":", ".");

        return res;
    }

    public void Save_File(List<string> scores)
    {
        MonoBehaviour.print("Saving...");
        int ind = 0;
        FilePath = Application.dataPath + "/Datas";
        FilePath += "/" + Name + "_" + Date_trait() + ".csv";
        writer = File.AppendText(FilePath);
        foreach ( string _ in Txt_fin)
        {
            string to_write = _;
            if (ind == 0)
                to_write += ";Level " + PlayerPrefs.GetInt("Level") +";"+ scores[ind];
            if (ind < 3 && ind != 0)
                to_write += ";;" + scores[ind];
            ind++;
            to_write += "\n";
            writer.Write(to_write);
        }
        writer.Close();
        MonoBehaviour.print("File Saved as : " + FilePath);
        
    }
}



public class analogManager : MonoBehaviour
{
    public string FilePath;
    public string Name;
    public GameObject[] objs,objs_g,objs_d;
    Record Recorder;
    bool On_mes = false;
    void Start()
    {
        Name = PlayerPrefs.GetString("Name_player");
    }
    void Update()
    {
        PlayerPrefs.SetString("Name_player",Name);
        if (On_mes)
        {
            string txt = "";
            foreach (GameObject objet in objs)
            {
                txt += (System.Math.Round(objet.GetComponent<MeterManager>().Get_value(),2)).ToString();
                txt += ";";
            }
            foreach (GameObject objet in objs)
            {
                txt += (System.Math.Round(objet.GetComponent<MeterManager>().Get_value_filt(), 2)).ToString();
                txt += ";";
            }
            txt += (GameObject.Find("ActivatorsG").transform.Find("Activator").GetComponent<Activator>().active ? 1 : 0).ToString()+";";
            txt += (GameObject.Find("ActivatorsG").transform.Find("Activator (1)").GetComponent<Activator>().active ? 1 : 0).ToString()+";";
            Recorder.Write(txt);
        }
        // on get les values des meters
        // on l'ecrit apres dans le fichier 
        // que si la mesure est sur true
    }
    public void Start_relax()
    {
        foreach (GameObject objet in objs)
        {
            objet.GetComponent<MeterManager>().Relax(true);
        }
    }

    public void Stop_relax()
    {
        foreach (GameObject objet in objs)
        {
            objet.GetComponent<MeterManager>().Relax(false);
        }
    }

    public void Start_contract1()
    {
        foreach (GameObject objet in objs_g)
        {
            objet.GetComponent<MeterManager>().Seuils(true);
        }
    }

    public void Stop_contract1()
    {
        foreach (GameObject objet in objs_g)
            objet.GetComponent<MeterManager>().Seuils(false);
    }

    public void Start_contract2()
    {
        foreach (GameObject objet in objs_d)
        {
            objet.GetComponent<MeterManager>().Seuils(true);
        }
    }

    public void Stop_contract2()
    {
        foreach (GameObject objet in objs_d)
            objet.GetComponent<MeterManager>().Seuils(false);
    }

    public void Start_mesures()
    {
        Recorder = new Record(Name);
        On_mes = true;
    }

    public void Save_all(List<string> scores)
    {
        Recorder.Save_File(scores);
    }
}
