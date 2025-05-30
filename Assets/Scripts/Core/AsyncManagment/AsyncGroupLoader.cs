using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZLinq;

namespace Core
{
    public enum AsyncGroupType
    {
        Sequential,
        Parallel,
        Hybrid
    }

    public class AsyncGroup
    {
        public AsyncGroupType Type { get; }
        private readonly List<Func<UniTask>> _sequential = new();
        private readonly List<Func<UniTask>> _parallel = new();

        public AsyncGroup(AsyncGroupType type) => Type = type;

        public void Add(Func<UniTask> task, bool isSequential)
        {
            if (isSequential)
            {
                _sequential.Add(task);
            }
            else
            {
                _parallel.Add(task);
            }
        }

        public IReadOnlyList<Func<UniTask>> Sequential()
        {
            return _sequential;
        }

        public IReadOnlyList<Func<UniTask>> Parallel()
        {
            return _parallel;
        }
    }

    public interface IAsyncGroupLoader
    {
        void CreateGroup(AsyncGroupType groupType, string groupId, bool isPersistent = false);
        void AddToGroup(string groupId, Func<UniTask> task, bool isSequent = true);

        UniTask RunGroup(string groupId, Action<float> onProgress = null,
            CancellationToken cancellationToken = default);

        void RemoveGroup(string groupId);
    }

    public class AsyncGroupLoader : IAsyncGroupLoader
    {
        private readonly ILogger _logger;

        private readonly Dictionary<string, AsyncGroup> _groups;
        private readonly HashSet<string> _persistenceList;

        public AsyncGroupLoader(ILogger logger)
        {
            _logger = logger;
            _groups = new Dictionary<string, AsyncGroup>();
            _persistenceList = new HashSet<string>();
        }

        public void CreateGroup(AsyncGroupType groupType, string groupId, bool isPersistent = false)
        {
            if (_groups.TryGetValue(groupId, out var group))
            {
                _logger.LogWarning($"Group {groupId} already exists");
                return;
            }

            var asyncGroup = new AsyncGroup(groupType);

            _groups.Add(groupId, asyncGroup);

            if (isPersistent)
            {
                _persistenceList.Add(groupId);
            }
        }

        public void AddToGroup(string groupId, Func<UniTask> task, bool isSequent = true)
        {
            if (!_groups.TryGetValue(groupId, out var asyncGroup))
            {
                return;
            }

            asyncGroup.Add(task, isSequent);
        }

        public async UniTask RunGroup(string groupId, Action<float> onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (!_groups.TryGetValue(groupId, out var asyncGroup))
            {
                _logger.LogWarning($"Group {groupId} not found");
                return;
            }

            switch (asyncGroup.Type)
            {
                case AsyncGroupType.Sequential:
                    await RunSequentially(asyncGroup, onProgress, cancellationToken);
                    break;
                case AsyncGroupType.Parallel:
                    await RunParallel(asyncGroup, onProgress, cancellationToken);
                    break;
                case AsyncGroupType.Hybrid:
                    await RunHybridInterleaved(asyncGroup, onProgress, cancellationToken);
                    break;
                default:
                    _logger.LogWarning($"Group type with id {groupId} was not defined");
                    return;
            }

            CheckIfPersistent(groupId);
        }

        private async UniTask RunSequentially(AsyncGroup group, Action<float> onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var tasksSnapshot = group.Sequential().AsValueEnumerable().ToList();
            var total = tasksSnapshot.Count;
            var completed = 0;

            foreach (var task in tasksSnapshot)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await task();
                }
                catch (OperationCanceledException)
                {
                    _logger.Log("RunSequentially was canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    var methodName = task.Method.Name;
                    var targetType = task.Target?.GetType().Name ?? "<static>";
                    
                    _logger.LogWarning(
                        $"Running sequentially func failed: " +
                        $"{methodName} on {targetType} threw exception: {ex.GetType().Name}: {ex.Message}"
                    );
                }
                finally
                {
                    var progress = (float)Interlocked.Increment(ref completed) / total;
                    onProgress?.Invoke(progress);
                }
            }
        }

        private async UniTask RunParallel(AsyncGroup group, Action<float> onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = group.Parallel();
            var total = tasks.Count;
            var completed = 0;
            var tasksList = new List<UniTask>();

            foreach (var task in tasks)
            {
                var wrapped = UniTask.Create(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Parallel task failed {task.Method} with exception {ex.Message}");
                    }
                    finally
                    {
                        var progress = (float)Interlocked.Increment(ref completed) / total;
                        onProgress?.Invoke(progress);
                    }
                });

                tasksList.Add(wrapped);
            }

            await UniTask.WhenAll(tasksList);
        }

        private async UniTask RunHybridInterleaved(AsyncGroup group, Action<float> onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var seqTasks = group.Sequential();
            var parTasks = group.Parallel();

            var total = seqTasks.Count + parTasks.Count;
            var completed = 0;
            var runningPar = new List<UniTask>();
            var maxCount = Math.Max(seqTasks.Count, parTasks.Count);

            for (var i = 0; i < maxCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (i < seqTasks.Count)
                {
                    try
                    {
                        await seqTasks[i]();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Hybrid sequential task #{i} failed with exception {ex.Message}");
                    }
                    finally
                    {
                        var progress = (float)Interlocked.Increment(ref completed) / total;
                        onProgress?.Invoke(progress);
                    }
                }

                if (i < parTasks.Count)
                {
                    var index = i;
                    var wrapper = UniTask.Create(async () =>
                    {
                        try
                        {
                            await parTasks[index]();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Hybrid parallel task #{index} failed: {ex.Message}");
                        }
                        finally
                        {
                            var progress = (float)Interlocked.Increment(ref completed) / total;
                            onProgress?.Invoke(progress);
                        }
                    });
                    runningPar.Add(wrapper);
                }
            }

            if (runningPar.Count > 0)
                await UniTask.WhenAll(runningPar).AttachExternalCancellation(cancellationToken);
        }

        private void CheckIfPersistent(string groupId)
        {
            if (!_persistenceList.Contains(groupId))
            {
                _groups.Remove(groupId);
            }
        }

        public void RemoveGroup(string groupId)
        {
            _groups.Remove(groupId);
            _persistenceList.Remove(groupId);
        }
    }
}