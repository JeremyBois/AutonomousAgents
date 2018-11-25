using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AutonomousAgent
{
    public class SceneManager
    {

        // STATIC
        // Used to unsure only one instance is created at runtime
        static bool alreadyExist;


        private Scene _current;
        private Scene _previous;

        private Game1 _game;
        private bool _debugState;

        public bool DebugState
        {
            get {return _debugState;}
            set {_debugState = value;}
        }

        public Game1 Game
        {
            get {return _game;}
        }

        public SceneManager(Game1 game, bool debugOn=true)
        {
            if (alreadyExist)
            {
                throw new InvalidOperationException("Only one Scene manager must exists inside the program.");
            }

            alreadyExist = true;
            _game = game;
            _debugState = debugOn;
        }

        public void Update(GameTime gameTime)
        {
            if (_current != null)
            {
                _current.Update(gameTime);
            }
            else
            {
                throw new NullReferenceException("No active scene defined.");
            }
        }

        public void Draw(GameTime gameTime,
                         GraphicsDeviceManager graphics,
                         SpriteBatch spriteBatch)
        {
            if (_current != null)
            {
                _current.Draw(gameTime, graphics, spriteBatch);
            }
            else
            {
                throw new NullReferenceException("No active scene defined.");
            }
        }

        public Scene Current
        {
            get {return _current;}
            set {_current = value;}
        }

        public Scene Previous
        {
            get {return _previous;}
            set {_previous = value;}
        }


        public void ChangeScene(Scene scene)
        {
            _previous = _current;
            _current = scene;
            _previous.Exit();
            _current.Init();
        }

        public void RevertToPrevious()
        {
            if (_previous == null)
            {
                Console.Out.WriteLine("No previous Scene defined.");
            }
            Scene temp = _current;
            _current = _previous;
            _previous = temp;
            _previous.Exit();
            _current.Init();
        }
    }
}
