using UnityEngine;

public class DestroyObstacle : MonoBehaviour
{
    void Update()
    {
        // [최적화 안전장치] 혹시나 바닥을 통과해 화면 밖 아래로 떨어지면 즉시 삭제
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    // 상황 1: 단단한 바닥(Is Trigger가 꺼진 바닥)과 부딪혔을 때
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // 상황 2: 트리거 영역(Is Trigger가 켜진 바닥이나 센서)을 통과했을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}