using System;
using UnityEngine;

[DefaultExecutionOrder(-50)] // GameManager가 다른 스크립트보다 먼저 실행되도록 설정 <- Null 예외 발생 안나게 함. 중요
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 싱글톤 인스턴스

    public int Score { get; private set; } // 점수
    public event Action<int> OnScoreChanged; // 점수 변경 이벤트
    public void AddScore(int amount)
    {
        Score += amount; // 점수 추가
        OnScoreChanged?.Invoke(Score); // 점수 변경 이벤트 호출
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 싱글톤 인스턴스 설정(유니티에서 많이 사용되는 싱글톤 기법)
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 자신을 파괴
        }
    }
    void Start()
    {
        Score = 0; // 초기 점수 설정
    }

}
