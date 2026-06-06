using UnityEngine;
using System.Collections;

public class PatrolSpawnManager : MonoBehaviour
{
    public GameObject appimongPrefab; // Appimong 프리팹을 넣을 칸
    public float spawnY = -3.78f;     // 스크린샷에 찍힌 Appimong의 바닥 Y값 적용

    void Start()
    {
        // 바닥 릴레이 스폰 코루틴 시작
        StartCoroutine(PatrolSpawnLoop());
    }

    IEnumerator PatrolSpawnLoop()
    {
        // 게임 시작 후 첫 등장 전 2초 대기
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // 게임오버 상태라면 생성하지 않고 대기
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield return null;
                continue;
            }

            // 왼쪽 끝(-10) 혹은 오른쪽 끝(10) 무작위 선택
            int startSide = Random.Range(0, 2); // 0 또는 1
            float spawnX = (startSide == 0) ? -10f : 10f;
            int direction = (startSide == 0) ? 1 : -1; // 왼쪽에서 나오면 오른쪽(1)으로, 오른쪽에서 나오면 왼쪽(-1)으로

            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

            // Appimong 생성
            GameObject spawnedAppimong = Instantiate(appimongPrefab, spawnPos, Quaternion.identity);

            // 생성된 Appimong에게 이동 방향 지시하기
            AppimongMovement movement = spawnedAppimong.GetComponent<AppimongMovement>();
            if (movement != null)
            {
                movement.SetDirection(direction);
            }

            // [핵심 릴레이 로직] 생성된 Appimong이 화면 밖으로 나가서 파괴(null)될 때까지 숨참고 기다립니다.
            while (spawnedAppimong != null)
            {
                yield return null; // 파괴될 때까지 다음 프레임 대기
            }

            // 파괴되면 다음 생성을 위해 아주 잠깐(예: 0.5초) 쉬고 루프를 다시 돕니다.
            yield return new WaitForSeconds(0.5f);
        }
    }
}