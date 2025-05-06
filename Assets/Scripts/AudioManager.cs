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
    public AudioSource primarySource; // Current loop clip
    public AudioSource transitionSource1; // Preloaded transition clip 1
    public AudioSource transitionSource2; // Preloaded transition clip 2

    [Header("Fade Settings")]
    public float smallFadeDuration = 0.1f; // Duration of the small fade in seconds

    private bool isTransitioning = false;

    [Header("SFX Settings")]
    public AudioSource sfxSource; // A separate audio source for sound effects


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
        // Preload all audio clips into memory
        PreloadAudioClips();

        // Play the intro as a transition to LoopA
        PlayTransition(Intro, LoopA);
    }

    private void PreloadAudioClips()
    {
        // Preload all audio clips into memory
        Intro.LoadAudioData();
        LoopA.LoadAudioData();
        AtoB.LoadAudioData();
        LoopB.LoadAudioData();
        BtoC.LoadAudioData();
        LoopC.LoadAudioData();
        CtoB.LoadAudioData();
        BtoA.LoadAudioData();
        CtoA.LoadAudioData();
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
        PlayTransition(AtoB, LoopB);
    }

    public void PlayBtoC()
    {
        PlayTransition(BtoC, LoopC);
    }

    public void PlayCtoB()
    {
        PlayTransition(CtoB, LoopB);
    }

    public void PlayBtoA()
    {
        PlayTransition(BtoA, LoopA);
    }

    public void PlayCtoA()
    {
        PlayTransition(CtoA, LoopA);
    }

    private void PlayTransition(AudioClip transitionClip, AudioClip loopClip)
    {
        StartCoroutine(HandleTransition(transitionClip, loopClip));
    }

    private IEnumerator HandleTransition(AudioClip transitionClip, AudioClip loopClip)
    {
        isTransitioning = true;

        if (transitionClip == Intro)
        {
            // Determine which transition source to use
            AudioSource transitionSource = transitionSource1.clip == null ? transitionSource1 : transitionSource2;

            // Start the transition clip
            transitionSource.clip = transitionClip;
            transitionSource.loop = false;
            transitionSource.Play();

            yield return StartCoroutine(SmallFade(primarySource, transitionSource));

            PreLoadLoops(transitionClip);

            primarySource.Play();

            // Wait for the transition clip to finish
            yield return new WaitForSeconds(transitionClip.length - (smallFadeDuration * 8));

            // Small fade: Lower the transition source volume and increase the primary source volume
            yield return StartCoroutine(SmallFade(transitionSource, primarySource));

            // Preload the next possible transitions
            PreloadTransitions(loopClip);
        }
        // If a transition clip is provided, play it first
        else if (transitionClip != null)
        {
            // Determine which transition source to use
            AudioSource transitionSource = transitionSource1.clip == null ? transitionSource1 : transitionSource2;

            // Start the transition clip
            transitionSource.clip = transitionClip;
            transitionSource.loop = false;
            transitionSource.Play();

            PreLoadLoops(transitionClip);
            primarySource.volume = 0;
            primarySource.Play();

            // Small fade: Lower the primary source volume and increase the transition source volume
            yield return StartCoroutine(SmallFade(primarySource, transitionSource));

            // Wait for the transition clip to finish
            yield return new WaitForSeconds(transitionClip.length - (smallFadeDuration * 4));

            // Small fade: Lower the transition source volume and increase the primary source volume
            yield return StartCoroutine(SmallFade(transitionSource, primarySource));

            // Preload the next possible transitions
            PreloadTransitions(loopClip);
        }
        else
        {
            // If no transition clip, switch directly to the loop clip
            primarySource.clip = loopClip;
            primarySource.loop = true;
            primarySource.Play();

            // Preload the next possible transitions
            PreloadTransitions(loopClip);
        }

        isTransitioning = false;
    }

    private IEnumerator SmallFade(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        float elapsedTime = 0f;
        float startVolumeOut = fadeOutSource.volume;
        float startVolumeIn = fadeInSource.volume;

        while (elapsedTime < smallFadeDuration)
        {
            fadeOutSource.volume = Mathf.Lerp(startVolumeOut, 0, elapsedTime / smallFadeDuration);
            fadeInSource.volume = Mathf.Lerp(startVolumeIn, 1, elapsedTime / smallFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOutSource.volume = 0;
        fadeInSource.volume = 1;
    }

    private void PreloadTransitions(AudioClip loopClip)
    {
        if (loopClip == LoopA)
        {
            transitionSource1.clip = AtoB;
            transitionSource2.clip = null; // No second transition from LoopA
        }
        else if (loopClip == LoopB)
        {
            transitionSource1.clip = BtoC;
            transitionSource2.clip = BtoA;
        }
        else if (loopClip == LoopC)
        {
            transitionSource1.clip = CtoB;
            transitionSource2.clip = CtoA;
        }
    }

    private void PreLoadLoops(AudioClip transitionClip)
    {
        if (transitionClip == AtoB)
        {
            primarySource.clip = LoopB;
            primarySource.loop = true;
        }
        else if (transitionClip == BtoC)
        {
            primarySource.clip = LoopC;
            primarySource.loop = true;
        }
        else if (transitionClip == CtoB)
        {
            primarySource.clip = LoopB;
            primarySource.loop = true;
        }
        else if (transitionClip == BtoA)
        {
            primarySource.clip = LoopA;
            primarySource.loop = true;
        }
        else if (transitionClip == CtoA || transitionClip == Intro)
        {
            primarySource.clip = LoopA;
            primarySource.loop = true;
        }
    }
    public void PlayJumpSound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayDashSound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

}