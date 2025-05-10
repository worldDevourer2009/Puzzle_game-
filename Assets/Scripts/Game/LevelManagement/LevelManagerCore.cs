using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface ILevelManager
    {
        event Action OnPlayerCreated; 
        IEntity PlayerEntity { get; }
        
        UniTask CreateEntity(string id, Vector3 pos = default, Transform parent = null);
        UniTask TriggerLevelAction();
        UniTask ResetLevel();
        UniTask LoadNextLevel();
        UniTask LoadLevelByIndex(int index);
    }
    
    public sealed class LevelManagerCore : ILevelManager
    {
        public event Action OnPlayerCreated;
        public IEntity PlayerEntity => _playerEntity;

        private readonly IFactorySystem _factorySystem;
        private readonly IGameLoop _gameLoop;
        private readonly AddressablesIdsConfig _addressablesIdsConfig;
        
        private List<IEntity> _entities;
        private IEntity _playerEntity;

        public LevelManagerCore(IFactorySystem factorySystem, IGameLoop gameLoop, AddressablesIdsConfig addressablesIdsConfig)
        {
            _factorySystem = factorySystem;
            _gameLoop = gameLoop;
            _addressablesIdsConfig = addressablesIdsConfig;
            
            _entities = new List<IEntity>();
            
            Debug.LogWarning("Init level maanger");

            _gameLoop.OnAfterInit += () => InitPlayer().Forget();
        }

        private async UniTask InitPlayer()
        {
            var playerId = _addressablesIdsConfig.GetIdByType(ObjectType.Player);
            await CreateEntity(playerId, new Vector3(10f, 10f));
        }

        public async UniTask CreateEntity(string id, Vector3 pos = default, Transform parent = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Id is null");
                return;
            }

            GameObject obj;
            
            if (pos != default && parent == null)
            {
                obj = await _factorySystem.Create(id, pos);
            }
            else if (parent != null)
            {
                obj = await _factorySystem.Create(id, pos, parent);
            }
            else
            {
                obj = await _factorySystem.Create(id);
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