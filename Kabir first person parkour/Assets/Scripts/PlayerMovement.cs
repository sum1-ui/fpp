using UnityEngine;
using static System.Math;
using System;

public class PlayerMovement : MonoBehaviour
{

    [Header("PlayerMovement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float speedReductionMultiplier = .08f;
    public float speedCapTime = .1f;
    public float speedCap;
    private float xzVelocity;


    [Header("Jump")]
    public float playerHeight;
    Vector3 playertallness;
    Vector3 moveDirection;
    public float jumpForce;
    public float jumpCooldown;
    public float airmultiplier;
    bool readyToJump;
    public float airFallSpeed = 2;

    [Header("Ground")]
    public float distToGround = 1f;
    public LayerMask whatIsGround;
    private float heightAboveGround;
    public bool Grounded;
    public float groundCheckDownDistance;


    [Header("SlopeHandling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintkey = KeyCode.LeftShift;



    public Transform orientation;

    float horizontalInput;
    float verticalInput;

  

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
       Vector3 playerSize = GetComponentInChildren<Collider>().bounds.size;
        Debug.Log(playerSize.y);

        JumpReset();
        Grounded = true;
        xzVelocity = MathF.Sqrt(MathF.Pow(rb.linearVelocity.x,2) + MathF.Pow(rb.linearVelocity.z,2));
    }


    private void Update()
    {

        //ground check
        RaycastHit hit;
        if (OnSlope())
        {

        }

        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDownDistance))
            {
                heightAboveGround = hit.distance;
                print(hit.distance);
                if ((heightAboveGround > 0.045f) && (heightAboveGround < .05f))
                {
                    Grounded = true;
                }
                else
                {
                    Grounded = false;
                }
            }
  
        }


        GravitySpeedIncrease();

        MyInput();

        //walk sprint or in air
        StateHandler();

        //speed control
        if (Grounded)
        {
            SpeedControl();
        }
        if (!Grounded)
        {

        }

        // do drag
        if (Grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
       
    }

    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && Grounded)
        {
            readyToJump = false;

            Jump();
           Invoke(nameof(JumpReset), jumpCooldown);
            
        }
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if ((flatVel.magnitude > speedCap) && Grounded )
        {
            Invoke(("SpeedPossibilities"), speedCapTime);

            Debug.Log("speedcap");
        }
        if ((xzVelocity > speedCap))
            {

        } 

    }

     void SpeedPossibilities()
    {
        //new speed equals old speed minus x until new speed equals speed cap or player velocity increase too fast
        if (rb.linearVelocity.x > 0f && rb.linearVelocity.z > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * speedReductionMultiplier, rb.linearVelocity.y, (rb.linearVelocity.z) * speedReductionMultiplier);
        }

        else if (rb.linearVelocity.x < 0f && rb.linearVelocity.z > 0)
        {
            rb.linearVelocity = new Vector3((rb.linearVelocity.x) * speedReductionMultiplier, rb.linearVelocity.y, (rb.linearVelocity.z) * speedReductionMultiplier);
        }

        else if (rb.linearVelocity.x > 0f && rb.linearVelocity.z < 0)
        {
            rb.linearVelocity = new Vector3((rb.linearVelocity.x) * speedReductionMultiplier, rb.linearVelocity.y, (rb.linearVelocity.z) * speedReductionMultiplier);
        }

        else if (rb.linearVelocity.x < 0f && rb.linearVelocity.z < 0)
        {
            rb.linearVelocity = new Vector3((rb.linearVelocity.x) * speedReductionMultiplier, rb.linearVelocity.y, (rb.linearVelocity.z) * speedReductionMultiplier);
        }

        
    }

    


    private void StateHandler()
    {
        if (Grounded && Input.GetKey(sprintkey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        else if (Grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;
        }
    }


    private void MovePlayer() 
    {
         //calc mvmt direction

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

       if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }

        
        if(Grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
       else if(!Grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed *10f * airmultiplier, ForceMode.Force);
    }


    public void GravitySpeedIncrease()
    {
        if (rb.linearVelocity.y < -0.1f)
        {
            Physics.gravity = Physics.gravity * airFallSpeed;
        }
    }

    private void Jump()
    {
        // reset jump velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void JumpReset()
    {
        readyToJump = true;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position + moveDirection, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}

//maybe add boost system which increases with each bit of time above speed cap. 
//Tricks like wall run increase speed and therefore increase boost
//when boost reaches max turbo charge button available?