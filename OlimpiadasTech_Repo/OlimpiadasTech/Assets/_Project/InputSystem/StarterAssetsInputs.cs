using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        public Action onKillButtonPressed;

        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool crouch;
        public bool hack;
        public bool shoot;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
        public float zoom = 0f;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            CrouchInput(value.isPressed);
        }

        public void OnHack(InputValue value)
        {
            HackInput(value.isPressed);
        }

        public void OnKill(InputValue value)
        {
            onKillButtonPressed?.Invoke();
        }

        public void OnZoom(InputValue value)
        {
            ZoomInput(value.Get<Vector2>());
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void CrouchInput(bool newCrouchState)
        {
            crouch = newCrouchState;
        }

        public void HackInput(bool newHackState)
        {
            hack = newHackState;
        }

        public void ZoomInput(Vector2 newZoomState)
        {
            zoom = newZoomState.y;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        private void Awake()
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}