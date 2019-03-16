﻿using UnityEngine;
using Bolt;
using System.Collections;

namespace Gamemodes.Totem
{
    public class TotemColorChanger : GlobalEventListener
    {
        [Header("Colors")]
        public Color CurrentColor;

        [Header("Events")]
        public ColorEvent OnColorChange;

        [SerializeField] private Renderer _totemAuraRenderer;

        private Color _defaultColor;

        // CORE

        private void Awake()
        {
            _defaultColor = _totemAuraRenderer.material.color;
            CurrentColor = _defaultColor;
        }

        // BOLT

        public override void OnEvent(TotemWallHit evnt)
        {
            ChangeColor(evnt.Team);
            StartCoroutine(SecurityRoutine());
        }

        public override void OnEvent(TotemPicked evnt)
        {
            ResetToDefault();
        }

        // PUBLIC

        public void ChangeColor(Color c)
        {
            _totemAuraRenderer.material.color = c;
            CurrentColor = c;
            OnColorChange.Invoke(CurrentColor);
        }

        public void ResetToDefault()
        {
            ChangeColor(_defaultColor);
            StopAllCoroutines();
        }

        public bool ColorIsDefault()
        {
            return CurrentColor == _defaultColor;
        }

        // PRIVATE

        private IEnumerator SecurityRoutine()
        {
            yield return new WaitForSeconds(20f);
            if (!ColorIsDefault())
            {
                ResetToDefault();
            }
        }
    }
}