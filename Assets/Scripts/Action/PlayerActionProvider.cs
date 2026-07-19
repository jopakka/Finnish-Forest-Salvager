using UnityEngine;
using UnityEngine.InputSystem;

namespace Action
{
    public class PlayerActionProvider : MonoBehaviour, IActionProvider
    {
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask mouseRayLayerMask;

        private ActionCommand _actionCommand;

        public ActionCommand GetActionCommand()
        {
            ActionCommand actionCommand = _actionCommand;

            return actionCommand;
        }

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            bool didAttack = ctx.performed;
            if (didAttack)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = camera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
                if (Physics.Raycast(ray, out RaycastHit hit, 10000f, mouseRayLayerMask))
                {
                    _actionCommand.AttackPoint = hit.point;
                    _actionCommand.Attack = true;
                }
                else
                {
                    _actionCommand.Attack = false;
                }
            }
            else
            {
                _actionCommand.Attack = false;
            }
        }
    }
}