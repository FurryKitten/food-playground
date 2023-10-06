using UnityEngine;

public class AudioService : MonoBehaviour, IService
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _tetrisAudioSource;
    [SerializeField] private AudioSource _tetrisSpawnAudioSource;
    [SerializeField] private AudioSource _spoilerAudioSource;

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
    [SerializeField] private AudioClip _questFailJingle;
    [SerializeField] private AudioClip[] _breakDish;
    [SerializeField] private AudioClip[] _spoilerSpawn;
    [SerializeField] private AudioClip[] _foodSpoile;
    [SerializeField] private AudioClip[] _spoilerDie;

    private void Start()
    {
        _musicAudioSource.volume = 0.3f;
        _musicAudioSource.clip = _menuMusic;
        _musicAudioSource.Play();
    }

    public void PlayMusic()
    {
        _musicAudioSource.volume = 0.3f;
        _musicAudioSource.clip = _music;
        _musicAudioSource.Play();
    }

    public void StopMusic()
    {
        _musicAudioSource.Stop();
    }

    public void PlayMenuMusic()
    {
       _musicAudioSource.clip = _menuMusic;
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
        _tetrisAudioSource.clip = _tetrisJingle;
        _tetrisAudioSource.Play();
    }

    public void PlayFail()
    {
        _tetrisAudioSource.clip = _questFailJingle;
        _tetrisAudioSource.Play();
    }

    public void PlayBreakDish()
    {
        _tetrisSpawnAudioSource.PlayOneShot(_breakDish[Random.Range(0, _breakDish.Length)]);
    }

    public void PlaySpawnSpoiler()
    {
        _spoilerAudioSource.clip = _spoilerSpawn[Random.Range(0, _spoilerSpawn.Length)];
        _spoilerAudioSource.Play();
    }

    public void PlaySpoileFood()
    {
        _spoilerAudioSource.clip = _foodSpoile[Random.Range(0, _foodSpoile.Length)];
        _spoilerAudioSource.Play();
    }

    public void PlaySpoilerDie()
    {
        _spoilerAudioSource.PlayOneShot(_spoilerDie[Random.Range(0, _spoilerDie.Length)]);
    }

    public void PlaySelect()
    {
        _uiAudioSource.clip = _tetrisBadSpawn;
        _uiAudioSource.Play();
    }
}
