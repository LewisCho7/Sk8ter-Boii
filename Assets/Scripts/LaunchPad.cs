using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] private float launchPower = 25f; // 발사력

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player)) 
        {
            player.Launch(launchPower); // 플레이어 발사 처리
        }
    }
}
