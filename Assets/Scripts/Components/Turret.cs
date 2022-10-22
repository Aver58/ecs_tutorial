using Unity.Entities;
     
struct Turret : IComponentData {
    public Entity CannonBallSpawn;
    public Entity CannonBallPrefab;
}