using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer & Groups")]
    public AudioMixer mainMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup weaponGroup;

    [Header("Pooling Ayarları")]
    public int poolSize = 30; 
    private List<AudioSource> audioPool = new List<AudioSource>();
    
    public AudioSource musicSource;
    public AudioClip menuMusicClip;  
    public AudioClip battleMusicClip;
    
    void SetupMusicSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup; 
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null) SetupMusicSource();

        if (musicSource.clip == menuMusicClip && musicSource.isPlaying) return;

        musicSource.clip = menuMusicClip;
        musicSource.volume = 0.5f; 
        musicSource.Play();
    }
    public void PlayBattleMusic()
    {
        if (musicSource == null) SetupMusicSource();

        
        if (musicSource.clip == battleMusicClip && musicSource.isPlaying) return;

        
        musicSource.Stop();
        musicSource.clip = battleMusicClip;
        musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        InitializePool();
    }

    
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject("PooledAudioSource_" + i);
            go.transform.SetParent(this.transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioPool.Add(source);
        }
    }

    // Havuzdan boş bir AudioSource bul
    private AudioSource GetAvailableSource()
    {
        foreach (var source in audioPool)
        {
            if (!source.isPlaying) return source;
        }

        // Eğer hepsi doluysa, en eski sesin üzerine yazabiliriz veya havuzu genişletebiliriz
        // Şimdilik havuzu dinamik olarak 1 tane genişletelim
        GameObject go = new GameObject("PooledAudioSource_Extra");
        go.transform.SetParent(this.transform);
        AudioSource newSource = go.AddComponent<AudioSource>();
        audioPool.Add(newSource);
        return newSource;
    }

    // --- SES ÇALMA FONKSİYONLARI ---

    public void PlayShootSound(AudioClip clip, float volume = 0.8f)
    {
        PlaySound(clip, weaponGroup, volume, Random.Range(0.92f, 1.08f));
    }

    public void PlaySpecialSound(AudioClip clip, float volume = 1f)
    {
        PlaySound(clip, sfxGroup, volume, 1f);
    }

    private void PlaySound(AudioClip clip, AudioMixerGroup group, float volume, float pitch)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();
        source.clip = clip;
        source.outputAudioMixerGroup = group;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        // UI sesleri için pitch sabit 1f kalsın
        PlaySound(clip, sfxGroup, volume, 1f);
    }
    public void PlayUISoundFromButton(AudioClip clip)
    {
        // Burada pitch'i 1f olarak sabitliyoruz, sorun çıkmaz.
        PlayUISound(clip, 1f);
    }

    public void SetMusicVolume(float value)
    {
        // 0.0001f kullanıyoruz çünkü Log10(0) tanımsızdır ve hata verir
        float dbValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("MusicVol", dbValue);
    }

    // Efekt sesini ayarlar
    public void SetSFXVolume(float value)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("SFXVol", dbValue);
    }
}