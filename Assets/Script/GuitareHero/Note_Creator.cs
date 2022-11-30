using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Note_Creator : MonoBehaviour
{
    public float BPM = 120; // battement / minutes
    public float offset = 0; // unit
    public float Tmusic = 1; // seconds
    private int speed = 5; // unit/secondes
    public int saut_notes = 1;

    public GameObject Noter, Notev, list_notes, nr, nv;
    // Start is called before the first frame update
    void Start()
    {
        BPM = BPM / 60.0f / saut_notes; // battement / secondes
        Vector3 Pos = nr.transform.position;
        bool SideIsG = true;

        Pos.y += offset;
        GameObject prefab = Noter;
        for (int ii = 1; ii <= BPM*Tmusic; ii++)
        {
            if (SideIsG)
            {
                int r = Random.Range(0, 100);
                if (r > 70)
                {
                    SideIsG = false;
                    Pos.x = nv.transform.position.x;
                    prefab = Notev;
                }
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r > 70)
                {
                    SideIsG = true;
                    Pos.x = nr.transform.position.x;
                    prefab = Noter;
                }
            }

            Pos.y += speed/BPM;
            GameObject copy = Instantiate(prefab, Pos, Quaternion.identity, list_notes.transform) as GameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
