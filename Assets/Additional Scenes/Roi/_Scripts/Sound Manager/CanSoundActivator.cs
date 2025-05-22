using UnityEngine;

public class CanSoundActivator : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Can"))
        {
            SoundManager.instance.PlayAudio(AudioType.Can_SFX_01);
        }
    }
}