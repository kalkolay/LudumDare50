using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoLoad : MonoBehaviour
{
    float prevTime = 0.0f;
    public GameObject menu;
    public Image img;
    Color img_color;
    //public bool fadeIn = true;
    //public bool fadeOut = false;

    void Start()
    {
        prevTime = Time.time;
        //img = GetComponent<Image>();
        img_color = img.color;
        img.color = new Color(img_color.r, img_color.g, img_color.b, 0f);
        //StartCoroutine(fade_out(2.5f));
    }

    /*
    IEnumerator fade_out(float _t)
    {
        yield return new WaitForSeconds(_t);
        fadeOut = true;
        prevTime = Time.time;
    }
    void Update()
    {
        float dt = Time.time - prevTime;
        if (fadeIn)
        {
            if (dt > 1.0f)
            {
                fadeIn = false;
            }
            img.color = new Color(img_color.r, img_color.g, img_color.b, dt);
        }

        if (fadeOut)
        {
            if (dt > 1.0f)
            {
                dt = 1.0f;
                fadeOut = false;
                prevTime = Time.time;
                menu.SetActive(true);
            }
            img.color = new Color(img_color.r, img_color.g, img_color.b, 1 - dt);
        }
    }
    */
    public void SetAlpha(float a)
    {
        img.color = new Color(img_color.r, img_color.g, img_color.b, a);
    }
}
