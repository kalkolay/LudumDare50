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
    public DeathPlate deathPlate;
    Animator _anim;
    enum GameState {splash, main_menu, credits, game, pause};
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
        _anim = GetComponentInChildren<Animator>();
        fade_out = true;
        fade_in = false;
        Physics.autoSimulation = false;
        Time.timeScale = 0;
        dt = 0;
        deathPlate.onNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        switch (game_state)
        {
            case GameState.splash:
                if (fade_out)
                {
                    dt += 0.0025f;
                    logo.GetComponent<LogoLoad>().SetAlpha(dt);
                    if (dt > 1)
                    {
                        fade_out = false;
                        fade_in = true;
                    }
                }
                if (fade_in)
                {
                    dt -= 0.0025f;
                    if (dt < 0)
                    {
                        game_state = GameState.main_menu;
                        logo.SetActive(false);
                        main_menu.SetActive(true);
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
        }
        if ((Input.GetKeyDown(KeyCode.Escape)) && (credits.activeInHierarchy == true))
        {
            Debug.Log("ESC was pressed!");
            main_menu.SetActive(true);
            credits.SetActive(false);
        }
    }

    public void PlayPressed()
    {
        Debug.Log("Start pressed!");
        _anim.SetTrigger("Menu");
        game_state = GameState.game;
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        main_menu.SetActive(false);
        logo.SetActive(false);
        credits.SetActive(false);
    }

    public void CreditsPressed()
    {
        Debug.Log("main_menu"+ main_menu);
        main_menu.SetActive(false);
        credits.SetActive(true);
        game_state = GameState.credits;
        Debug.Log("Credits pressed!");
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit pressed!");
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
        // todo
    }
}

