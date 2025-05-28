using System;
using Core;
using Cysharp.Threading.Tasks;
using R3;

namespace Ui
{
    public class SettingsPresenter : IUIPresenter, IDisposable
    {
        private const string SettingsId = "SettingsView";
        
        private readonly CompositeDisposable _compositeDisposable;
        
        private readonly IFactorySystem _factorySystem;
        private readonly SettingsModel _settingsModel;
        private readonly IUISystem _uiSystem;
        
        private ISettingsView _settingsView;

        public SettingsPresenter(SettingsModel settingsModel, IFactorySystem factorySystem, IUISystem uiSystem)
        {
            _settingsModel = settingsModel;
            _factorySystem = factorySystem;
            _uiSystem = uiSystem;
            _compositeDisposable = new CompositeDisposable();
        }

        public async UniTask Initialize()
        {
            _settingsView = await _factorySystem.CreateFromInterface<ISettingsView>(SettingsId);
            _uiSystem.RegisterView(SettingsId, _settingsView);
            _settingsView.Hide();
            await _uiSystem.ParentUnderCanvas(_settingsView, CanvasType.Windows);
            
            _settingsView.MasterVolume.Value = _settingsModel.GetCurrentMasterVolume();
            _settingsView.MusicVolume.Value = _settingsModel.GetCurrentMusicVolume();
            _settingsView.SFXVolume.Value = _settingsModel.GetCurrentSFXVolume();
            _settingsView.UIVolume.Value = _settingsModel.GetCurrentUIVolume();

            _settingsView.MasterVolume.Subscribe(value => _settingsModel.SetMasterVolume(value))
                .AddTo(_compositeDisposable);

            _settingsView.MusicVolume.Subscribe(value => _settingsModel.SetMusicVolume(value))
                .AddTo(_compositeDisposable);

            _settingsView.SFXVolume.Subscribe(value => _settingsModel.SetSFXVolume(value))
                .AddTo(_compositeDisposable);

            _settingsView.UIVolume.Subscribe(value => _settingsModel.SetUIVolume(value))
                .AddTo(_compositeDisposable);

            _settingsView.OnClose += DisplayView;
        }

        private void DisplayView()
        {
            _settingsView.Hide();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}