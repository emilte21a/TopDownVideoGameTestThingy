

public class CollisionSystem : GameSystem
{
    public override void Update()
    {
        for (int x = 0; x < Game.entities.Count; x++)
        {
            Entity e = Game.entities[x];

            #region Hämta potentiella komponenter hos entitites
            PhysicsBody? physicsBody = e.GetComponent<PhysicsBody>();
            Collider? collider = e.GetComponent<Collider>();
            #endregion

            List<Tile> tilesCloseToEntity = new List<Tile>(); //Lista med alla rektanglar som screenRectangle kolliderar med

            #region Rektanglar som kollar spelarens kollisioner
            Rectangle ScreenRectangle = new Rectangle(e.position.X - Game.ScreenWidth / 4 - 15, e.position.Y - Game.ScreenHeight / 4 - 15, Game.ScreenWidth / 2 + 30, Game.ScreenHeight / 2 + 30); //Magiska nummer för offset
            #endregion

            if (collider != null && physicsBody != null) //Kolla så att entityns physicsbody och collider inte är null
            {

                tilesCloseToEntity = WorldGeneration.tilesInWorld.Where(tile => Raylib.CheckCollisionRecs(ScreenRectangle, tile.rectangle)).ToList();


                if (e.tag == "Player")
                    WorldGeneration.tilesThatShouldRender = tilesCloseToEntity;

                #region bestäm senaste riktningen samt om entityn nuddar marken eller ej
                if (physicsBody.velocity.X > 0) { e.lastDirection.X = 1; }

                else if (physicsBody.velocity.X < 0) { e.lastDirection.X = -1; }

                if (physicsBody.velocity.Y > 0) { e.lastDirection.Y = 1; }

                else if (physicsBody.velocity.Y < 0) { e.lastDirection.Y = -1; }

                #endregion

                #region Korrigera y positionen 

                for (int i = 0; i < tilesCloseToEntity.Count; i++)
                {
                    //Räkna ut spelarens y-position i nästa frame med en rektangel
                    Rectangle nextBounds = new Rectangle(e.position.X, e.position.Y + physicsBody.velocity.Y, collider.boxCollider.Width, collider.boxCollider.Height);
                    Rectangle collisionRectangle = Raylib.GetCollisionRec(tilesCloseToEntity[i].rectangle, nextBounds);

                    if (collisionRectangle.Width > collisionRectangle.Height && tilesCloseToEntity[i].tag == "Tile" && tilesCloseToEntity[i].isSolid)
                    {
                        if (physicsBody.velocity.Y > 0 || e.lastDirection.Y == 1)
                        {
                            //Korrigera Y hastigheten ifall spelaren faller
                            e.position = new Vector2(e.position.X, tilesCloseToEntity[i].rectangle.Y - collider.boxCollider.Height);
                            physicsBody.velocity.Y = 0;
                        }
                        else if (physicsBody.velocity.Y < 0 || e.lastDirection.Y == -1)
                        {
                            //Korrigera Y hastigheten ifall spelaren hoppar
                            e.position = new Vector2(e.position.X, tilesCloseToEntity[i].rectangle.Y + tilesCloseToEntity[i].rectangle.Height);
                            physicsBody.velocity.Y = 0;
                        }
                    }
                }
                #endregion

                #region Korrigera x positionen
                for (int i = 0; i < tilesCloseToEntity.Count; i++)
                {
                    //Räkna ut spelarens x-position i nästa frame med en rektangel
                    Rectangle nextBounds = new Rectangle(e.position.X + physicsBody.velocity.X, e.position.Y, collider.boxCollider.Width, collider.boxCollider.Height);

                    if (Raylib.CheckCollisionRecs(nextBounds, tilesCloseToEntity[i].rectangle) && tilesCloseToEntity[i].tag == "Tile" && tilesCloseToEntity[i].isSolid) //Kolla kollisioner mellan alla tiles som är inom ett visst område av spelaren och spelarens rektangel i nästa frame
                    {
                        if (physicsBody.velocity.X > 0 || e.lastDirection.X == 1)
                        {
                            //Korrigera spelarens hastighet åt höger när den kolliderar
                            e.position = new Vector2(tilesCloseToEntity[i].rectangle.X - collider.boxCollider.Width, e.position.Y);
                            physicsBody.velocity.X = 0;
                        }

                        else if (physicsBody.velocity.X < 0 || e.lastDirection.X == -1)
                        {
                            //Korrigera spelarens hastighet åt vänster när den kolliderar
                            e.position = new Vector2(tilesCloseToEntity[i].rectangle.X + tilesCloseToEntity[i].rectangle.Width, e.position.Y);
                            physicsBody.velocity.X = 0;
                        }
                    }
                }
                #endregion
            }
        }
    }
}