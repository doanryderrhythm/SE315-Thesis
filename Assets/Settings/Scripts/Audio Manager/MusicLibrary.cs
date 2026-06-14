using UnityEngine;

[System.Serializable]

public struct MusicTrack
{
    public string name;
    public AudioClip clip;
}
public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] tracks;

    public AudioClip GetClipFromName(string name)
    {
        foreach (MusicTrack m in tracks)
        {
            if (m.name == name)
            {
                return m.clip;
            }
        }
        Debug.LogWarning("Music track not found: " + name);

        return null;
    }
}
