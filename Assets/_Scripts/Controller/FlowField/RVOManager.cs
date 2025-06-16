using System;
using RVO;
using Zenject;
using Unity.Mathematics;
using UnityEngine;

public class RVOManager : MonoBehaviour
{
    private Simulator _simulator;
    private float2 _currentGoal;
    
    public Simulator GetSimulator()
    {
        if (_simulator == null)
        {
            var simulator = new Simulator();

            simulator.SetTimeStep(1 / 60f);
            simulator.SetAgentDefaults(3f, 10, 4f, 4f, 0.5f, 5f, new float2(0f, 0f));

            _simulator = simulator;
        }

        return _simulator;
    }
    private void Update()
    {
        _simulator.DoStep();
        // 确保所有的RVO操作都完成
        _simulator.EnsureCompleted();
    }


}