using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    private float maxLoopVolume = 0.5f;

    private Coroutine sfxFadeCoroutine;

    private void Awake()
    {
        
    }

    public void PlaySound3D(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlaySound3D(string name, Vector3 position)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(name), position);
    }

    public void PlaySound2D(string name, float pitch = 1f)
    {
        if (name == "UI_Hover" || name == "UI_Click")
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
        }
        else
        {
            sfxSource.pitch = pitch;
        }

        sfxSource.PlayOneShot(sfxLibrary.GetClipFromName(name));
    }

    public void PlayLoop(string name, float pitch = 1f)
    {
        AudioClip newClip = sfxLibrary.GetClipFromName(name);

        if (loopSource.isPlaying && loopSource.clip == newClip) return;

        if (sfxFadeCoroutine != null) StopCoroutine(sfxFadeCoroutine);

        loopSource.clip = newClip;
        loopSource.pitch = pitch;
        loopSource.loop = true;
        loopSource.volume = 0f;
        loopSource.Play();

        sfxFadeCoroutine = StartCoroutine(Fade(0f, maxLoopVolume));
    }

    public void StopLoop()
    {
        if(sfxFadeCoroutine != null) StopCoroutine(sfxFadeCoroutine);
        sfxFadeCoroutine = StartCoroutine(Fade(maxLoopVolume, 0f));
    }

    private IEnumerator Fade(float from, float to, float fadeDuration = 0.5f)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += Time.unscaledDeltaTime / fadeDuration;
            loopSource.volume = Mathf.Lerp(from, to, percent);
            yield return null;
        }

        if (loopSource.volume < 0.1f)
        {
            loopSource.Stop();
        }
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
        PlaySound2D("UI_Hover");
    }
    public void PlayClickSound()
    {
        PlaySound2D("UI_Click");
    }
    #endregion
}