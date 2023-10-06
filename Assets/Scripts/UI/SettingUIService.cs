using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIService : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _settingWindow;
    private AudioService _audioService;

    private void Start()
    {
        _audioService = ServiceLocator.Current.Get<AudioService>();
        _backButton.onClick.AddListener(() =>
        {
            _settingWindow.SetActive(false);
            _audioService.PlayButtonPress();
        });

        _musicSlider.onValueChanged.AddListener(_audioService.SetMusicVolume);
        _soundSlider.onValueChanged.AddListener(_audioService.SetSoundVolume);
    }



}
