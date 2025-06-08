using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Logger = Core.Logger;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableItem : InteractableLogic
    {
        [Header("Scales")] 
        [SerializeField] private Vector3 _resizedScale = new(0.3f, 0.3f, 0.3f);

        [Header("Speeds")] 
        [SerializeField] private float _speedToHand;

        [Header("Durations")] 
        [SerializeField] private float _moveToHandDuration;
        [SerializeField] private float _scaleDuration;

        private ILevelManager _levelManager;
        private IPlayerDataHolder _playerDataHolder;
        private ICameraManager _cameraManager;

        private Rigidbody _rigidbody;
        private Vector3 _defaultScale;

        private Transform _defaultParent;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _interacted;

        [Inject]
        public void Construct(ILevelManager levelManager, IPlayerDataHolder playerDataHolder,
            ICameraManager cameraManager)
        {
            _levelManager = levelManager;
            _playerDataHolder = playerDataHolder;
            _cameraManager = cameraManager;

            _defaultScale = transform.localScale;
            _defaultParent = transform.parent;
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override async void Interact()
        {
            if (_interacted)
            {
                return;
            }

            var player = _levelManager.PlayerEntity;

            if (player == null)
            {
                Logger.Instance.LogWarning("Player is null, can't interact");
                return;
            }

            if (player.RightHandTransform == null)
            {
                Logger.Instance.LogWarning("Player hand is null, can't interact");
                return;
            }

            _rigidbody.isKinematic = true;

            var list = new List<UniTask>
            {
                SetPosition(transform.position, player.RightHandTransform.position),
                SetScale(_resizedScale)
            };

            await UniTask.WhenAll(list);

            transform.SetParent(player.RightHandTransform);

            _interacted = true;
        }

        private async UniTask SetPosition(Vector3 startPosition, Vector3 endPosition)
        {
            var elapsedTime = 0f;

            _cancellationTokenSource = new CancellationTokenSource();

            while (elapsedTime < _moveToHandDuration)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / _moveToHandDuration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }

            transform.position = endPosition;
        }

        private async UniTask SetScale(Vector3 scale)
        {
            var elapsedTime = 0f;
            var startScale = _defaultScale;

            _cancellationTokenSource = new CancellationTokenSource();

            while (elapsedTime < _scaleDuration)
            {
                transform.localScale = Vector3.Lerp(startScale, scale, elapsedTime / _scaleDuration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }

            transform.localScale = scale;
        }

        public override void StopInteraction()
        {
            var throwProperty = _playerDataHolder.PlayerThrowForce;
            var playerCamera = _cameraManager.GetPlayerCamera();
            _rigidbody.isKinematic = false;

            if (throwProperty != null)
            {
                var throwVector = playerCamera.transform.forward.normalized * throwProperty.Value;
                transform.localScale = _defaultScale;
                _rigidbody.AddForce(throwVector, ForceMode.Impulse);
            }

            transform.SetParent(_defaultParent);

            _interacted = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource?.Cancel();
        }
    }
}