using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] CapColor ringColor; // ring 색깔 enum
    [SerializeField] private float bobAmount = 0.08f; // ring이 위아래로 움직이는 정도
    [SerializeField] private float bobSpeed = 2f; // ring이 위아래로 움직이는 속도
    [SerializeField] private float glowAmount = 0.4f; // ring이 빛나는 정도
    [SerializeField] private AudioClip ringClip;


    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D ringCollider;
    private ParticleSystem ringParticleSystem;
    private RingGroup ringGroup; // ring이 속한 ring group
    private Vector3 startPos; // ring의 초기 위치
    private float phase; // ring이 위아래로 움직이는 위상
    private Color baseColor; // ring의 기본 색상

    private bool judged = false; // 플레이어가 ring을 통과했는지 여부

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = CapType.GetColor(ringColor); // ring 색깔에 따라 색상 변경하기
        ringCollider = GetComponent<CapsuleCollider2D>();
        ringParticleSystem = GetComponentInChildren<ParticleSystem>();
        ringGroup = GetComponentInParent<RingGroup>();

        startPos = transform.localPosition; // ring의 초기 위치 저장
        phase = Random.value * Mathf.PI * 2f; // 위상 랜덤 초기화
        baseColor = CapType.GetColor(ringColor); // ring 색깔에 따라 기본 색상 저장

        Color fxColor = Color.Lerp(baseColor, Color.white, 0.35f); // ring 색깔과 흰색 사이
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.startColor = fxColor; // 파티클 색깔 변경
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!spriteRenderer.enabled) return; // ring sprite가 비활성화되어 있으면 Update를 실행하지 않음

        float s = Mathf.Sin(Time.time * bobSpeed + phase); // 위아래로 움직이는 정도 계산
        transform.localPosition = startPos + Vector3.up * s * bobAmount; // ring 위치 업데이트

        float t = (Mathf.Sin(Time.time * bobSpeed * 1.5f + phase) * 0.5f + 0.5f) * glowAmount; // 빛나는 정도 계산
        spriteRenderer.color = Color.Lerp(baseColor, Color.white, t); // ring 색깔과 흰색 사이를 보간하여 빛나는 효과

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(judged) return; // 이미 통과했으면 return
        if(!collision.CompareTag("Player")) return; // 플레이어가 아니면 return

        Vector3 center = collision.bounds.center; // 플레이어의 중심 좌표
        
        if(center.x < ringCollider.bounds.center.x) return;
        judged = true; // 플레이어가 ring을 통과했음

        // 플레이어가 ring의 높이 안에 있는지 확인
        bool insideHeight = Mathf.Abs(center.y - ringCollider.bounds.center.y) <= ringCollider.bounds.extents.y; 
        
        // tryGetComponent를 사용하여 PlayerController를 가져옴. null 검사를 따로 하지 않아도 되므로 메모리 측면에서 효율적.
        // 플레이어가 ring의 높이 안에 있고, 모자 색깔이 ring 색깔과 같으면
        if(insideHeight && collision.TryGetComponent(out PlayerController player) && player.CurrentCap == ringColor) 
        {
            spriteRenderer.enabled = false; // ring sprite 비활성화
            ringCollider.enabled = false; // ring collider 비활성화
            ringParticleSystem.Play(); // 파티클 재생
            SoundManager.Instance.Play(ringClip); // ring 통과 효과음 재생
            if(ringGroup != null)
                ringGroup.OnRingCollected(transform.position); // RingGroup에 통과한 ring을 알림 -> 점수 추가
        }
    }

    private void OnValidate() // 에디터에서 값이 변경될 때마다 호출되는 메서드. 
    {
        //enum 값이 변경될 때마다 ring 색깔에 따라 색상 변경
        if (TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = CapType.GetColor(ringColor);
        }
    }

    private void OnDrawGizmos() // 에디터에서 ring collider를 시각화하기 위해 사용(판정 범위)
    {   
        if (TryGetComponent(out Collider2D c))
        {
            Gizmos.color = CapType.GetColor(ringColor);
            Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
        }
    }
}
