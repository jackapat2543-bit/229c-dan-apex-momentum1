using UnityEngine;

public class SimplePlayer : MonoBehaviour // library สำหรับตอน gameplay
{
    private Rigidbody2D rigid; // ดึง component Rigidbody2D
     
    private Animator anim; // ดึง component Animator
    private ParticleSystem grassPar; // particle
    private ParticleSystem.EmissionModule emission; // ตัวปล่อย particle
    private Collider2D hitboxCollider;
    private bool isAttacking = false;

     [Header("Attack")]
     [SerializeField] private GameObject hitboxObject;
     [SerializeField] private float attackDuration = 0.2f;


    [Header("Ground And Wall Check")]
    [SerializeField] private float groundDistCheck = 1f; // ระยะไกลสุดที่ใช้ sensor ตรวจหาพื้น
    [SerializeField] private float wallDistCheck = 1f; // ระยะไกลสุดที่ใช้ sensor ตรวจหาผนัง
    [SerializeField] private LayerMask groundLayer; // layermask ของ ground และ platform
    public bool isGrounded = false; // เจอพื้น ?
    public bool isWalled = false; // เจอผนัง ?

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f; // ความเร็วตัวละครแนวราบ
    public float X_input; // เก็บค่า keyboard a,d
    public float Y_input; // เก็บค่า keyboard w,s
    public int facing; // หันหน้าตรงข้ามเวลาโดดออกจากกำแพง wallJump

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f; // แรงในการกระโดดขึ้น
    [SerializeField] private Vector2 wallJumpForce = new Vector2(10f, 15f); // แรงในการกระโดด wallJump
    public bool isJumping = false;
    public bool isWallJumping = false;
    public bool isWallSliding = false;
    public bool canDoubleJump = false; // ใชับังคับให้โดด doubleJump ได้แค่ครั้งเดียว

    [SerializeField] private float coyoteTimeLimit = .5f; // ระยะเวลาที่สามารถโดดกลางอากาศได้ถ้ากำลังตก
    [SerializeField] private float bufferTimeLimit = .5f; // ระยะเวลาที่สามารถกดกระโดดก่อนถึงพื้นได้
    public float coyoteTime; // เริ่มจับเวลาสำหรับ coyoteJump
    public float bufferTime; // เริ่มจับเวลาสำหรับ bufferJump

    private void Awake() // method ที่เกิดตอนที่ gameobject นี้โดนสร้างใน game จะมาก่อน Start()
    {
        hitboxCollider = hitboxObject.GetComponent<Collider2D>();
        hitboxCollider.enabled = false;
        rigid = GetComponent<Rigidbody2D>(); // ดึง component จากตัวมันเอง
        anim = GetComponentInChildren<Animator>(); // ดึง component จากลูกที่เป็น sprite
        grassPar = GetComponentInChildren<ParticleSystem>(); // ดึง particle จากลูก
        emission = grassPar.emission; // ดึง emission จาก particle อีกที
    }

