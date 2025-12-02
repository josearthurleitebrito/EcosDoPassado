using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierWire : MonoBehaviour
{
    LineRenderer lr;

    Transform a, b;          // pontos inicial e final
    Vector3 liveEnd;         // ponta viva quando arrastando
    bool followMouse;

    [Range(6, 40)] public int segments = 24;
    public float curve = 1.2f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null) lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = segments + 1;
    }

    public void SetColor(Color c)
    {
        lr.startColor = c;
        lr.endColor = c;
    }

    public void SetStaticPoints(Transform start, Transform end)
    {
        a = start; b = end; followMouse = false;
        liveEnd = end ? end.position : start.position;
        UpdateCurve();
    }

    public void Follow(Transform start)
    {
        a = start; followMouse = true;
        liveEnd = start.position;
        UpdateCurve();
    }

    public void SetLiveEnd(Vector3 worldPos)
    {
        liveEnd = worldPos;
        UpdateCurve();
    }

    public void SnapTo(Transform start, Transform end)
    {
        a = start; b = end; followMouse = false;
        liveEnd = end.position;
        UpdateCurve();
    }

    void Update()
    {
        if (followMouse) UpdateCurve();
    }

    void UpdateCurve()
    {
        if (!a) return;

        Vector3 p0 = a.position;
        Vector3 p3 = followMouse ? liveEnd : (b ? b.position : liveEnd);

        // control points simples para um arco agradável
        Vector3 p1 = p0 + (Vector3.right * 1.2f + Vector3.up * 0.5f) * curve;
        Vector3 p2 = p3 + (Vector3.left  * 1.2f - Vector3.up * 0.5f) * curve;

        int count = Mathf.Max(2, segments) + 1;
        if (lr.positionCount != count) lr.positionCount = count;

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            Vector3 pt =
                Mathf.Pow(1 - t, 3) * p0 +
                3 * Mathf.Pow(1 - t, 2) * t * p1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                Mathf.Pow(t, 3) * p3;

            lr.SetPosition(i, pt);
        }
    }
}
