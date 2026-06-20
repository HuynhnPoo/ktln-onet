using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonBase<SoundManager>
{
    [SerializeField] private AudioClip musicGame;

    [SerializeField] private AudioClip[] sfxList;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSoucre;

    private Dictionary<string, AudioClip> sfxDic = new Dictionary<string, AudioClip>();

    private static bool isResseted = false;

    public bool IsResseted { get => isResseted; set => isResseted = value; }

    protected override void Awake()
    {
        base.Awake();

        musicSource = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        sfxSoucre = transform.GetChild(1).gameObject.GetComponent<AudioSource>();
    }

    private void OnSceneLoaded(Scene sceen, LoadSceneMode sceneMode)
    {
        musicGame = Resources.Load<AudioClip>("Sound/MainSound");
        sfxList = Resources.LoadAll<AudioClip>("Sound/SFX");

        PlayMusic(musicGame);

        foreach (AudioClip sfx in sfxList)
        {
            if (sfx != null && !sfxDic.ContainsKey(sfx.name))
            {
                sfxDic.Add(sfx.name, sfx);
                Debug.Log("tên của cac sfx là"+ sfx.name);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Start is called before the first frame update


    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.volume = 0.7f;
        musicSource.Play();

    }

    private void Update()
    {

    }

    public void PlaySfx(string nameClip)
    {
        if (sfxDic.TryGetValue(nameClip, out AudioClip clip))
        {
            sfxSoucre.PlayOneShot(clip, sfxSoucre.volume);
        }
        else
        {
            Debug.LogWarning($"SFX '{nameClip}' not found!");
        }
    }

    public float SetMusicGame(float amount)
    {
        musicSource.volume = Mathf.Clamp01(amount);

        return musicSource.volume;
    }

    public float SetSFXGame(float amount)
    {

        sfxSoucre.volume = Mathf.Clamp01(amount);
        return sfxSoucre.volume;
    }


    public void ResetMusicAll()
    {
        //PlaySfx("");
        isResseted = true;

        //PlayerPrefs.DeleteKey(StringManager.musicSave);
        //PlayerPrefs.DeleteKey(StringManager.sfxSave);

        float defaultVolume = 0.7f;

        SetMusicGame(defaultVolume);
        SetSFXGame(defaultVolume);

        //PlayerPrefs.SetFloat(StringManager.musicSave, defaultVolume);
        //PlayerPrefs.SetFloat(StringManager.sfxSave, defaultVolume);

    }

}