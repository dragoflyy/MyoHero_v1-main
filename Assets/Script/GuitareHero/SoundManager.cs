using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    GameObject brass;
    GameObject computer;
    bool OnPlay = false;
    private void Start()
    {
        brass = GameObject.Find("brass_effect");
        computer = GameObject.Find("computer_effect");
    }
    public void Play_sound()
    {
        if (!OnPlay)
        {
            OnPlay = true;
            brass.GetComponent<AudioSource>().Play();
            computer.GetComponent<AudioSource>().Play();
            StartCoroutine(Count());
        }
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(1);
        OnPlay = false;
    }
}
