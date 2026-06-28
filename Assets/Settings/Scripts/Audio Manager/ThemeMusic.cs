using UnityEngine;

public class ThemeMusic : MonoBehaviour
{
    [SerializeField] private string defaultTrackName = "Battle Theme 1";

    private void Start()
    {
        string trackToPlay = defaultTrackName;
        if (!string.IsNullOrEmpty(GameSettings.selectedMapsThemeTrack))
        {
            trackToPlay = GameSettings.selectedMapsThemeTrack;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Music.PlayMusic(trackToPlay);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found.");
        }
    }
}
