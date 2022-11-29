using System.Collections;
using UnityEngine;
using QualisysRealTime.Unity;


public class Activator : MonoBehaviour
{
    SpriteRenderer sr;
    public KeyCode key;
    public bool active = false;
    GameObject note,gm;
    Color old;
    public int cote;
    public GameObject analog;
    public bool isEMG = true ;
    private bool wasConctracted = false;
    public bool Reset_if_fail = false;
    public bool Creation_mode = false;
    public GameObject Note;
    GameObject list_notes;
    public string act_side;


    // Start is called before the first frame update

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        Reset_if_fail = (PlayerPrefs.GetInt("Allow_cocontractions") == 0);
        list_notes = GameObject.Find("Notes_list_"+act_side);
        if (cote == 0)
            gm = GameObject.Find("GameManagerG");
        else
            gm = GameObject.Find("GameManagerD");
        old = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("Start") == 1)
        
        {
            if (Creation_mode)
            {
                if (!isEMG || !wasConctracted)
                {
                    if ((Input.GetKeyDown(key) && !isEMG) || (isEMG && analog.GetComponent<MeterManager>().isContracted))
                    {
                        GameObject copy = Instantiate(Note, transform.position, Quaternion.identity, list_notes.transform) as GameObject;
                    }
                }
            }
            else
            {
                // Detection activation
                if (!isEMG || !wasConctracted)
                {
                    if ((Input.GetKeyDown(key) && !isEMG) || (isEMG && analog.GetComponent<MeterManager>().isContracted))
                    {
                        StartCoroutine(Pressed());

                        if (active)
                        {
                            Destroy(note);
                            gm.GetComponent<GameManager>().AddStreak(cote, act_side);
                            AddScore();
                            active = false;
                        }
                        else
                        {
                            if (Reset_if_fail)
                                gm.GetComponent<GameManager>().ResetStreak(cote);
                        }
                    }
                }

                if (isEMG)
                    wasConctracted = analog.GetComponent<MeterManager>().isContracted;
            }
        }
    }
        

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Winnote" && cote == 0 && act_side == "D")
        {
            gm.GetComponent<GameManager>().Win();
        }
        if (col.gameObject.tag == "Note")
            active = true;
            note = col.gameObject;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        active = false;
    }

    void AddScore()
    {
        if (cote == 0)
            PlayerPrefs.SetInt("ScoreG", PlayerPrefs.GetInt("ScoreG") + gm.GetComponent<GameManager>().GetScore());
        else
            PlayerPrefs.SetInt("ScoreD", PlayerPrefs.GetInt("ScoreD") + gm.GetComponent<GameManager>().GetScore());
    }


    IEnumerator Pressed()
    {
        sr.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.05f);
        sr.color = old;
    }


    
}
