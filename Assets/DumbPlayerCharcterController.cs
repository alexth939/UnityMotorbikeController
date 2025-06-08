using UnityEngine;

public class DumbPlayerCharcterController : MonoBehaviour
{
    [SerializeField] UltEvents.UltEvent _startedRunning;

    [SerializeField] private CharacterController _controller;
    private Vector3 _rawMoveDirection;
    [Tooltip("0 is idle; 1 is joggling, 2 is sprint;")]
    [SerializeField] private float _normalizedRunSpeedTarget = 0;
    private Vector3 _worldBodyLookTarget;

    private float _yVelocity = -4;

    private float fallingSpeed = 0.01f;

    [SerializeField] private AnimationCurve _runSpeedCurve;

    [SerializeField] private Animator _animator;

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
        
    //}

    // Don't set it too low, becuse he wont be able to jump when running down on inclined surface.
    [SerializeField] private float _gravity = 0.08f;

    private void FixedUpdate()
    {
        //if(_controller.isGrounded == false)
        //    transform.Translate(Vector3.down * fallingSpeed);

        float runSpeedTarget = _runSpeedCurve.Evaluate(_normalizedRunSpeedTarget);
        Vector3 rawMoveDirectionWithGravity = new()
        {
            x = _rawMoveDirection.x * runSpeedTarget,
            y = _yVelocity,
            z = _rawMoveDirection.z * runSpeedTarget,
        };

        _ = _controller.Move(transform.rotation * rawMoveDirectionWithGravity);

        if(_isInAir && _controller.isGrounded)
        {
            _isInAir = false;
            OnGrounded();
        }

        float runSpeed = _rawMoveDirection.magnitude * _normalizedRunSpeedTarget;
        _animator.SetFloat("moveSpeed", runSpeed);

        _yVelocity = _controller.isGrounded ? -_gravity : _yVelocity - 0.008f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _worldBodyLookTarget = transform.forward;
    }

    [SerializeField] private float _movingAccelerationSpeed = 0.01f;
    [SerializeField] private float _movingDecelerationSpeed = 0.01f;

    [SerializeField] private bool _isInAir = false;

    private void OnGrounded()
    {
        _animator.SetTrigger("Grounded");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            _startedRunning.Invoke();

        if(Input.GetKey(KeyCode.LeftShift))
            _normalizedRunSpeedTarget = Mathf.MoveTowards(_normalizedRunSpeedTarget, 2.0f, _movingAccelerationSpeed);
        else
            _normalizedRunSpeedTarget = Mathf.MoveTowards(_normalizedRunSpeedTarget, 1.0f, _movingDecelerationSpeed);

        float bodyRotationSpeed = 200.0f;
        float yAxisRotationDelta = Input.GetAxis("Mouse X") * bodyRotationSpeed * Time.deltaTime;
        _worldBodyLookTarget = Quaternion.Euler(0, yAxisRotationDelta, 0) * _worldBodyLookTarget;
        float maxDegreesDelta = 1500 * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_worldBodyLookTarget, Vector3.up), maxDegreesDelta);

        if(_isInAir && Input.GetKey(KeyCode.Space))
        {
            Vector3 zombieCheckVector = transform.forward + Vector3.down;
            
            Ray zombieCheckRay = new(transform.position, zombieCheckVector);
            float zombieCheckDistance = 0.5f;
            int zombieLayerMask = LayerMask.GetMask("Character");

            if(Physics.SphereCast(zombieCheckRay, 0.5f, out RaycastHit hitInfo, zombieCheckDistance, zombieLayerMask, QueryTriggerInteraction.Collide))
            //if(Physics.Raycast(zombieCheckRay, out RaycastHit hitInfo, zombieCheckDistance, zombieLayerMask, QueryTriggerInteraction.Collide))
            {
                Jump();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            Jump();
        }

        _rawMoveDirection = new Vector3()
        {
            x = Input.GetAxis("Horizontal"),
            //y = _yVelocity,
            z = Input.GetAxis("Vertical"),
        };
    }

    private void Jump()
    {
        _yVelocity = 0.3f;//0.15f;
        _isInAir = true;
        _animator.SetTrigger("Jump");
    }
}
