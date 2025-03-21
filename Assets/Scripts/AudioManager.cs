using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip Intro;
    public AudioClip LoopA;
    public AudioClip AtoB;
    public AudioClip LoopB;
    public AudioClip BtoC;
    public AudioClip LoopC;
    public AudioClip CtoB;
    public AudioClip BtoA;
    public AudioClip CtoA;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f; // Duration of fade in/out in seconds

    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayIntro();
    }

    public void PlayIntro()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayIntroSequence());
        }
    }

    private IEnumerator PlayIntroSequence()
    {
        isTransitioning = true;

        // Play the Intro clip
        audioSource.clip = Intro;
        audioSource.loop = false;
        audioSource.Play();

        // Wait for the Intro clip to finish
        yield return new WaitForSeconds(Intro.length);

        // Fade in LoopA
        yield return StartCoroutine(FadeToClip(LoopA));

        isTransitioning = false;
    }

    public void PlayLoopA()
    {
        if (!isTransitioning && audioSource.clip != LoopA)
        {
            StartCoroutine(FadeToClip(LoopA));
        }
    }

    public void PlayLoopB()
    {
        if (!isTransitioning && audioSource.clip != LoopB)
        {
            StartCoroutine(FadeToClip(LoopB));
        }
    }

    public void PlayLoopC()
    {
        if (!isTransitioning && audioSource.clip != LoopC)
        {
            StartCoroutine(FadeToClip(LoopC));
        }
    }

    public void PlayAtoB()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayTransition(AtoB, LoopB));
        }
    }

    public void PlayBtoC()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayTransition(BtoC, LoopC));
        }
    }

    public void PlayCtoB()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayTransition(CtoB, LoopB));
        }
    }

    public void PlayBtoA()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayTransition(BtoA, LoopA));
        }
    }

    public void PlayCtoA()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayTransition(CtoA, LoopA));
        }
    }

    private IEnumerator PlayTransition(AudioClip transitionClip, AudioClip loopClip)
    {
        isTransitioning = true;

        // Fade out the current clip
        yield return StartCoroutine(FadeOutClip());

        // Play the transition clip
        audioSource.clip = transitionClip;
        audioSource.loop = false;
        audioSource.Play();

        // Wait for the transition clip to finish
        yield return new WaitForSeconds(transitionClip.length);

        // Fade in the loop clip
        yield return StartCoroutine(FadeToClip(loopClip));

        isTransitioning = false;
    }

    private IEnumerator FadeToClip(AudioClip clip)
    {
        // Fade out the current clip (if any)
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutClip());
        }

        // Fade in the new clip
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();

        float elapsedTime = 0f;
        float startVolume = 0f;
        float targetVolume = 1f;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutClip()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume
    }
}