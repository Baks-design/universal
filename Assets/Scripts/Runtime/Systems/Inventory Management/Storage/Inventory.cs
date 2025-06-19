using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] InventoryView view;
        [SerializeField] int capacity = 20;
        [SerializeField] List<ItemDetails> startingItems = new();

        public InventoryController Controller { get; private set; }

        void Awake()
        => Controller = new InventoryController.Builder(view)
            .WithStartingItems(startingItems)
            .WithCapacity(capacity)
            .Build();
    }
}