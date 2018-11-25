using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AutonomousAgent
{
    public abstract class Scene
    {
        private System.Timers.Timer _timerKeyboard;
        private bool keyboardAllowed;


        protected SceneManager _sceneManager;
        protected EntityManager _entities;


        public EntityManager Entities
        {
            get {return _entities;}
        }


        public SceneManager Manager
        {
            get {return _sceneManager;}
        }


        protected Scene(SceneManager manager)
        {
            // Assign scene SM
            _sceneManager = manager;

            // Wait for 0.5 seconds without blocking ...
            _timerKeyboard = new System.Timers.Timer(500);
            // ... before allowing keyboard inputs
            _timerKeyboard.Elapsed += OnTimerKeyboardDone;
            _timerKeyboard.Enabled = true;

            // Entities manager
            _entities = new EntityManager();
        }


        public abstract void Init();
        public abstract void Draw(GameTime gameTime,
                                  GraphicsDeviceManager graphics,
                                  SpriteBatch spriteBatch);


        /// <summary>
        /// Must be called in child to Account for keyboard events.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (keyboardAllowed) { HandleKeyboardInputs(Keyboard.GetState()); }
        }

        public virtual void Exit(){}


        /// <summary>
        /// Handle keyboard input to update vehicle behavior.
        /// </summary>
        protected virtual void HandleKeyboardInputs (KeyboardState keyboardState)
        {

        }


        /// <summary>
        /// Delegate to be called when timer done.
        /// </summary>
        private void OnTimerKeyboardDone(object sender, EventArgs e)
        {
            keyboardAllowed = true;
        }
    }
}
