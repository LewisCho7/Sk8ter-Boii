using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private Text label; // 점수 텍스트
    [SerializeField] private Outline outline; // 외곽선으로 강조
    [SerializeField] private float lifetime = 0.9f; // 점수 팝업이 화면에 표시되는 시간
    [SerializeField] private float riseDistance = 1.2f; // 점수 팝업이 위로 올라가는 거리

    private Vector3 startPos;
    private Color textColor;
    private Color outlineColor;
    private float elapsed;
    private float baseScale;
    private float sizeScale = 1f;

    private void Awake()
    {
        baseScale = transform.localScale.x;
        startPos = transform.position;
    }

    public void Play(string text, Color color, float scale)
    {
        label.text = text;
        textColor = color;
        outlineColor = color.grayscale > 0.2f ? Color.black : Color.white;
        sizeScale = scale;
    }

    private void Update()
    {
        // 점수 팝업이 화면에 표시되는 시간 동안 위치, 크기, 투명도 업데이트 수학 부분은 AI 도움 받음
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        float eased = 1f - (1f - t) * (1f - t);
        transform.position = startPos + Vector3.up * (riseDistance * eased);

        float pop = 1f + 0.3f * (1f - Mathf.Clamp01(t / 0.2f));
        transform.localScale = Vector3.one * (baseScale * sizeScale * pop);

        float alpha = 1f - Mathf.InverseLerp(0.6f, 1f, t);
        label.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
        outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alpha);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}