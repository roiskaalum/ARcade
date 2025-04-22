using UnityEngine;
using UnityEngine.Audio;

public class InitVolumeParameters : MonoBehaviour
{
    public string [] volumeParameters;
    public float multiplier = 20f;

    public AudioMixer mixer;

    private void Start()
    {
        foreach(string item in volumeParameters)
        {
            if(PlayerPrefs.HasKey(item))
            {
                float value = PlayerPrefs.GetFloat(item);
                mixer.SetFloat(item, Mathf.Log10(value) * multiplier);
            }
            else
            {
                PlayerPrefs.SetFloat(item, 1f);
                mixer.SetFloat(item, Mathf.Log10(1f) * multiplier);
            }
        }
    }
}
