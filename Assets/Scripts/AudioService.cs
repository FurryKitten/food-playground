using UnityEngine;

public class AudioService : MonoBehaviour, IService
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _tetrisAudioSource;
    [SerializeField] private AudioSource _tetrisSpawnAudioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip _music;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _uiButtonPress;
    [SerializeField] private AudioClip _uiBuyPress;
    [SerializeField] private AudioClip _tetrisSpawn;
    [SerializeField] private AudioClip _tetrisGoldSpawn;
    [SerializeField] private AudioClip _tetrisBadSpawn;
    [SerializeField] private AudioClip _tetrisLanding;
    [SerializeField] private AudioClip _tetrisJingle;

    private void Start()
    {
        _musicAudioSource.volume = 0.1f;
        _musicAudioSource.clip = _menuMusic;
        _musicAudioSource.Play();
    }

    public void PlayMusic()
    {
        _musicAudioSource.volume = 0.02f;
        _musicAudioSource.clip = _music;
        _musicAudioSource.Play();
    }

    public void PlayButtonPress()
    {
        _uiAudioSource.clip = _uiButtonPress;
        _uiAudioSource.Play();
    }

    public void PlayBuyPress()
    {
        _uiAudioSource.clip = _uiBuyPress;
        _uiAudioSource.Play();
    }

    public void PlayTetrisSpawn()
    {
        _tetrisSpawnAudioSource.clip = _tetrisSpawn;
        _tetrisSpawnAudioSource.Play();
    }

    public void PlayTetrisGoldSpawn()
    {
        _tetrisSpawnAudioSource.clip = _tetrisGoldSpawn;
        _tetrisSpawnAudioSource.Play();
    }

    public void PlayTetrisBadSpawn()
    {
        _tetrisSpawnAudioSource.clip = _tetrisBadSpawn;
        _tetrisSpawnAudioSource.Play();
    }

    public void PlayTetrisLanding()
    {
        _tetrisAudioSource.clip = _tetrisLanding;
        _tetrisAudioSource.Play();
    }

    public void PlayTetrisJingle()
    {
        _tetrisAudioSource.volume = 0.2f;
        _tetrisAudioSource.clip = _tetrisJingle;
        _tetrisAudioSource.Play();
        _tetrisAudioSource.volume = 0.9f;
    }
}
