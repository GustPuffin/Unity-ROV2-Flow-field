using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FlowFieldGenerator: IInitializable
{
    
    [Inject] private GridManager _gridManager;

    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };
    
    public void Initialize()
    {
        // throw new System.NotImplementedException();
    }
    
    public void GenerateFlowField(Vector2Int targetGridPos)
    {
        InitializeIntegrationField(targetGridPos);
        BuildFlowDirection();
    }

    private void InitializeIntegrationField(Vector2Int targetPos)
    {
        var width = _gridManager.Width;
        var height = _gridManager.Height;

        foreach (var cell in IterateGrid())
        {
            cell.integrationValue = float.MaxValue;
        }

        var targetCell = _gridManager.GetCell(targetPos.x, targetPos.y);
        if (targetCell == null || !targetCell.walkable)
            return;

        Queue<GridCell> queue = new Queue<GridCell>();
        targetCell.integrationValue = 0;
        queue.Enqueue(targetCell);

        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                var neighbor = _gridManager.GetCell(nx, ny);
                if (neighbor != null && neighbor.walkable)
                {
                    float moveCost = current.cost; // ✅ 注意加的是“当前格子的代价”
                    float newCost = current.integrationValue + moveCost;

                    if (newCost < neighbor.integrationValue)
                    {
                        neighbor.integrationValue = newCost;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    private void BuildFlowDirection()
    {
        foreach (var cell in IterateGrid())
        {
            if (!cell.walkable) continue;

            GridCell bestNeighbor = null;
            float bestValue = cell.integrationValue;

            foreach (var dir in directions)
            {
                int nx = cell.x + dir.x;
                int ny = cell.y + dir.y;
                var neighbor = _gridManager.GetCell(nx, ny);

                if (neighbor != null && neighbor.integrationValue < bestValue)
                {
                    bestValue = neighbor.integrationValue;
                    bestNeighbor = neighbor;
                }
            }

            if (bestNeighbor != null)
            {
                Vector2 rawDir = new Vector2(bestNeighbor.x - cell.x, bestNeighbor.y - cell.y).normalized;
                cell.flowDirection = rawDir;
            }
            else
            {
                cell.flowDirection = Vector2.zero;
            }
            // Debug.Log($"Cell ({cell.x},{cell.y}) direction: {cell.flowDirection}");
        }
    }

    private IEnumerable<GridCell> IterateGrid()
    {
        for (int x = 0; x < _gridManager.Width; x++)
        {
            for (int y = 0; y < _gridManager.Height; y++)
            {
                yield return _gridManager.GetCell(x, y);
            }
        }
    }


}
