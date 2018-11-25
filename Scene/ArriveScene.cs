using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AutonomousAgent
{
    public sealed class ArriveScene : Scene
    {
        private Vehicle _mainEntity;
        private Vehicle _target;

        public ArriveScene(SceneManager manager)
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
                    0.1f, 1000f, 2000.0f,
                    new Vector2(0.2f, 0.2f),
                    Color.White
                );
            _mainEntity.Behavior.CombinaisonMethodUsed = CombinaisonMethod.Weighted;
            _mainEntity.Behavior.ArriveOn();
            _mainEntity.ViewDistance = 120;

            _target = new Vehicle
                (
                    Manager.Game,
                    new Vector2(100, 100), new Vector2(1, 0),
                    1f, 100f, 100.0f,
                    new Vector2(0.1f, 0.1f),
                    Color.Chocolate
                );

            _mainEntity.Behavior.Target = _target;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Target is the mouse
            var mouse = Mouse.GetState();
            _target.Pos = new Vector2(mouse.X, mouse.Y);


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
            _target.Draw(spriteBatch, Manager.DebugState);

            // Add a Line in heading direction for DEBUG
            if (Manager.DebugState == true)
            {
                Tools.Graphics.GraphicsHelper.DrawLine
                    (
                        graphics.GraphicsDevice, spriteBatch,
                        _mainEntity.Pos,
                        _mainEntity.Pos + _mainEntity.Heading * _mainEntity.ViewDistance,
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
                        Manager.ChangeScene(new FleeScene(Manager));
                        break;
                    case Keys.P:
                        Manager.ChangeScene(new SeekScene(Manager));
                        break;
                }
            }
        }
    }
}
