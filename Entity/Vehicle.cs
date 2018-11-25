using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace AutonomousAgent
{
	using Tools.Vector;
    using Tools.Graphics;


	public class Vehicle: MovingEntity
	{
        SteeringBehavior _behavior;
        Game1 _world;


        // Drawing support
        private Texture2D _sprite;
        private Color _color;
        private Texture2D _fieldOfView;


        /// <summary>
        /// Access to vehicle steering behavior.
        /// </summary>
        public SteeringBehavior Behavior
        {
            get{ return _behavior; }
        }

        /// <summary>
        /// Access to world where entity lives.
        /// </summary>
        public Game1 World
        {
            get{ return _world; }
        }


		public Vehicle(Game1 world,
                       Vector2 position,
                       Vector2 velocity,
                       float mass,
                       float maxSpeed,
                       float maxForce,
                       Vector2 scale,
                       Color color
                       )
            : base(position, velocity, mass, maxSpeed, maxForce, scale)
		{
            _world = world;
            _behavior = new SteeringBehavior(this);

            Active = true;

            // Create textures
            _color = color;
            LoadContent(_world.Content, @"Graphics/Ghost");

            // Add bounding radius
            BRadius = Math.Max(_sprite.Width * Scale.X, _sprite.Height * Scale.Y);
            BrakeRadius = 5;
		}

        /// <summary>
        /// Update according to Gametime
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // No active behavior does not mean no mouvement
            // but in case of a demo is much easier
            if ( !Active || !Behavior.HasActiveBehavior() )
            {
                return;
            }
            double timeElapsed = gameTime.ElapsedGameTime.TotalSeconds;

            // Wander need time elapsed to be time independant
            Vector2 steeringForce = Behavior.Calculate(timeElapsed);

            // Get acceleration using steering forces
            Vector2 acceleration = Vector2.Divide(steeringForce, Mass);

            // Update velocity using acceleration
            Velocity += Vector2.Multiply(acceleration, (float)timeElapsed);
            // Trim back velocity using extension method
            Velocity = Velocity.TruncateExt(MaxSpeed);

            // To avoid stange behavior when agent arrive
            // update only if velocity is not too close to 0
            if (Velocity.LengthSquared() > 0.00000001)
            {
                // Update internal values
                UpdateOrientation();
                UpdateLocalBase();
            }


            // Update position
            Pos += Vector2.Multiply(Velocity, (float)timeElapsed);

            // Make world toroidal
            WrapAround(_world.GraphicsDevice.Viewport.Bounds);
        }

        public int ViewDistance
        {
            get
            {
				return Behavior.ViewDistance;
            }
            set
            {
                Behavior.ViewDistance = value;
                _fieldOfView = GraphicsHelper.CreateCircle(Behavior.ViewDistance,
                                                           _world.GraphicsDevice,
                                                           _color);
            }
        }


        /// <summary>
        /// Draw entity if active
        /// </summary>
        public override void Draw (SpriteBatch spriteBatch, bool addDebug)
        {
            if (!Active)
            {
                return;
            }

            // Entity first
            spriteBatch.Draw(_sprite, Pos, null,
			                 _color, (float)Orientation,
                             new Vector2(_sprite.Width / 2, _sprite.Height / 2),
                             Scale, SpriteEffects.None, 1);

            // Then its field of view
            if (addDebug)
            {
                spriteBatch.Draw(_fieldOfView, Pos, null,
                                 _color * 1.0f, 0.0f,
                                 new Vector2(_fieldOfView.Width / 2, _fieldOfView.Height / 2),
                                 Vector2.One, SpriteEffects.None, 1);
            }
        }


        /// <summary>
        /// Loads the content for the teddy bear and prepare texture
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the teddy bear</param>
        private void LoadContent(ContentManager contentManager, string spriteName)
        {
            // Load entity texture
            _sprite = contentManager.Load<Texture2D>(spriteName);

            // Create field of view texture
            _fieldOfView = GraphicsHelper.CreateCircle(Behavior.ViewDistance,
                                                       _world.GraphicsDevice,
                                                       _color);
        }

	}
}
