using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource sfxSource;

    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

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
    }

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

    public void AssignButtonSound()
    {
        Button[] buttons = GameObject.FindObjectsByType<Button>();
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