using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WireHandle : MonoBehaviour
{
    public int wireIndex;
    public Color wireColor;

    WireManager manager;
    BezierWire line;
    Camera cam;
    bool locked;
    bool isHeld;

    public void Init(WireManager m, int index, Color color, BezierWire bezier)
    {
        manager = m;
        wireIndex = index;
        wireColor = color;
        line = bezier;
        cam = m.cam;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = color;

        line.SetColor(color);
        line.Follow(transform);
        line.SetLiveEnd(transform.position); // evita linha solta
    }

    // Chamados pelo WireManager (input centralizado)
    public void BeginDrag()
    {
        if (locked) return;
        isHeld = true;
        line.Follow(transform);
        line.SetLiveEnd(transform.position);
        Debug.Log($"[WireHandle] BeginDrag wire={wireIndex}");
    }

    public void DragTo(Vector3 worldPos)
    {
        if (!isHeld || locked) return;
        line.SetLiveEnd(worldPos);
    }

    public void EndDrag()
    {
        isHeld = false;
        Debug.Log($"[WireHandle] EndDrag wire={wireIndex}");
    }

    public void Lock()   { locked = true;  }
    public void Unlock() { locked = false; }
}
