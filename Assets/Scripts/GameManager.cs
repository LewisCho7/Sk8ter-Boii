using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-50)] // GameManager가 다른 스크립트보다 먼저 실행되도록 설정 <- Null 예외 발생 안나게 함. 중요
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 싱글톤 인스턴스
    public enum GameState // 게임 상태 enum
    {
        Playing,
        GameOver,
        Clear
    }
    [SerializeField] private float star2Ratio = 0.5f; // 별 2개 기준 점수 비율
    [SerializeField] private float star3Ratio = 0.9f; // 별 3개 기준 점수 비율

    public int Score { get; private set; } // 점수
    public event Action<int> OnScoreChanged; // 점수 변경 이벤트
    public int BestScore { get; private set; } // 최고 점수
    public GameState State { get; private set; } = GameState.Playing; // 현재 게임 상태

    public event Action<GameState> OnStateChanged; // 게임 상태 변경 이벤트

    public int Stars { get; private set; } // 획득한 별 개수

    private string bestScoreKey;


    void Awake()
    {
        Instance = this; // 싱글톤 인스턴스 설정(유니티에서 많이 사용되는 싱글톤 기법)
        bestScoreKey = "BestScore_" + SceneManager.GetActiveScene().buildIndex; // 현재 씬의 빌드 인덱스를 기반으로 최고 점수 키 생성
        BestScore = PlayerPrefs.GetInt(bestScoreKey, 0); // 최고 점수 불러오기, 없으면 0으로 초기화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToTitle(); // Escape 키를 누르면 타이틀 화면으로 이동
        }
    }

    public void AddScore(int amount)
    {
        if (State != GameState.Playing) return; // 게임 상태가 Playing이 아닐 경우 점수 추가하지 않음
        Score += amount; // 점수 추가
        OnScoreChanged?.Invoke(Score); // 점수 변경 이벤트 호출
    }

    public void GameOver()
    {
       if (State != GameState.Playing) return; // 게임 상태가 Playing이 아닐 경우 GameOver 처리하지 않음
        SetState(GameState.GameOver); // 게임 상태를 GameOver로 변경
    }

    public void Retry()
    {
        Time.timeScale = 1f; // 시간 스케일을 1로 설정하여 게임 속도를 정상으로 되돌림
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 재시작
    }

    public void Clear()
    {
        if (State != GameState.Playing) return; // 게임 상태가 Playing이 아닐 경우 클리어 처리하지 않음
        Stars = CalculateStars(); // 획득한 별 개수 계산
        if (Score > BestScore) // 현재 점수가 최고 점수보다 높으면
        {
            BestScore = Score; // 최고 점수 갱신
            PlayerPrefs.SetInt(bestScoreKey, BestScore); // 최고 점수 저장
        }
        SetState(GameState.Clear); // 게임 상태를 Clear로 변경
    }
    private int CalculateStars()
    {
        // 씬에 존재하는 모든 RingGroup을 찾음. 클리어 순간에만 한번 호출(update에서 호출하면 성능에 영향이 있을 수 있음)
        RingGroup[] ringGroups = FindObjectsByType<RingGroup>(FindObjectsSortMode.None); 

        int maxScore = 0;
        foreach (var ringGroup in ringGroups)
        {
            maxScore += ringGroup.MaxScore; // 각 RingGroup의 최대 점수를 합산
        }

        if (maxScore <= 0) return 1; 

        float ratio = (float)Score / maxScore; // 현재 점수와 최대 점수의 비율 계산
        if (ratio >= star3Ratio) return 3; // 별 3개 기준 비율 이상이면 별 3개
        if (ratio >= star2Ratio) return 2; // 별 2개 기준 비율 이상이면 별 2개
        return 1; // 그 외에는 별 1개
    }

    private void SetState(GameState newState)
    {
        if (State == newState) return; // 현재 상태와 새 상태가 같으면 변경하지 않음
        State = newState; // 게임 상태 변경
        OnStateChanged?.Invoke(State); // 게임 상태 변경 이벤트 호출
    }

    public void NextStage() // 다음 스테이지로 이동
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            next = 0;
        }
        SceneManager.LoadScene(next);
    }

    public void GoToTitle() // 타이틀 화면으로 이동
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
