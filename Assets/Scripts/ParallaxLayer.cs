using UnityEngine;

[DefaultExecutionOrder(100)] // CameraFollow보다 늦게 실행되도록 설정
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float factorX;
    [SerializeField] private float factorY = 1f;

    private Transform cam;
    private Vector3 startPos;
    private Vector3 camStartPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform; // 메인 카메라 Transform 가져오기
        startPos = transform.position; // ParallaxLayer의 초기 위치 저장
        camStartPos = cam.position; // 카메라의 초기 위치 저장
    }

    void LateUpdate()
    {
        Vector3 camDelta = cam.position - camStartPos; // 카메라 이동량 계산
        transform.position = new Vector3(startPos.x + camDelta.x *(1 - factorX), startPos.y + camDelta.y *(1 - factorY), startPos.z); // ParallaxLayer 위치 업데이트
    }
}
