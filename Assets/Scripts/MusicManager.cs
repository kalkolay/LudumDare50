using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //public DragRigidbodyBetter dragRigidBodyScript;
    private Menu menuScript;

    AudioSource source;
    public AudioClip game_track;
    public AudioClip deathmenu_track;

    const float max_volume = 1.0f;
    const float min_volume = 0.0f;
    const float dv = 0.01f;

    bool flag_music_loop = false;

    public bool isFalling = false;

    bool _isStarting = false;
    bool isFadeIn = false;

    bool deathmenu_track_enabled = false;
    bool game_track_enabled = false;


    static bool _initialized = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!_initialized)
        {
            isFalling = false;

            GameObject menu = GameObject.FindGameObjectWithTag("Menu");
            menuScript = menu.GetComponent<Menu>();

            //dragRigidBodyScript.onDedFall += DragRigidBodyScript_onDedFall;
            menuScript.OnRestart += MenuScript_OnRestart;
            source = GetComponent<AudioSource>();

            StartMusic();
        }
    }

    public void setFallingStart()
    {
        isFalling = true;
    }

    void Start()
    {
    }

    void StartMusic()
    {
        source.loop = true;
        source.clip = deathmenu_track;
        source.Play();

        isFalling = true;
        isFadeIn = true;

        _initialized = true;
    }

    private void MenuScript_OnRestart()
    {
        _isStarting = true;
    }

    //private void DragRigidBodyScript_onDedFall()
    //{
    //    isFalling = true;
    //}

    // Update is called once per frame
    void Update()
    {
        UpdateDeathMenuTrack();
        UpdateGameTrack();
    }

    void FadeOut()
    {
        if (source.volume > min_volume)
            source.volume -= dv;
    }
    void FadeIn()
    {
        if (source.volume < max_volume)
            source.volume += dv;
    }

    void UpdateDeathMenuTrack()
    {
        if (isFalling && isFadeIn)
        {
            if (!deathmenu_track_enabled)
            {
                source.Stop();
                game_track_enabled = false;

                source.clip = deathmenu_track;
                source.Play();
                deathmenu_track_enabled = true;
            }

            FadeIn();

            if (source.volume >= max_volume)
            {
                isFalling = false;
                isFadeIn = false;
            }
        }
        
        if (_isStarting && !isFadeIn)
        {
            FadeOut();

            if (source.volume <= min_volume)
            {
                isFadeIn = true;
            }
        }
    }

    void UpdateGameTrack()
    {
        if (_isStarting && isFadeIn)
        {
            if (!game_track_enabled)
            {
                source.Stop();
                deathmenu_track_enabled = false;

                source.clip = game_track;
                source.Play();
                game_track_enabled = true;
            }

            FadeIn();

            if (source.volume >= max_volume)
            {
                _isStarting = false;
                isFadeIn = false;
            }
        }

        if (isFalling && !isFadeIn)
        {

            FadeOut();

            if (source.volume <= min_volume)
            {
                isFadeIn = true;
            }
        }
    }

    public void PlayGameMusic(AudioClip _track)
    {
        game_track = _track;
        flag_music_loop = true;

        _isStarting = true;
    }

    public void PlayMenuMusic(AudioClip _track)
    {
        deathmenu_track = _track;
    }
}
