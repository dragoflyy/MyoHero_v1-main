using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    bool called = false, dest = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (PlayerPrefs.GetInt("Start") == 1 && !called)
        {
            rb.velocity = new Vector2(0, -speed);
            called = true;
        }
    }

    public IEnumerator Delete()
    {
        if (!dest)
        {
            Vector3 Scale = this.transform.localScale;
            Color Col1 = this.GetComponent<SpriteRenderer>().color, Col2 = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            rb.velocity = new Vector2(0, 0);
            for(int kk = 0; kk < 20; kk++)
            {
                Scale += new Vector3(0.02f, 0.02f, 0);
                Col1.a -= 0.05f;
                Col2.a -= 0.05f;

                this.transform.localScale = Scale;
                this.GetComponent<SpriteRenderer>().color = Col1;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col2;

                yield return new WaitForSeconds(0.01f);
            }

            GameObject.Destroy(gameObject);
        }
    }
}
