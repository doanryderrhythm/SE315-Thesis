using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private MusicLibrary musicLibrary;
    [SerializeField] private AudioSource musicSource;

    private Coroutine musicFadeCoroutine;

    public void PlayMusic(string name, float fadeDuration = 0.5f)
    {
        AudioClip nextMusic = musicLibrary.GetClipFromName(name);
        
        if (nextMusic == musicSource.clip && musicSource.isPlaying)
        {
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(MusicCrossFade(musicLibrary.GetClipFromName(name), fadeDuration));
    }

    public void PlaySceneMusic(int sceneNumber) //temp =))
    {
        if (sceneNumber < 0 || sceneNumber >= musicLibrary.tracks.Length)
        {
            Debug.LogWarning("Scene index out of range.");
            return;
        }

        string name = sceneNumber switch
        {
            0 => "Shadows in Combat",
            1 => "Shadows in Combat",
            2 => "Blazing Blades",
            3 => "Havoc Protocol",
            4 => "High Velocity",
            5 => "Shadows in Combat",
            _ => null
        };

        if (name != null)
        {
            PlayMusic(name);
        }
    }

    IEnumerator MusicCrossFade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0f;
        float startVolume = musicSource.volume;
        while (percent < 1f)
        {
            percent += Time.unscaledDeltaTime / fadeDuration;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        Debug.Log(musicSource.clip.name);
        musicSource.Play();

        percent = 0f;
        while (percent < 1f)
        {
            percent += Time.unscaledDeltaTime / fadeDuration;
            musicSource.volume = Mathf.Lerp(0f, startVolume, percent);
            yield return null;
        }
    }
}
