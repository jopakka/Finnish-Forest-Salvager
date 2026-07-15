using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.05f;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = 25f;

    private Vector3 _velocity;
    private Vector2 _lookInput;
    private float _yaw;
    private float _pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        _yaw += _lookInput.x * sensitivity * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position,
            ref _velocity,
            smoothTime
        );
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
    
    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }
}