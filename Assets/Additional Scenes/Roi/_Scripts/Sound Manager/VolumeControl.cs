using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 30.0f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }
    private void OnEnable()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
        mixer.SetFloat(volumeParameter, Mathf.Log10(slider.value) * multiplier);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        if(value <= 0.0001f)
        {
            PlayerPrefs.SetFloat(volumeParameter, -80);
        }
    }
}
