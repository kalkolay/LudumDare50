using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource source;
    AudioClip track;
    float music_volume_scale = 0.1f;
    bool flag_music_loop = false;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(source.isPlaying && flag_music_loop)
        {
            PlayGameMusic(track);
        }
    }

    public void PlayGameMusic(AudioClip _track)
    {
        source.PlayOneShot(_track, music_volume_scale);
    }
}
