using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Manager : MonoBehaviour
{
    public static Sound_Manager instance;

    [Header("---Audio Source---")]
    [SerializeField] AudioSource bgm_AudioSource;
    [SerializeField] AudioSource other_SFX_AudioSource;
    [SerializeField] AudioSource player_SFX_AudioSource;

    [Header("---AudioClip---")]
    public AudioClip onClick;

    public bool isPlay;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SFXPlay_OneShot(AudioClip clip)
    {
        Debug.Log("Àç»ý");
        player_SFX_AudioSource.clip = clip;
        player_SFX_AudioSource.PlayOneShot(clip);
        StartCoroutine(nameof(Delay));
    }

    public void SFXPlay_Loop(AudioClip clip)
    {
        player_SFX_AudioSource.clip = clip;
        player_SFX_AudioSource.loop = true;
        player_SFX_AudioSource.Play();
    }

    public void Play_Running_Sound(AudioClip clip)
    {
        player_SFX_AudioSource.clip = clip;
        player_SFX_AudioSource.loop = true;

        if(!player_SFX_AudioSource.isPlaying)
        {
            player_SFX_AudioSource.Play();
        }
    }

    public void Stop_Running_Sound(AudioClip clip)
    {
        player_SFX_AudioSource.clip = clip;
        player_SFX_AudioSource.Stop();
        player_SFX_AudioSource.clip = null;
    }

    public void Play_Dash_Sound(AudioClip clip)
    {
        player_SFX_AudioSource.clip = null;
        player_SFX_AudioSource.clip = clip;
        player_SFX_AudioSource.PlayOneShot(clip);
        //StartCoroutine(nameof(Player_AudioDelay));
    }

    public void Play_Enemy_Contact_Sound(AudioClip clip)
    {
        other_SFX_AudioSource.loop = false;
        other_SFX_AudioSource.PlayOneShot(clip);
        //StartCoroutine(nameof(Delay));
    }

    private IEnumerator Delay()
    {
        isPlay = true;
        while (player_SFX_AudioSource.isPlaying)
        {
            yield return null;
        }
        isPlay = false;
        player_SFX_AudioSource.clip = null;
    }

    private IEnumerator Player_AudioDelay()
    {
        isPlay = true;
        while (player_SFX_AudioSource.isPlaying)
        {
            yield return null;
        }
        isPlay = false;
        player_SFX_AudioSource.clip = null;
    }
}
