using System;
using R3;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ui
{
    public class LoadingModel : IDisposable
    {
        //TODO Убрать модель
        public readonly ReactiveProperty<bool> OnLoading;
        
        private readonly CompositeDisposable _compositeDisposable;
        private readonly ISceneLoader _sceneLoader;
        
        private readonly Action _onLoad;
        private readonly Action _onLoaded;

        private Camera _camera;

        public LoadingModel(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            OnLoading = new ReactiveProperty<bool>();
            _compositeDisposable = new CompositeDisposable();

            _compositeDisposable.Add(OnLoading);

            _onLoad = async () => await HandleLoad();
            _onLoaded = async () => await HandleLoaded();

            _sceneLoader.OnLoad += _onLoad;
            _sceneLoader.OnLoaded += _onLoaded;
        }

        private UniTask HandleLoaded()
        {
            OnLoading.Value = false;
            return UniTask.CompletedTask;
        }

        private UniTask HandleLoad()
        {
            OnLoading.Value = true;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _sceneLoader.OnLoad -= _onLoad;
            _sceneLoader.OnLoaded -= _onLoaded;
            _compositeDisposable.Dispose();
        }
    }
}