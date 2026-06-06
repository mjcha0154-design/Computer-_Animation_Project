using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public bool isGameOver = false;

    [Header("UI Text References")]
    public TextMeshProUGUI scoreText; // 우측 상단 점수판 (그대로 유지)
    public TextMeshProUGUI endText;   // ★ 화면 정중앙 엔딩 텍스트 (새로 추가)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();

        // 게임 시작 시 정중앙 엔딩 문구는 깨끗하게 비워둡니다.
        if (endText != null)
        {
            endText.text = "";
        }
    }

    void Update()
    {
        // ★ ESC 키를 누르면 게임 종료 (추가됨)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("게임 종료 키(ESC) 입력됨");
            Application.Quit();
        }

        // 게임 오버 상태일 때 R키를 누르면 재시작
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateScoreUI(); // 점수판 갱신 (오른쪽 위는 가만히 유지됨)

        // 목표 점수인 10점에 도달했는지 확인합니다.
        if (score >= 10)
        {
            GameClear();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // 10점 달성 시 정중앙에 SUCCESS 팝업
    void GameClear()
    {
        isGameOver = true;
        if (endText != null)
        {
            endText.text = "SUCCESS!\n(Press R)"; // 정중앙 텍스트에 문구 출력!
        }
        Debug.Log("10점 달성! 게임 클리어!");
        Time.timeScale = 0f;
    }

    // 가시에 맞았을 때 정중앙에 GAME OVER 팝업
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        if (endText != null)
        {
            endText.text = "GAME OVER\n(Press R)"; // 정중앙 텍스트에 문구 출력!
        }
        Debug.Log("게임 오버!");
        Time.timeScale = 0f;
    }
}