using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScaleManager : MonoBehaviour
{
    GameObject texte, b1,b2, analog_manager;
    public KeyCode key;
    // Start is called before the first frame update
    void Start()
    {
        texte = GameObject.Find("Relax");
        texte.SetActive(false);
        b1 = GameObject.Find("button_quit");
        b2 = GameObject.Find("Button_scale");
        analog_manager = GameObject.Find("Jauges");
    }

    public void auto_scale()
    {
        StartCoroutine(auto_scale_m());
    }

    IEnumerator auto_scale_m()
    {
        b1.SetActive(false);
        b2.SetActive(false);
        texte.SetActive(true);
        texte.GetComponent<Text>().text = "PRET?";
        while (!Input.GetKeyDown(key))
            yield return null;
        texte.GetComponent<Text>().text = "RELAX";
        analog_manager.GetComponent<analogManager>().Start_relax();
        yield return new WaitForSeconds(3f);
        analog_manager.GetComponent<analogManager>().Stop_relax();
        texte.GetComponent<Text>().text = "PRET?";
        while (!Input.GetKeyDown(key))
            yield return null;
        texte.GetComponent<Text>().text = "ROUGE";
        analog_manager.GetComponent<analogManager>().Start_contract1();
        yield return new WaitForSeconds(5f);
        analog_manager.GetComponent<analogManager>().Stop_contract1();
        texte.GetComponent<Text>().text = "PRET?";
        while (!Input.GetKeyDown(key))
            yield return null;
        texte.GetComponent<Text>().text = "VERT";
        analog_manager.GetComponent<analogManager>().Start_contract2();
        yield return new WaitForSeconds(5f);
        analog_manager.GetComponent<analogManager>().Stop_contract2();
        texte.GetComponent<Text>().text = "PRET!";
        yield return new WaitForSeconds(2f);
        texte.SetActive(false);

        b1.SetActive(true);
        b2.SetActive(true);
    }
}
