using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AutonomousAgent
{
    public sealed class FlockScene : Scene
    {
        private Vehicle _wanderer;
        private Texture2D wanderCircle;

        private int flockSize;


        public FlockScene(SceneManager manager)
            : base(manager)
        {
            flockSize = 100;
        }

        public override void Init()
        {
            // Define objects
            _wanderer = new Vehicle
                (
                    Manager.Game,
                    new Vector2(100, 100), new Vector2(1, 0),
                    0.2f, 500f, 500.0f,
                    new Vector2(0.3f, 0.3f),
                    Color.White
                );


            _wanderer.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
            _wanderer.Behavior.WanderOn();
            _wanderer.ViewDistance = 120;

            Entities.Add(_wanderer);

            // Add flock
            CreateFlock();
        }


        private void CreateFlock()
        {
            Vehicle entity;
            for (int i = 0; i < flockSize; i++)
            {
                int x = Tools.RandomHelper.Next(100, (int)Manager.Game.WorldLimit.X);
                int y = Tools.RandomHelper.Next(100, (int)Manager.Game.WorldLimit.Y);

                entity = new Vehicle(Manager.Game,
                                   new Vector2(x, y), new Vector2(1, 0),
                                   0.2f, 500f, 500.0f,
                                   new Vector2(0.1f, 0.1f),
                                   Color.DodgerBlue);
                entity.Behavior.ViewDistance = 120;

                // Wander behavior
                entity.Behavior.WanderOn();
                entity.Behavior.WanderingJitter = 500;


                // Evade behavior
                entity.Behavior.EvadeOn();
                entity.Behavior.ToAvoid = _wanderer;

                // Weighted sum weights
                entity.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
                entity.Behavior.AssignWeight(BehaviorType.Separation, 3f);
                entity.Behavior.AssignWeight(BehaviorType.Alignment, 4.0f);
                entity.Behavior.AssignWeight(BehaviorType.Cohesion, 2f);
                entity.Behavior.AssignWeight(BehaviorType.Evade, 10);
                entity.Behavior.AssignWeight(BehaviorType.Flee, 10);
                entity.Behavior.AssignWeight(BehaviorType.Wander, 0.5f);

                // Add them to manager
                Entities.Add(entity);
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Entities
            Entities.Update(gameTime);
        }


        public override void Draw(GameTime gameTime,
                         GraphicsDeviceManager graphics,
                         SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            Entities.Draw(spriteBatch, Manager.DebugState);

            // Add a Line in heading direction for DEBUG
            if (Manager.DebugState == true)
            {
                Tools.Graphics.GraphicsHelper.DrawLine
                    (
                        graphics.GraphicsDevice, spriteBatch,
                        _wanderer.Pos,
                        _wanderer.Behavior.wanderTargetPos,
                        Color.IndianRed,
                        3
                    );

                // Wandering circle
                SteeringBehavior b = _wanderer.Behavior;
                wanderCircle = Tools.Graphics.GraphicsHelper.CreateCircle((int)b.WanderingRadius,
                                                                          graphics.GraphicsDevice,
                                                                          Color.Black);

                spriteBatch.Draw(wanderCircle, b.wanderCirclePos, null,
                                 Color.Black * 1.0f, 0.0f,
                                 new Vector2(wanderCircle.Width / 2, wanderCircle.Height / 2),
                                 Vector2.One, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }

        private void FlockOn()
        {
            foreach (BaseEntity entity in _entities.Entities)
            {
                // Try cast to vehicle
                var vehicle = entity as Vehicle;

                // It’s a vehicle and different from the enemi
                if (vehicle != null && vehicle.Id != _wanderer.Id)
                {
                    // Active group beahvior
                    vehicle.Behavior.SeparationOn();
                    vehicle.Behavior.AlignmentOn();
                    vehicle.Behavior.CohesionOn();
                }
            }
        }

        private void FlockOff()
        {
            foreach (BaseEntity entity in _entities.Entities)
            {
                // Try cast to vehicle
                var vehicle = entity as Vehicle;

                // It’s a vehicle and different from the enemi
                if (vehicle != null && vehicle.Id != _wanderer.Id)
                {
                    // Active group beahvior
                    vehicle.Behavior.SeparationOff();
                    vehicle.Behavior.AlignmentOff();
                    vehicle.Behavior.CohesionOff();
                }
            }
        }


        /// <summary>
        /// Handle keyboard input to update vehicle behavior.
        /// </summary>
        protected override void HandleKeyboardInputs (KeyboardState keyboardState)
        {
            // Handle keyboard for vehicle 1
            Keys[] currentPressedKeys = keyboardState.GetPressedKeys();
            foreach (Keys key in currentPressedKeys)
            {
                switch (key)
                {
                    case Keys.N:
                        Manager.ChangeScene(new SeekScene(Manager));
                        break;
                    case Keys.P:
                        Manager.ChangeScene(new EscapeScene(Manager));
                        break;
                    case Keys.F:
                        FlockOff();
                        break;
                    case Keys.G:
                        FlockOn();
                        break;
                }
            }
        }
    }
}
