using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial class TankMovementSystem : SystemBase {
    protected override void OnUpdate() {
        var dt = SystemAPI.Time.DeltaTime;
        Entities.WithAll<Tank>().ForEach((Entity entity, TransformAspect transform) => {
            // 经典柏林噪声 Classic Perlin Noise (cnoise)
            // https://www.bit-101.com/blog/2021/07/mapping-perlin-noise-to-angles/
            var pos = transform.Position;
            pos.y = entity.Index;
            var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;

            var dir = float3.zero;
            math.sincos(angle, out dir.x, out dir.z);
            transform.Position += dir * dt * 5.0f;
            transform.Rotation = quaternion.RotateY(angle);
        }).ScheduleParallel();
        // 三种控制流
        // Run (main thread),
        // Schedule (single thread, async)
        // ScheduleParallel (multiple threads, async).
    }
}