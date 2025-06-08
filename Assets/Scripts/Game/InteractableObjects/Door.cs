using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Logger = Core.Logger;

namespace Game
{
    public class Door : InteractableLogic
    {
        public override InteractableType Type => InteractableType.Unusable;

        [SerializeField] private float _openAngle;
        [SerializeField] private float _closeAngle = 0f;
        [SerializeField] private float _openDuration;
        [SerializeField] private float _closeDuration;
        [SerializeField] private AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isOpen = false;
        private bool _isAnimating = false;

        public override async void Interact()
        {
            if (_isAnimating)
            {
                return;
            }

            if (_isOpen)
            {
                await CloseDoor();
            }
            else
            {
                await OpenDoor();
            }
        }

        public override void StopInteraction()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async UniTask OpenDoor()
        {
            if (_isOpen || _isAnimating)
            {
                Logger.Instance.LogWarning("Can't open door");
                return;
            }

            _isAnimating = true;
            await RotateDoor(_closeAngle, _openAngle, _openDuration);
            _isOpen = true;
            _isAnimating = false;
        }

        private async UniTask CloseDoor()
        {
            _isAnimating = true;
            await RotateDoor(_openAngle, _closeAngle, _closeDuration);
            _isAnimating = false;
            _isOpen = false;
        }

        private async UniTask RotateDoor(float fromAngle, float toAngle, float duration)
        {
            var elapsedTime = 0f;
            var startRotation = transform.localEulerAngles;
            var targetRotation = new Vector3(startRotation.x, fromAngle, startRotation.z);
            var endRotation = new Vector3(startRotation.x, toAngle, startRotation.z);

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    var progress = elapsedTime / duration;
                    var curveValue = _rotationCurve.Evaluate(progress);

                    var currentRotation = Vector3.Lerp(targetRotation, endRotation, curveValue);
                    transform.localEulerAngles = currentRotation;

                    await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                }

                transform.localEulerAngles = endRotation;
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource?.Cancel();
        }
    }
}