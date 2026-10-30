using UnityEngine;

public class RingGroup : MonoBehaviour
{
    [SerializeField] private int ringScore = 100; // ring 점수
    [SerializeField] private int bonusScore = 300; // 모든 ring 통과 시 보너스 점수
    public int MaxScore => ringScore * totalRings + bonusScore; // 링 그룹에서 얻을 수 있는 최대 점수 계산 => 는 읽기 전용 프로퍼티
    private int totalRings; // Ring 개수
    private int collectedRings; // 통과한 Ring 개수
    void Awake()
    {
        totalRings = GetComponentsInChildren<Ring>().Length; 
    }

    public void OnRingCollected(Vector3 position)
    {
        collectedRings++;
        GameManager.Instance.AddScore(ringScore); // 점수 추가
        PopupManager.Instance.Show(position + new Vector3(2f, 0.5f, 0f), "+" + ringScore, Color.white); // 점수 팝업 표시
        if (collectedRings >= totalRings)
        {
            GameManager.Instance.AddScore(bonusScore); // 보너스 점수 추가
            PopupManager.Instance.Show(position + Vector3.up * 1.2f, "+" + bonusScore, new Color(1f, 0.85f, 0.2f), 1.4f); // 보너스 점수 팝업 표시
        }
    }
    
}
