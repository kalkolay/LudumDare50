using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource _audio;
    AudioSource _audio2;
    AudioClip svist_1;
    AudioClip svist_2;
    AudioClip svist_3;
    AudioClip svist_4;
    AudioClip svist_5;
    AudioClip shmyak_1;
    AudioClip shmyak_2;
    AudioClip shmyak_3;
    AudioClip shmyak_4;
    AudioClip shmyak_5;
    AudioClip shmyak_6;
    AudioClip shmyak_7;
    AudioClip shmyak_8;
    AudioClip zhelezo_1;
    AudioClip zhelezo_2;
    AudioClip zhelezo_3;
    AudioClip zhelezo_4;
    AudioClip zhelezo_5;
    AudioClip zhelezo_6;
    AudioClip zhelezo_7;
    AudioClip zhelezo_8;
    AudioClip derevo_1;
    AudioClip derevo_2;
    AudioClip derevo_3;
    AudioClip derevo_4;
    AudioClip derevo_5;
    AudioClip derevo_6;
    AudioClip derevo_7;
    AudioClip derevo_8;
    AudioClip derevo_9;
    AudioClip derevo_10;
    AudioClip game_end;
    AudioClip grab;
    AudioClip release;
    AudioClip stones_big;
    AudioClip stones_medium;
    AudioClip stones_small;
    AudioClip stones_punch;
    AudioClip brick_colliding;
    List<AudioClip> gryaz = new List<AudioClip>();
    List<AudioClip> svist = new List<AudioClip>();
    List<AudioClip> zhelezo = new List<AudioClip>();
    List<AudioClip> derevo = new List<AudioClip>();
    public enum sound_type
    {
        dirt = 1,
        metal = 2,
        wood = 3,
        brick = 4
    }

    private void Awake()
    {
        svist_1 = (AudioClip)Resources.Load("Sounds/svist/svist_1");
        svist_2 = (AudioClip)Resources.Load("Sounds/svist/svist_2");
        svist_3 = (AudioClip)Resources.Load("Sounds/svist/svist_3");
        svist_4 = (AudioClip)Resources.Load("Sounds/svist/svist_4");
        svist_5 = (AudioClip)Resources.Load("Sounds/svist/svist_5");
        svist.Add(svist_1);
        svist.Add(svist_2);
        svist.Add(svist_3);
        svist.Add(svist_4);
        svist.Add(svist_5);

        shmyak_1 = (AudioClip)Resources.Load("Sounds/tolkan/gryaz_1");
        shmyak_2 = (AudioClip)Resources.Load("Sounds/tolkan/po_gryazi_1");
        shmyak_3 = (AudioClip)Resources.Load("Sounds/tolkan/po_gryazi_2");
        shmyak_4 = (AudioClip)Resources.Load("Sounds/tolkan/po_gryazi_3");
        shmyak_5 = (AudioClip)Resources.Load("Sounds/tolkan/shmyak_1");
        shmyak_6 = (AudioClip)Resources.Load("Sounds/tolkan/shmyak_2");
        shmyak_7 = (AudioClip)Resources.Load("Sounds/tolkan/shmyak_3");
        shmyak_8 = (AudioClip)Resources.Load("Sounds/tolkan/shmyak_4");
        gryaz.Add(shmyak_1);
        gryaz.Add(shmyak_2);
        gryaz.Add(shmyak_3);
        gryaz.Add(shmyak_4);
        gryaz.Add(shmyak_5);
        gryaz.Add(shmyak_6);
        gryaz.Add(shmyak_7);
        gryaz.Add(shmyak_8);

        game_end = (AudioClip)Resources.Load("Sounds/tolkan/game_end");

        zhelezo_1 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_1");
        zhelezo_2 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_2");
        zhelezo_3 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_3");
        zhelezo_4 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_4");
        zhelezo_5 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_5");
        zhelezo_6 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_6");
        zhelezo_7 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_7");
        zhelezo_8 = (AudioClip)Resources.Load("Sounds/zhelezo/zhelezo_8");
        zhelezo.Add(zhelezo_1);
        zhelezo.Add(zhelezo_2);
        zhelezo.Add(zhelezo_3);
        zhelezo.Add(zhelezo_4);
        zhelezo.Add(zhelezo_5);
        zhelezo.Add(zhelezo_6);
        zhelezo.Add(zhelezo_7);
        zhelezo.Add(zhelezo_8);

        derevo_1 = (AudioClip)Resources.Load("Sounds/derevo/derevo_1");
        derevo_2 = (AudioClip)Resources.Load("Sounds/derevo/derevo_2");
        derevo_3 = (AudioClip)Resources.Load("Sounds/derevo/derevo_3");
        derevo_4 = (AudioClip)Resources.Load("Sounds/derevo/derevo_4");
        derevo_5 = (AudioClip)Resources.Load("Sounds/derevo/derevo_5");
        derevo_6 = (AudioClip)Resources.Load("Sounds/derevo/derevo_6");
        derevo_7 = (AudioClip)Resources.Load("Sounds/derevo/derevo_7");
        derevo_8 = (AudioClip)Resources.Load("Sounds/derevo/derevo_8");
        derevo_9 = (AudioClip)Resources.Load("Sounds/derevo/derevo_9");
        derevo_10 = (AudioClip)Resources.Load("Sounds/derevo/derevo_10");
        derevo.Add(derevo_1);
        derevo.Add(derevo_2);
        derevo.Add(derevo_3);
        derevo.Add(derevo_4);
        derevo.Add(derevo_5);
        derevo.Add(derevo_6);
        derevo.Add(derevo_7);
        derevo.Add(derevo_8);
        derevo.Add(derevo_9);
        derevo.Add(derevo_10);

        grab = (AudioClip)Resources.Load("Sounds/add_to_wall");
        release = (AudioClip)Resources.Load("Sounds/cut_from_wall");
        stones_big = (AudioClip)Resources.Load("Sounds/kamni_big");
        stones_medium = (AudioClip)Resources.Load("Sounds/kamni_medium");
        stones_small = (AudioClip)Resources.Load("Sounds/kamni_small");
        stones_punch = (AudioClip)Resources.Load("Sounds/lopata_4");

        brick_colliding = (AudioClip)Resources.Load("Sounds/brick_x_brick_short");
    }

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        _audio = sources[0];
        _audio2 = sources[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip _clip)
    {
        _audio.PlayOneShot(_clip);
    }

    public AudioClip GetContactSound(sound_type st)
    {
        //int rand;
        //switch (st)
        //{
        //    case sound_type.dirt:
        //        rand = Random.Range(0, gryaz.Count);
        //        return gryaz[rand];
        //    case sound_type.metal:
        //        rand = Random.Range(0, zhelezo.Count);
        //        return zhelezo[rand];
        //    case sound_type.wood:
        //        rand = Random.Range(0, derevo.Count);
        //        return derevo[rand];
        //    default: return gryaz[0];
        //}
        switch(st)
        {
            case sound_type.brick:
                return brick_colliding;
            default:
                return stones_punch;
        }
    }

    public AudioClip GetFlyingSound()
    {
        //int rand = Random.Range(0, svist.Count);
        //return svist[rand];
        return stones_big;
    }

    public void PlayGrabSound()
    {
        if (!_audio.isPlaying)
        {
            _audio.clip = grab;
            _audio.PlayOneShot(grab);
        }
        else
        {
            _audio2.clip = grab;
            _audio2.PlayOneShot(grab);
        }
    }

    public void PlayReleaseSound()
    {
        if (!_audio.isPlaying)
        {
            _audio.clip = release;
            _audio.PlayOneShot(release);
        }
        else
        {
            _audio.clip = release;
            _audio.PlayOneShot(release);
        }
    }

    public void SetVolume(float vol)
    {
        if (_audio.volume != vol)
            _audio.volume = vol;
        if (_audio2.volume != vol)
            _audio2.volume = vol;
    }

    public AudioClip GetFlyingSound(int type)
    {
        switch (type)
        {
            case 2:
                return stones_small;
            case 5:
                return stones_medium;
            case 10:
                return stones_big;
            default:
                return stones_big;

        }
    }
}
