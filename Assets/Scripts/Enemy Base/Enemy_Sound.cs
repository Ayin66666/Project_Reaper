using UnityEngine;


public class Enemy_Sound : MonoBehaviour
{
    [Header("---Sound Setting---")]
    [SerializeField] private AudioClip spawnClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;
    public enum PublicSound { Spawn, Hit, Die }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SoundPlay_public(PublicSound type)
    {
        switch (type)
        {
            case PublicSound.Spawn:
                audioSource.PlayOneShot(spawnClip);
                break;

            case PublicSound.Hit:
                audioSource.PlayOneShot(hitClip);
                break;

            case PublicSound.Die:
                audioSource.PlayOneShot(dieClip);
                break;
        }
    }

    public void SoundPlay_Other(int soundIndex)
    {
        if (soundIndex > audioClips.Length) return;
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }
}
