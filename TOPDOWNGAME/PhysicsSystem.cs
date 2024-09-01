public class PhysicsSystem : GameSystem
{
    public override void Update()
    {
        foreach (Entity e in Game.entities)
        {
            PhysicsBody? physicsBody = e.GetComponent<PhysicsBody>();

            if (physicsBody != null && physicsBody.UseGravity == PhysicsBody.Gravity.enabled)
            {
                //Uppdatera positionen
                e.position += physicsBody.velocity * Raylib.GetFrameTime() * 100;
            }
        }
    }
}