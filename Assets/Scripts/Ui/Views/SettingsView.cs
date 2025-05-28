using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public interface ISettingsView : IUIView
    {
        event Action OnClose;
        ReactiveProperty<float> MasterVolume { get; }
        ReactiveProperty<float> SFXVolume { get; }
        ReactiveProperty<float> MusicVolume { get; }
        ReactiveProperty<float> UIVolume { get; }
    }

    public class SettingsView : MonoBehaviour, ISettingsView
    {
        public bool IsVisible => gameObject.activeInHierarchy;
        public event Action OnClose;
        public ReactiveProperty<float> MasterVolume => _masterVolume;
        public ReactiveProperty<float> SFXVolume => _sfxVolume;
        public ReactiveProperty<float> MusicVolume => _musicVolume;
        public ReactiveProperty<float> UIVolume => _uiVolume;

        private readonly ReactiveProperty<float> _masterVolume = new();
        private readonly ReactiveProperty<float> _sfxVolume = new();
        private readonly ReactiveProperty<float> _musicVolume = new();
        private readonly ReactiveProperty<float> _uiVolume = new();
        
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _uiSlider;
        
        [SerializeField] private Button _closeButton;

        public void Start()
        {
            _masterSlider.value = MasterVolume.Value;
            _sfxSlider.value = SFXVolume.Value;
            _musicSlider.value = MusicVolume.Value;
            _uiSlider.value = UIVolume.Value;
            
            _masterSlider.onValueChanged.AddListener((x) => _masterVolume.Value = x);
            _sfxSlider.onValueChanged.AddListener((x) => _sfxVolume.Value = x);
            _musicSlider.onValueChanged.AddListener((x) => _musicVolume.Value = x);
            _uiSlider.onValueChanged.AddListener((x) => _uiVolume.Value = x);
            
            _closeButton.onClick.AddListener(() => OnClose?.Invoke());
        }

        public void OnDestroy()
        {
            _masterSlider.onValueChanged.RemoveAllListeners();
            _sfxSlider.onValueChanged.RemoveAllListeners();
            _musicSlider.onValueChanged.RemoveAllListeners();
            _uiSlider.onValueChanged.RemoveAllListeners();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Parent(Transform parent)
        {
            transform.SetParent(parent, false);
        }
    }
}