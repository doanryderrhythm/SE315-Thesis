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
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttons)
        {
            EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            eventTrigger.triggers.RemoveAll(t => t.eventID == EventTriggerType.PointerEnter);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { PlayHoverSound(); });

            eventTrigger.triggers.Add(entry);
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
}
