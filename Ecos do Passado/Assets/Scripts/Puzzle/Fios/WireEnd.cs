using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WireEnd : MonoBehaviour
{
    public int wireIndex;   // índice correto para este end
    public Color wireColor;
    public bool isOccupied;

    public void Init(WireManager m, int index, Color color)
    {
        wireIndex = index;
        wireColor = color;
        isOccupied = false;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = color;
    }
}
