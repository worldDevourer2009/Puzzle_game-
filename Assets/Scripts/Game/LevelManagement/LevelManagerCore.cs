using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

namespace Core
{
    public interface ILevelManager
    {
        event Action OnPlayerCreated;
        event Action<int> OnLevelLoaded;
        IPlayerFacade PlayerEntity { get; }

        UniTask InitPlayer(Vector3 position);
        UniTask SetEntity(ObjectType id, Vector3 pos = default, Transform parent = null);
        UniTask LoadNextLevel();
        UniTask LoadCurrentLevel();
        UniTask LoadLevelByName(string name);
    }

    public sealed class LevelManagerCore : ILevelManager
    {
        public event Action OnPlayerCreated;
        public event Action<int> OnLevelLoaded;
        public IPlayerFacade PlayerEntity => _playerFacade;

        private readonly ICameraManager _cameraManager;
        private readonly ITriggerSystem _triggerSystem;
        private readonly IFactorySystem _factorySystem;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILogger _logger;

        private readonly AddressablesIdsConfig _addressablesIdsConfig;
        private readonly LevelsConfig _levelsConfig;

        private readonly Dictionary<string, List<IEntity>> _entities;
        private readonly Dictionary<string, List<IInteractable>> _interactables;
        private readonly Dictionary<string, List<IActivatable>> _activatables;

        private LevelData CurrentLevel
        {
            get
            {
                if (_currentLevelIndex >= 0 && _currentLevelIndex < _levelsConfig.LevelData.Count)
                {
                    return _levelsConfig.LevelData[_currentLevelIndex];
                }

                return default;
            }
        }

        private IPlayerFacade _playerFacade;

        private int _currentLevelIndex = -1;

        public LevelManagerCore(IFactorySystem factorySystem,
            ICameraManager cameraManager,
            ILogger logger,
            ISceneLoader sceneLoader,
            ITriggerSystem triggerSystem,
            AddressablesIdsConfig addressablesIdsConfig,
            LevelsConfig levelsConfig)
        {
            _factorySystem = factorySystem;
            _cameraManager = cameraManager;
            _logger = logger;
            _sceneLoader = sceneLoader;
            _triggerSystem = triggerSystem;
            _addressablesIdsConfig = addressablesIdsConfig;
            _levelsConfig = levelsConfig;

            _entities = new Dictionary<string, List<IEntity>>();
            _interactables = new Dictionary<string, List<IInteractable>>();
            _activatables = new Dictionary<string, List<IActivatable>>();
        }

        public async UniTask InitPlayer(Vector3 position)
        {
            await SetEntity(ObjectType.Player, position);

            if (_playerFacade == null)
            {
                _logger.LogWarning("PlayerFacade not found after spawn");
                return;
            }

            var playerEyesTransform = _playerFacade.EyesTransform;

            if (playerEyesTransform == null)
            {
                _logger.LogWarning("Can't find player's eyes transform");
            }

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

            if (obj.TryGetComponent<IEntity>(out var comp))
            {
                RegisterEntity(objectType, comp);
            }

            if (obj.TryGetComponent<IInteractable>(out var interactable))
            {
                RegisterInteractable(interactable);
            }

            if (obj.TryGetComponent<IActivatable>(out var activatable))
            {
                RegisterActivatable(activatable);
            }
        }

        private void RegisterEntity(ObjectType type, IEntity comp)
        {
            if (type == ObjectType.Player)
            {
                if (comp is IPlayerFacade facade)
                {
                    _playerFacade = facade;
                }
                else
                {
                    _logger.LogWarning($"Entity has not player facade");
                }
            }
            else
            {
                var levelName = CurrentLevel.LevelName;

                if (string.IsNullOrWhiteSpace(levelName))
                {
                    _logger.LogWarning("Can't register entity because level name is empty");
                    return;
                }

                if (!_entities.TryGetValue(levelName, out var list))
                {
                    list = new List<IEntity>();
                    _entities[levelName] = list;
                }

                list.Add(comp);
            }
        }

        private void RegisterInteractable(IInteractable comp)
        {
            var levelName = CurrentLevel.LevelName;
            if (string.IsNullOrWhiteSpace(levelName))
            {
                _logger.LogWarning("Can't register interactable");
                return;
            }

            if (!_interactables.TryGetValue(levelName, out var list))
            {
                list = new List<IInteractable>();
                _interactables[levelName] = list;
            }

            list.Add(comp);
        }

