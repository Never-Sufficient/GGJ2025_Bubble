using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
/// <summary>
/// 声音管理类
/// </summary>
public class SoundManager : MonoBehaviour
{
    public float musicVol, effectsVol;
    public static SoundManager Instance;

    [Serializable]
    public struct PrefabData
    {
        public string _clipName;
        public AudioClip _clipSource;
    }
    [Tooltip("可以使用字符串绑定相应的音乐资源，方便统一调用和录入")]
    public List<PrefabData> _soundClipList = new List<PrefabData>();
    public GameObject _soundPrefab;

    public Dictionary<string, AudioClip> soundClip = new Dictionary<string, AudioClip>();
    private Dictionary<AudioClip, float> soundTime = new Dictionary<AudioClip, float>();

    private void Start()
    {
        // 字典内容添加
        for (int i = 0; i < _soundClipList.Count; i++)
        {
            soundClip.Add(_soundClipList[i]._clipName, _soundClipList[i]._clipSource);
            soundTime.Add(_soundClipList[i]._clipSource, 0f);
        }
        musicVol = _musicSource.volume;
        effectsVol = _effectsSource.volume;
    }


    [SerializeField] private AudioSource _musicSource, _effectsSource, _environment, leaves, wind, bird,engine,hook;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void MusicPlayClip(AudioClip clip)
    {
        //if (_musicSource.clip != null) soundTime[_musicSource.clip] = _musicSource.time;
        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.Play();
        //_musicSource.time = soundTime[clip];
    }
    public void PlaySound()
    {
        leaves.volume = _musicSource.volume;
        wind.volume = _musicSource.volume;
        leaves.Play();
        wind.Play();
    }
    public void StopSound()
    {
        leaves.Stop();
        wind.Stop();
    }

    public void PlayBirdSound()
    {
        bird.volume = _musicSource.volume;
        bird.Play();
    }
    public void StopBirdSound()
    {
        bird.Stop();
    }
    public void PlayEngine()
    {
        engine.volume = 0.3f;
        engine.Play();
    }
    public void StopEngine()
    {
        engine.Stop();
    }
    public void PlayHookSound()
    {
        hook.volume = _musicSource.volume;
        hook.Play();
    }
    public void StopHookSound()
    {
        hook.Stop();
    }
    public bool getHookState()
    {
        return hook.isPlaying;
    }
    public void EffectPlayClip(AudioClip clip)
    {
        GameObject audioSource = Instantiate(_soundPrefab);
        audioSource.GetComponent<AudioSource>().volume = _effectsSource.volume;
        audioSource.GetComponent<AudioSource>().mute = _effectsSource.mute;
        audioSource.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(audioSource, 5.0f);
    }

    public void ChangeVolumeMusic(float value)
    {
        _musicSource.volume = value;
        musicVol = value;
    }
    public void ChangeVolumeEffects(float value)
    {
        _effectsSource.volume = value;
        _environment.volume = value;
        effectsVol = value;
    }
    public float getEngineVolume()
    {
        return engine.volume;
    }
    public void enLargeEngineVoulume()
    {
        engine.volume += 0.01f;
    }
    public void resetEngineVolume()
    {
        engine.volume = 0.5f;
    }

    public void ToggleEffects()
    {
        _effectsSource.mute = !_effectsSource.mute;
        _environment.mute = !_effectsSource.mute;
    }
    public void StopMusic()
    {

        _musicSource.Stop();
        engine.Stop();
    }

    public void MusicPlayStr(string str)
    {
        if (soundClip.ContainsKey(str)) MusicPlayClip(soundClip[str]);
    }
    public void EffectPlayStr(string str)
    {
        if (soundClip.ContainsKey(str)) EffectPlayClip(soundClip[str]);
    }

    public void EnvironmentStop()
    {
        _environment.Stop();
    }
    public void EnvironmentPlay()
    {
        _environment.Play();
    }



}
