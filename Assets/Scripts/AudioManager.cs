using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class NamedTrack
    {
        public string trackName;
        public AudioClip clip;
    }

    public NamedTrack[] tracks;
    public static AudioManager instance = null;
    public AudioSource audioSource;
    public AudioClip bgmClip;
    public bool debugPlayNextTrack = false;
    public int currentTrackIndex = 0;

    void Awake()
    {
        VerifySingleInstance();
        VerifyAudioManager();
    }

    void VerifySingleInstance()
    {
        // Only one audioManager can exist
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void VerifyAudioManager()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (!audioSource)
        {
            Debug.LogWarning("Audio source does not exist. Please add to AudioManager");
            return;
        }
    }

    void Start()
    {
        PlayDefaultBGM();
        LoadVolume();
    }

    void Update()
    {
        if (!audioSource.isPlaying && tracks != null && tracks.Length > 0)
        {
            PlayNextTrackInRange(0, 2);
        }

        // Debug functionality to manually play the next track
        if (debugPlayNextTrack)
        {
            debugPlayNextTrack = false;
            PlayNextTrackInRange(0, 3);
        }

    }

    // playing the audio
    public void PlayBGM()
    {
        if (audioSource.clip != bgmClip)
        {
            audioSource.clip = bgmClip;
        }
        audioSource.Play();
        Debug.Log("Playing BGM clip: " + bgmClip.name);
    }

    public void PauseBGM()
    {
        audioSource.Pause();
        Debug.Log("BGM paused");
    }

    public void ResumeBGM()
    {
        audioSource.UnPause();
        Debug.Log("BGM resumed");
    }

    public void StopBGM()
    {
        audioSource.Stop();
        Debug.Log("BGM stopped");
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
        Debug.Log("BGM volume set to: " + volume);
    }

    // changing the audio
    public void PlayDefaultBGM()
    {
        if (bgmClip != null)
            PlayBGM();
        else if (tracks != null && tracks.Length > 0)
            PlayTrackByIndex(0);
        else
            Debug.LogWarning("No BGM clip or track available to play.");
    } 

    public void ChangeTrack(AudioClip newClip)
    {
        if (newClip == null || newClip == audioSource.clip) return;

        bgmClip = newClip;
        audioSource.clip = newClip;

        audioSource.Play();
        Debug.Log("Switched to new BGM: " + newClip.name);
    }

    public void PlayTrackByName(string trackName)
    {
        foreach (NamedTrack track in tracks)
        {
            if (track.trackName == trackName)
            {
                ChangeTrack(track.clip);
                return;
            }
        }

        Debug.LogWarning("Track not found: " + trackName);
    }

    public void PlayTrackByIndex(int trackIndex)
    {
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogWarning("Track list is empty or not assigned");
            return;
        }

        if (trackIndex < 0 || trackIndex >= tracks.Length)
        {
            Debug.LogWarning("Track index out of bounds: " + trackIndex);
            return;
        }

        NamedTrack newTrack = tracks[trackIndex];
        if (newTrack == null)
        {
            Debug.LogWarning("The track at index " + trackIndex + " does not exist");
            return;
        }
        
        ChangeTrack(newTrack.clip);
    }

    public void PlayNextTrackInRange(int startIndex, int endIndex)
    {
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogWarning("Track list is empty or not assigned");
            return;
        }

        // Ensure the range is valid
        if (startIndex < 0 || endIndex >= tracks.Length || startIndex > endIndex)
        {
            Debug.LogError("Invalid track range specified");
            return;
        }

        // Increment the current track index and loop back to the start of the range if necessary
        currentTrackIndex++;
        if (currentTrackIndex > endIndex)
        {
            currentTrackIndex = startIndex; // Loop back to the start of the range
        }

        // Play the next track
        PlayTrackByIndex(currentTrackIndex);
    }

    // save system
    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("bgmVolume", 1f);
        SetVolume(savedVolume);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("bgmVolume", audioSource.volume);
        PlayerPrefs.Save();
    }

}
