using System.Collections.Generic;
using Universal.Runtime.Systems.Persistence.Data;

namespace Universal.Runtime.Systems.Persistence.Interfaces
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}