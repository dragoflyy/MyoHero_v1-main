using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PPText : MonoBehaviour
{
    public string name;
    public string addon = "";
    string res;

    void Update()
    {
        if (name == "HighScoreLvl")
        {
            res = PlayerPrefs.GetInt("HighScore" + PlayerPrefs.GetInt("Level").ToString()).ToString();
        }
        else if (name == "Music_Name")
            res = PlayerPrefs.GetString(name);
        else
            res = PlayerPrefs.GetInt(name).ToString();
        GetComponent<Text>().text = addon+ res + "";
    }


}
