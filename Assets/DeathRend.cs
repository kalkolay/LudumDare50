using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathRend : MonoBehaviour
{
    public Image img;
    Color img_color;
    Color bck_color;
    Color text_color;
    Image bck_img;
    public Text score_text;
    public Button restart_btn;
    public Button exit_btn;

    void Start()
    {
        img_color = img.color;
        bck_img = GetComponent<Image>();
        if (bck_img != null)
            bck_color = GetComponent<Image>().color;
        else
            bck_color = Color.black;
        text_color = score_text.color;
        restart_btn.gameObject.SetActive(false);
        exit_btn.gameObject.SetActive(false);
        img.color = new Color(img_color.r, img_color.g, img_color.b, 0);
        bck_img.color = new Color(bck_color.r, bck_color.g, bck_color.b, 0);
        score_text.color = new Color(text_color.r, text_color.g, text_color.b, 0);

    }
    
    void Update()
    {
        
    }

    public void SetAlpha(float a)
    {
        float bck_a = a;
        if (bck_a > 1) bck_a = 1;
        if (bck_a < 0) bck_a = 0;
        bck_img.color = new Color(bck_color.r, bck_color.g, bck_color.b, bck_a);

        float img_a = a - 1;
        if (img_a > 1) img_a = 1;
        if (img_a < 0) img_a = 0;
        img.color = new Color(img_color.r, img_color.g, img_color.b, img_a);

        float text_a = a - 2;
        if (text_a > 1) text_a = 1;
        if (text_a < 0) text_a = 0;
        score_text.color = new Color(text_color.r, text_color.g, text_color.b, text_a);
        if (a >= 3)
        {
            restart_btn.gameObject.SetActive(true);
            exit_btn.gameObject.SetActive(true);
        }
    }

    public void SetTransparent()
    {
        img.color = new Color(img_color.r, img_color.g, img_color.b, 0);
        bck_img.color = new Color(bck_color.r, bck_color.g, bck_color.b, 0);
        score_text.color = new Color(text_color.r, text_color.g, text_color.b, 0);
        restart_btn.gameObject.SetActive(false);
        exit_btn.gameObject.SetActive(false);
    }

    public void SetScore(int score)
    {
        score_text.text += score.ToString();
    }

    public void ResetScore()
    {
        score_text.text = "your score: ";
    }
}
