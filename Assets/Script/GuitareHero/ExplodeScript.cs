using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeScript : MonoBehaviour
{
    public Animator anim;
    SpriteRenderer sr;
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void Activate()
    {
        StartCoroutine(Anim());
    }

    IEnumerator Anim()
    {
        sr.enabled = true;
        anim.Play("explode1",0,0f);
        yield return new WaitForSeconds(2);
        sr.enabled = false;
    }

}