        private void RegisterActivatable(IActivatable comp)
        {
            var levelName = CurrentLevel.LevelName;
            if (string.IsNullOrWhiteSpace(levelName))
            {
                _logger.LogWarning("Can't register interactable");
                return;
            }

            if (!_activatables.TryGetValue(levelName, out var list))
            {
                list = new List<IActivatable>();
                _activatables[levelName] = list;
            }

            list.Add(comp);
        }

        public async UniTask LoadNextLevel()
        {
            var nextIndex = _currentLevelIndex + 1;

            if (nextIndex >= _levelsConfig.LevelData.Count)
            {
                _logger.LogWarning("Can't find level");
                return;
            }

            var nextLevel = _levelsConfig.LevelData[nextIndex].LevelName;
            await LoadLevelByName(nextLevel);
        }

        public async UniTask LoadCurrentLevel()
        {
            if (_currentLevelIndex < 0)
            {
                _currentLevelIndex =
                    Mathf.Clamp(_levelsConfig.StartingLevelIndex, 0, _levelsConfig.LevelData.Count - 1);
            }

            var lvlName = _levelsConfig.LevelData[_currentLevelIndex].LevelName;

            if (string.IsNullOrEmpty(lvlName))
            {
                _logger.LogError("Has not name in config");
                return;
            }

            await LoadLevelByName(lvlName);
        }

        public async UniTask LoadLevelByName(string name)
        {
            string previousLevelName = null;

            if (_currentLevelIndex >= 0 && _currentLevelIndex < _levelsConfig.LevelData.Count)
            {
                previousLevelName = _levelsConfig.LevelData[_currentLevelIndex].LevelName;
            }

            var index = _levelsConfig.LevelData.FindIndex(x => x.LevelName == name);

            if (index < 0)
            {
                _logger.LogError($"Has not level with name {name}");
                return;
            }

            _currentLevelIndex = index;
            var lvlData = _levelsConfig.LevelData[index];

            await _sceneLoader.LoadSceneById(lvlData.LevelName);

            CleanupPreviousLevelEntities(previousLevelName);

            await InitLevelManager(lvlData);

            InitTriggers(lvlData);

            OnLevelLoaded?.Invoke(index);
        }

        private async UniTask InitLevelManager(LevelData lvlData)
        {
            await InitPlayer(lvlData.PlayerSpawn);

            foreach (var entityConfig in lvlData.Entities)
            {
                if (TryParseObjectType(entityConfig.EntityId, out var objType))
                {
                    await SetEntity(objType, entityConfig.Position);
                }
                else
                {
                    _logger.LogWarning($"Can't parse entity with id {entityConfig.EntityId}");
                }
            }
        }

        private bool TryParseObjectType(string entityId, out ObjectType result)
        {
            return Enum.TryParse(entityId, true, out result);
        }

        private void InitTriggers(LevelData lvlData)
        {
            _triggerSystem.ClearAllTriggers();

            var ids = lvlData.Triggers.AsValueEnumerable().Select(t => t.TriggerId).ToList();

            _triggerSystem.SetTriggers(ids);

            foreach (var triggerConfig in lvlData.Triggers)
            {
                _triggerSystem.SetTriggerState(triggerConfig.TriggerId, triggerConfig.InitialState).Forget();
            }
        }

        private void CleanupPreviousLevelEntities(string levelNameToCleanup)
        {
            if (string.IsNullOrWhiteSpace(levelNameToCleanup))
            {
                return;
            }

            if (_entities.TryGetValue(levelNameToCleanup, out var entity))
            {
                foreach (var ent in entity)
                {
                    if (ent is Component c)
                    {
                        Object.Destroy(c.gameObject);
                    }
                }
                
                _entities[levelNameToCleanup].Clear();
                _entities.Remove(levelNameToCleanup);
            }

            if (_activatables.TryGetValue(levelNameToCleanup, out var activatables))
            {
                foreach (var activatable in activatables)
                {
                    var component = activatable as Component;

                    if (activatable != null && component != null && component.gameObject != null)
                    {
                        Object.Destroy(component.gameObject);
                    }
                }
                
                _activatables[levelNameToCleanup].Clear();
                _activatables.Remove(levelNameToCleanup);
            }

            if (_interactables.TryGetValue(levelNameToCleanup, out var interactables))
            {
                foreach (var interactable in interactables)
                {
                    var component = interactable as Component;

                    if (interactable != null && component != null && component.gameObject != null)
                    {
                        Object.Destroy(component.gameObject);
                    }
                }
                
                _interactables[levelNameToCleanup].Clear();
                _interactables.Remove(levelNameToCleanup);
            }

            _playerFacade = null;
        }
    }
}