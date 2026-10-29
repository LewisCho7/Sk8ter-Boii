using UnityEngine;

public class GoalFlag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player)) // 충돌한 객체가 Player 태그를 가진 경우
        {
            player.Finish(); // 플레이어 게임 클리어 처리
            GameManager.Instance.Clear(); // 게임 클리어 처리
        }
    }
}
