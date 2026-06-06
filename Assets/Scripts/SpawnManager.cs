using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public GameObject ballPrefab;      // 공 프리팹을 넣을 칸
    public GameObject obstaclePrefab;  // 새로 만든 가시 장애물 프리팹을 넣을 칸 (FallingObstacle)
    public GameObject obstacle1Prefab; // FallingObstacle1 프리팹을 넣을 칸

    public float xRange = 8f;          // 하늘에서 떨어질 좌우 범위

    private bool isLastSpawnBall = false; // 직전에 공을 스폰했는지 기억하는 변수

    void Start()
    {
        // 물체의 위치를 실시간으로 체크하는 코루틴을 시작합니다.
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // 게임 시작 후 첫 물체가 나오기 전 1초 대기
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // GameManager가 있고, 게임오버 상태라면 더 이상 생성하지 않고 대기합니다.
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield return null;
                continue;
            }

            // 무작위 X 좌표 설정 및 하늘 높이(Y=5.7) 위치 잡기
            float randomX = Random.Range(-xRange, xRange);
            Vector3 spawnPos = new Vector3(randomX, 5.7f, 0f);

            GameObject spawnedObject = null;

            // [완벽히 수정된 확률 및 연속 스폰 방지 로직]
            if (isLastSpawnBall)
            {
                // 1. 직전에 공을 떨어뜨렸다면 무조건 장애물만 스폰 (공 연속 스폰 절대 불가)
                // 가시 장애물과 FallingObstacle1을 50% 반반 확률로 스폰합니다.
                int obstacleChoice = Random.Range(0, 2);

                if (obstacleChoice == 0)
                {
                    spawnedObject = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                }
                else
                {
                    spawnedObject = Instantiate(obstacle1Prefab, spawnPos, Quaternion.identity);
                }

                isLastSpawnBall = false; // 이번엔 장애물이 나왔으므로 false로 변경
            }
            else
            {
                // 2. 직전에 장애물이었다면, 정확히 35% 확률로 공을 스폰합니다.
                // Random.value는 0.0부터 1.0 사이의 소수를 무작위로 뽑습니다.
                float chance = Random.value;

                if (chance < 0.35f) // 0.0 ~ 0.35 미만 일 때 (정확히 35% 확률)
                {
                    spawnedObject = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
                    isLastSpawnBall = true; // 이번에 공을 뽑았으므로 true로 변경
                }
                else if (chance < 0.675f) // 0.35 ~ 0.675 미만 일 때 (약 32.5% 확률)
                {
                    spawnedObject = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                    isLastSpawnBall = false;
                }
                else // 0.675 ~ 1.0 이하 일 때 (약 32.5% 확률)
                {
                    spawnedObject = Instantiate(obstacle1Prefab, spawnPos, Quaternion.identity);
                    isLastSpawnBall = false;
                }
            }

            // Y=5.7에서 출발해 Y=1.7까지 내려올 때까지만 기다립니다.
            while (spawnedObject != null && spawnedObject.transform.position.y > 1.7f)
            {
                yield return null; // 다음 프레임까지 실시간 감시 대기
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ground"))
            return;

        Destroy(ballPrefab);
    }
}