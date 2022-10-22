using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct CannonBallAspect : IAspect {
    public readonly Entity Self;

    readonly TransformAspect Transform;
    // RefRW：read write.
    readonly RefRW<CannonBall> CannonBall;

    // 可以直接用Public字段，而不用属性，用属性主要为了避免链式调用： "aspect.aspect.aspect.component.value.value".
    public float3 Position {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float3 Speed {
        get => CannonBall.ValueRO.Speed;
        set => CannonBall.ValueRW.Speed = value;
    }
}