using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum SFX {

        HIT_WALL = 0,
        HIT00,
        HIT01,
        HIT02,
        SHOT00,
        SHOT01,
        GAME_START,
        GAME_OVER,
        THUMP
    };

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip[] audioClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Play(SFX index)
    {
        //audioSrc.clip = audioClip[(int)index];
        //audioSrc.Play();
        audioSrc.PlayOneShot(audioClip[(int)index]);    // mixed together
    }

    // POLYMORPHISM (overloading)
    public void Play(SFX index, float vol)
    {
        audioSrc.PlayOneShot(audioClip[(int)index], vol);    // mixed together
    }

    public void PlayExplosion()
    {
        int index = Random.Range((int)SFX.HIT00, (int)SFX.HIT02 + 1);

        Play((SFX)index);
    }

    // POLYMORPHISM (overloading)
    public void PlayExplosion(float vol)
    {
        int index = Random.Range((int)SFX.HIT00, (int)SFX.HIT02 + 1);

        Play((SFX)index, vol);
    }
}
