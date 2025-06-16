using UnityEngine;

public class GridCell
{
    public int x;
    public int y;
    public Vector3 worldPosition;

    public bool walkable = true;
    public float cost = 1f;

    public float integrationValue = float.MaxValue;
    public Vector2 flowDirection = Vector2.zero;

    public GridCell(int x, int y, Vector3 worldPosition)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPosition;
    }
}