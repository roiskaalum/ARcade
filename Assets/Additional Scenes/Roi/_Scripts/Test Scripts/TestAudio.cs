using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public SoundManager soundManager;
    private float pitch = 1.0f;

    void Awake()
    {
        soundManager = SoundManager.instance;
    }


    #region Unity Functions
#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            pitch += 0.1f;
            if (pitch > 1.2f)
            {
                pitch = 1.2f;
            }
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            pitch -= 0.1f;
            if (pitch < 0.2f)
            {
                pitch = 0.2f;
            }
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            soundManager.PlayAudio(AudioType.ST_01, true, 0, pitch);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            soundManager.StopAudio(AudioType.ST_01, true, 0);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            soundManager.RestartAudio(AudioType.ST_01, true, 0, pitch);
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            soundManager.PlayAudio(AudioType.ST_02, true, 0, pitch);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            soundManager.StopAudio(AudioType.ST_02);
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            soundManager.RestartAudio(AudioType.ST_02, true, 0, pitch);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.ChangePitch(pitch, AudioType.ST_01);
        }
        Debug.Log(pitch);
    }
#endif
    #endregion
}