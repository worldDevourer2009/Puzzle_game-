using System;
using Cysharp.Threading.Tasks;
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
        ReactiveProperty<float> Sensitivity { get; }
        void BindSliders();
        void SetMaxSensitivity(float value);
        void SetMinSensitivity(float value);
        void SetCurrentSensitivity(float value);
    }

    public class SettingsView : MonoBehaviour, ISettingsView
    {
        public bool IsVisible => gameObject.activeInHierarchy;
        public event Action OnClose;
        public ReactiveProperty<float> MasterVolume => _masterVolume;
        public ReactiveProperty<float> SFXVolume => _sfxVolume;
        public ReactiveProperty<float> MusicVolume => _musicVolume;
        public ReactiveProperty<float> UIVolume => _uiVolume;
        public ReactiveProperty<float> Sensitivity => _sensitivity;

        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<float> _masterVolume = new();
        private readonly ReactiveProperty<float> _sfxVolume = new();
        private readonly ReactiveProperty<float> _musicVolume = new();
        private readonly ReactiveProperty<float> _uiVolume = new();
        private readonly ReactiveProperty<float> _sensitivity = new();

        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _uiSlider;
        [SerializeField] private Slider _sensitivitySlider;

        [SerializeField] private Button _closeButton;

        public void BindSliders()
        {
            BindSlider(_masterVolume, _masterSlider);
            BindSlider(_sfxVolume, _sfxSlider);
            BindSlider(_musicVolume, _musicSlider);
            BindSlider(_uiVolume, _uiSlider);
            
            _closeButton.onClick
                .AsObservable()
                .Subscribe(_ => OnClose?.Invoke())
                .AddTo(_compositeDisposable);
            
            _sensitivitySlider.onValueChanged
                .AddListener(x => _sensitivity.Value = x);
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

        public void SetMaxSensitivity(float value)
        {
            _sensitivitySlider.maxValue = value;
        }

        public void SetMinSensitivity(float value)
        {
            _sensitivitySlider.minValue = value;
        }

        public void SetCurrentSensitivity(float value)
        {
            _sensitivitySlider.value = value;
        }

        public void OnDestroy()
        {
            _sensitivitySlider.onValueChanged.RemoveAllListeners();  
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