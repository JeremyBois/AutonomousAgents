using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AutonomousAgent
{
    public sealed class EscapeScene : Scene
    {
        private Vehicle _escaper;
        private Vehicle _pursuer;
        private Texture2D wanderCircle;


        public EscapeScene(SceneManager manager)
            : base(manager)
        {
        }

        public override void Init()
        {
            // Define objects
            _escaper = new Vehicle
                (
                    Manager.Game,
                    new Vector2(100, 100), new Vector2(1, 0),
                    0.2f, 500f, 500.0f,
                    new Vector2(0.2f, 0.2f),
                    Color.White
                );

            _pursuer = new Vehicle
                (
                    Manager.Game,
                    new Vector2(500, 600), new Vector2(1, 0),
                    0.2f, 500, 500.0f,
                    new Vector2(0.3f, 0.3f),
                    Color.Chocolate
                );
            _pursuer.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
            _pursuer.Behavior.PursuitOn();
            _pursuer.ViewDistance = 120;
            _pursuer.Behavior.Target = _escaper;


            _escaper.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
            _escaper.Behavior.EvadeOn();
            _escaper.Behavior.WanderOn();
            _escaper.ViewDistance = 200;
            _escaper.Behavior.ToAvoid = _pursuer;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update entity
            _escaper.Update(gameTime);
            _pursuer.Update(gameTime);
        }


        public override void Draw(GameTime gameTime,
                         GraphicsDeviceManager graphics,
                         SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            _escaper.Draw(spriteBatch, Manager.DebugState);
            _pursuer.Draw(spriteBatch, Manager.DebugState);

            // Add a Line in heading direction for DEBUG
            if (Manager.DebugState == true)
            {
                Tools.Graphics.GraphicsHelper.DrawLine
                    (
                        graphics.GraphicsDevice, spriteBatch,
                        _escaper.Pos,
                        _escaper.Behavior.wanderTargetPos,
                        Color.IndianRed,
                        3
                    );

                // Wandering circle
                SteeringBehavior b = _escaper.Behavior;
                wanderCircle = Tools.Graphics.GraphicsHelper.CreateCircle((int)b.WanderingRadius,
                                                                          graphics.GraphicsDevice,
                                                                          Color.Black);

                spriteBatch.Draw(wanderCircle, b.wanderCirclePos, null,
                                 Color.Black * 1.0f, 0.0f,
                                 new Vector2(wanderCircle.Width / 2, wanderCircle.Height / 2),
                                 Vector2.One, SpriteEffects.None, 1);

                Tools.Graphics.GraphicsHelper.DrawLine
                    (
                        graphics.GraphicsDevice, spriteBatch,
                        _pursuer.Pos,
                        _pursuer.Pos + _pursuer.Heading * _pursuer.ViewDistance,
                        Color.IndianRed,
                        3
                    );
            }

            spriteBatch.End();
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
                        Manager.ChangeScene(new FlockScene(Manager));
                        break;
                    case Keys.P:
                        Manager.ChangeScene(new WanderScene(Manager));
                        break;
                }
            }
        }
    }
}
