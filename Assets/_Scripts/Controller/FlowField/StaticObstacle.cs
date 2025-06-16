using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class StaticObstacle : MonoBehaviour
{
    [Inject] private GridManager _gridManager;

    private Collider _collider;
    private List<GridCell> occupiedCells = new List<GridCell>();

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void RegisterObstacle()
    {
        Bounds bounds = _collider.bounds;

        int minX = Mathf.FloorToInt((bounds.min.x - _gridManager.CellSize / 2) / _gridManager.CellSize);
        int maxX = Mathf.FloorToInt((bounds.max.x + _gridManager.CellSize / 2) / _gridManager.CellSize);
        int minY = Mathf.FloorToInt((bounds.min.z - _gridManager.CellSize / 2) / _gridManager.CellSize);
        int maxY = Mathf.FloorToInt((bounds.max.z + _gridManager.CellSize / 2) / _gridManager.CellSize);

        for (int gx = minX; gx <= maxX; gx++)
        {
            for (int gy = minY; gy <= maxY; gy++)
            {
                var cell = _gridManager.GetCell(gx, gy);
                if (cell != null)
                {
                    Vector3 center = cell.worldPosition;
                    Vector3 halfExtents = Vector3.one * _gridManager.CellSize * 0.4f;

                    if (_collider.bounds.Contains(center) &&
                        Physics.CheckBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Obstacle"), QueryTriggerInteraction.Ignore))
                    {
                        cell.walkable = false;
                        cell.cost = float.MaxValue;
                        occupiedCells.Add(cell);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (occupiedCells == null) return;

        Gizmos.color = Color.red;
        foreach (var cell in occupiedCells)
        {
            Gizmos.DrawCube(cell.worldPosition, new Vector3(_gridManager.CellSize * 0.9f, 0.05f, _gridManager.CellSize * 0.9f));
        }
    }
}
