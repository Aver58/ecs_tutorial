using Unity.Entities;

class TurretAuthoring : UnityEngine.MonoBehaviour {
    public UnityEngine.GameObject CannonBallPrefab;
    public UnityEngine.Transform CannonBallSpawn;
}
     
class TurretBaker : Baker<TurretAuthoring> {
    public override void Bake(TurretAuthoring authoring) {
        AddComponent(new Turret {
            CannonBallPrefab = GetEntity(authoring.CannonBallPrefab),
            CannonBallSpawn = GetEntity(authoring.CannonBallSpawn)
        });
        
        AddComponent<Shooting>();
    }
}