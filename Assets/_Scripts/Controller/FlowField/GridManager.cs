using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private Vector3 originPosition;


    public Transform target; 
    // public Vector2Int gridPosition;
    
    private GridCell[,] gridCells;

    public int Width => gridWidth;
    public int Height => gridHeight;
    public float CellSize => cellSize;
    
    [Inject]
    private readonly FlowFieldGenerator _flowFieldGenerator;
    
    public Transform staticObstacleGroup;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "更新"))
        {
            SetTargetPos();
        }

    }

    private void Start()
    {
        InitializeGrid(gridWidth,gridHeight,cellSize);
        GenerateObstacle();
        SetTargetPos();


    }

    public void SetTargetPos()
    {
        Vector2Int pos = new Vector2Int((int)target.position.x, (int)target.position.z);
        _flowFieldGenerator.GenerateFlowField(pos);
    }

    public void GenerateObstacle()
    {
        var list = staticObstacleGroup.GetComponentsInChildren<StaticObstacle>().ToList();
        foreach (var item in list)
        {
            item.RegisterObstacle();
        }
    }


    public void InitializeGrid(int width, int height, float size)
    {
        gridWidth = width;
        gridHeight = height;
        cellSize = size;

        gridCells = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                gridCells[x, y] = new GridCell(x, y, worldPos);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return originPosition + new Vector3(x * cellSize + cellSize / 2, 0, y * cellSize + cellSize / 2);
    }

    public bool TryGetGridPosition(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize);
        y = Mathf.FloorToInt((worldPos.z - originPosition.z) / cellSize);
        return x >= 0 && y >= 0 && x < gridWidth && y < gridHeight;
    }

    public GridCell GetCell(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
            return gridCells[x, y];
        return null;
    }
    
    
    public GridCell GetBestNeighbor(int x, int y)
    {
        GridCell current = GetCell(x, y);
        GridCell bestNeighbor = null;
        float bestValue = current.integrationValue;

        foreach (var dir in _dirs)
        {
            var neighbor = GetCell(x + dir.x, y + dir.y);
            if (neighbor != null && neighbor.walkable && neighbor.integrationValue < bestValue)
            {
                bestValue = neighbor.integrationValue;
                bestNeighbor = neighbor;
            }
        }
        return bestNeighbor;
    }
    
    
    public readonly Vector2Int[] _dirs = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };
    
    
    

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (gridCells == null)
            return;

        
        float maxValue = 1f;

        // 🔍 第一次遍历获取最大值（用于归一化）
        foreach (var cell in gridCells)
        {
            if (cell.integrationValue < float.MaxValue)
            {
                maxValue = Mathf.Max(maxValue, cell.integrationValue);
            }
        }

        // 🎨 第二次绘制可视化格子
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var cell = gridCells[x, y];
                Vector3 pos = cell.worldPosition;

                // 🔢 归一化 [0,1] 范围（避免除以零）
                float normValue = Mathf.Clamp01(cell.integrationValue / maxValue);

                // 🌈 热力图颜色（红 → 蓝）
                Color heatColor = Color.Lerp(Color.red, Color.blue, normValue);

                // 绘制格子轮廓
                Gizmos.color = Color.grey;
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));

                // 绘制方向箭头
                if (cell.flowDirection != Vector2.zero)
                {
                    Vector3 dir3D = new Vector3(cell.flowDirection.x, 0, cell.flowDirection.y);
                    Gizmos.color = heatColor;
                    Gizmos.DrawRay(pos, dir3D.normalized * (cellSize * 0.4f));
                }
            }
        }
        
        
        // for (int x = 0; x < gridWidth; x++)
        // {
        //     for (int y = 0; y < gridHeight; y++){
        //         var cell = gridCells[x, y];
        //         Vector3 pos = cell.worldPosition;
        //         
        //         if (!cell.walkable) continue;
        //         // 绘制数字（使用 Handles.Label）
        //         Handles.color = Color.grey;
        //         Handles.Label(pos + Vector3.up * 0.2f, $"{(int)cell.integrationValue}");
        //
        //     }
        // }
        
#endif
    }


}
