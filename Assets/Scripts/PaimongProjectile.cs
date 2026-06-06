using UnityEngine;

public class PaimongProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveDirection = 1f; // 1이면 오른쪽, -1이면 왼쪽으로 이동

    public void Setup(float direction, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = direction;

        // 파이몽이 날아가는 방향에 맞춰 이미지 좌우 반전 처리
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Paimong 이미지가 기본적으로 왼쪽을 보고 있다면 (moveDirection > 0일 때 flipX를 켭니다)
            // 반대로 오른쪽을 보고 있다면 (moveDirection < 0일 때 flipX를 켭니다)
            // 게임 시뮬레이션을 해보고 방향이 어색하면 true/false 기준을 바꾸시면 됩니다.
            spriteRenderer.flipX = (moveDirection > 0);
        }

        // 중력의 영향을 받지 않고 수평으로 일직선 비행하도록 설정
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(moveDirection * speed, 0f);
    }

    private void Update()
    {
        // 화면 바깥으로 멀리 나가면 메모리 관리를 위해 스스로 삭제 (X축 -15 ~ 15 범위를 벗어날 때)
        if (Mathf.Abs(transform.position.x) > 15f)
        {
            Destroy(gameObject);
        }
    }
}