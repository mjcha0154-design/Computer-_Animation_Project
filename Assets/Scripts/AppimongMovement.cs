using UnityEngine;

public class AppimongMovement : MonoBehaviour
{
    public float speed = 4f;          // 이동 속도
    private int moveDirection = 1;    // 1이면 오른쪽, -1이면 왼쪽 이동

    // SpawnManager에서 생성할 때 방향을 정해줄 함수입니다.
    public void SetDirection(int direction)
    {
        moveDirection = direction;

        // 방향에 맞춰 캐릭터의 좌우 이미지를 뒤집어줍니다 (선택사항)
        if (moveDirection == -1)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        // 지정된 방향으로 매 프레임 이동합니다.
        transform.Translate(Vector3.right * moveDirection * speed * Time.deltaTime);

        // 화면 밖(X축 기준 좌우로 약 11 이상)으로 나가면 자동으로 소멸합니다.
        if (transform.position.x > 11f || transform.position.x < -11f)
        {
            Destroy(gameObject);
        }
    }
}