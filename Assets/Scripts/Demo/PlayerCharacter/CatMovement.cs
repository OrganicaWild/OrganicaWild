using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    Animator animator;

    private int isWalkingHash;
    private int isRunningHash;
    private int timeSincePreviousInputHash;

    private PlayerInput input;
    
    private Vector2Int CurrentMovement { get; set; }
    private bool MovementPressed => CurrentMovement.magnitude != 0f;
    private bool RunPressed { get; set; }

    private float TimeSinceLatestInput { get; set; } = 0f;

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
        animator = GetComponent<Animator>();
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
        animator.SetFloat(timeSincePreviousInputHash, TimeSinceLatestInput += Time.deltaTime);
    }

    void HandleMovement()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);

        if (MovementPressed && !isWalking) animator.SetBool(isWalkingHash, true);
        if (!MovementPressed && isWalking) animator.SetBool(isWalkingHash, false);

        if (MovementPressed && RunPressed && !isRunning) animator.SetBool(isRunningHash, true);
        if (!(MovementPressed || RunPressed) && isRunning) animator.SetBool(isRunningHash, false);
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
