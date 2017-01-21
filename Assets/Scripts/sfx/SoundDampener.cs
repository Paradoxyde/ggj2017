using UnityEngine;

public class SoundDampener
{
    private float _currentVolume = 1f;

    private float _dampeningPerPlay = 0.25f; // How much the sound is dampened every time the clip is played.
    private float _threshold = 0.5f; // Clips won't be played below this volume.
    private float _volumeIncreaseWhenIgnored = 0.15f; // If a clip is ignored, this value will be added to the volume.
    private float _volumeIncreasePerSecond = 1f;

    private bool _capAtThreshold = false; // If true, the value will never go below the threshold value.

    public SoundDampener() { }

    public SoundDampener (float dampeningPerPlay, float threshold = 0.5f, float volumeIncreaseWhenIgnored = 0.15f, float volumeIncreasePerSecond = 1f, bool capAtThreshold = false)
    {
        _dampeningPerPlay = dampeningPerPlay;
        _threshold = threshold;
        _volumeIncreaseWhenIgnored = volumeIncreaseWhenIgnored;
        _volumeIncreasePerSecond = volumeIncreasePerSecond;
        _capAtThreshold = capAtThreshold;
    }

    public float getNextVolume()
    {
        if (_currentVolume < _threshold)
        {
            _currentVolume += _volumeIncreaseWhenIgnored;
            return 0;
        }
        else
        {
            float volume = _currentVolume;
            _currentVolume -= _dampeningPerPlay;

            if (_capAtThreshold && _currentVolume < _threshold)
                _currentVolume = _threshold;
            return volume;
        }
    }

    public void regenVolume()
    {
        _currentVolume = Mathf.Min(_currentVolume + _volumeIncreasePerSecond * Time.deltaTime, 1f);
    }

    public void setShouldCapAtThreshold(bool doesCap)
    {
        _capAtThreshold = doesCap;
    }
}
