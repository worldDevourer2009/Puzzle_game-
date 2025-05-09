using UnityEngine;

namespace Core
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string json);
    }

    public class JsonSerializer : IJsonSerializer
    {
        public JsonSerializer()
        {
        }

        public string Serialize<T>(T data)
        {
            var serializedData = JsonUtility.ToJson(data, true);
            return serializedData;
        }

        public T Deserialize<T>(string json)
        {
            var type = typeof(T);
            var deserializedData = JsonUtility.FromJson(json, type);
            return (T)deserializedData;
        }
    }
}