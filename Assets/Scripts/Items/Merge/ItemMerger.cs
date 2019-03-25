﻿using UnityEngine;
using Common.PhysicsUtils;
using Bolt;

namespace Items.Merge
{
    public class ItemMerger : EntityBehaviour<IKartState>, IControllable
    {
        [SerializeField] private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [Header("Settings")]
        [Tooltip("Seconds with button pressed before merging the item")]
        [SerializeField] private FloatVariable _secondsBeforeMerging;

        [Header("Item Source")]
        [SerializeField] private Inventory _inventory;
        [SerializeField] private Lottery.Lottery _lottery;

        [Header("Bonuses References")]
        [SerializeField] private Boost _boost;
        [SerializeField] private BoostSettings _boostSettings;
        [SerializeField] private Health.Health _health;

        private float _timer = 0f;
        private bool _canMerge = true;

        private enum MergeMode { Full, Small };

        // CORE

        private void Update()
        {
            if(entity.isAttached && entity.isOwner)
            {
                MapInputs();
            }
        }

        // PUBLIC

        public void MapInputs()
        {
            if (Enabled)
            {
                if (Input.GetButton(Constants.Input.MergeItem))
                {
                    _timer += Time.deltaTime;

                    if (_timer > _secondsBeforeMerging.Value && _canMerge)
                    {
                        ConsumeItem();
                        _timer = 0f;
                        _canMerge = false;
                    }
                }
                if (Input.GetButtonUp(Constants.Input.MergeItem))
                {
                    _timer = 0f;
                    _canMerge = true;
                }
            }
        }

        // PRIVATE

        private void ConsumeItem()
        {
            if (_inventory.CurrentItem != null)
            {
                //var mergeMode = _inventory.CurrentItemCount == _inventory.CurrentItem.Count ? MergeMode.Full : MergeMode.Small;
                _inventory.SetCount(_inventory.CurrentItemCount - 1);
                GrantBoosts(MergeMode.Full);
            }
            else if (_lottery.LotteryStarted)
            {
                _lottery.StopAllCoroutines();
                _lottery.ResetLottery();
                GrantBoosts(MergeMode.Small);
            }
        }

        private void GrantBoosts(MergeMode mode)
        {
            ItemMerging itemMergingEvent = ItemMerging.Create();
            itemMergingEvent.Entity = entity;
            switch (mode)
            {
                case MergeMode.Full:
                    itemMergingEvent.Full = true;
                    break;
                case MergeMode.Small:
                    itemMergingEvent.Full = false;
                    _boost.CustomBoostFromBoostSettings(_boostSettings);
                    break;
            }
            itemMergingEvent.Send();
        }
    }
}
