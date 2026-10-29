using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RailGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(1f, 0.9f, 0f, 0.35f);
        Gizmos.DrawCube(box.offset, box.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(box.offset, box.size);
    }
}