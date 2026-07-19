using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMovementProvider : MonoBehaviour, IMovementProvider
    {
        [SerializeField] private Transform camera;

        private MovementCommand _moveCommand;
        private Vector2 _moveDirection;

        private void Update()
        {
            Vector3 forward = camera.forward;
            Vector3 right = camera.right;
            
            forward.y = 0;
            right.y = 0;
            
            forward.Normalize();
            right.Normalize();

            _moveCommand.Move = forward * _moveDirection.y + right * _moveDirection.x;
        }

        public MovementCommand GetMovementCommand()
        {
            MovementCommand result = _moveCommand;
            return result;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveDirection = ctx.ReadValue<Vector2>();
        }
    }
}