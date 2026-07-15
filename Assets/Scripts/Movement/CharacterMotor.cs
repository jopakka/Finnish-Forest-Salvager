using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 2;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundMask;

        private CharacterController _controller;
        private IMovementProvider _movementProvider;
        private Vector3 _velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movementProvider = GetComponent<IMovementProvider>();
        }

        private void Update()
        {
            MovementCommand command = _movementProvider.GetMovementCommand();

            if (IsGrounded())
            {
                if (_velocity.y < 0)
                {
                    _velocity.y = -0f;
                }

                if (command.Jump)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }
            }

            _velocity.y += gravity * Time.deltaTime;

            Vector3 move = new Vector3(command.Move.x, 0, command.Move.y);
            move *= speed;
            move += Vector3.up * _velocity.y;

            _controller.Move(move * Time.deltaTime);
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(
                groundCheck.position,
                groundRadius,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            if (groundCheck != null)
            {
                Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
            }
        }
    }
}