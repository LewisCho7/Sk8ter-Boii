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

    public int Score { get; private set; } // 점수
    public event Action<int> OnScoreChanged; // 점수 변경 이벤트
    public int BestScore { get; private set; } // 최고 점수
    public GameState State { get; private set; } = GameState.Playing; // 현재 게임 상태

    public event Action<GameState> OnStateChanged; // 게임 상태 변경 이벤트

    public int Stars { get; private set; } // 획득한 별 개수

    private const string BestScoreKey = "BestScore";


    void Awake()
    {
        Instance = this; // 싱글톤 인스턴스 설정(유니티에서 많이 사용되는 싱글톤 기법)
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0); // 최고 점수 불러오기
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

    private void SetState(GameState newState)
    {
        if (State == newState) return; // 현재 상태와 새 상태가 같으면 변경하지 않음
        State = newState; // 게임 상태 변경
        OnStateChanged?.Invoke(State); // 게임 상태 변경 이벤트 호출
    }
}
