using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject main_menu;
    public GameObject credits;
    public GameObject logo;
    public GameObject pause;
    public GameObject death;
    public GameObject filler;           // background filler related with fixed sized sprite of main menu

    Animator _anim;
    enum GameState {splash, main_menu, credits, game, pause, death};
    GameState game_state;
    bool fade_out;
    bool fade_in;
    float dt;
    float prev_time;

    // Start is called before the first frame update
    void Start()
    {
        game_state = GameState.splash;
        main_menu.SetActive(false);
        credits.SetActive(false);
        pause.SetActive(false);
        logo.SetActive(true);
        death.SetActive(false);
        filler.SetActive(false);
        _anim = GetComponentInChildren<Animator>();
        fade_out = true;
        fade_in = false;
        Physics.autoSimulation = false;
        Time.timeScale = 0;
        dt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (game_state)
        {
            case GameState.splash:
                if (fade_out)
                {
                    dt += 0.008f;
                    logo.GetComponent<LogoLoad>().SetAlpha(dt);
                    if (dt > 1)
                    {
                        fade_out = false;
                        fade_in = true;
                    }
                }
                if (fade_in)
                {
                    dt -= 0.008f;
                    if (dt < 0)
                    {
                        game_state = GameState.main_menu;
                        logo.SetActive(false);
                        main_menu.SetActive(true);
                        filler.SetActive(true);
                    }
                    logo.GetComponent<LogoLoad>().SetAlpha(dt);
                    
                }
                break;
            case GameState.main_menu:

                break;
            case GameState.credits:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    main_menu.SetActive(true);
                    credits.SetActive(false);
                }
                break;
            case GameState.game:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Physics.autoSimulation = false;
                    Time.timeScale = 0;
                    pause.SetActive(true);
                    game_state = GameState.pause;
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Death();
                }
                break;
            case GameState.pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Physics.autoSimulation = true;
                    Time.timeScale = 1;
                    pause.SetActive(false);
                    game_state = GameState.game;
                }
                break;
            case GameState.death:
                if (fade_out)
                {
                    dt += 0.008f;
                    death.GetComponent<DeathRend>().SetAlpha(dt);
                    if (dt > 3)
                    {
                        fade_out = false;
                    }
                }
                break;
        }
        if ((Input.GetKeyDown(KeyCode.Escape)) && (credits.activeInHierarchy == true))
        {
            main_menu.SetActive(true);
            credits.SetActive(false);
        }
    }

    public void PlayPressed()
    {
        _anim.SetTrigger("Menu");
        game_state = GameState.game;
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        main_menu.SetActive(false);
        logo.SetActive(false);
        credits.SetActive(false);
        filler.SetActive(false);
    }

    public void CreditsPressed()
    {
        Debug.Log("main_menu"+ main_menu);
        main_menu.SetActive(false);
        credits.SetActive(true);
        game_state = GameState.credits;
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    public void ResumePressed()
    {
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        pause.SetActive(false);
        game_state = GameState.game;
    }

    public void RestartPressed()
    {
        if (game_state == GameState.death)
        {
            death.GetComponent<DeathRend>().SetTransparent();
            death.GetComponent<DeathRend>().ResetScore();
            death.SetActive(false);
            game_state = GameState.game;
        }
    }

    public void Death()
    {
        game_state = GameState.death;
        dt = 0;
        fade_out = true;
        death.SetActive(true);
        death.GetComponent<DeathRend>().SetScore(1488);
    }
}

