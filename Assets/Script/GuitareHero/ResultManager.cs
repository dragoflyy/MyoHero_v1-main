using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public GameObject[] ToErase;
    public GameObject Back;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("Solo") == 1)
        {
            Back.SetActive(true);
            foreach(GameObject go in ToErase)
            {
                go.SetActive(false);
            }
        }
        else
        {
            Back.SetActive(false);
            foreach(GameObject go in ToErase)
            {
                go.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
