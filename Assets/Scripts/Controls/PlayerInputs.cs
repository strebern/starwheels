﻿using UnityEngine;
using Kart;

namespace Controls
{
    /*
     * Class for handling player inputs
     */
    public class PlayerInputs : BaseKartComponent
    {
        public bool Enabled = true;
        public bool DisableUseItem;
        public bool DisableMovement;
        public GameObject EscapeMenu;

        private new void Awake()
        {
            base.Awake();
            kartEvents.OnHit += () => SetInputEnabled(false);
            kartEvents.OnHitRecover += () => SetInputEnabled(true);
        }

        void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (Enabled && kartHub != null)
                {
                    Axis();
                    ButtonsPressed();
                }
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (Enabled && kartHub != null)
                {
                    ButtonsDown();
                    ButtonsUp();
                    AxisOnUse();
                }
            }
        }

        void SetInputEnabled(bool b)
        {
            Enabled = b;
        }

        void Axis()
        {
            kartHub.Accelerate(Input.GetAxis(Constants.AccelerateButton));
            kartHub.Decelerate(Input.GetAxis(Constants.DecelerateButton));
            kartHub.Turn(Input.GetAxis(Constants.TurnAxis));
        }

        void ButtonsDown()
        {
            // Keyboard & GamePad
            if (Input.GetButtonDown(Constants.UseAbilityButton))
            {
                kartHub.UseAbility(Input.GetAxis(Constants.TurnAxis), Input.GetAxis(Constants.UpAndDownAxis));
            }
            if (Input.GetButtonDown(Constants.DriftButton))
            {
                kartHub.InitializeDrift(Input.GetAxis(Constants.TurnAxis));
            }
            if (Input.GetButtonDown(Constants.UseItemButton))
            {
                kartHub.UseItem(Input.GetAxis(Constants.UpAndDownAxis));
            }
            if (Input.GetButtonDown(Constants.BackCamera))
            {
                KartEvents.Instance.OnBackCameraStart(true);
            }
            if (Input.GetButtonDown(Constants.ResetCamera))
            {
                KartEvents.Instance.OnCameraTurnReset();
            }

            // Mouse
            if (Input.GetButtonDown(Constants.UseItemForwardButton))
            {
                kartHub.UseItemForward();
            }
            if (Input.GetButtonDown(Constants.UseItemBackwardButton))
            {
                kartHub.UseItemBackward();
            }
        }

        void ButtonsPressed()
        {
            if (Input.GetButton(Constants.DriftButton))
            {
                kartHub.DriftTurns(Input.GetAxis(Constants.TurnAxis));
            }
        }

        void ButtonsUp()
        {
            if (Input.GetButtonUp(Constants.DriftButton))
            {
                kartHub.StopDrift();
            }
            if (Input.GetButtonUp(Constants.BackCamera))
            {
                KartEvents.Instance.OnBackCameraEnd(false);
            }
        }
        void AxisOnUse()
        {
            if (KartEvents.Instance.OnCameraTurnStart != null)
            {
                KartEvents.Instance.OnCameraTurnStart(Input.GetAxis(Constants.TurnCamera));
            }
        }
    }
}
