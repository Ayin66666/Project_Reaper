using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Player_Sound : MonoBehaviour
{
    public static Player_Sound instance;

    AudioSource audioSource;

    public AudioClip[] utility_Sound;
    public AudioClip[] groundAttack_Sound;
    public AudioClip[] airAttack_Sound;
    public AudioClip[] blue_Skill_Sound;
    public AudioClip[] red_Skill_Sound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void SFXPlay(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }
}
