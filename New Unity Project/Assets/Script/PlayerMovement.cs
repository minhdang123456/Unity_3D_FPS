using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Assign những thứ quan trong như quay và camera
    public Transform playerCamera;//Camera để người chơi nhìn thấy
    public Transform orientation;// Cho người chơi quay được

    //rigidbody
    private Rigidbody rb;


    //Wall Run
    [Header("WallRun")]
    //public LayerMask whatIsWall;
    RaycastHit wallR, wallL;
    public float maxWallrunTime;
    public float wallrunForce, wallRunUpwardForce, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning,readyToWallRun;
    bool resetDoubleJump = true;
    public int wallJumps, wallJumpsLeft;
    public GameObject lastWall;

    //Camera Tilting
    [Header("Camera Tilting")]
    public float maxWallRunCameraTilt, wallRunCameraTilt;


    //Movement
    [Header("Movement")]
    public float movingSpeed = 4500f;//Tốc độ di chuyển của người chơi
    public float maxSpeed = 20f;// cao nhất cho wall running và sliding
    public bool grounded;
    public LayerMask whatIsGround;

    //CounterMovement
    public float Friction = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch
    private Vector3 playerScale;
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);

    //Sliding
    [Header("Sliding")]
    public float slidingForce = 400f;
    public float slidingFriction = 0.2f;
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallVector;

    //Jumping
    [Header("Jumping")]
    private bool readyToJump = true;
    private float jumpCoolDown = 0.75f;
    public float jumpForce = 550f;//lực nhảy phải ít đi và cool down của nhảy phải nhiều lên

    public int startDoubleJumps = 2;
    int doubleJumpLeft;

    //Sensitivity và rotation
    private float xRotation = 0;
    private float sensitivity = 150f;
    private float sensMultiply = 1f;

    //Climbing
    [Header("Climbing")]
    public float climbForce, climbSpeedAdd;
    public LayerMask whatIsLadder;
    bool alreadyStoppedAtLadder;



    //Input
    private Vector3 inputDirection ;
    bool jumping,crouching;

    //Kiểm tra có phải tường không
    private void CheckForWall() 
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right,out wallR ,1f, whatIsGround);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right,out wallL ,1f, whatIsGround);

        //leave wall run
        if (!isWallLeft && !isWallRight && isWallRunning) StopWallRun();
        if ((isWallLeft || isWallRight) && resetDoubleJump) doubleJumpLeft = startDoubleJumps;
    }
    //Cái này sẽ được lấy trước khi game bắt đầu
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        MouseCursor();
    }

    //Cho con chuột cố định 1 chỗ
    void MouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //FixedUpdate sẽ update nhiều frame do tính toán những vật lý cho nhân vật
    private void FixedUpdate()
    {
        Movement();
       
    }

    //Update sẽ update 1 lần tốt cho việc như nhận input
    private void Update()
    {

        MyInput();
        Look();
        CheckForWall();
        WallRunInput();

    }

    //Tìm input của người chơi
    private void MyInput()
    {
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetButtonDown("Jump") && !grounded && doubleJumpLeft >= 1)
        {
            Jump();
        }

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();

        //climbing
         if (Physics.Raycast(transform.position, orientation.forward, 1, whatIsLadder) && inputDirection.y > .9f)
            Climb();
         else alreadyStoppedAtLadder = false;

        //double jump
        if (Input.GetButtonDown("Jump") && !grounded && doubleJumpLeft >= 1)
        {
            Jump();
        }
        //Wall jumping
        if (Input.GetButtonDown("Jump") && wallJumpsLeft >= 1 && isWallRunning)
        {
            Jump();
        }

        //Wallrun
        if (isWallRight && !grounded && readyToWallRun) StartWallrun();
        if (isWallLeft && !grounded && readyToWallRun) StartWallrun();
        //reset wallrun
        if (!isWallRight && !isWallLeft && !readyToWallRun) readyToWallRun = true;


    }

    //Cho ngồi xuống
    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slidingForce);
            }
        }

    }


    //Dừng ngồi xuống
    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }


    public void Movement()
    {
        //Thêm trọng lực
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Tìm Vận tốc tương đương với góc nhìn của nhân vật
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;


        //CounterMovement
        CounterMovement(inputDirection.x, inputDirection.y, mag);
        //Nhảy
        if (readyToJump && jumping)
        {
            Jump();
        }


        //Cho maxSpeed default thành maxSpeed của hiện tại
        float maxSpeed = this.maxSpeed;
        //slide xuống dốc tăng tốc độ và cho nó dính vào mặt đất
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        

        if(grounded)
        {
            doubleJumpLeft = startDoubleJumps;
        }

        //reset tốc độ về không để không cho nhân vật đi quá maxSpeed
        if (inputDirection.x > 0 && xMag > maxSpeed) inputDirection.x = 0;
        if (inputDirection.x < 0 && xMag < -maxSpeed) inputDirection.x = 0;
        if (inputDirection.y > 0 && yMag > maxSpeed) inputDirection.y = 0;
        if (inputDirection.y < 0 && yMag < -maxSpeed) inputDirection.y = 0;


        //multipliers
        float multiplier = 1f, multiplierV = 1f;


        //di chuyển khi trên không
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // tốc độ di chuyển khi slide
        if (grounded && crouching)
        {
            multiplierV = 0f;
        }

        //cho force đẩy người chơi di chuyển
        rb.AddForce(orientation.transform.forward * inputDirection.y * movingSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * inputDirection.x * movingSpeed * Time.deltaTime * multiplier);


    }


    public void Jump()
    {
        //Nhảy bình thường
        if (grounded)
        {
            readyToJump = false;
            //Lực nhảy
            rb.AddForce(Vector3.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //reset vận tốc nhảy lúc đang rơi xuông
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            //delay jump khi đã nhảy rồi
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
        //Double Jump
        if(!grounded && !isWallRunning && doubleJumpLeft >= 1)
        {
            readyToJump = false;
            doubleJumpLeft--;
            //Debug.Log("Double Jump");
            
            //Nhảy theo hướng trước
            if(Input.GetKey("w"))
            {
                rb.AddForce(orientation.forward * jumpForce * 2.5f);
                //Debug.Log("Jump");

            }
            else if(Input.GetKey("s"))
            {
                rb.AddForce(-orientation.forward * jumpForce * 2.5f);
            }

            if(Input.GetKey("a"))
            {
                rb.AddForce(-orientation.right * jumpForce * 2.5f);
            }
            else if(Input.GetKey("d"))
            {
                rb.AddForce(orientation.right * jumpForce * 2.5f);
            }
            //Lực nhảy
            rb.AddForce(Vector3.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //Reset vận tốc
            rb.velocity = Vector3.zero;

            Invoke(nameof(ResetJump),jumpCoolDown);
        }
        //Wall Jump
        if(isWallRunning&& wallJumpsLeft >= 1 )
        {
            readyToJump = false;
            wallJumpsLeft--;

            //Normal Jump
            rb.AddForce(Vector2.up * jumpForce * 1.0f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            rb.AddForce(orientation.forward * jumpForce * 1.5f);
            if (isWallRight) rb.AddForce(-orientation.right * jumpForce * 2.5f);
            if (isWallLeft) rb.AddForce(orientation.right * jumpForce * 2.5f);


            //WallHop
           /* if(isWallRight || isWallLeft && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                rb.AddForce(-orientation.up * jumpForce *1f);
            }
            if (isWallRight && Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right*jumpForce*3.2f);
            if (isWallLeft && Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * jumpForce * 3.2f);*/

            //Thêm Lực đẩy về phía trước 
            rb.AddForce(orientation.forward * jumpForce * 1f);

            //Reset Vận tốc
            rb.velocity = Vector3.zero;

            Invoke(nameof(ResetJump),jumpCoolDown);
        }


    }

    //Reset lại jump để có thể nhảy tiếp 
    private void ResetJump()
    {
        readyToJump=true;
    }




    //Cho người chơi sử dụng chuột để nhìn xung quanh
    private float Xfov;
    private void Look()
    {
        float MouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiply;
        float MouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiply;


        // Tìm rotation hiện tại của người chơi
        Vector3 rotate = playerCamera.transform.localRotation.eulerAngles;
        Xfov = rotate.y + MouseX;

        //Ngăn việc over hoặc under rotate
        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        //Cho người chơi sử dụng chuột để nhìn xung quanh
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, Xfov, wallRunCameraTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, Xfov, 0);


        //Quay Camera khiwall run
        if(Mathf.Abs(wallRunCameraTilt)<maxWallRunCameraTilt && isWallRunning&&isWallRight)
        {
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        }
        if(Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
        {
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        }

        //Quay Camera Trở về ban đầu nếu ko wall run
        if(wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
        {
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        }
        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
        {
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        }

    }





    //Counter movement để cho nhân vật không được tăng tốc và slide khắp nơi
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping||isWallRunning) return;

        //Làm chậm slide
        if (crouching)
        {
            rb.AddForce(movingSpeed * Time.deltaTime * -rb.velocity.normalized * Friction);
            return;
        }


        //Counter Movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(movingSpeed * orientation.transform.right * Time.deltaTime * -mag.x * Friction);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(movingSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * Friction);
        }

        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }


    }

    //Tìm vận tốc tương đương với góc nhìn của nhân vật
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }


    //Kiểm tra sàn có phẳng ko
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnCollisionStay(Collision other)
    {
        //kiểm tra những nơi mình di chuyển được
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Kiểm tra va chạm vào tường
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }


            if(isWallRunning)
            {
                if(lastWall != other.gameObject)
                {
                    //Debug.Log("WallChanged");
                    lastWall = other.gameObject;
                    wallJumpsLeft = wallJumps;
                }
            }
        }

        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }

    //input cho WallRun
    private void WallRunInput()
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();
    }

    float elapsedWallTime;
    //Wall Run Function
    private void StartWallrun()
    {
        //Debug.Log("WallRunning");
        if (grounded) StopWallRun();

        elapsedWallTime += Time.deltaTime;

        if(elapsedWallTime >= maxWallrunTime)
        {
            StopWallRun();
        }

        //rb.useGravity = false;
        isWallRunning = true;
        //thêm lực đẩy lên
        rb.AddForce(orientation.up * wallRunUpwardForce * Time.deltaTime);
        if (rb.velocity.magnitude <= 25f + maxWallSpeed)
        {
            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            //Make sure char sticks to wall
            if (isWallRight)
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
            else
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
        }
    }


    //Ngưng wall run
    private void StopWallRun()
    {
        isWallRunning = false;
        readyToWallRun = false;

        elapsedWallTime = 0;
    }

    private void Climb()
    {
        //Makes possible to climb even when falling down fast
        Vector3 vel = rb.velocity;
        if (rb.velocity.y < 0.5f && !alreadyStoppedAtLadder)
        {
            rb.velocity = new Vector3(vel.x, 0, vel.z);
            //Make sure char get's at wall
            alreadyStoppedAtLadder = true;
            rb.AddForce(orientation.forward * 500 * Time.deltaTime);
        }

        //Push character up
        if (rb.velocity.magnitude < movingSpeed + climbSpeedAdd)
            rb.AddForce(orientation.up * climbForce * Time.deltaTime);
        else if (rb.velocity.magnitude > movingSpeed + climbSpeedAdd)
            rb.AddForce(-orientation.up * climbForce * Time.deltaTime);


        //Doesn't Push into the wall
        if (!Input.GetKey(KeyCode.S)) inputDirection.y = 0;
    }

}
