using UnityEngine;
using Zenject;

namespace Core
{
    public interface IContextResolver
    {
        DiContainer ResolveFor(GameObject obj);
        DiContainer ResolveFor<T>(T obj);
    }
    
    public class ContextResolver : IContextResolver
    {
        private readonly ILogger _logger;

        public ContextResolver(ILogger logger)
        {
            _logger = logger;
        }

        public DiContainer ResolveFor(GameObject obj)
        {
            var sceneContexts = Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None);
            
            foreach (var context in sceneContexts)
            {
                if (context.Container != null && context.gameObject.scene == obj.scene)
                    return context.Container;
            }
            
            if (ProjectContext.HasInstance)
            {
                return ProjectContext.Instance.Container;
            }
            
            _logger.LogWarning("ContextResolver: Could not resolve DiContainer, falling back to ProjectContext");
            return ProjectContext.Instance.Container;
        }

        public DiContainer ResolveFor<T>(T obj)
        {
            if (ProjectContext.HasInstance)
            {
                return ProjectContext.Instance.Container;
            }
            
            _logger.LogWarning("ContextResolver: Could not resolve DiContainer, falling back to ProjectContext");
            return ProjectContext.Instance.Container;
        }
    }
}