using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFlyingSound : MonoBehaviour
{
    AudioClip contact_sound;
    AudioClip svist_sound;
    AudioSource audio_source;
    SoundManager sound_manager;
    Queue<AudioClip> small_rocks = new Queue<AudioClip>();
    bool collided_once;
    int stone_size;

    GameObject goPlayerHead = null;

    void Awake()
    {
        MakeDefault();
    }

    public void Update()
    {
        if (goPlayerHead.Equals(null))
        {
            GameObject[] lPlayerHeadGOs = GameObject.FindGameObjectsWithTag("Player");

            if (lPlayerHeadGOs.Length > 0)
            {
                goPlayerHead = lPlayerHeadGOs[0];
            }
        }

        if((small_rocks.Count != 0) && (!audio_source.isPlaying))
        {
            contact_sound = small_rocks.Dequeue();
            audio_source.clip = contact_sound;
        }
    }

    public void SetStoneSound(int type, bool delay = false)
    {
        if (!delay)
        {
            contact_sound = sound_manager.GetContactSound(type);
            audio_source.clip = contact_sound;
        }
        else
        {
            small_rocks.Enqueue(sound_manager.GetContactSound(type));
        }
    }
    
    void PlaySoundContact()
    {
        audio_source.Stop();
        audio_source.clip = contact_sound;
        audio_source.PlayOneShot(contact_sound);
    }

    public void MakeDefault()
    {
        GameObject[] sm = GameObject.FindGameObjectsWithTag("SoundManager");
        GameObject[] lPlayerHeadGOs = GameObject.FindGameObjectsWithTag("Player");

        if (lPlayerHeadGOs.Length > 0)
        {
            goPlayerHead = lPlayerHeadGOs[0];
        }

        sound_manager = sm[0].GetComponent<SoundManager>();
        audio_source = gameObject.AddComponent<AudioSource>() as AudioSource;
        //contact_sound = sound_manager.GetContactSound(SoundManager.sound_type.dirt);
        collided_once = false;
        audio_source.loop = false;
        audio_source.playOnAwake = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!goPlayerHead.Equals(null))
        {
            if (!collided_once && gameObject.transform.position.y < goPlayerHead.transform.position.y + 3.0f)
            {
                if (collision.gameObject.tag == "Stone")
                {
                    //contact_sound = sound_manager.GetContactSound(SoundManager.sound_type.brick);
                }
                else
                {
                    if (Random.Range(1, 10) > 8)
                        sound_manager.DedOret();
                }
                PlaySoundContact();
                collided_once = true;
            }
        }
    }
}
