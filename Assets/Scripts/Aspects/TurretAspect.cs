using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

readonly partial struct TurretAspect : IAspect {
    readonly RefRO<Turret> m_Turret;
    readonly RefRO<URPMaterialPropertyBaseColor> m_BaseColor;

    public Entity CannonBallSpawn => m_Turret.ValueRO.CannonBallSpawn;
    public Entity CannonBallPrefab => m_Turret.ValueRO.CannonBallPrefab;
    public float4 Color => m_BaseColor.ValueRO.Value;
}