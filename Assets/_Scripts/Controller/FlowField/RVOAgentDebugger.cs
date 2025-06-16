// using UnityEngine;
// using RVO;
// using Unity.Mathematics;
// using Zenject;
//
// public class RVOAgentDebugger : MonoBehaviour
// {
//     public int agentId;
//     public Color velocityColor = Color.green;
//     public Color prefVelocityColor = Color.blue;
//     public Color neighborColor = Color.cyan;
//     public Color orcaLineColor = Color.red;
//
//     public float agentRadius = 0.5f;
//     public float neighborDist = 5f;
//     
//     public RVOManager _rvoManager;
//
//     private void OnDrawGizmos()
//     {
//         if ( agentId < 0 || _rvoManager.Simulator == null)
//             return;
//
//         var pos = _rvoManager.Simulator.GetAgentPosition(agentId);
//         Vector3 worldPos = new Vector3(pos.x, 0.1f, pos.y);
//
//         // 绘制 Agent 半径（身体）
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(worldPos, agentRadius);
//
//         // 绘制邻居感知圈
//         Gizmos.color = neighborColor;
//         Gizmos.DrawWireSphere(worldPos, neighborDist);
//
//         // 绘制当前速度
//         Vector2 velocity = _rvoManager.Simulator.GetAgentVelocity(agentId);
//         Gizmos.color = velocityColor;
//         Gizmos.DrawLine(worldPos, worldPos + new Vector3(velocity.x, 0, velocity.y));
//
//         // 绘制偏好速度
//         Vector2 prefVel = _rvoManager.Simulator.GetAgentPrefVelocity(agentId);
//         Gizmos.color = prefVelocityColor;
//         Gizmos.DrawLine(worldPos, worldPos + new Vector3(prefVel.x, 0, prefVel.y));
//
//
//     }
// }