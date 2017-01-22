using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour
{

    public static BGM Instance { get; private set; }

    public AudioClip introMenu;
    public AudioClip coreMenu;
    public AudioClip introGame;
    public AudioClip coreGame;
    public float fadeSpeed = 0.08f;

    public float tempo = 110f;
    public float Time { get { return _gameSource.time; } }

    private AudioSource _menuSource;
    private AudioSource _gameSource;

    public bool IsInGame = true;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _menuSource = gameObject.AddComponent<AudioSource>();
        _menuSource.clip = introMenu;
        _menuSource.loop = false;
        _menuSource.Play();
        _menuSource.volume = IsInGame ? 0f : 1f;

        GameObject introObj = new GameObject();
        introObj.transform.parent = transform;
        _gameSource = introObj.AddComponent<AudioSource>();

        _gameSource.clip = introGame;
        _gameSource.loop = false;
        _gameSource.Play();
        _menuSource.volume = IsInGame ? 1f : 0f;
    }

    private void Update()
    {
        // Intro/Loop transitions
        if (!_menuSource.isPlaying)
        {
            _menuSource.clip = coreMenu;
            _menuSource.loop = true;
            _menuSource.Play();
        }

        if (!_gameSource.isPlaying)
        {
            _gameSource.clip = coreGame;
            _gameSource.loop = true;
            _gameSource.Play();
        }


        // Crossfades
        if (IsInGame)
        {
            CrossFade(_menuSource, _gameSource);
        }
        else
        {
            CrossFade(_gameSource, _menuSource);
        }
    }
    
    private void CrossFade(AudioSource from, AudioSource to)
    {
        float maxVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        to.volume = Mathf.Min(to.volume + fadeSpeed, maxVolume);
        from.volume = Mathf.Max(from.volume - fadeSpeed, 0f);
    }

}
