using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFlyingSound : MonoBehaviour
{
    AudioClip contact_sound;
    AudioClip svist_sound;
    AudioSource audio_source;
    
    void Start()
    {
        GameObject[] sm = GameObject.FindGameObjectsWithTag("SoundManager");
        SoundManager sound_manager = sm[0].GetComponent<SoundManager>();
        audio_source = gameObject.AddComponent<AudioSource>() as AudioSource;
        contact_sound = sound_manager.GetContactSound(SoundManager.sound_type.dirt);
        svist_sound = sound_manager.GetFlyingSound();
        audio_source.clip = svist_sound;
        audio_source.PlayOneShot(svist_sound);
        audio_source.volume = 0.4f;
    }
    
    void PlaySoundContact()
    {
        audio_source.Stop();
        audio_source.clip = contact_sound;
        audio_source.PlayOneShot(contact_sound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySoundContact();
    }
}
