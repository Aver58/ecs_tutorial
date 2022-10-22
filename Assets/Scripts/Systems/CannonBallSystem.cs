using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct CannonBallJob : IJobEntity {
    // 普通的 EntityCommandBuffer 不能在并行Job上调用，所以必须显式指明 ParallelWriter 
    public EntityCommandBuffer.ParallelWriter ECB;
    public float DeltaTime;

    // 一个数量级塞到chunk，在不同Job进行
    void Execute([ChunkIndexInQuery] int chunkIndex, ref CannonBallAspect cannonBall) {
        var gravity = new float3(0.0f, -9.82f, 0.0f);
        var invertY = new float3(1.0f, -1.0f, 1.0f);

        cannonBall.Position += cannonBall.Speed * DeltaTime;
        // 撞地面，给一个反向的力
        if (cannonBall.Position.y < 0.0f) {
            cannonBall.Position *= invertY;
            cannonBall.Speed *= invertY * 0.8f;
        }

        cannonBall.Speed += gravity * DeltaTime;

        // 没速度就移除
        var speed = math.lengthsq(cannonBall.Speed);
        if (speed < 0.1f)
            ECB.DestroyEntity(chunkIndex, cannonBall.Self);
    }
}

[BurstCompile]
partial struct CannonBallSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var cannonBallJob = new CannonBallJob {
            ECB = ecb.AsParallelWriter(),
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        cannonBallJob.ScheduleParallel();
    }
}