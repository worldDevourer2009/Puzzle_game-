using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core
{
    public interface IContextResolver
    {
        DiContainer ResolveFor(GameObject obj);
        DiContainer ResolveFor<T>(T obj);
        void Register(Scene scene, DiContainer container);
    }
    
    public class ContextResolver : IContextResolver
    {
        private readonly ILogger _logger;
        private readonly Dictionary<Scene, DiContainer> _diContainers;

        public ContextResolver(ILogger logger)
        {
            _logger = logger;
            _diContainers = new Dictionary<Scene, DiContainer>();
        }

        public DiContainer ResolveFor(GameObject obj)
        {
            if (obj == null)
            {
                _logger.LogError("Trying to get container for null obj");
                return GetFallBack();
            }

            var objScene = obj.scene;
            
            if (_diContainers.TryGetValue(objScene, out var container))
            {
                return container;
            }
            
            var sceneContexts = Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None);
            
            foreach (var context in sceneContexts)
            {
                if (context.Container != null && objScene == context.gameObject.scene)
                {
                    _diContainers[objScene] = context.Container;
                    return context.Container;
                }
            }
            
            if (ProjectContext.HasInstance)
            {
                _logger.LogWarning($"No scene contexte found for scene {objScene.name}, returning project context");
                return ProjectContext.Instance.Container;
            }
            
            return GetFallBack();
        }
        
        public void Register(Scene scene, DiContainer container)
        {
            _diContainers[scene] = container;
        }

        private DiContainer GetFallBack()
        {
            _logger.LogWarning("Can't resolve DiContainer, back to ProjectContext, or obj is null");
            return ProjectContext.Instance.Container;
        }

        public DiContainer ResolveFor<T>(T obj)
        {
            if (ProjectContext.HasInstance)
            {
                return ProjectContext.Instance.Container;
            }
            
            return GetFallBack();
        }
    }
}