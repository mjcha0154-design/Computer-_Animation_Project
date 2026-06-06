using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // 1단계에서 만든 일시정지 텍스트 오브젝트를 연결할 칸입니다.
    public GameObject pauseTextObject;

    // 현재 게임이 멈췄는지 기억하는 변수
    private bool isPaused = false;

    void Update()
    {
        // 키보드의 'E' 키를 누르는 순간을 감지합니다.
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPaused)
            {
                ResumeGame(); // 이미 멈춰있었다면 게임 재개
            }
            else
            {
                PauseGame();  // 진행 중이었다면 게임 일시정지
            }
        }
    }

    // 게임을 멈추는 함수
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 유니티의 시간 흐름을 0으로 만들어 게임을 멈춥니다.

        // 일시정지 글씨가 화면에 보이도록 켭니다.
        if (pauseTextObject != null)
        {
            pauseTextObject.SetActive(true);
        }

        Debug.Log("게임 일시정지 ('E' 키 누름)");
    }

    // 게임을 다시 시작하는 함수
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 유니티의 시간 흐름을 정상(1배속)으로 돌립니다.

        // 일시정지 글씨를 다시 화면에서 숨깁니다.
        if (pauseTextObject != null)
        {
            pauseTextObject.SetActive(false);
        }

        Debug.Log("게임 재개 ('E' 키 다시 누름)");
    }

    // 게임을 끈 상태에서 다른 씬으로 넘어가거나 재시작할 때 
    // 시간이 멈춰있는 버그를 방지하기 위해 안전장치를 둡니다.
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}