using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MusicManager : MonoBehaviour
{
    bool called = false;
    public AudioClip clip;
    string Path, Audio_name;
    public bool Creation = false;
    void Start()
    {
        if (!Creation)
        {
            Path = Application.dataPath + "/Audios/";
            Audio_name = PlayerPrefs.GetString("Music_Name");
            StartCoroutine(Load(Path, Audio_name));
        }
        if (Creation)
        {
            GetComponent<AudioSource>().clip = clip;
        }
        
    }
    void Update()
    {
        if (PlayerPrefs.GetInt("Start") == 1 && !called)
        {
            GetComponent<AudioSource>().Play();
            called = true;
        }
    }

    

    IEnumerator Load(string path, string Audio_name)
    {
        WWW request = GetAudio(path, Audio_name);
        yield return request;
        clip = request.GetAudioClip();
        clip.name = Audio_name;
        GetComponent<AudioSource>().clip = clip;


    }

    private WWW GetAudio(string path, string Audio_name)
    {
        string audio = string.Format(path + "{0}", Audio_name);
        WWW request = new WWW(audio);
        return request;
    }
}
