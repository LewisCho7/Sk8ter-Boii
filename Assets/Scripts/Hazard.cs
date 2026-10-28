using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private CrashReason reason; // 충돌 원인

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            player.Crash(reason); // 플레이어 충돌 처리
        }
    }
}
