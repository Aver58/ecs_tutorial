using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Requires the Turret type without processing it (it's not part of the Execute method).
[WithAll(typeof(Turret))]
[BurstCompile]
partial struct SafeZoneJob : IJobEntity {
    // When running this job in parallel, the safety system will complain about a
    // potential race condition with TurretActiveFromEntity because accessing the same entity
    // from different threads would cause problems.
    // But the code of this job is written in such a way that only the entity being currently
    // processed will be looked up in TurretActiveFromEntity, making this process safe.
    // So we can disable the parallel safety check.
    [NativeDisableParallelForRestriction] public ComponentLookup<Shooting> TurretActiveFromEntity;

    public float SquaredRadius;

    void Execute(Entity entity, TransformAspect transform) {
        // 超出范围，disable掉
        TurretActiveFromEntity.SetComponentEnabled(entity, math.lengthsq(transform.Position) > SquaredRadius);
    }
}

[BurstCompile]
partial struct SafeZoneSystem : ISystem {
    ComponentLookup<Shooting> m_TurretActiveFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Config>();
        m_TurretActiveFromEntity = state.GetComponentLookup<Shooting>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
        const float debugRenderStepInDegrees = 20;

        for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees) {
            var a = float3.zero;
            var b = float3.zero;
            math.sincos(math.radians(angle), out a.x, out a.z);
            math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
            // 绘制安全区范围
            UnityEngine.Debug.DrawLine(a * radius, b * radius);
        }

        m_TurretActiveFromEntity.Update(ref state);
        var safeZoneJob = new SafeZoneJob {
            TurretActiveFromEntity = m_TurretActiveFromEntity,
            SquaredRadius = radius * radius
        };
        safeZoneJob.ScheduleParallel();
    }
}