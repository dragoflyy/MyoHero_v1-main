using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toogle_co : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Toggle>().isOn = (PlayerPrefs.GetInt("Allow_cocontractions")==1);
    }

    public void Update_bool()
    {
        PlayerPrefs.SetInt("Allow_cocontractions", this.GetComponent<Toggle>().isOn ? 1 : 0) ;
        print(PlayerPrefs.GetInt("Allow_cocontractions"));
    }
}
