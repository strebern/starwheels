﻿using UnityEngine;
using Multiplayer;

namespace SW.Abilities
{
    [DisallowMultipleComponent]
    public class AbilityMenuSetter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PlayerSettings _playerSettings;

        [Header("Previews")]
        [SerializeField] private GameObject _wallAbilityPreview;
        [SerializeField] private GameObject _jumpingAbilityPreview;
        [SerializeField] private GameObject _tpBackAbilityPreview;

        private GameObject[] _abilityPreviews;

        // CORE

        private void Awake()
        {
            _abilityPreviews = new GameObject[3] { _wallAbilityPreview, _jumpingAbilityPreview, _tpBackAbilityPreview, };
            SetAbilityIndexAndPreview(_playerSettings.AbilityIndex);
        }

        // PUBLIC

        public void SetAbilityIndexAndPreview(int index)
        {
            _playerSettings.AbilityIndex = index;
            SetAbilityPreview(index);
        }

        public void SetAbilityPreview(int index)
        {
            for (int i = 0; i < _abilityPreviews.Length; i++)
            {
                if (_abilityPreviews[i] != null)
                {
                    _abilityPreviews[i].SetActive(i == index);
                }
            }
        }

        public void SetNextAbility()
        {
            SetAbilityIndexAndPreview((_playerSettings.AbilityIndex + 1) % 3);
        }

        public void SetPreviousAbility()
        {
            SetAbilityIndexAndPreview((_playerSettings.AbilityIndex + 2) % 3);
        }
    }
}
