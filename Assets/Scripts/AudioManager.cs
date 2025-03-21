using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip Intro; // Treated as a transition
    public AudioClip LoopA;
    public AudioClip AtoB;
    public AudioClip LoopB;
    public AudioClip BtoC;
    public AudioClip LoopC;
    public AudioClip CtoB;
    public AudioClip BtoA;
    public AudioClip CtoA;

    [Header("Audio Sources")]
    public AudioSource primarySource; // Current track
    public AudioSource secondarySource; // Next track

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // Duration of crossfade in seconds

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
        // Play the intro as a transition to LoopA
        PlayTransition(Intro, LoopA);
    }

    public void PlayLoopA()
    {
        if (!isTransitioning && primarySource.clip != LoopA)
        {
            PlayTransition(null, LoopA);
        }
    }

    public void PlayLoopB()
    {
        if (!isTransitioning && primarySource.clip != LoopB)
        {
            PlayTransition(null, LoopB);
        }
    }

    public void PlayLoopC()
    {
        if (!isTransitioning && primarySource.clip != LoopC)
        {
            PlayTransition(null, LoopC);
        }
    }

    public void PlayAtoB()
    {
        if (!isTransitioning)
        {
            PlayTransition(AtoB, LoopB);
        }
    }

    public void PlayBtoC()
    {
        if (!isTransitioning)
        {
            PlayTransition(BtoC, LoopC);
        }
    }

    public void PlayCtoB()
    {
        if (!isTransitioning)
        {
            PlayTransition(CtoB, LoopB);
        }
    }

    public void PlayBtoA()
    {
        if (!isTransitioning)
        {
            PlayTransition(BtoA, LoopA);
        }
    }

    public void PlayCtoA()
    {
        if (!isTransitioning)
        {
            PlayTransition(CtoA, LoopA);
        }
    }

    private void PlayTransition(AudioClip transitionClip, AudioClip loopClip)
    {
        if (!isTransitioning)
        {
            StartCoroutine(CrossFade(transitionClip, loopClip));
        }
    }

    private IEnumerator CrossFade(AudioClip transitionClip, AudioClip loopClip)
    {
        isTransitioning = true;

        // If a transition clip is provided, play it first
        if (transitionClip != null)
        {
            // Set up the secondary source with the transition clip
            secondarySource.clip = transitionClip;
            secondarySource.loop = false;
            secondarySource.Play();

            // Fade out the primary source and fade in the secondary source
            yield return StartCoroutine(FadeSources(primarySource, secondarySource));

            // Wait until the transition clip is almost finished
            float transitionEndTime = transitionClip.length - fadeDuration;
            yield return new WaitForSeconds(transitionEndTime);

            // Set up the primary source with the loop clip and synchronize it
            primarySource.clip = loopClip;
            primarySource.loop = true;

            // Sync positions for seamless transition
            primarySource.timeSamples = secondarySource.timeSamples % loopClip.samples;
            primarySource.Play();

            // Fade out the secondary source and fade in the primary source
            yield return StartCoroutine(FadeSources(secondarySource, primarySource));
        }
        else
        {
            // If no transition clip, crossfade directly to the loop clip
            secondarySource.clip = loopClip;
            secondarySource.loop = true;
            secondarySource.Play();

            yield return StartCoroutine(FadeSources(primarySource, secondarySource));

            // Swap the roles of primary and secondary sources
            AudioSource temp = primarySource;
            primarySource = secondarySource;
            secondarySource = temp;
        }

        isTransitioning = false;
    }

    private IEnumerator FadeSources(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        float elapsedTime = 0f;
        float startVolumeOut = fadeOutSource.volume;
        float startVolumeIn = fadeInSource.volume;

        while (elapsedTime < fadeDuration)
        {
            fadeOutSource.volume = Mathf.Lerp(startVolumeOut, 0, elapsedTime / fadeDuration);
            fadeInSource.volume = Mathf.Lerp(startVolumeIn, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOutSource.Stop();
        fadeOutSource.volume = startVolumeOut; // Reset volume
        fadeInSource.volume = 1f; // Ensure full volume
    }
}