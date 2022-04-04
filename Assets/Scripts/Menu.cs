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
    public DeathPlate deathPlate;
    public PlayerScript player_script;
    public GameState game_state_obj;

    Animator _anim;
    enum GameStateEnum {splash, main_menu, credits, game, pause, death};
    GameStateEnum game_state;
    bool fade_out;
    bool fade_in;
    float dt;
    float prev_time;

    // Start is called before the first frame update
    void Start()
    {
        game_state = GameStateEnum.splash;
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
        // strange thing - it is deleted after one update after death
        if (player_script == null || game_state_obj == null)
            RestartReinitScript();

        switch (game_state)
        {
            case GameStateEnum.splash:
                if (Input.GetMouseButton(0))
                {
                    game_state = GameStateEnum.main_menu;
                    logo.SetActive(false);
                    main_menu.SetActive(true);
                    filler.SetActive(true);
                }
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
                        game_state = GameStateEnum.main_menu;
                        logo.SetActive(false);
                        main_menu.SetActive(true);
                        filler.SetActive(true);
                    }
                    logo.GetComponent<LogoLoad>().SetAlpha(dt);
                    
                }
                break;
            case GameStateEnum.main_menu:

                break;
            case GameStateEnum.credits:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    main_menu.SetActive(true);
                    credits.SetActive(false);
                }
                break;
            case GameStateEnum.game:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Physics.autoSimulation = false;
                    Time.timeScale = 0;
                    pause.SetActive(true);
                    game_state = GameStateEnum.pause;
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Death();
                }
                break;
            case GameStateEnum.pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Physics.autoSimulation = true;
                    Time.timeScale = 1;
                    pause.SetActive(false);
                    game_state = GameStateEnum.game;
                }
                break;
            case GameStateEnum.death:
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
    }

    public void PlayPressed()
    {
        _anim.SetTrigger("Menu");
        game_state = GameStateEnum.game;
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        main_menu.SetActive(false);
        logo.SetActive(false);
        credits.SetActive(false);
        filler.SetActive(false);
        deathPlate.onNewGame();
    }

    public void CreditsPressed()
    {
        Debug.Log("main_menu"+ main_menu);
        main_menu.SetActive(false);
        credits.SetActive(true);
        game_state = GameStateEnum.credits;
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
        game_state = GameStateEnum.game;
    }

    public void RestartPressed()
    {
        if (game_state == GameStateEnum.death)
        {
            death.GetComponent<DeathRend>().SetTransparent();
            death.GetComponent<DeathRend>().ResetScore();
            death.SetActive(false);
            game_state = GameStateEnum.game;
            player_script.Restart();
            game_state_obj.Restart();
            deathPlate.Restart();
        }
    }

    public void Death()
    {
        game_state = GameStateEnum.death;
        dt = 0;
        fade_out = true;
        death.SetActive(true);
    }

    public void RestartReinitScript()
    {
        GameObject[] Fathers = GameObject.FindGameObjectsWithTag("GrandDed");
        GameObject GrandDed = Fathers[0];
        player_script = GrandDed.transform.Find("Controller").GetComponent<PlayerScript>();
        GameObject[] GS = GameObject.FindGameObjectsWithTag("GameState");
        game_state_obj = GS[0].GetComponent<GameState>();
    }
}

