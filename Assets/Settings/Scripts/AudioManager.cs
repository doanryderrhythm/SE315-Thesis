using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource musicSource, sfxSource;
    private Coroutine musicFadeCoroutine;

    [Header("Scene Music")]
    [SerializeField] AudioClip[] sceneMusic;

    [Header("UI Sounds")]
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(InitializeVolume());
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignButtonSound();
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneMusic(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetVolume(string key, float volume)
    {
        audioMixer.SetFloat(key, Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        Debug.Log(volume);

        PlayerPrefs.SetFloat(key, volume);
    }

    private void LoadVolume()
    {
        float masterVolume = PlayerPrefs.GetFloat("Master", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
        float bgmVolume = PlayerPrefs.GetFloat("BGM", 1f);
        SetVolume("Master", masterVolume);
        SetVolume("SFX", sfxVolume);
        SetVolume("BGM", bgmVolume);
    }

    private System.Collections.IEnumerator InitializeVolume()
    {
        yield return null;
        LoadVolume();
    }

    #region UI Sound Management
    public void AssignButtonSound()
    {
        Button[] buttons = GameObject.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (Button button in buttons)
        {
            if (button.gameObject.GetComponent<HoverSound>() == null)
            {
                button.gameObject.AddComponent<HoverSound>();
            }
            if (button.gameObject.GetComponent<ClickSound>() == null)
            {
                button.gameObject.AddComponent<ClickSound>();
            }
        }
    }
    public void PlayHoverSound()
    {
        if (hoverSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(hoverSound);
        }
    }
    public void PlayClickSound()
    {
        if (clickSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(clickSound);
        }
    }
    #endregion

    #region Scene Music Management
    public void SceneMusic(int sceneNumber)
    {
        if (sceneNumber < 0 || sceneNumber >= sceneMusic.Length)
        {
            Debug.LogWarning("Scene index larger than number of musics.");
            return;
        }

        AudioClip nextMusic = sceneMusic[sceneNumber];

        if (musicSource.clip == nextMusic && musicSource.isPlaying)
        {
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }
        
        musicFadeCoroutine = StartCoroutine(CrossFadeMusic(nextMusic, 1f));
    }

    private IEnumerator CrossFadeMusic(AudioClip nextClip, float duration)
    {
        float elapsedTime = 0f;
        float startVolume = musicSource.volume;
        while (elapsedTime < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        musicSource.Stop();

        musicSource.clip = nextClip;
        Debug.Log(musicSource.clip.name);
        musicSource.Play();
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        musicSource.volume = startVolume;
        musicSource.Play();
    }
    #endregion
}

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayHoverSound();
    }
}

public class ClickSound : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayClickSound();
    }
}