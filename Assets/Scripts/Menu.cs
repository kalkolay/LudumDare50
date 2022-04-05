using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject FullGamePrefab;
    public GameObject main_menu;
    public GameObject credits;
    public GameObject logo;
    public GameObject pause;
    public GameObject death;
    public GameObject filler;           // background filler related with fixed sized sprite of main menu
    //public DeathPlate deathPlate;
    public GameObject win;
    //public PlayerScript player_script;
    //public GameState game_state_obj;
    public SoundManager sound_manager;

    Animator _anim;
    enum GameStateEnum {splash, main_menu, credits, game, pause, death, win};
    GameStateEnum game_state;
    bool fade_out;
    bool fade_in;
    float dt;
    float prev_time;
    private GameObject _currentGame;

    public event System.Action OnRestart;

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
        win.SetActive(false);
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
        //if (player_script == null || game_state_obj == null)
        //    RestartReinitScript();

        switch (game_state)
        {
            case GameStateEnum.splash:
                sound_manager.SetVolume(0);
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
                sound_manager.SetVolume(0.4f);
                break;
            case GameStateEnum.credits:
                if (Input.anyKey)
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
                    Win();
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
                    dt += 0.01f;
                    death.GetComponent<DeathRend>().SetAlpha(dt);
                    if (dt > 1)
                    {
                        fade_out = false;
                    }
                }
                break;
            case GameStateEnum.win:
                if (fade_out)
                {
                    dt += 0.016f;
                    win.GetComponent<DeathRend>().SetAlpha(dt);
                    if (dt > 1)
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
        StartGame();
        //deathPlate.onNewGame();
        //player_script.Restart();
        //game_state_obj.Restart();
        //deathPlate.Restart();
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

    public void ExitToMenu()
    {
        if (game_state == GameStateEnum.pause)
            pause.SetActive(false);
        if (game_state == GameStateEnum.death)
            death.SetActive(false);
        if (game_state == GameStateEnum.win)
            win.SetActive(false);
        Physics.autoSimulation = false;
        Time.timeScale = 0;
        game_state = GameStateEnum.main_menu;
        main_menu.SetActive(true);
        filler.SetActive(true);
    }

    public void ResumePressed()
    {
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        pause.SetActive(false);
        filler.SetActive(false);
        game_state = GameStateEnum.game;
    }

    public void RestartPressed()
    {
        Physics.autoSimulation = true;
        Time.timeScale = 1;
        //player_script.Restart();
        //game_state_obj.Restart();
        //deathPlate.Restart();
        StartGame();
        if (game_state == GameStateEnum.death)
        {
            //death.GetComponent<DeathRend>().ResetScore();
            //player_script.Restart();
            //game_state_obj.Restart();
            //deathPlate.Restart();
            death.SetActive(false);
        }
        if (game_state == GameStateEnum.pause)
        {
            pause.SetActive(false);
        }
        if (game_state == GameStateEnum.win)
        {
            win.GetComponent<DeathRend>().SetTransparent();
            win.SetActive(false);
        }
        filler.SetActive(false);
        game_state = GameStateEnum.game;

        OnRestart?.Invoke();
    }

    public void Death(string owo)
    {
        Physics.autoSimulation = false;
        Time.timeScale = 0;
        game_state = GameStateEnum.death;
        dt = 0;
        fade_out = true;
        death.SetActive(true);
        filler.SetActive(true);
    }

    public void RestartReinitScript()
    {
        GameObject[] Fathers = GameObject.FindGameObjectsWithTag("GrandDed");
        GameObject GrandDed = Fathers[0];
        //player_script = GrandDed.transform.Find("Controller").GetComponent<PlayerScript>();
        GameObject[] GS = GameObject.FindGameObjectsWithTag("GameState");
        //game_state_obj = GS[0].GetComponent<GameState>();
    }

    private void StartGame()
    {
        if (_currentGame != null)
            Destroy(_currentGame);
        Camera.main.transform.position = new Vector3(0, 0, -10);
        _currentGame = Instantiate(FullGamePrefab);
        GameState.instance.AddMenu(this);
    }

    public void Win()
    {
        Physics.autoSimulation = false;
        Time.timeScale = 0;
        game_state = GameStateEnum.win;
        dt = 0;
        fade_out = true;
        win.SetActive(true);
        filler.SetActive(true);
    }
}

