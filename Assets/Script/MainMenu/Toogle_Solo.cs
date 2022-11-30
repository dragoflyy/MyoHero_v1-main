using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toogle_Solo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Toggle>().isOn = (PlayerPrefs.GetInt("Solo") == 0);
    }

    public void Update_bool()
    {
        PlayerPrefs.SetInt("Solo", this.GetComponent<Toggle>().isOn ? 0 : 1);
        Debug.Log(PlayerPrefs.GetInt("Solo"));
    }
}
