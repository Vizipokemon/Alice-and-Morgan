using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> keyboardClicks;
    private const float keyboardClicksVolume = 0.45f;
    [SerializeField] private List<AudioClip> keyboardMultiClicks;
    private const float keyboardMultiClicksVolume = 0.45f;
    [SerializeField] private AudioClip pop;
    private const float popVolume = 1f;

    private static AudioPlayer instance;
    public static AudioPlayer Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayAudio(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void PlayRandomFromList(List<AudioClip> audioClips, float volume)
    {
        int index = Random.Range(0, audioClips.Count);
        PlayAudio(audioClips[index], volume);
    }

    public void PlayRandomKeyboardClick()
    {
        PlayRandomFromList(keyboardClicks, keyboardClicksVolume);
    }

    public void PlayRandomKeyBoardMultiClick()
    {
        PlayRandomFromList(keyboardMultiClicks, keyboardMultiClicksVolume);
    }

    public void PlayPop()
    {
        PlayAudio(pop, popVolume);
    }
}
