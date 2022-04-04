﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFlyingSound : MonoBehaviour
{
    AudioClip contact_sound;
    AudioClip svist_sound;
    AudioSource audio_source;
    SoundManager sound_manager;
    bool collided_once;
    int stone_size;

    GameObject goPlayerHead = null;

    void Awake()
    {
        GameObject[] sm = GameObject.FindGameObjectsWithTag("SoundManager");
        GameObject[] lPlayerHeadGOs = GameObject.FindGameObjectsWithTag("Player");

        if (lPlayerHeadGOs.Length > 0)
        {
            goPlayerHead = lPlayerHeadGOs[0];
        }

        sound_manager = sm[0].GetComponent<SoundManager>();
        audio_source = gameObject.AddComponent<AudioSource>() as AudioSource;
        contact_sound = sound_manager.GetContactSound(SoundManager.sound_type.dirt);
        collided_once = false;
        audio_source.loop = false;
        audio_source.playOnAwake = false;
    }

    public void SetStoneSound(int type)
    {
        svist_sound = sound_manager.GetFlyingSound(type);
        audio_source.clip = svist_sound;
        audio_source.volume = 0.4f;
        audio_source.PlayOneShot(svist_sound);
    }
    
    void PlaySoundContact()
    {
        audio_source.Stop();
        audio_source.clip = contact_sound;
        audio_source.PlayOneShot(contact_sound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collided_once && !(goPlayerHead is null) && gameObject.transform.position.y < goPlayerHead.transform.position.y + 3.0f)
        {
            PlaySoundContact();
            collided_once = true;
        }
    }
}
