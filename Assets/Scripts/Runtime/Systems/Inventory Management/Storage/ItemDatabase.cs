using System.Collections.Generic;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public static class ItemDatabase
    {
        static Dictionary<SerializableGuid, ItemDetails> itemDetailsDictionary;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize()
        {
            itemDetailsDictionary = new Dictionary<SerializableGuid, ItemDetails>();

            var itemDetails = Resources.LoadAll<ItemDetails>("");
            for (var i = 0; i < itemDetails.Length; i++)
            {
                var item = itemDetails[i];
                itemDetailsDictionary.Add(item.Id, item);
            }
        }

        public static ItemDetails GetDetailsById(SerializableGuid id)
        {
            try
            {
                return itemDetailsDictionary[id];
            }
            catch
            {
                Debug.LogError($"Cannot find item details with id {id}");
                return null;
            }
        }
    }
}