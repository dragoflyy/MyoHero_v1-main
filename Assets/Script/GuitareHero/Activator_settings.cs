using System.Collections;
using UnityEngine;
using QualisysRealTime.Unity;

public class Activator_settings : MonoBehaviour
{
    SpriteRenderer sr;
    Color old;
    public GameObject Analog,note;
    private bool wasConctracted = false;
    string nb;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        old = sr.color;
        nb = this.name;
        nb = nb.Substring(nb.Length - 1, 1);
        note = GameObject.Find("note" + nb);
    }

    void Update()
    {
        // Detection activation
        bool isContracted = Analog.GetComponent<MeterManager>().isContracted;
        if (!wasConctracted)
        {
            if (isContracted)
            {
                StartCoroutine(Pressed());

            }
        }
        wasConctracted = isContracted;
    }
    IEnumerator Pressed()
    {
        note.GetComponent<AudioSource>().Play();
        sr.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.05f);
        sr.color = old;
    }


}