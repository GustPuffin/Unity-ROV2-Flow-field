using RVO;
using UnityEngine;
using Zenject;

public class LevelInitializer : MonoInstaller
{
    public override void InstallBindings()
    {
        //导航网格注册
        Container.Bind<GridManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<FlowFieldGenerator>().AsSingle().NonLazy();
        // Container.Bind<Simulator>().AsSingle();        
        Container.Bind<RVOManager>().FromComponentInHierarchy().AsSingle().NonLazy();        
    }
}