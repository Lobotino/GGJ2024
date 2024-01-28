using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public GameController gameController;
    [SerializeField] public AudioSource backgroundAudio;

    [SerializeField] public AudioClip angryKing1;
    [SerializeField] public AudioClip angryKing2;
    [SerializeField] public AudioClip angryKing3;
    [SerializeField] public AudioClip goodKing1;
    [SerializeField] public AudioClip goodKing2;

    [SerializeField] public AudioClip goodEnding;
    [SerializeField] public AudioClip badEnding;
    [SerializeField] public AudioClip balance;
    [SerializeField] public AudioClip haha;
    [SerializeField] public AudioClip balls;
    [SerializeField] public AudioClip knifes;

    private AudioSource _audioSource;

    private float currentKingFun = 100f;

    private float lastKingSoundTimer = 0f;

    private void UpdateFun(float newFun)
    {
        if (KingSoundSoundLongTimeAgo())
        {
            if (newFun > currentKingFun)
            {
                switch (newFun - 5)
                {
                    case >= 60:
                        lastKingSoundTimer = 0;
                        PlayHappyKing1();
                        break;
                    case >= 40:
                        lastKingSoundTimer = 0;
                        PlayHappyKing2();
                        break;
                }
            }
            else
            {
                switch (newFun + 5)
                {
                    case <= 20:
                        lastKingSoundTimer = 0;
                        PlayAngryKing3();
                        break;
                    case <= 40:
                        lastKingSoundTimer = 0;
                        PlayAngryKing2();
                        break;
                    case <= 60:
                        lastKingSoundTimer = 0;
                        PlayAngryKing1();
                        break;
                }
            }
        }

        currentKingFun = newFun;
    }

    private void PlayAngryKing3()
    {
        _audioSource.PlayOneShot(angryKing3);
    }

    private void PlayAngryKing2()
    {
        _audioSource.PlayOneShot(angryKing2);
    }

    private void PlayHappyKing2()
    {
        _audioSource.PlayOneShot(goodKing2);
    }

    private void PlayHappyKing1()
    {
        _audioSource.PlayOneShot(goodKing1);
    }

    private bool KingSoundSoundLongTimeAgo()
    {
        if (!(lastKingSoundTimer >= 15f)) return false;
        return true;
    }

    void Update()
    {
        UpdateFun(gameController.localKingFun);

        lastKingSoundTimer += Time.deltaTime;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAngryKing1()
    {
        _audioSource.PlayOneShot(angryKing1);
    }

    public void PlayGoodEnding()
    {
        _audioSource.PlayOneShot(goodEnding);
    }

    public void PlayBadEnding()
    {
        _audioSource.PlayOneShot(badEnding);
    }

    public void PlayBalls()
    {
        _audioSource.PlayOneShot(balls);
    }

    public void PlayBalance()
    {
        _audioSource.PlayOneShot(balance);
    }

    public void StopSound()
    {
        _audioSource.Stop();
        backgroundAudio.Stop();
    }

    public void PlayHaha()
    {
        _audioSource.PlayOneShot(haha);
    }
}