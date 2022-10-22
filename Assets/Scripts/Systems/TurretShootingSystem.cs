using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct TurretShootingSystem : ISystem {
    ComponentLookup<LocalToWorldTransform> m_LocalToWorldTransformFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        // 拿到所有对象坐标Map 
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorldTransform>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        m_LocalToWorldTransformFromEntity.Update(ref state);
        // 实例化操作有可能会超过一帧的时间，用 CommandBuffer 确保Job跑完
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var turretShootJob = new TurretShoot {
            LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
            ECB = ecb
        };

        // 在线程上执行，不会阻塞主线程
        turretShootJob.Schedule();
    }
}

[WithAll(typeof(Shooting))] 
[BurstCompile]
partial struct TurretShoot : IJobEntity {
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> LocalToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;

    // in 是访问修饰符：read only. 为了避免造成不同结果，原则上尽量用 in 
    void Execute(in TurretAspect turret) {
        var instance = ECB.Instantiate(turret.CannonBallPrefab);
        // 设置坐标
        var spawnLocalToWorld = LocalToWorldTransformFromEntity[turret.CannonBallSpawn];
        var cannonBallTransform = UniformScaleTransform.FromPosition(spawnLocalToWorld.Value.Position);
        // 设置缩放
        cannonBallTransform.Scale = LocalToWorldTransformFromEntity[turret.CannonBallPrefab].Value.Scale;
        // 赋值Transform
        ECB.SetComponent(instance, new LocalToWorldTransform {
            Value = cannonBallTransform
        });

        // 赋值 CannonBall 组件的 Speed
        ECB.SetComponent(instance, new CannonBall {
            Speed = spawnLocalToWorld.Value.Forward() * 20.0f
        });
        
        ECB.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = turret.Color });
    }
}