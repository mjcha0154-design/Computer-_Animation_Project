using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded = true;

    // [2단 점프용 변수 추가]
    private int jumpCount = 0;         // 현재 점프한 횟수
    public int maxJumpCount = 2;       // 최대 가능한 점프 횟수 (2로 설정하면 2단 점프)

    [Header("Animation")]
    public RuntimeAnimatorController player_idle;
    public RuntimeAnimatorController player_run;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;       // 대시 속도
    public float dashTime = 0.2f;        // 대시가 유지되는 시간 (초)
    public float dashCooldown = 1f;    // 대시 재사용 대기시간 (초)

    private bool isDashing = false;      // 현재 대시 중인지 체크
    private bool canDash = true;         // 대시를 사용할 수 있는 상태인지 체크

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 대시 중일 때는 입력을 받거나 애니메이션을 처리하지 않음
        if (isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        // 캐릭터 방향 전환 및 애니메이션 컨트롤러 교체
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            RunAnimation();
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            RunAnimation();
        }
        else if (moveInput == 0)
        {
            // [버그 수정]: 점프 코드가 실행될 수 있도록 함수를 완전히 종료하는 return;을 제거했습니다.
            if (anim.runtimeAnimatorController != player_idle)
            {
                anim.runtimeAnimatorController = player_idle;
            }
        }

        // 달리기 애니메이션 파라미터 연동
        if (anim != null)
        {
            if (moveInput != 0) anim.SetBool("isRunning", true);
            else anim.SetBool("isRunning", false);
        }

        // [2단 점프 로직으로 수정된 점프 기능]
        // 기존 'isGrounded' 조건 대신 'jumpCount < maxJumpCount'를 체크하여 공중에서도 작동하게 합니다.
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++; // 점프할 때마다 카운트 증가
            isGrounded = false;

            if (anim != null) anim.SetBool("isJumping", true); // 점프 시작
        }

        // 방향키를 누르고 있는 상태에서 Z 키를 누르면 대시 발동
        if (canDash && moveInput != 0 && Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Dash(moveInput));
        }
    }

    void FixedUpdate()
    {
        // 대시 중일 때는 FixedUpdate의 기본 이동 물리 연산을 차단하여 충돌/밀림을 방지
        if (isDashing) return;

        // 좌우 이동 기능
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥에 닿았을 때 점프 횟수 초기화 및 애니메이션 종료
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // 바닥에 착지하면 점프 카운트를 0으로 리셋!
            if (anim != null) anim.SetBool("isJumping", false); // 점프 종료
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공을 먹었을 때
        if (collision.CompareTag("Ball"))
        {
            GameManager.Instance.AddScore(1);
            Destroy(collision.gameObject);
        }
        // 하늘에서 떨어지는 가시 장애물에 찔렸을 때
        else if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("가시에 찔림! 게임 오버!");
            GameManager.Instance.GameOver();
        }
    }

    void RunAnimation() // 달리기 애니메이션 처리
    {
        if (moveInput > 0 || moveInput < 0)
        {
            anim.runtimeAnimatorController = player_run;
        }
        else return;
    }

    void JumpAnimation() // 점프 애니메이션 처리
    {
        if (isGrounded == false)
        {
            anim.runtimeAnimatorController = player_idle;
        }
        else return;
    }

    // 대시 처리를 위한 코루틴 함수
    IEnumerator Dash(float direction)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // 대시 중에는 중력을 0으로 만들어 직선 대시 구현

        // 입력받은 방향으로 순간적인 속도 부여
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        // 대시가 끝나면 속도를 정지하고 중력 복구
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}