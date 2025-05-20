using UnityEngine;

public class CanSoundActivator : MonoBehaviour
{
    SoundManager soundManager;

    void Awake()
    {
        soundManager = SoundManager.instance;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Can"))
        {
            soundManager.PlayAudio(AudioType.Can_SFX_01);
        }
    }
}