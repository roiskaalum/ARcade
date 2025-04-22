using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // members
    public static SoundManager instance;

    public bool debug;

    public AudioTrack[] tracks;

    private Hashtable m_AudioTable; // relationship between audio types (key) and audio tracks (value)
    private Hashtable m_JobTable;   // relationship between audio types (key) and jobs (value) (coroutine, IEnumerator)

    [System.Serializable]
    public class AudioObject
    {
        public AudioType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audio;
    }

    private class AudioJob
    {
        public AudioAction action;

        public AudioType type;
        public bool fade;
        public float delay;
        public float pitch;

        public AudioJob(AudioAction action, AudioType type, bool fade, float delay, float pitch)
        {
            this.action = action;
            this.type = type;
            this.fade = fade;
            this.delay = delay;
            this.pitch = pitch;
        }
    }

    private enum AudioAction
    {
        START,
        STOP,
        RESTART
    }


    #region Unity Functions
    private void Awake()
    {
        if (!instance)
        {
            Configure();
        }
    }

    private void OnDisable()
    {
        Dispose();
    }


    #endregion

    #region Public Functions

    public void PlayAudio(AudioType type, bool fade = false, float delay = 0.0f, float pitch = 1.0f)
    {
        AddJob(new AudioJob(AudioAction.START, type, fade, delay, pitch));
    }
    public void StopAudio(AudioType type, bool fade = false, float delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.STOP, type, fade, delay, 1.0f));
    }
    public void RestartAudio(AudioType type, bool fade = false, float delay = 0.0f, float pitch = 1.0f)
    {
        AddJob(new AudioJob(AudioAction.RESTART, type, fade, delay, pitch));
    }

    public void ChangePitch(float newPitch, AudioType audioType = AudioType.None, AudioSource source = null)
    {
        if ((audioType != AudioType.None))
        {
            if (!m_AudioTable.ContainsKey(audioType))
            {
                LogWarning($"Cannot change pitch, AudioType[{audioType}] not found.");
                return;
            }

            AudioTrack track = (AudioTrack)m_AudioTable[audioType];
            track.source.pitch = newPitch;
            Log($"Changed pitch of AudioType[{audioType}] to {newPitch}");
            return;
        }

        if (source == null)
        {
            LogWarning("Cannot change pitch, AudioSource is null.");
            return;
        }

        foreach (DictionaryEntry entry in m_AudioTable)
        {
            AudioType type = (AudioType)entry.Key;
            AudioTrack track = (AudioTrack)entry.Value;

            if (track.source == source)
            {
                track.source.pitch = newPitch;
                Log($"Changed pitch of AudioType[{type}] to {newPitch}");
                return;
            }
        }
        LogWarning("Cannot change pitch, AudioSource not found in any AudioTrack.");
    }

    #endregion

        #region Private Functions

    private void Configure()
    {
        instance = this;

        m_AudioTable = new Hashtable();
        m_JobTable = new Hashtable();
        GenerateAudioTable();
    }

    private void Dispose()
    {
        foreach (DictionaryEntry entry in m_JobTable)
        {
            IEnumerator job = (IEnumerator)entry.Value;
            StopCoroutine(job);
        }
    }

    private void GenerateAudioTable()
    {
        foreach (AudioTrack _track in tracks)
        {
            foreach (AudioObject _obj in _track.audio)
            {
                // Do not duplicate keys
                if (m_AudioTable.ContainsKey(_obj.type))
                {
                    LogWarning("You're trying to register audio [" + _obj.type + "] that has already been registered.");
                }
                else
                {
                    m_AudioTable.Add(_obj.type, _track);
                    Log("Registered Audio: [" + _obj.type + "]");
                }

            }
        }
    }

    private IEnumerator RunAudioJob(AudioJob job)
    {
        yield return new WaitForSeconds(job.delay);

        AudioTrack track = (AudioTrack)m_AudioTable[job.type];
        track.source.clip = GetAudioClipFromAudioTrack(job.type, track);

        switch (job.action)
        {
            case AudioAction.START:
                track.source.pitch = job.pitch;
                track.source.Play();
                break;
            case AudioAction.STOP:
                if (!job.fade)
                    track.source.Stop();
                break;
            case AudioAction.RESTART:
                track.source.Stop();
                track.source.pitch = job.pitch;
                track.source.Play();
                break;
        }

        if (job.fade)
        {
            float initial = job.action == AudioAction.START || job.action == AudioAction.RESTART ? 0.0f : 1.0f;
            float target = initial == 0.0f ? 1.0f : 0.0f;
            float duration = 1.0f;
            float timer = 0.0f;

            while (timer <= duration)
            {
                track.source.volume = Mathf.Lerp(initial, target, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            if (job.action == AudioAction.STOP)
            {
                track.source.Stop();
            }
        }

        m_JobTable.Remove(job.type);
        Log("Job count: " + m_JobTable.Count);

        yield return null;
    }

    private void AddJob(AudioJob job)
    {
        //Remove Conflicting Jobs
        RemoveConflictingJobs(job.type);

        //Add and Start the job
        IEnumerator jobRunner = RunAudioJob(job);
        m_JobTable.Add(job.type, jobRunner);
        StartCoroutine(jobRunner);
        Log("Added Job: [" + job.type + "] with operation: [" + job.action + "]");
    }

    private void RemoveJob(AudioType type)
    {
        if (!m_JobTable.ContainsKey(type))
        {
            LogWarning("You're trying to stop a job [" + type + "] that is not running.");
        }

        IEnumerator runningJob = (IEnumerator)m_JobTable[type];
        StopCoroutine(runningJob);
        m_JobTable.Remove(type);
    }

    private void RemoveConflictingJobs(AudioType type)
    {
        if (m_JobTable.ContainsKey(type))
        {
            RemoveJob(type);
        }

        AudioType conflictAudio = AudioType.None;
        foreach (DictionaryEntry entry in m_JobTable)
        {
            AudioType audioType = (AudioType)entry.Key;
            AudioTrack audioTrackInUse = (AudioTrack)m_AudioTable[audioType];
            AudioTrack audioTrackNeeded = (AudioTrack)m_AudioTable[type];
            if (audioTrackNeeded.source == audioTrackInUse.source)
            {
                conflictAudio = audioType;
            }
        }
        if (conflictAudio != AudioType.None)
        {
            RemoveJob(conflictAudio);
        }
    }

    public AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
    {
        foreach (AudioObject obj in track.audio)
        {
            if (obj.type == type)
            {
                return obj.clip;
            }
        }
        return null;
    }

    private void Log(string msg)
    {
        if (!debug) return;
        Debug.Log("[Sound Manager]: " + msg);
    }
    private void LogWarning(string msg)
    {
        if (!debug) return;
        Debug.LogWarning("[Sound Manager]: " + msg);
    }

    #endregion
}