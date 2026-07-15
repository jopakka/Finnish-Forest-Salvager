using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMovementProvider : MonoBehaviour, IMovementProvider
    {
        private MovementCommand _moveCommand;

        public MovementCommand GetMovementCommand()
        {
            MovementCommand result = _moveCommand;
            
            // One-shot actions are consumed
            _moveCommand.Jump = false;
            
            return result;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveCommand.Move = ctx.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            Debug.Log("Jump requested");
            _moveCommand.Jump = true;
        }
    }
}