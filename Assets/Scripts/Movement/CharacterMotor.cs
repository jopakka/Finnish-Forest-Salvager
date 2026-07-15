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
        private float _groundDistance = float.PositiveInfinity;
        private float _groundCheckOffset = 0.1f;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movementProvider = GetComponent<IMovementProvider>();
        }

        private void Update()
        {
            MovementCommand command = _movementProvider.GetMovementCommand();

            RaycastHit? groundHit = GroundHit();
            float distance = groundHit?.distance ?? float.PositiveInfinity;
            _groundDistance = distance;
            
            if (!float.IsInfinity(distance))
            {
                if (distance < _groundCheckOffset + 0.01f)
                {
                    _velocity.y = 0f;
                }
                
                if (command.Jump)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }
            }

            _velocity.y += gravity * Time.deltaTime;

            Vector3 move = command.Move * speed;
            move.y = _velocity.y;

            _controller.Move(move * Time.deltaTime);

            move.y = 0f;

            if (move.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(move);
            }
        }

        private RaycastHit? GroundHit()
        {
            RaycastHit hit;

            if (
                Physics.SphereCast(
                    groundCheck.position + Vector3.up * (groundRadius + _groundCheckOffset),
                    groundRadius,
                    Vector3.down,
                    out hit,
                    0.2f + _groundCheckOffset + groundRadius,
                    groundMask
                )
            )
            {
                return hit;
            }
        
            return null;
        }

        private void OnDrawGizmosSelected()
        {
            if (float.IsInfinity(_groundDistance)) return;
            Gizmos.color = Color.green;
            
            Vector3 position = groundCheck.position + Vector3.up * (groundRadius - _groundDistance);
            Gizmos.DrawWireSphere(position, groundRadius);
            Gizmos.DrawLine(groundCheck.position, position);
        }
    }
}