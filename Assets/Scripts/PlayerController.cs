using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float acceleration = 10f;
    public float deceleration = 12f;
    public float gravity = -20f;
    public float rotationSpeed = 15f;
    public bool justLanded = true;
    public bool isGrounded;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private CharacterController controller;
    private AbilitySystem abilitySystem;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 currentMove;
    private const float groundedGraceTime = 0.1f;
    
    [HideInInspector] public Vector3 externalVelocity;
	[HideInInspector] public bool overrideMovement = false;
	[HideInInspector] public Vector3 lastMoveDirection;
    public float yVelocity;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        abilitySystem = GetComponent<AbilitySystem>();
        foreach (var a in GetComponentsInChildren<Animator>())
        {
            if (a.runtimeAnimatorController != null) { animator = a; break; }
        }
        Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

    }
    
    

    void Update()
    {
    
    	isGrounded = Physics.CheckSphere(
			transform.position,
			groundCheckRadius,
			groundLayer
		);
    	if (!overrideMovement)
		{
       		HandleRotation();
        
        }
        if (!overrideMovement)
		{
			HandleMovement();
		}

        HandleGravity();
        
        abilitySystem.SetHeld(ActionSlot.Primary, Input.GetMouseButton(0));   // Left click
		abilitySystem.SetHeld(ActionSlot.Secondary, Input.GetMouseButton(1)); // Right click

        
        if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			abilitySystem.TriggerDash(ActionSlot.Dash);
		}

		if (Input.GetButtonDown("Jump"))
        {
			abilitySystem.TriggerJump(ActionSlot.Jump);
		}

        abilitySystem.SetHeld(ActionSlot.Action1, Input.GetKey(KeyCode.E));

        Vector3 move = overrideMovement ? externalVelocity : currentMove;

		Vector3 finalMove = move + Vector3.up * yVelocity;

        controller.Move(finalMove * Time.deltaTime);

        //if (isGrounded)
            //groundedTimer = groundedGraceTime;
        //else
            //groundedTimer -= Time.deltaTime;

        float moveX = currentMove.magnitude > 0.01f
            ? Vector3.Dot(currentMove.normalized, transform.right)
            : 0f;

        animator?.SetFloat("Speed", currentMove.magnitude);
        animator?.SetFloat("MoveX", moveX);
        animator?.SetBool("IsGrounded", isGrounded);
    }

	void HandleRotation()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		Plane groundPlane = new Plane(Vector3.up, transform.position);

		if (groundPlane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			Vector3 direction = hitPoint - transform.position;

			direction.y = 0f;

			if (direction.sqrMagnitude > 0.001f)
			{
				Quaternion targetRotation = Quaternion.LookRotation(direction);

				transform.rotation = Quaternion.Slerp(
					transform.rotation,
					targetRotation,
					rotationSpeed * Time.deltaTime
				);
			}
		}
	}



	void HandleMovement()
	{
		float h = Input.GetAxisRaw("Horizontal"); // A/D
		float v = Input.GetAxisRaw("Vertical");   // W/S

		Vector3 camForward = Camera.main.transform.forward;
		Vector3 camRight = Camera.main.transform.right;

		// Flatten camera vectors
		camForward.y = 0f;
		camRight.y = 0f;

		camForward.Normalize();
		camRight.Normalize();

		Vector3 targetMove =
			(camForward * v + camRight * h).normalized * moveSpeed;

		if (targetMove.magnitude > 0.1f)
		{
			lastMoveDirection = targetMove.normalized;
			currentMove = Vector3.Lerp(
				currentMove,
				targetMove,
				acceleration * Time.deltaTime
			);
		}
		else
		{
			currentMove = Vector3.Lerp(
				currentMove,
				Vector3.zero,
				deceleration * Time.deltaTime
			);
		}
	}



    void HandleGravity()
    {
        if (isGrounded)
        {
            if (yVelocity < 0){
            	yVelocity = -2f;
            }
            if (!justLanded)
            {
                justLanded = true;
                abilitySystem.TriggerLand(ActionSlot.Jump);
            }

        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
			justLanded=false;

        }
    }
}
