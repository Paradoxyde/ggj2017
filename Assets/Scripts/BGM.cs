using UnityEngine;
using System.Collections;

public class BGM : TakuBehaviour
{

    public AudioClip intro;
    public AudioClip core;
    public float fadeSpeed = 0.08f;

    private AudioSource _introSource;
    private AudioSource _coreSource;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _coreSource = gameObject.AddComponent<AudioSource>();
        _coreSource.clip = core;

        GameObject introObj = new GameObject();
        introObj.transform.parent = transform;
        _introSource = introObj.AddComponent<AudioSource>();

        _introSource.clip = intro;
        _introSource.loop = true;
        _introSource.Play();
    }

    private void Update()
    {
        if (_coreSource.isPlaying)
        {
            _coreSource.volume = Mathf.Min(_coreSource.volume + fadeSpeed, 1f);
            _introSource.volume -= fadeSpeed;
        }
    }

    public void ActivateTransition()
    {
        if (!_coreSource.isPlaying)
        {
            float block = intro.length / 4f;
            _coreSource.Play();
            _coreSource.time = _introSource.time % block;
            _coreSource.volume = 0;
            _coreSource.loop = true;
        }
    }

}
