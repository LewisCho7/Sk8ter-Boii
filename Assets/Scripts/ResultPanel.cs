using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // 게임 오버 패널
    [SerializeField] private Text gameOverScoreText; // 점수 표시 UI 텍스트
    [SerializeField] private GameObject clearPanel; // 게임 클리어 패널
    [SerializeField] private Text clearScoreText; // 점수 표시 UI 텍스트
    [SerializeField] private Text bestScoreText; // 최고 점수 표시 UI 텍스트
    [SerializeField] private Image[] starsImages; // 획득한 별 개수 표시 UI 이미지
    private void OnEnable()
    {
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged; // 게임 종료 이벤트 구독
        }
    }

    private void HandleStateChanged(GameManager.GameState state)
    {
        gameOverPanel.SetActive(state == GameManager.GameState.GameOver); // state가 GameOver일 때 게임 오버 패널 활성화
        clearPanel.SetActive(state == GameManager.GameState.Clear); // state가 Clear일 때 게임 클리어 패널 활성화
        GameManager gameManager = GameManager.Instance; 

        if (state == GameManager.GameState.GameOver)
        {
            gameOverScoreText.text = "Score: " + gameManager.Score; // 게임 오버 시 점수만 표시
        }
        else if (state == GameManager.GameState.Clear) // 게임 클리어 시 점수, 최고 점수, 별 표시
        {
            clearScoreText.text = "Score: " + gameManager.Score; 
            bestScoreText.text = "Best Score: " + gameManager.BestScore;
            for (int i = 0; i < starsImages.Length; i++)
                starsImages[i].enabled = i < gameManager.Stars;
        }
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged; // 게임 종료 이벤트 구독 해제
        }
    }
}
