//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && DA_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Simple first person controller to walk around the voxel samples.
    /// WASD move, mouse look, Shift sprint, Space jump, F flashlight, V fly mode, Esc frees the mouse
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class DAVoxelPlayer : MonoBehaviour
    {
        public Transform cameraTransform;
        public Light flashlight;

        public float walkSpeed = 5;
        public float sprintSpeed = 10;
        public float flySpeed = 20;
        public float jumpHeight = 1.2f;
        public float gravity = -20;
        public float mouseSensitivity = 0.1f;

        private CharacterController controller;
        private float pitch;
        private float verticalSpeed;
        private bool flying;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            LockCursor(true);
        }

        void Update()
        {
            if (WasPressed(KeyId.Escape)) LockCursor(false);
            if (WasMousePressed()) LockCursor(true);
            if (WasPressed(KeyId.Flashlight) && flashlight != null) flashlight.enabled = !flashlight.enabled;
            if (WasPressed(KeyId.Fly)) { flying = !flying; verticalSpeed = 0; }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                var look = GetLook() * mouseSensitivity;
                transform.Rotate(0, look.x, 0);
                pitch = Mathf.Clamp(pitch - look.y, -89, 89);
                if (cameraTransform != null) cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
            }

            var input = GetMove();
            var move = transform.right * input.x + transform.forward * input.y;
            if (move.sqrMagnitude > 1) move.Normalize();

            if (flying)
            {
                // fly along the view direction
                var forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
                move = transform.right * input.x + forward * input.y;
                if (IsDown(KeyId.Jump)) move += Vector3.up;
                var speed = IsDown(KeyId.Sprint) ? flySpeed * 3 : flySpeed;
                controller.Move(move * (speed * Time.deltaTime));
                return;
            }

            if (controller.isGrounded)
            {
                verticalSpeed = -1;
                if (WasPressed(KeyId.Jump)) verticalSpeed = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            verticalSpeed += gravity * Time.deltaTime;

            var horizontalSpeed = IsDown(KeyId.Sprint) ? sprintSpeed : walkSpeed;
            var velocity = move * horizontalSpeed + Vector3.up * verticalSpeed;
            controller.Move(velocity * Time.deltaTime);
        }

        public void Teleport(Vector3 position)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
            verticalSpeed = 0;
        }

        private static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private enum KeyId { Escape, Flashlight, Fly, Jump, Sprint }

#if ENABLE_INPUT_SYSTEM && DA_INPUT_SYSTEM
        private static KeyControl GetKey(KeyId id)
        {
            var k = Keyboard.current;
            if (k == null) return null;
            switch (id)
            {
                case KeyId.Escape: return k.escapeKey;
                case KeyId.Flashlight: return k.fKey;
                case KeyId.Fly: return k.vKey;
                case KeyId.Jump: return k.spaceKey;
                default: return k.leftShiftKey;
            }
        }

        private static bool WasPressed(KeyId id) { var key = GetKey(id); return key != null && key.wasPressedThisFrame; }
        private static bool IsDown(KeyId id) { var key = GetKey(id); return key != null && key.isPressed; }
        private static bool WasMousePressed() { return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame; }
        private static Vector2 GetLook() { return Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero; }

        private static Vector2 GetMove()
        {
            var k = Keyboard.current;
            if (k == null) return Vector2.zero;
            var x = (k.dKey.isPressed ? 1 : 0) - (k.aKey.isPressed ? 1 : 0);
            var y = (k.wKey.isPressed ? 1 : 0) - (k.sKey.isPressed ? 1 : 0);
            return new Vector2(x, y);
        }
#else
        private static KeyCode GetKey(KeyId id)
        {
            switch (id)
            {
                case KeyId.Escape: return KeyCode.Escape;
                case KeyId.Flashlight: return KeyCode.F;
                case KeyId.Fly: return KeyCode.V;
                case KeyId.Jump: return KeyCode.Space;
                default: return KeyCode.LeftShift;
            }
        }

        private static bool WasPressed(KeyId id) { return Input.GetKeyDown(GetKey(id)); }
        private static bool IsDown(KeyId id) { return Input.GetKey(GetKey(id)); }
        private static bool WasMousePressed() { return Input.GetMouseButtonDown(0); }
        // legacy mouse axes are already scaled, so undo the sensitivity used for raw deltas
        private static Vector2 GetLook() { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 20; }
        private static Vector2 GetMove() { return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); }
#endif
    }
}
