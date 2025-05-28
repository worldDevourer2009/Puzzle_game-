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

        private readonly CompositeDisposable _compositeDisposable = new();

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
            BindSlider(_masterVolume, _masterSlider);
            BindSlider(_sfxVolume, _sfxSlider);
            BindSlider(_musicVolume, _musicSlider);
            BindSlider(_uiVolume, _uiSlider);

            _closeButton.onClick.AsObservable()
                .Subscribe(_ => OnClose?.Invoke())
                .AddTo(_compositeDisposable);
        }
        
        private void BindSlider(ReactiveProperty<float> property, Slider slider)
        {
            property
                .Subscribe(value => slider.value = value)
                .AddTo(_compositeDisposable);

            slider.onValueChanged
                .AsObservable()
                .Subscribe(value => property.Value = value)
                .AddTo(_compositeDisposable);
        }

        public void OnDestroy()
        {
            _compositeDisposable.Dispose();
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