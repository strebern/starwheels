﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu.Options
{
    public class CustomizationMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] _VCams;

        public void DisableVCams()
        {
            foreach (GameObject _VCam in _VCams)
            {
                _VCam.SetActive(false);
            }
        }
    }
}

