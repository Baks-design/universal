using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Universal.Runtime.Systems.Persistence.Interfaces;

namespace Universal.Runtime.Systems.EntityPersistence
{
    public class FileDataService : IDataService
    {
        readonly ISerializer serializer;
        readonly string dataPath;
        readonly string fileExtension;

        public FileDataService(ISerializer serializer)
        {
            this.serializer = serializer;
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
        }

        string GetPathToFile(string fileName) => Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));

        public void Save(GameData data, bool overwrite = true)
        {
            var fileLocation = GetPathToFile(data.Name);
            if (!overwrite && File.Exists(fileLocation))
                throw new IOException($"The file '{data.Name}.{fileExtension}' already exists and cannot be overwritten.");
            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            var fileLocation = GetPathToFile(name);
            if (!File.Exists(fileLocation))
                throw new ArgumentException($"No persisted GameData with name '{name}'");
            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            var fileLocation = GetPathToFile(name);
            if (File.Exists(fileLocation))
                File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            var array = Directory.GetFiles(dataPath);
            for (var i = 0; i < array.Length; i++)
                File.Delete(array[i]);
        }

        public IEnumerable<string> ListSaves()
        => from path in Directory.EnumerateFiles(dataPath)
           where Path.GetExtension(path) == fileExtension
           select Path.GetFileNameWithoutExtension(path);
    }
}