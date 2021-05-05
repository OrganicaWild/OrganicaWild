using System;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 1f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity; 
    public Animator animator;

    private int isWalkingHash;
    private int isRunningHash;
    private int timeSincePreviousInputHash;

    private PlayerInput input;
    
    private Vector2Int CurrentMovement { get; set; }
    private Vector3 CurrentMovementToVector3 => new Vector3(-CurrentMovement.x, 0f, -CurrentMovement.y);
    private bool MovementPressed => CurrentMovement.magnitude != 0f;
    private bool RunPressed { get; set; }

    private float TimeSinceLatestInput { get; set; } = 0f;

    private bool IsWalking
    {
        //get;
        //set;
        get => animator.GetBool(isWalkingHash);
        set => animator.SetBool(isWalkingHash, value);
    }

    private bool IsRunning
    {
        //get;
        //set;
        get => animator.GetBool(isRunningHash);
        set => animator.SetBool(isRunningHash, value);
    }
 
    void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Forward.performed += ctx =>
        {
            ResetTimeSinceLatestInput();
            CurrentMovement += ctx.ReadValueAsButton() ? Vector2Int.up : Vector2Int.down;
        };
        input.CharacterControls.Left.performed += ctx =>
        {
            ResetTimeSinceLatestInput();
            CurrentMovement += ctx.ReadValueAsButton() ? Vector2Int.left : Vector2Int.right;
        };
        input.CharacterControls.Backward.performed += ctx =>
        {
            ResetTimeSinceLatestInput();
            CurrentMovement += ctx.ReadValueAsButton() ? Vector2Int.down : Vector2Int.up;
        };
        input.CharacterControls.Right.performed += ctx =>
        {
            ResetTimeSinceLatestInput();
            CurrentMovement += ctx.ReadValueAsButton() ? Vector2Int.right : Vector2Int.left;
        };

        input.CharacterControls.Run.performed += ctx =>
        {
            ResetTimeSinceLatestInput();
            RunPressed = ctx.ReadValueAsButton();
        };

    }

    void Start()
    {
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        timeSincePreviousInputHash = Animator.StringToHash("TimeSincePreviousInput");
    }

    void Update()
    {
        PassTimeSincePreviousInput();
        HandleMovement();
    }

    private void PassTimeSincePreviousInput()
    {
        if (animator != null) animator.SetFloat(timeSincePreviousInputHash, TimeSinceLatestInput += Time.deltaTime);
    }

    void HandleMovement()
    {
        bool isRunning = IsRunning;
        bool isWalking = IsWalking;

        if (MovementPressed && !isWalking) IsWalking = true;
        if (!MovementPressed && isWalking) IsWalking = false;

        if (MovementPressed && RunPressed && !isRunning) IsRunning = true;
        if (!(MovementPressed || RunPressed) && isRunning) IsRunning = false;

        if (isWalking)
        {
            Vector3 direction = CurrentMovementToVector3.normalized;
            characterController.Move(direction * speed * Time.deltaTime);

            // TODO: figure out what's wrong with this
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, GetTargetAngle(), ref turnSmoothVelocity,
                turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private float GetTargetAngle()
    {
        if (CurrentMovement == Vector2.up) return 270;
        if (CurrentMovement == Vector2.up + Vector2.right) return 315;
        if (CurrentMovement == Vector2.right) return 0;
        if (CurrentMovement == Vector2.right + Vector2.down) return 45;
        if (CurrentMovement == Vector2.down) return 90;
        if (CurrentMovement == Vector2.down + Vector2.left) return 135;
        if (CurrentMovement == Vector2.left) return 180;
        return 225;
    }

    void OnEnable()
    {
        input.CharacterControls.Enable();
    }

    void OnDisable()
    {
        input.CharacterControls.Disable();
    }

    private void ResetTimeSinceLatestInput()
    {
        this.TimeSinceLatestInput = 0f;
    }
}
