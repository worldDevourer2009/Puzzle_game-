using System;
using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace Core
{
    [CreateAssetMenu(fileName = "Addressables Ids Config", menuName = "Configs/Addressables Ids", order = 4)]
    public class AddressablesIdsConfig : ScriptableObject
    {
        [SerializeField] private List<AddressableId> AddressableIds;

        public string GetIdByType(ObjectType type)
        {
            return AddressableIds.AsValueEnumerable().FirstOrDefault(x => x.Type == type && !string.IsNullOrEmpty(x.Id)).Id;
        }
        
        public ObjectCategory GetCategoryByType(ObjectType type)
        {
            return AddressableIds.AsValueEnumerable().FirstOrDefault(x => x.Type == type && !string.IsNullOrEmpty(x.Id)).Category;
        }
    }

    [Serializable]
    public struct AddressableId
    {
        public ObjectType Type;
        public ObjectCategory Category;
        public string Id;
    }

    public enum ObjectCategory
    {
        Interactable,
        Player,
        Activatable
    }

    public enum ObjectType
    {
        Player,
        Cube,
        NextLevel,
        ThrowableCube,
        PressableButton,
    }
}