    private void Attack()
{
    if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
    {
        StartCoroutine(AttackCoroutine());
    }
}

private System.Collections.IEnumerator AttackCoroutine()
{
    isAttacking = true;
    hitboxCollider.enabled = true;

    float timer = 0f;

    while (timer < attackDuration)
    {
        float dir = transform.localScale.x > 0 ? 1f : -1f;
        hitboxObject.transform.localPosition = new Vector2(dir * 1f, 0);

        timer += Time.deltaTime;
        yield return null; // รอ frame ถัดไป
    }

    hitboxCollider.enabled = false;
    isAttacking = false;
}
    private void Update() // รันทุก frame
    {
        Attack();
        JumpState(); // ตรวจสถานะ takeoff, landing, wallJumping, wallSliding, อื่นๆ
        Jump(); // สั่งให้กระโดด
        WallSlide(); // สั่งให้ wallSlide
        InputVal(); // เก็บ input จาก player
        Move(); // เคลื่อนที่ เน้นแนวราบ/ แกน X ตอนกระโดด
        Flip(); // หันตัวละครไปซ้าย/ขวา
        GroundAndWallCheck(); // ตรวจหาพื้นและผนัง
        Animation(); // สั่งให้ play animation
    }
    private void JumpState() // ตรวจสถานะตัวละคร
    {
        if(!isGrounded && !isJumping) // พึ่งกระโดดขึ้น takeoff อาจกระโดดขึ้น หรือกำลังตก
        {
            isJumping = true;

            if(rigid.linearVelocityY <= 0f) // กำลังตก fall
            {
                coyoteTime = Time.time; // เริ่มการจับเวลา coyote
            }
        }

        if(isGrounded && isJumping) // กำลัง landing
        {
            isJumping = false;
            isWallJumping = false;
            isWallSliding = false;
            canDoubleJump = false;
        }

        if (isWalled) // อยู่ติดผนัง wallSliding
        {
            isJumping = false;
            isWallJumping = false;
            canDoubleJump = false;

            if (isGrounded) // อยู่บนพื้น
            {
                isWallSliding = false;
            }
            else // ไม่อยู่บนพื้น
            {
                isWallSliding = true; // slide ต่อถ้าไม่เจอพื้น
            }
        }
        else // ไม่ติดผนัง
        {
            isWallSliding = false;
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ถ้ากด spacebar
        {
            if (!isWalled) // ถ้าไม่ติดผนัง
            {
                if (isGrounded) // ถ้าอยู่บนพื้น ***normalJump
                {
                    canDoubleJump = true; // ยอมให้โดด doubleJump ได้
                    rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce); // กระโดด
                }
                else // ถ้าไม่อยู่บนพื้น
                {
                    if(rigid.linearVelocityY > 0f && canDoubleJump) // ***doubleJump
                    {
                        canDoubleJump = false; // กันโดดซ้ำ
                        rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce); // กระโดด
                    }

                    if(rigid.linearVelocityY <= 0f) // coyoteJump หรือเริ่มนับ bufferJump
                    {
                        if(Time.time < coyoteTime + coyoteTimeLimit) // *** coyoteJump
                        {
                            coyoteTime = 0f; // reset
                            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce); // กระโดด
                        }
                        else // เริ่มนับ bufferJump
                        {
                            bufferTime = Time.time;
                        }
                    }
                }
            }
            else // ถ้าติดผนัง *** wallJumping
            {
                isWallJumping = true;
                rigid.linearVelocity = new Vector2(wallJumpForce.x * facing, wallJumpForce.y);
            }
        }
        else // ถ้าไม่กด spacebar
        {
            if(isGrounded && Time.time < bufferTime + bufferTimeLimit) // ***bufferJump
            {
                bufferTime = 0f; // reset
                rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce); // กระโดด
            }
        }
    }
    private void WallSlide()
    {
        if (!isWalled || isGrounded || isWallJumping || rigid.linearVelocityY > 0f)
            return; // ไม่อ่านบรรทัดที่เหลือ

        float Y_slide = Y_input < 0f ? 1f : 0.5f; //  ถ้ากด s จะตกเร็ว 1 เท่า ถ้าไม่กดจะตกช้าลงครึ่งนึง
        rigid.linearVelocity = new Vector2(X_input * moveSpeed, rigid.linearVelocityY * Y_slide); // ตกลงช้าๆ
    }
    private void InputVal()
    {
        X_input = Input.GetAxisRaw("Horizontal"); // เก็บค่า keyboard a,d
        Y_input = Input.GetAxisRaw("Vertical");  // เก็บค่า keyboard w,s
    }
    private void Move()
    {
        if (isWallJumping)
            return; // ข้ามบรรทัดที่เหลือ

        if (isGrounded) // ควบคุมตามปรกติ
        {
            rigid.linearVelocity = new Vector2(X_input * moveSpeed, rigid.linearVelocityY); // ผลัก rigid ให้เคลื่อนที่เฉพาะแนวราบ
        }
        else // ถ้าอยู่กลางอากาศ
        {
            // ถ้ากระโดดอยู่ แล้วไม่กด w หรือ d มันเคลื่อนที่ต่อด้วยแรงผลัก
            float X_airMove = X_input != 0f ? X_input * moveSpeed : rigid.linearVelocityX;
            rigid.linearVelocity = new Vector2(X_airMove, rigid.linearVelocityY);
        }
        
    }
    private void Flip()
    {
        if(rigid.linearVelocityX > 0.1f) // ถ้ามันเคลื่อนที่ไปทางขวา
        {
            facing = -1; // หันตรงข้ามกับกำแพง ไว้สำหรับ wallJump
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // หันทางขวา
        }
        if (rigid.linearVelocityX < -0.1f) // ถ้ามันเคลื่อนที่ไปทางซ้าย
        {
            facing = 1; // หันตรงข้ามกับกำแพง ไว้สำหรับ wallJump
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // หันทางซ้าย
        }
    }
    private void GroundAndWallCheck()
    {
        //Physics2D.Raycast(จุดเริ่ม, ทิศ, ความยาว, layerที่จะหา)
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundDistCheck, groundLayer); // sensor ตรวจพื้น
        isWalled = Physics2D.Raycast(transform.position, transform.right, wallDistCheck, groundLayer); // sensor ตรวจผนัง
    }
    private void OnDrawGizmos() // แสดงผล UI หรือ sensor ตรวจพื้นและผนัง
    {
        // Gizmos.DrawLine(จุดเริ่ม, จุดจบ)
        Gizmos.color = Color.blue; // สีน้ำเงิน
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistCheck); // UI ของ ground
        Gizmos.color = Color.red; // สีแดง
        Gizmos.DrawLine(transform.position, transform.position + transform.right * wallDistCheck); // UI ของ wall
    }
    private void Animation()
    {
        anim.SetBool("isGrounded", isGrounded); // เลือกว่าจะ animate ท่า idle/run หรือ jump
        anim.SetBool("isWallSliding", isWallSliding); // เลือกว่าจะ animate ท่า idle/run หรือ jump
        anim.SetFloat("velX", rigid.linearVelocityX); // เลือกว่าจะ animate ท่า idle หรือ run
        anim.SetFloat("velY", rigid.linearVelocityY); // เลือกว่าจะ animate ท่าโดดขึ้น หรือ ลง

        emission.enabled = isGrounded; // ถ้าอยู่้บนพื้นปล่อย particle ได้
    }

     





}
