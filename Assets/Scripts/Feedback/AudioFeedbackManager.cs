using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gère tous les feedbacks audio : voix du coach, sons de validation, musique d'ambiance
/// </summary>
public class AudioFeedbackManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Voice Coaching Clips")]
    [SerializeField] private AudioClip[] encouragementClips;
    [SerializeField] private AudioClip[] correctionClips;
    [SerializeField] private AudioClip[] countingClips;
    [SerializeField] private AudioClip repCompleteClip;
    [SerializeField] private AudioClip setCompleteClip;
    [SerializeField] private AudioClip workoutCompleteClip;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip goodMovementSFX;
    [SerializeField] private AudioClip perfectMovementSFX;
    [SerializeField] private AudioClip badMovementSFX;
    [SerializeField] private AudioClip errorSFX;
    [SerializeField] private AudioClip achievementSFX;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.3f;

    [Header("Settings")]
    [SerializeField] private float voiceVolume = 1f;
    [SerializeField] private float sfxVolume = 0.8f;
    [SerializeField] private float minTimeBetweenVoiceClips = 2f;
    [SerializeField] private bool enableVoiceCoaching = true;
    [SerializeField] private bool enableSFX = true;
    [SerializeField] private bool enableMusic = true;

    private float lastVoiceClipTime;
    private Queue<AudioClip> voiceQueue;
    private bool isPlayingVoice;

    private void Awake()
    {
        voiceQueue = new Queue<AudioClip>();
        InitializeAudioSources();
    }

    private void Start()
    {
        if (enableMusic && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }

    /// <summary>
    /// Initialise les AudioSources avec les paramètres appropriés
    /// </summary>
    private void InitializeAudioSources()
    {
        if (voiceSource == null)
        {
            GameObject voiceObj = new GameObject("VoiceSource");
            voiceObj.transform.SetParent(transform);
            voiceSource = voiceObj.AddComponent<AudioSource>();
        }
        voiceSource.volume = voiceVolume;
        voiceSource.playOnAwake = false;

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }
        sfxSource.volume = sfxVolume;
        sfxSource.playOnAwake = false;

        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
        }
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    /// <summary>
    /// Joue un encouragement vocal aléatoire
    /// </summary>
    public void PlayEncouragement()
    {
        if (!enableVoiceCoaching || encouragementClips.Length == 0)
            return;

        AudioClip clip = encouragementClips[Random.Range(0, encouragementClips.Length)];
        QueueVoiceClip(clip);
    }

    /// <summary>
    /// Joue une correction vocale aléatoire
    /// </summary>
    public void PlayCorrection()
    {
        if (!enableVoiceCoaching || correctionClips.Length == 0)
            return;

        AudioClip clip = correctionClips[Random.Range(0, correctionClips.Length)];
        QueueVoiceClip(clip);
    }

    /// <summary>
    /// Joue un compte vocal (1, 2, 3...)
    /// </summary>
    public void PlayCount(int count)
    {
        if (!enableVoiceCoaching || countingClips.Length == 0)
            return;

        int index = Mathf.Clamp(count - 1, 0, countingClips.Length - 1);
        QueueVoiceClip(countingClips[index]);
    }

    /// <summary>
    /// Joue le son de répétition complétée
    /// </summary>
    public void PlayRepComplete()
    {
        if (repCompleteClip != null)
            QueueVoiceClip(repCompleteClip);
    }

    /// <summary>
    /// Joue le son de série complétée
    /// </summary>
    public void PlaySetComplete()
    {
        if (setCompleteClip != null)
            QueueVoiceClip(setCompleteClip);
    }

    /// <summary>
    /// Joue le son d'entraînement terminé
    /// </summary>
    public void PlayWorkoutComplete()
    {
        if (workoutCompleteClip != null)
            QueueVoiceClip(workoutCompleteClip);
    }

    /// <summary>
    /// Ajoute un clip vocal à la queue
    /// </summary>
    private void QueueVoiceClip(AudioClip clip)
    {
        if (clip == null) return;

        voiceQueue.Enqueue(clip);

        if (!isPlayingVoice)
        {
            StartCoroutine(ProcessVoiceQueue());
        }
    }

    /// <summary>
    /// Traite la queue de clips vocaux
    /// </summary>
    private IEnumerator ProcessVoiceQueue()
    {
        isPlayingVoice = true;

        while (voiceQueue.Count > 0)
        {
            // Attends le délai minimum entre les clips
            float timeSinceLastClip = Time.time - lastVoiceClipTime;
            if (timeSinceLastClip < minTimeBetweenVoiceClips)
            {
                yield return new WaitForSeconds(minTimeBetweenVoiceClips - timeSinceLastClip);
            }

            AudioClip clip = voiceQueue.Dequeue();
            voiceSource.PlayOneShot(clip);
            lastVoiceClipTime = Time.time;

            // Attends que le clip soit terminé
            yield return new WaitForSeconds(clip.length);
        }

        isPlayingVoice = false;
    }

    /// <summary>
    /// Joue un effet sonore selon la qualité du mouvement
    /// </summary>
    public void PlayMovementFeedback(MovementQuality quality)
    {
        if (!enableSFX) return;

        AudioClip clip = quality switch
        {
            MovementQuality.Perfect => perfectMovementSFX,
            MovementQuality.Good => goodMovementSFX,
            MovementQuality.Bad => badMovementSFX,
            _ => null
        };

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Joue le son d'erreur
    /// </summary>
    public void PlayError()
    {
        if (enableSFX && errorSFX != null)
        {
            sfxSource.PlayOneShot(errorSFX);
        }
    }

    /// <summary>
    /// Joue le son d'achievement
    /// </summary>
    public void PlayAchievement()
    {
        if (enableSFX && achievementSFX != null)
        {
            sfxSource.PlayOneShot(achievementSFX);
        }
    }

    /// <summary>
    /// Lance la musique de fond
    /// </summary>
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Active/désactive le coaching vocal
    /// </summary>
    public void SetVoiceCoachingEnabled(bool enabled)
    {
        enableVoiceCoaching = enabled;

        if (!enabled)
        {
            voiceSource.Stop();
            voiceQueue.Clear();
            isPlayingVoice = false;
        }
    }

    /// <summary>
    /// Active/désactive les effets sonores
    /// </summary>
    public void SetSFXEnabled(bool enabled)
    {
        enableSFX = enabled;

        if (!enabled)
        {
            sfxSource.Stop();
        }
    }

    /// <summary>
    /// Active/désactive la musique
    /// </summary>
    public void SetMusicEnabled(bool enabled)
    {
        enableMusic = enabled;

        if (enabled)
        {
            PlayBackgroundMusic();
        }
        else
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Ajuste le volume général
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        voiceSource.volume = volume * voiceVolume;
        sfxSource.volume = volume * sfxVolume;
        musicSource.volume = volume * musicVolume;
    }
}

public enum MovementQuality
{
    Perfect,
    Good,
    Bad
}