﻿using UnityEngine;
using Items;
using Bolt;

namespace KBA.Debug
{
    [RequireComponent(typeof(Inventory))]
    public class ItemSwitcher : EntityBehaviour, IControllable
    {
        private Item[] _itemsList;
        private Inventory _inventory;
        private int _actualItemIndex = 0;

        // CORE

        private void Awake()
        {
            _itemsList = Resources.Load<ItemListData>("Data/ItemList").Items;
            _inventory = GetComponent<Inventory>();
        }

        // BOLT

        public override void SimulateController()
        {
            MapInputs();
        }

        // PUBLIC

        public void MapInputs()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchToNextItem();
            }
        }

        // PRIVATE

        private void SwitchToNextItem()
        {
            var nextItem = _itemsList[(_actualItemIndex++) % _itemsList.Length];
            if (nextItem.name == "IonBeam")
                _inventory.SetItem(nextItem, 1);
            else
                _inventory.SetItem(nextItem, 50);
        }
    }
}
