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
        private Animator _animator;
        private Vector3 _velocity;
        private Vector3 _prevPosition;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movementProvider = GetComponent<IMovementProvider>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            MovementCommand command = _movementProvider.GetMovementCommand();
            Vector3 move = command.Move * speed;
            _controller.Move(move * Time.deltaTime);

            move.y = 0f;
            float moveSpeed = move.sqrMagnitude;

            if (moveSpeed > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(move);
            }

            AnimateCharacter(moveSpeed);
        }

        private void AnimateCharacter(float moveSpeed)
        {
            _animator.SetFloat(AnimatorParamSpeed, moveSpeed);
        }
    }
}