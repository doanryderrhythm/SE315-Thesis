using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfxSource;

    private const string hoverSound = "UI_Hover";
    private const string clickSound = "UI_Click";

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

    public void PlaySound2D(string name)
    {
        if (name == hoverSound || name == clickSound)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
        }
        else
        {
            sfxSource.pitch = 1f;
        }

        sfxSource.PlayOneShot(sfxLibrary.GetClipFromName(name));
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
        if (hoverSound != null)
        {
            PlaySound2D(hoverSound);
        }
    }
    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            PlaySound2D(clickSound);
        }
    }
    #endregion
}