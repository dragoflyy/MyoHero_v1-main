using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using QualisysRealTime.Unity;
using QTMRealTimeSDK;
using System.Collections.Generic;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    int[] multiplier= { 1, 1 };
    int[] streak = { 0, 0 };
    int rm;
    GameObject[] flammes;
    GameObject flamme_long;
    GameObject Explode;
    public int cote;
    string Side;
    GameObject texte,sound,b1,b2, analog_manager,bkground,bpause,bstop,bsave,bback;
    private List<DiscoveryResponse> discoveryResponses;
    bool on_p=true;
    DateTime Start_time,PauseTime;
    TimeSpan Decalage;
    public bool create = false;
    public GameObject[] ToErase;

    void Start()
    {
        PlayerPrefs.SetInt("Start", 0);
        if (create)
            PlayerPrefs.SetInt("Create", 1);
        else
            PlayerPrefs.SetInt("Create", 0);

        try
        {
            bpause = GameObject.Find("button_pause");
            bpause.SetActive(false);
            bstop = GameObject.Find("button_stop");
            bstop.SetActive(false);
            bsave = GameObject.Find("button_save");
            bsave.SetActive(false);
            bback = GameObject.Find("button_back");
            bback.SetActive(false);
        } catch { }
     

        

        sound = GameObject.Find("Sound_loss");
        if (cote == 0)
        {
            texte = GameObject.Find("ready");
            texte.SetActive(false);
            analog_manager = GameObject.Find("Jauges");
            b1 = GameObject.Find("Button_start");
        }
        else
        {
            if (PlayerPrefs.GetInt("Solo") == 1)
            {
                foreach (GameObject o in ToErase)
                    o.SetActive(false);
                gameObject.SetActive(false);
            }
            else
            {
                GameObject.Find("Cache").SetActive(false);
            }
        }
            try
            {
                discoveryResponses = RTClient.GetInstance().GetServers();
                DiscoveryResponse server = discoveryResponses[0];
                print(server.IpAddress);
                RTClient.GetInstance().StartConnecting(server.IpAddress, 1025, false, false, false, false, true, false);
            }
            catch { }

        if (!create)
        {
            if (cote == 0)
            {
                // lecture et chargement des musiques/notes :
                string Path = Application.dataPath + "/GuitareHero_Lvl/Niveaux.txt";
                StreamReader reader = new StreamReader(Path);
                string line = "";
                for (int k = 0; (k < PlayerPrefs.GetInt("Level")); k++)
                    line = reader.ReadLine();
                int lvl_nb = 0, music_len = 0, desc_len = 0, state = 0;
                for (int k = 0; k < line.Length; k++)
                {
                    if (line.Substring(k, 1) == ";")
                        state++;
                    else
                    {
                        if (state == 0)
                            lvl_nb++;
                        if (state == 1)
                            music_len++;
                        if (state == 2)
                            desc_len++;
                    }

                }
                reader.Close();
                PlayerPrefs.SetInt("Level", int.Parse(line.Substring(0, lvl_nb)));
                PlayerPrefs.SetString("Music_Name", line.Substring(lvl_nb + 1, music_len) + ".wav");
                print(PlayerPrefs.GetString("Music_Name"));
                GameObject.Find("Notes_list_G").GetComponent<NoteManager>().Load_Notes_from_file();
            }
            if (cote == 0)
                Side = "G";
            else
                Side = "D";
            PlayerPrefs.SetInt("Score" + Side, 0);
            PlayerPrefs.SetInt("StreakG", 0);
            PlayerPrefs.SetInt("StreakD", 0);
            PlayerPrefs.SetInt("MultG", 1);
            PlayerPrefs.SetInt("MultD", 1);
            PlayerPrefs.SetInt("RockMeter", 25);
            PlayerPrefs.SetInt("HighStreakG", 0);
            PlayerPrefs.SetInt("HighStreakD", 0);
            PlayerPrefs.SetInt("NotesHit", 0);
            PlayerPrefs.SetInt("NotesHitG" + Side, 0);
            PlayerPrefs.SetInt("NotesHitD" + Side, 0);
            PlayerPrefs.SetInt("ContractionsG" + Side, 0);
            PlayerPrefs.SetInt("ContractionsD" + Side, 0);
            flamme_long = GameObject.Find("flame_long");
            flammes = GameObject.FindGameObjectsWithTag("Flammes" + Side);
            bkground = GameObject.Find("Background");
            Explode = GameObject.Find("Explosion" + Side);
        }
        
        
            // Faire passer les settings d'une scene à une autre !
    }
    public void launch_crea()
    {
        StartCoroutine(launch_game());
    }
    public void launch()
    {
        StartCoroutine(launch_game());
    }

    IEnumerator launch_game()
    {
        texte.SetActive(true);
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("Start", 1);
        if (!create)
            analog_manager.GetComponent<analogManager>().Start_mesures();
        else
        {
            Start_time = DateTime.Now;
            bpause.SetActive(true);
            bstop.SetActive(true);
        }
        texte.SetActive(false);
        b1.SetActive(false);
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
        ResetStreak(cote);
    }

    public void AddStreak(int cote, string act_side)
    {
        if(PlayerPrefs.GetInt("RockMeter")<50)
            PlayerPrefs.SetInt("RockMeter", PlayerPrefs.GetInt("RockMeter") +1);
        streak[cote]++;
        PlayerPrefs.SetInt("NotesHit", PlayerPrefs.GetInt("NotesHit") + 1);
        PlayerPrefs.SetInt("NotesHit" + act_side + Side, PlayerPrefs.GetInt("NotesHit" + act_side + Side) + 1);
        multiplier[cote] = 1+streak[cote] / 5;
        UpdateGUI();
        foreach (GameObject flamme in flammes) {
            flamme.GetComponent<FlammeManager>().Activate(streak[cote]);
        }
        bkground.GetComponent<BackgroundManager>().Activate((PlayerPrefs.GetInt("StreakG") + PlayerPrefs.GetInt("StreakD")) / 2);
        flamme_long.GetComponent<FlammeManager>().Activate((PlayerPrefs.GetInt("StreakG") + PlayerPrefs.GetInt("StreakD"))/2);
        if (streak[cote] % 10 == 0)
            Explode.GetComponent<ExplodeScript>().Activate();

        if (streak[cote] > PlayerPrefs.GetInt("HighStreak"+Side))
            PlayerPrefs.SetInt("HighStreak"+Side, streak[cote]);

    }

    public void AddContraction(int cote, string act_side)
    {
        PlayerPrefs.SetInt("Contractions" + act_side + Side, PlayerPrefs.GetInt("Contractions" + act_side + Side) + 1);
    }

    public int GetStreak()
    {
        return streak[cote];
    }

    public void ResetStreak(int cote)
    {
        sound.GetComponent<SoundManager>().Play_sound();
        PlayerPrefs.SetInt("RockMeter", PlayerPrefs.GetInt("RockMeter") );
        if (PlayerPrefs.GetInt("RockMeter") < 0)
            Lose();
            streak[cote] = 0;
        multiplier[cote] = 1;
        UpdateGUI();
        foreach (GameObject flamme in flammes)
        {
            flamme.GetComponent<FlammeManager>().Deactive(0);
        }
        bkground.GetComponent<BackgroundManager>().Deactive(((PlayerPrefs.GetInt("StreakG") + PlayerPrefs.GetInt("StreakD")) / 2));
        flamme_long.GetComponent<FlammeManager>().Deactive((PlayerPrefs.GetInt("StreakG") + PlayerPrefs.GetInt("StreakD")) / 2);
    }
    void Lose()
    {
        PlayerPrefs.SetInt("Start", 0);
        SceneManager.LoadScene(2);
    }

    public void Win()
    {
        PlayerPrefs.SetInt("NotesHitG", PlayerPrefs.GetInt("NotesHitGG") + PlayerPrefs.GetInt("NotesHitDG"));
        PlayerPrefs.SetInt("NotesHitD", PlayerPrefs.GetInt("NotesHitGD") + PlayerPrefs.GetInt("NotesHitDD"));
        List<string> scores = new List<string>();
        scores.Add("Joueur;Score;Notes hit;% Reussite R;Offset EMG1;Onset EMG1;% Reussite V;Offset EMG2;Onset EMG2");
        int Score_ind = PlayerPrefs.GetInt("ScoreG");
        int notes_hit = PlayerPrefs.GetInt("NotesHitGG") + PlayerPrefs.GetInt("NotesHitDG");
        float ratio1, ratio2;
        if (notes_hit != 0) {
            ratio1 = 100*(float)PlayerPrefs.GetInt("NotesHitGG") / PlayerPrefs.GetInt("Count_notes_red");
            ratio2 = 100*(float)PlayerPrefs.GetInt("NotesHitDG") / PlayerPrefs.GetInt("Count_notes_green");
        }
        else {
            ratio1 = 0;
            ratio2 = 0;
        }

        float sh1 = GameObject.Find("Meter3").GetComponent<MeterManager>().seuil_high, sh2 = GameObject.Find("Meter4").GetComponent<MeterManager>().seuil_high;
        float o1 = GameObject.Find("Meter3").GetComponent<MeterManager>().offset, o2 = GameObject.Find("Meter4").GetComponent<MeterManager>().offset;

        scores.Add("1;" +Score_ind.ToString()+";"+ notes_hit.ToString() + ";" + ratio1.ToString() + ";" + o1.ToString() + ";" + sh1.ToString() + ";" + ratio2.ToString() + ";" + o2.ToString() + ";" + sh2.ToString());

        Score_ind = PlayerPrefs.GetInt("ScoreD");
        notes_hit = PlayerPrefs.GetInt("NotesHitGD") + PlayerPrefs.GetInt("NotesHitDD");
        if (notes_hit != 0)
        {
            ratio1 = 100*(float)PlayerPrefs.GetInt("NotesHitGD") / PlayerPrefs.GetInt("Count_notes_red");
            ratio2 = 100*(float)PlayerPrefs.GetInt("NotesHitDD") / PlayerPrefs.GetInt("Count_notes_green");
        }
        else
        {
            ratio1 = 0;
            ratio2 = 0;
        }
        if (PlayerPrefs.GetInt("Solo") == 0)
        {
            sh1 = GameObject.Find("Meter1").GetComponent<MeterManager>().seuil_high;
            sh2 = GameObject.Find("Meter2").GetComponent<MeterManager>().seuil_high;
            o1 = GameObject.Find("Meter1").GetComponent<MeterManager>().offset;
            o2 = GameObject.Find("Meter2").GetComponent<MeterManager>().offset;
        }
        else
        {
            sh1 = 200; sh2 = 200; o1 = 50; o2 = 50;
        }
        scores.Add("2;" + Score_ind.ToString() + ";" + notes_hit.ToString() + ";" + ratio1.ToString() + ";" + o1.ToString() + ";" + sh1.ToString() + ";" + ratio2.ToString() + ";" + o2.ToString() + ";" + sh2.ToString());
        analog_manager.GetComponent<analogManager>().Save_all(scores);

        PlayerPrefs.SetInt("Start", 0);
        PlayerPrefs.SetInt("HighScore", 0);
        if (PlayerPrefs.GetInt("ScoreG") > PlayerPrefs.GetInt("HighScore" + PlayerPrefs.GetInt("Level").ToString()))
        {
            print("là");
            PlayerPrefs.SetInt("HighScore" + PlayerPrefs.GetInt("Level").ToString(), PlayerPrefs.GetInt("ScoreG"));
        }
        if (PlayerPrefs.GetInt("ScoreD") > PlayerPrefs.GetInt("HighScore" + PlayerPrefs.GetInt("Level").ToString()))
        {
            PlayerPrefs.SetInt("HighScore" + PlayerPrefs.GetInt("Level").ToString(), PlayerPrefs.GetInt("ScoreD"));
        }
        SceneManager.LoadScene(1);
    }
    void UpdateGUI()
    {
        if (cote == 0)
        {
            PlayerPrefs.SetInt("StreakG", streak[0]);
            PlayerPrefs.SetInt("MultG", multiplier[0]);
        }
        else
        {
            PlayerPrefs.SetInt("StreakD", streak[1]);
            PlayerPrefs.SetInt("MultD", multiplier[1]);
        }
    }
    public int GetScore()
    {
        return 100 * multiplier[cote];
    }

    public void Play_Pause()
    {
        GameObject Music_player = GameObject.Find("Music");
        if (on_p) // on met sur pause et on retient le temps de pause
        {
            PauseTime = DateTime.Now;
            Music_player.GetComponent<AudioSource>().Pause();
            on_p = false;
            foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
            {
                Note.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
        else   // On remet sur play et on ajoute au decalage le tps de pause
        {
            Decalage += DateTime.Now - PauseTime;
            Music_player.GetComponent<AudioSource>().Play();
            on_p = true;
            foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
            {
                Note.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -5);
            }
        }
    }

    public void RollBack()
    {
        GameObject Music_player = GameObject.Find("Music");
        Music_player.GetComponent<AudioSource>().Stop();
        if (!on_p) {
            Decalage += DateTime.Now - PauseTime;
        }
        on_p = false;
        TimeSpan D_rel = (DateTime.Now - Start_time) - Decalage;
        float dec = 5f * (D_rel.Minutes * 60f + D_rel.Seconds + D_rel.Milliseconds * 0.001f);
        foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
        {
            Note.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Note.GetComponent<Transform>().position += new Vector3(0, dec, 0);
        }
        PauseTime = DateTime.Now;
        Decalage = TimeSpan.Zero;
        Start_time = DateTime.Now;
    }

    public void Stop_creation()
    {
        TimeSpan D_rel = (DateTime.Now - Start_time) - Decalage;
        float dec = 5f*(D_rel.Minutes*60f+D_rel.Seconds+D_rel.Milliseconds*0.001f);
        print(D_rel.Seconds);
        on_p = false;
        PauseTime = DateTime.Now;
        Decalage = TimeSpan.Zero;
        Start_time = DateTime.Now;
        GameObject Music_player = GameObject.Find("Music");
        Music_player.GetComponent<AudioSource>().Stop();
        foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
        {
            Note.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Note.GetComponent<Transform>().position += new Vector3(0, dec, 0);
        }
        bsave.SetActive(true);
        bback.SetActive(true);
        bstop.SetActive(false);
    }

    public void Save()
    {
        RollBack();
        // On ecrit dans le fichier Niveau.txt une nouvelle ligne avec le numero, le nom de la musique et la description.
        // On ecrit les notes dans le fichier avec le bon numero de niveau 
        GameObject.Find("Notes_list_G").GetComponent<NoteManager>().Save_Notes();
        // Ajout dans le fichier niveaux.txt...
        string FilePath = Application.dataPath+"/Niveaux.txt";
        StreamReader reader = new StreamReader(FilePath);
        string line = reader.ReadLine();
        int k = 1;
        while (line != null)
        {
            line = reader.ReadLine();
            if (k > 100)
                break;
            k++;
        }
        string txt_to_write = (k).ToString() + ";";
        string txt_mus = GameObject.Find("Music").GetComponent<AudioSource>().clip.name;
        txt_to_write += txt_mus + ";";

        GameObject Field = GameObject.Find("Entry_desc"); // On recupere la description.
        string res = Field.GetComponent<InputField>().text;
        if (res == "")
        {
            res = "Description...;";
        }
        txt_to_write += res + "\n";
        reader.Close();
        TextWriter writer = File.AppendText(FilePath);
        writer.Write(txt_to_write);
        writer.Close();
        SceneManager.LoadScene(3);


    }
}
