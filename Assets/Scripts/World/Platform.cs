using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class Platform : MonoBehaviour
{
    [Header("Detección de límites")]
    public bool useChildrenColliders = true;
    public Vector2 padding = new Vector2(0.5f, 0.5f);

    [Header("Límites calculados (solo lectura)")]
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 0f;
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 0f;

    bool calculated = false;

    void Awake()
    {
        CalculateBounds();
    }

    void OnValidate()
    {
        // en editor, recalcula cuando cambian propiedades
        CalculateBounds();
    }

    [ContextMenu("Recalcular bounds")]
    public void CalculateBounds()
    {
        Collider2D[] cols;

        if (useChildrenColliders)
        {
            // toma todos los colliders hijos (incluye el propio si existe)
            cols = GetComponentsInChildren<Collider2D>(includeInactive: false);
        }
        else
        {
            var c = GetComponent<Collider2D>();
            cols = c != null ? new Collider2D[] { c } : new Collider2D[0];
        }

        if (cols == null || cols.Length == 0)
        {
            // sin colliders: deja valores por defecto y marca calculado
            minX = maxX = minY = maxY = 0f;
            calculated = true;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            return;
        }

        Bounds b = cols[0].bounds;
        for (int i = 1; i < cols.Length; i++)
            b.Encapsulate(cols[i].bounds);

        minX = b.min.x - padding.x;
        maxX = b.max.x + padding.x;
        minY = b.min.y - padding.y;
        maxY = b.max.y + padding.y;

        calculated = true;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    // getters públicos
    public float GetMinX() { if (!calculated) CalculateBounds(); return minX; }
    public float GetMaxX() { if (!calculated) CalculateBounds(); return maxX; }
    public float GetMinY() { if (!calculated) CalculateBounds(); return minY; }
    public float GetMaxY() { if (!calculated) CalculateBounds(); return maxY; }

    void OnDrawGizmosSelected()
    {
        if (!calculated) CalculateBounds();
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.25f);
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, transform.position.z);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), 0.1f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }
}
