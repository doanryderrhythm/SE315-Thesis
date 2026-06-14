using UnityEngine;

[System.Serializable]

public struct SoundEffect
{
    public string name;
    public AudioClip clip;
}
public class SoundLibrary : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    public AudioClip GetClipFromName(string name)
    {
        foreach (SoundEffect s in soundEffects)
        {
            if (s.name == name)
            {
                return s.clip;
            }
        }
        Debug.LogWarning("Sound effect not found: " + name);

        return null;
    }
}
