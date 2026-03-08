using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("기본 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("바닥 체크 설정")]
    public LayerMask whatIsGround;     
    public Transform groundCheck;      
    public float checkRadius = 0.2f;   
    public bool canMove = true;        
    
    [Header("무기 시스템 연결")]
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SPUM_Prefabs spum;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spum = GetComponent<SPUM_Prefabs>();
        
        // SPUM 초기화
        spum.PopulateAnimationLists();
        spum.OverrideControllerInit();
    }

    void Update()
    {
        // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = new Vector2(input.x, 0);

        if (moveInput.x > 0) 
        {
            transform.localScale = new Vector3(-2, 2, 2); 
            spum.PlayAnimation(PlayerState.MOVE, 0);    
        }
        else if (moveInput.x < 0) 
        {
            transform.localScale = new Vector3(2, 2, 2);
            spum.PlayAnimation(PlayerState.MOVE, 0);    
        }
        else 
        {
            spum.PlayAnimation(PlayerState.IDLE, 0);
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ★ 공격 입력 처리 (Input System에 'Attack' 액션이 있어야 합니다!)
    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            // 1. 공격 애니메이션 재생 (SPUM)
            // 0번은 보통 기본 공격입니다. 무기마다 다르게 하고 싶다면 나중에 변수로 빼면 됩니다.
            spum.PlayAnimation(PlayerState.ATTACK, 0); 

           
        }
    }
}