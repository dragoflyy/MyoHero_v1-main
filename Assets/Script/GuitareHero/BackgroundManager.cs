using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Animator anim;
    SpriteRenderer sr;
    Color old;
    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void Activate(float val)
    {
        sr.enabled = true;
        Set_a(val);
    }

    public void Deactive(float val)
    {
        StartCoroutine(decrease(sr.color, val));
    }

    public void Set_a(float val)
    {
        old = sr.color;
        old[3] = (val) / 30;
        sr.color = old;
    }

    IEnumerator decrease(Color old, float val)
    {
        while (old[3] >= val / 30)
        {
            old[3] -= 0.03f;
            sr.color = old;
            yield return new WaitForSeconds(0.03f);
        }
        if (val == 0)
            sr.enabled = false;

    }
}
