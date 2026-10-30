using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private ScorePopup popupPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(Vector3 worldPos, string text, Color color, float scale = 1f) // popup prefab 생성
    {
        ScorePopup popup = Instantiate(popupPrefab, worldPos, Quaternion.identity);
        popup.Play(text, color, scale);
    }
}