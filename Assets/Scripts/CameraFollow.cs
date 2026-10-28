using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 플레이어 따라가는 카메라
    [SerializeField] private float smoothTimeY = 0.25f; // 카메라 부드러운 점프 이동
    [SerializeField] private float followStartHeight = 1.5f; // 카메라가 플레이어를 따라가기 시작하는 높이
    [SerializeField] private float followRatio = 0.4f; // 카메라가 플레이어를 따라가는 비율

    private Vector2 offset; // 카메라와 플레이어 사이의 거리
    private float yVelocity; 
    private PlayerController player; // 플레이어 컨트롤러
    private float currentGroundY; // 플레이어가 마지막으로 땅에 닿았을 때의 Y좌표

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = target.GetComponent<PlayerController>(); // 플레이어 컨트롤러 가져오기
        offset = transform.position - target.position; // 카메라와 플레이어 사이의 거리 벡터 계산
        currentGroundY = target.position.y;
    }

    void LateUpdate()
    {
        if (player.IsGrounded)
        {
            currentGroundY = target.position.y; // 플레이어가 땅에 닿아있으면 현재 Y좌표를 저장
        }
        float height = target.position.y - currentGroundY; // 플레이어가 얼마나 높이 올라갔는지 계산
        float extra = Mathf.Max(0, height - followStartHeight) * followRatio; // 플레이어가 followStartHeight 이상 올라갔을 때만 extra 계산

        float x = target.position.x + offset.x; // 카메라 X좌표는 플레이어 X좌표 + offset.x
        float y = Mathf.SmoothDamp(transform.position.y, currentGroundY + offset.y + extra, ref yVelocity, smoothTimeY); // 카메라 Y좌표는 SmoothDamp를 이용하여 부드럽게 이동

        transform.position = new Vector3(x, y, transform.position.z); // 카메라 위치를 업데이트
    }
}
