using System;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] CapColor ringColor; // ring 색깔 enum

    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D ringCollider;
    private ParticleSystem ringParticleSystem;
    private RingGroup ringGroup; // ring이 속한 ring group

    private bool judged = false; // 플레이어가 ring을 통과했는지 여부

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = CapType.GetColor(ringColor); // ring 색깔에 따라 색상 변경하기
        ringCollider = GetComponent<CapsuleCollider2D>();
        ringParticleSystem = GetComponentInChildren<ParticleSystem>();
        ringGroup = GetComponentInParent<RingGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        
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
            if(ringGroup != null)
                ringGroup.OnRingCollected(); // RingGroup에 통과한 ring을 알림 -> 점수 추가
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
