using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


public class NoteManager : MonoBehaviour
{
    public bool create = false;
    public void Load_Notes_from_file()
    {
        if (!create)
        {
            // load des notes 
            string FilePath = Application.dataPath + "/GuitareHero_Lvl/Niveaux/" + PlayerPrefs.GetInt("Level").ToString() + ".txt";
            StreamReader reader = new StreamReader(FilePath);
            string str_pos_g1 = reader.ReadLine();
            string str_pos_g2 = reader.ReadLine();
            string pos_win_str = reader.ReadLine();
            List<float> pos_g1, pos_g2;
            pos_g1 = Str_2_list(str_pos_g1);
            pos_g2 = Str_2_list(str_pos_g2);
            PlayerPrefs.SetInt("Count_notes_red", pos_g1.Count);
            PlayerPrefs.SetInt("Count_notes_green", pos_g2.Count);
            PlayerPrefs.SetInt("Count_notes", PlayerPrefs.GetInt("Count_notes_red") + PlayerPrefs.GetInt("Count_notes_green"));
            print("Nb notes : " + PlayerPrefs.GetInt("Count_notes"));



            float pos_win = float.Parse(pos_win_str);
            load_notes("G", pos_g1, pos_g2);
            load_notes("D", pos_g1, pos_g2);
            Object Winnote = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Winnote.prefab", typeof(GameObject));
            Vector3 pos = new Vector3(0, pos_win, 0);
            GameObject copy = Instantiate(Winnote, pos, Quaternion.identity) as GameObject;

        }

    }

    void load_notes(string side, List<float> pos_g1, List<float> pos_g2)
    {
        float[] pos_acts = Find_act_pos(side);
        float gauche = pos_acts[0], droite = pos_acts[1];
        Object Note = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Note_R.prefab", typeof(GameObject));
        GameObject list_g1 = GameObject.Find("Notes_list_"+side);
        foreach (float _ in pos_g1)
        {
            Vector3 pos = new Vector3( gauche, _, 0f );
            GameObject copy = Instantiate(Note, pos, Quaternion.identity,list_g1.transform) as GameObject;
        }
        Note = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Note_V.prefab", typeof(GameObject));
        foreach (float _ in pos_g2)
        {
            Vector3 pos = new Vector3(droite, _, 0f);
            GameObject copy = Instantiate(Note, pos, Quaternion.identity, list_g1.transform) as GameObject;
        }
    }

    List<float> Str_2_list(string entry)
    {
        List<float> Output = new List<float>();
        string Temp = "";
        for (int k=0;k<entry.Length;k++)
        {
            string _ = entry.Substring(k, 1);
            if (_ == ";")
            {
                Output.Add(float.Parse(Temp));
                Temp = "";
            }
            else
                Temp += _;
        }
        return Output;
    }


    float[] Find_act_pos(string side)
    {
        float gauche, droite;
        GameObject Acts = GameObject.Find("Activators"+side);
        Transform[] Trans_acts = Acts.GetComponentsInChildren<Transform>();
        gauche = Trans_acts[0].position[0]; // on initialise la recherche du minimum et maximum
        droite = gauche;                   // pour trouver les positions des activateurs.
        foreach (Transform items in Trans_acts)
        {
            if (gauche > items.position[0])
                gauche = items.position[0];
            if (droite < items.position[0])
                droite = items.position[0];
        }
        float[] res = { gauche, droite };
        return res;
    }

    public void Save_Notes()
    {
        string FilePath = Application.dataPath + "/Niveaux";
        string[] truc = Directory.GetFiles(FilePath);
        int Level_nb = truc.Length/2+1;
        FilePath += "/"+Level_nb.ToString()+".txt";


        List<float> positions_g1 = new List<float>();
        List<float> positions_g2 = new List<float>();
        float gauche, droite;
        float[] pos_act = Find_act_pos("G");
        gauche = pos_act[0];
        droite = pos_act[1];
        
        Transform[] Notes;
        Notes = GetComponentsInChildren<Transform>();
        float maxi = 0;
        foreach (Transform N in Notes)
        {
            if (N.tag == "Note") {
                float[] _ = { N.position[0], N.position[1] };
                if (Mathf.Abs(_[0] - gauche) <= 0.3)
                    positions_g1.Add(_[1]);
                if (Mathf.Abs(_[0] - droite) <= 0.3)
                    positions_g2.Add(_[1]);
                if (_[1] > maxi)
                    maxi = _[1];
            }
        }
        string Towrite="";
        foreach (float _ in positions_g1)
        {
            Towrite += _.ToString() + ";";
        }
        Towrite += "\n";
        foreach (float _ in positions_g2)
        {
            Towrite += _.ToString() + ";";
        }
        Towrite += "\n"+(maxi+20).ToString();


        File.WriteAllText(FilePath, Towrite);

    }
     
}

