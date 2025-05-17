using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace Core
{
    public interface ILevelManager
    {
        event Action OnPlayerCreated;
        IPlayerFacade PlayerEntity { get; }

        UniTask InitPlayer();
        UniTask SetEntity(ObjectType id, Vector3 pos = default, Transform parent = null);
        UniTask TriggerLevelAction();
        UniTask ResetLevel();
        UniTask LoadNextLevel();
        UniTask LoadLevelByIndex(int index);
    }

    public sealed class LevelManagerCore : ILevelManager
    {
        public event Action OnPlayerCreated;
        public IPlayerFacade PlayerEntity => _playerFacade;

        private readonly ICameraManager _cameraManager;
        private readonly IFactorySystem _factorySystem;
        private readonly ILogger _logger;
        private readonly AddressablesIdsConfig _addressablesIdsConfig;

        private readonly Dictionary<string, List<IEntity>> _entities;
        private IPlayerFacade _playerFacade;

        public LevelManagerCore(IFactorySystem factorySystem,
            ICameraManager cameraManager,
            ILogger logger,
            AddressablesIdsConfig addressablesIdsConfig)
        {
            _factorySystem = factorySystem;
            _cameraManager = cameraManager;
            _logger = logger;
            _addressablesIdsConfig = addressablesIdsConfig;

            _entities = new Dictionary<string, List<IEntity>>();
        }

        public async UniTask InitPlayer()
        {
            await SetEntity(ObjectType.Player, new Vector3(10f, 10f));

            if (_playerFacade == null)
            {
                _logger.LogWarning("PlayerFacade not found after spawn");
                return;
            }

            var playerEyesTransform = _playerFacade.EyesTransform;

            await _cameraManager.SetActiveCamera(CustomCameraType.PlayerCamera, playerEyesTransform);

            OnPlayerCreated?.Invoke();
        }

        public async UniTask SetEntity(ObjectType objectType, Vector3 pos = default, Transform parent = null)
        {
            var id = _addressablesIdsConfig.GetIdByType(objectType);
            
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("id is null or empty");
                return;
            }

            GameObject obj;

            if (parent != null)
            {
                obj = await _factorySystem.Create(id, pos, parent);
            }
            else if (pos != Vector3.zero)
            {
                obj = await _factorySystem.Create(id, pos);
            }
            else
            {
                obj = await _factorySystem.Create(id);
            }

            if (!obj.TryGetComponent<IEntity>(out var comp))
            {
                _logger.LogWarning($"spawned object with id {id} does not implement IEntity");
                return;
            }

            RegisterEntity(objectType, comp);
        }

        private void RegisterEntity(ObjectType type, IEntity comp)
        {
            var playerId = _addressablesIdsConfig.GetIdByType(type);
            var id = _addressablesIdsConfig.GetIdByType(type);

            if (!id.Equals(playerId, StringComparison.OrdinalIgnoreCase))
            {
                if (!_entities.TryGetValue(id, out var list))
                {
                    list = new List<IEntity>();
                    _entities[id] = list;
                }

                list.Add(comp);
            }
            else
            {
                if (comp is IPlayerFacade facade)
                {
                    _playerFacade = facade;
                }
                else
                {
                    _logger.LogWarning($"entity {id} is not an IPlayerFacade");
                }
            }
        }

        public async UniTask TriggerLevelAction()
        {
            //throw new NotImplementedException();
        }

        public async UniTask ResetLevel()
        {
            //throw new NotImplementedException();
        }

        public async UniTask LoadNextLevel()
        {
            //throw new NotImplementedException();
        }

        public async UniTask LoadLevelByIndex(int index)
        {
            //throw new NotImplementedException();
        }
    }
}