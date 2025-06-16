using System;
using UnityEngine;
using Zenject;
using RVO;
using Unity.Mathematics;
using Vector2 = UnityEngine.Vector2;

public class UnitMover : MonoBehaviour
{
    private GridManager _gridManager;
    private RVOManager _rvoManager;
    private Simulator _simulator;


    [SerializeField] private float stoppingDistance = 0.5f;

    public float moveSpeed;
    private int _agentId;

    [Inject]
    public void Construct(GridManager gridManager, RVOManager rvoManager)
    {
        _gridManager = gridManager;
        _rvoManager = rvoManager;
    }

    private void Awake()
    {
        _simulator = _rvoManager.GetSimulator();
    }

    private void OnEnable()
    {
        var position = new float2(this.transform.position.x, this.transform.position.z);
        _agentId = _simulator.AddAgent(position);
        _simulator.SetAgentMaxSpeed(_agentId, moveSpeed);
        _simulator.SetAgentRadius(_agentId, 0.5f); // 稍微放大
    }


    private void Start()
    {
        // Debug.Log($"Agent {name} ID: {_agentId}, Total agents: {_simulator.GetNumAgents()}");
    }

    private void Update()
    {
        //位置判断
        float2 rvoPos = _simulator.GetAgentPosition(_agentId);
        // 获取目标点中心位置
        float2 goalPos = new float2(_gridManager.target.position.x, _gridManager.target.position.z);
        // 计算与目标距离
        float distanceToGoal = math.distance(rvoPos, goalPos);
        if (distanceToGoal <= stoppingDistance)
        {
            _simulator.SetAgentPrefVelocity(_agentId, float2.zero);
            return;
        }

        //获取当前位置网格坐标
        int x = Mathf.FloorToInt((rvoPos.x - _gridManager.transform.position.x) / _gridManager.CellSize);
        int y = Mathf.FloorToInt((rvoPos.y - _gridManager.transform.position.z) / _gridManager.CellSize);
        GridCell current = _gridManager.GetCell(x, y);


        // 宽容策略：当前格子可通行，或者当前格子不可通行但至少存在2个可通行邻居（贴边滑动）
        if (current == null || (!current.walkable && CountWalkableNeighbors(x, y) <= 1))
            return;

        //获取附近格子
        GridCell bestNeighbor = _gridManager.GetBestNeighbor(x, y);
        if (bestNeighbor == null) return;

        float2 goal = new float2(bestNeighbor.worldPosition.x, bestNeighbor.worldPosition.z);
        // float2 goal = new float2(_gridManager.target.transform.position.x, _gridManager.target.transform.position.z);
        float2 dir = goal - rvoPos;

        float2 preferredVel = math.normalize(dir) * moveSpeed;
        preferredVel += (float2)UnityEngine.Random.insideUnitCircle * 0.01f; // 加入微扰动
        // Debug.Log($"Agent {name} ID: {_agentId}, PrefVelocity: {preferredVel}");
        _simulator.SetAgentPrefVelocity(_agentId, preferredVel);

        // // 设置朝向
        SetPreferredRotate(preferredVel);
    }

    private void LateUpdate()
    {
        // 更新位置
        float2 newPos = _simulator.GetAgentPosition(_agentId);
        // Debug.Log($"Agent {name} ID: {_agentId}, newPos: {newPos}");
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
    }

    private int CountWalkableNeighbors(int x, int y)
    {
        int count = 0;
        foreach (var dir in _gridManager._dirs)
        {
            var neighbor = _gridManager.GetCell(x + dir.x, y + dir.y);
            if (neighbor != null && neighbor.walkable)
                count++;
        }

        return count;
    }

    private void SetPreferredRotate(float2 flowDir)
    {
        Vector3 moveDir = new Vector3(flowDir.x, 0, flowDir.y);
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_rvoManager?.GetSimulator() == null) return;

        float2 pos = _simulator.GetAgentPosition(_agentId);
        float2 vel = _simulator.GetAgentVelocity(_agentId);
        float2 pref = _simulator.GetAgentPrefVelocity(_agentId);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(pos.x, 1f, pos.y), new Vector3(pos.x + pref.x, 1f, pos.y + pref.y));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(pos.x, 1f, pos.y), new Vector3(pos.x + vel.x, 1f, pos.y + vel.y));
    }


    private void OnDestroy()
    {
        if (_simulator == null)
        {
            return;
        }

        _simulator.RemoveAgent(_agentId);
        _agentId = -1;
    }
}