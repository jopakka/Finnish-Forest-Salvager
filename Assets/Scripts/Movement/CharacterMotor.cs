using Action;
using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        
        private static readonly int AnimatorParamSpeed = Animator.StringToHash("speed");

        private CharacterController _controller;
        private IMovementProvider _movementProvider;
        private IActionProvider _actionProvider;
        private Animator _animator;
        private Vector3 _velocity;
        private Vector3 _prevPosition;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movementProvider = GetComponent<IMovementProvider>();
            _actionProvider = GetComponent<IActionProvider>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            ActionCommand actionCommand = _actionProvider.GetActionCommand();
            if (actionCommand.Attack)
            {
                Vector3 attackPoint = actionCommand.AttackPoint;
                attackPoint.y = 0f;
                Vector3 start = transform.position + new Vector3(0f, 2f, 0f);
                Vector3 dir = (attackPoint - transform.position).normalized;
                Debug.DrawRay(start, dir, Color.red);
            }

            MovementCommand command = _movementProvider.GetMovementCommand();
            Vector3 move = GetMove(command);

            MoveCharacter(move);
            AnimateMovement(move.sqrMagnitude);
        }

        private Vector3 GetMove(MovementCommand command)
        {
            Vector3 move = command.Move * speed;
            move.y = 0f;
            return move;
        }

        private void MoveCharacter(Vector3 move)
        {
            _controller.Move(move * Time.deltaTime);

            if (move.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(move);
            }
        }

        private void AnimateMovement(float moveSpeed)
        {
            _animator.SetFloat(AnimatorParamSpeed, moveSpeed);
        }

        private void AnimateAttack()
        {
            // _animator.SetFloat(AnimatorParamSpeed, moveSpeed);
        }
    }
}