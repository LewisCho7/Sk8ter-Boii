using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText; // 점수 표시 UI 텍스트

    void OnEnable()
    {
        GameManager.Instance.OnScoreChanged += UpdateScoreText; // 점수 변경 이벤트 구독
    }
    void Start()
    {
        UpdateScoreText(GameManager.Instance.Score); // 초기 점수 표시
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString(); // 점수 표시 업데이트
        Debug.Log("Score updated: " + score); // 점수 변경 로그 출력
    }

    void OnDisable()
    {
        GameManager.Instance.OnScoreChanged -= UpdateScoreText; // 점수 변경 이벤트 구독 해제
    }
}
