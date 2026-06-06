using System.Collections;
using UnityEngine;

public class PaimongSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject paimongPrefab;      // 생성할 Paimong 프리팹
    public Transform playerTransform;     // 플레이어의 위치 (Y축 추적용)

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;      // 발사 간격 (초)
    public float paimongSpeed = 7f;       // 날아오는 속도
    public float spawnXOffset = 11f;      // 파이몽이 스폰될 좌우 벽의 X 좌표 거리

    void Start()
    {
        // 플레이어 오브젝트를 자동으로 찾아서 세팅
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // 파이몽 발사 루프 코루틴 시작
        StartCoroutine(SpawnPaimongRoutine());
    }

    IEnumerator SpawnPaimongRoutine()
    {
        // GameManager가 존재하고 게임오버 상태가 아닐 때만 작동
        while (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (playerTransform != null)
            {
                SpawnPaimong();
            }
        }
    }

    void SpawnPaimong()
    {
        // 1. 왼쪽 벽에서 쏠지, 오른쪽 벽에서 쏠지 무작위로 결정 (0이면 왼쪽, 1이면 오른쪽)
        bool isLeftSpawn = Random.Range(0, 2) == 0;

        // 2. 스폰될 X축 위치 계산 및 방향 설정
        float spawnX = isLeftSpawn ? -spawnXOffset : spawnXOffset;
        float moveDirection = isLeftSpawn ? 1f : -1f; // 왼쪽에서 스폰되면 오른쪽(1)으로, 오른쪽에서 스폰되면 왼쪽(-1)으로

        // 3. 현재 플레이어의 Y축 높이를 실시간으로 획득
        float spawnY = playerTransform.position.y;

        // 4. 최종 스폰 위치 지정 후 파이몽 생성
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        GameObject newPaimong = Instantiate(paimongPrefab, spawnPosition, Quaternion.identity);

        // 5. 파이몽 스크립트에 방향과 속도를 주입하여 발사시킴
        PaimongProjectile projectileScript = newPaimong.GetComponent<PaimongProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Setup(moveDirection, paimongSpeed);
        }
    }
}