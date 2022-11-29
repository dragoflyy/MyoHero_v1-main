using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScroll : MonoBehaviour
{
    List<string> Descriptions = new List<string>(), musics = new List<string>();
    GameObject text_song,text_desc;

    void Start()
    {
        PlayerPrefs.SetInt("Level", 1);
        text_song = GameObject.Find("Song_txt");
        text_desc = GameObject.Find("Desc");
        Dropdown Barre = GetComponent<Dropdown>();
        Barre.onValueChanged.AddListener(delegate { OnSubmit(Barre); });
        // load all levels
        string FilePath = Application.dataPath + "/GuitareHero_Lvl/Niveaux.txt";
        StreamReader reader = new StreamReader(FilePath);
        string line;
        List<string> List_lvl = new List<string>();
        int k = 1;
        Barre.ClearOptions();
        while (true) {
            line = reader.ReadLine();
            if (line == null) {
                break;
            }
            Decoup_line(line);
            List_lvl.Add(k.ToString());

            if (k > 100)
            { // On a defini un max de 100 niveaux !! 
                Debug.Log("100 recherches de niveau dépassé !");
                break;
            }
            k++;
        }
        // Update drowdown list
        Barre.AddOptions(List_lvl);
        Update_Gui(0);


    }

    void Decoup_line(string line)
    {
        int chiffre_len=0,music_len = 0, desc_len = 0, state = 0;
        for (int k = 0; k < line.Length; k++)
        {
            if (line.Substring(k, 1) == ";")
                state++;
            else
            {
                if (state == 0)
                    chiffre_len++;
                if (state == 1)
                    music_len++;
                if (state == 2)
                    desc_len++;
            }

        }
        musics.Add(line.Substring(chiffre_len+1,music_len));
        Descriptions.Add(line.Substring(music_len + chiffre_len +2, desc_len));
    }

    public void OnSubmit(Dropdown Barre)
    {
        Update_Gui(Barre.value);
    }

    public void Update_Gui(int nb)
    {
        PlayerPrefs.SetInt("Level", nb+1);
        text_song.GetComponent<Text>().text = musics[nb];
        text_desc.GetComponent<Text>().text = Descriptions[nb];
    }

    void Update()
    {
        
    }
}
