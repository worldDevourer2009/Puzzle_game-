using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Logger = Core.Logger;

namespace Game
{
    public class PressableButton : ActivatableLogic
    {
        public override ActivationState State => _state;
        
        [SerializeField] private float _offsetY;
        [SerializeField] private float _goDownDuration;
        [SerializeField] private float _goUpDuration;
        
        private ActivationState _state;
        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _defaultUnpressedPosition;
        private bool _isPressed;
        private bool _isReleasingPressing;
        
        // [Inject]
        // public void Construct()
        // {
        //     _defaultUnpressedPosition = transform.localPosition;
        //     _state = ActivationState.Inactive;
        // }

        private void Start()
        {
            _defaultUnpressedPosition = transform.position;
            _state = ActivationState.Inactive;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.TryGetComponent(out IInteractable interactable))
            {
                Activate();
            }
            else
            {
                Logger.Instance.Log("Can't activate, object is not interactable");
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if(other.gameObject.TryGetComponent(out IInteractable interactable))
            {
                Deactivate();
            }
            else
            {
                Logger.Instance.Log("Can't deactivate, object is not interactable");
            }
        }

        public override async void Activate()
        {
            try
            {
                await Press();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogWarning($"Can't press, exception is {ex.Message}");
            }
        }

        public override async void Deactivate()
        {
            try
            {
                await Release();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogWarning($"Can't release, exception is {ex.Message}");
            }
        }

        private async UniTask Press()
        {
            if (_isPressed)
            {
                return;
            }
            
            var elapsedTime = 0f;
            var targetPos = _defaultUnpressedPosition + Vector3.down * _offsetY;
            _isPressed = true;
            _isReleasingPressing = true;
            
            while (elapsedTime < _goDownDuration)
            {
                elapsedTime += Time.deltaTime;
                var time= Mathf.Clamp01(elapsedTime / _goDownDuration);
                transform.position = Vector3.Lerp(_defaultUnpressedPosition, targetPos, time);
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }

            _isReleasingPressing = false;
            transform.position = targetPos;
            _state = ActivationState.Active;
        }

        private async UniTask Release()
        {
            if (_isReleasingPressing)
            {
                return;
            }
            
            var elapsedTime = 0f;
            var startPos = transform.position;
            var targetPos = _defaultUnpressedPosition;
            
            while (elapsedTime < _goUpDuration)
            {
                elapsedTime += Time.deltaTime;
                var time= Mathf.Clamp01(elapsedTime / _goDownDuration);
                transform.position = Vector3.Lerp(startPos, targetPos, time);
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }

            _isPressed = false;
            transform.position = _defaultUnpressedPosition;
            _state = ActivationState.Inactive;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}