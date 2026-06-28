using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioMixer audioMixer;
    public MusicManager Music { get; private set; }
    public SoundManager Sound { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Music = GetComponentInChildren<MusicManager>();
            Sound = GetComponentInChildren<SoundManager>();

            StartCoroutine(InitializeVolume());
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Sound.AssignButtonSound();
    }

    public void SetVolume(string key, float volume)
    {
        audioMixer.SetFloat(key, Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        Debug.Log(volume);

        PlayerPrefs.SetFloat(key, volume);
    }

    private void LoadVolume()
    {
        SetVolume("Master", PlayerPrefs.GetFloat("Master", 1f));
        SetVolume("SFX", PlayerPrefs.GetFloat("SFX", 1f));
        SetVolume("BGM", PlayerPrefs.GetFloat("BGM", 1f));
    }

    private System.Collections.IEnumerator InitializeVolume()
    {
        yield return null;
        LoadVolume();
    }
}

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Sound.PlayHoverSound();
    }
}

public class ClickSound : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Sound.PlayClickSound();
    }
}