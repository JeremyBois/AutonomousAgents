using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AutonomousAgent
{
    public sealed class WanderScene : Scene
    {
        private Vehicle _mainEntity;
        private Texture2D wanderCircle;


        public WanderScene(SceneManager manager)
            : base(manager)
        {
        }

        public override void Init()
        {
            // Define objects
            _mainEntity = new Vehicle
                (
                    Manager.Game,
                    new Vector2(100, 100), new Vector2(1, 0),
                    0.1f, 500f,500.0f,
                    new Vector2(0.2f, 0.2f),
                    Color.White
                );
            _mainEntity.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
            _mainEntity.Behavior.WanderOn();
            _mainEntity.ViewDistance = 120;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update entity
            _mainEntity.Update(gameTime);
        }


        public override void Draw(GameTime gameTime,
                                  GraphicsDeviceManager graphics,
                                  SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            _mainEntity.Draw(spriteBatch, Manager.DebugState);

            // Add a Line in heading direction for DEBUG
            if (Manager.DebugState == true)
            {
                Tools.Graphics.GraphicsHelper.DrawLine
                    (
                        graphics.GraphicsDevice, spriteBatch,
                        _mainEntity.Pos,
                        _mainEntity.Behavior.wanderTargetPos,
                        Color.IndianRed,
                        3
                    );

                // Wandering circle
                SteeringBehavior b = _mainEntity.Behavior;
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
                        Manager.ChangeScene(new EscapeScene(Manager));
                        break;
                    case Keys.P:
                        Manager.ChangeScene(new FleeScene(Manager));
                        break;
                }
            }
        }
    }
}
