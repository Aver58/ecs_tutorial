using Unity.Entities;

struct Config : IComponentData {
    public Entity TankPrefab;
    public int TankCount;
    public float SafeZoneRadius;
}