using UnityEngine;

public enum RampKind
{
    Kicker,
    QuarterPipe
}

[RequireComponent(typeof(PolygonCollider2D))] // PolygonCollider2D 컴포넌트가 없으면 자동으로 추가
public class RampShape : MonoBehaviour
{
    [SerializeField] private RampKind rampKind; // Ramp 종류
    [SerializeField] private bool mirror; // Ramp 좌우 반전 여부
    [SerializeField] private float length = 5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float radius = 7f;
    [SerializeField] private float topAngle = 45f;
    [SerializeField] private int segments = 12;
    [SerializeField] private SpriteRenderer visual;



    private void OnValidate()
    {
        visual.flipX = mirror; // SpriteRenderer의 flipX 속성 설정
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        Vector2[] points = rampKind == RampKind.Kicker ? BuildKicker() : BuildQuarterPipe();

        if (mirror) // X 좌표 반전해서 ramp 좌우 반전
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].x = -points[i].x;
            }
        }

        poly.SetPath(0, points); // PolygonCollider2D의 경로 설정
    }

    private Vector2[] BuildKicker()
    {
        return new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(length, height),
            new Vector2(length, 0f)
        };
    }

    private Vector2[] BuildQuarterPipe()
    {
        float topRad = topAngle * Mathf.Deg2Rad;
        Vector2[] points = new Vector2[segments + 2];

        for (int i = 0; i <= segments; i++)
        {
            float t = topRad * i / segments;
            points[i] = new Vector2(radius * Mathf.Sin(t), radius * (1f - Mathf.Cos(t)));
        }

        points[segments + 1] = new Vector2(radius * Mathf.Sin(topRad), 0f);
        return points;
    }

    private void OnDrawGizmos()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly == null) return;

        Vector2[] points = poly.GetPath(0);
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 a = transform.TransformPoint(points[i]);
            Vector3 b = transform.TransformPoint(points[(i + 1) % points.Length]);
            Gizmos.DrawLine(a, b);
        }
    }    
}
