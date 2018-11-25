using System;
using Microsoft.Xna.Framework;

namespace AutonomousAgent
{

    using Tools.Vector;

    /// <summary>
    /// A entity which can move and can be render to the screen.
    /// It has a steering behavior.
    /// </summary>
    public class MovingEntity : BaseEntity
    {
        // Dynamic support
        protected float _mass;
        protected float _maxSpeed;
        protected float _maxForce;
        protected float _maxRotate;
        protected float _maxRotation = 0.001f;

        // Represent local coordinate system used to go
        // from local to world coordinates using
        // counterclockwise rotation for perp vector
        protected Vector2 _heading;
        protected Vector2 _perp;

        public MovingEntity (Vector2 position, Vector2 velocity,
                             float mass,
                             float maxSpeed, float maxForce,
                             Vector2 scale)
            : base(EntityType.Moving, position, scale, 0, 0)
        {
            Mass = mass;
            MaxSpeed = maxSpeed;
            MaxForce = maxForce;
            // Define start velocity
            Velocity = velocity;
            // Initialize heading using velocity
            UpdateLocalBase();
        }

        /// <summary>
        /// Treat world as toroidal (no check for AABB collisions in the other side)
        /// </summary>
		protected void WrapAround (Rectangle bounds)
        {
			if ( Pos.X > bounds.Width)
            {
                Pos = new Vector2(0, Pos.Y);
            }
            else if ( Pos.X < 0 )
            {
				Pos = new Vector2(bounds.Width - 1, Pos.Y);
            }
			if ( Pos.Y > bounds.Height)
            {
                Pos = new Vector2(Pos.X, 0);
            }
            else if ( Pos.Y < 0 )
            {
                Pos = new Vector2(Pos.X, bounds.Height - 1);
            }
        }

        protected void UpdateLocalBase ()
        {
            // Update the heading if the vehicle has a non zero velocity
            if (Velocity.LengthSquared() > 0.000001)
            {
                Heading = Vector2.Normalize(Velocity);
            }
        }

        /// <summary>
        /// Update entity orientation
        /// </summary>
        public bool RotateTowards (Vector2 targetPos)
        {
            Vector2 desiredDirection = Vector2.Normalize(targetPos - Pos);
            float angle = (float)Math.Acos(Vector2.Dot(_heading, desiredDirection));

            if (angle < 0.0001)
            {
                return true;
            }
            // Clamp
            angle = Math.Min(angle, _maxRotation);

            // Desired direction to the right when determinant(heading, desiredDirection) is negative
            angle = Math.Sign(_heading.SignExt(desiredDirection)) * angle;

            // Now apply rotation
            Velocity = Vector2.Transform(Velocity, Matrix.CreateRotationZ(angle));

            // Update local base based on new velocity
            UpdateLocalBase();
            UpdateOrientation();

            return false;
        }

        /// <summary>
        /// Get corresponding orientation using velocity.
        /// A dynamic approach will used Align behavior.
        /// </summary>
        protected void UpdateOrientation ()
        {
            if (_heading.Length() > 0)
            {
                // Map angle to [-Pi, Pi)
                Orientation = Math.Atan2(_heading.Y, _heading.X);
            }
        }

        public float MaxSpeed
        {
            get{ return _maxSpeed; }
            set
            {
                if ( value > 0 )
                {
                    _maxSpeed = value;
                }
            }
        }

        public float Mass
        {
            get{ return _mass; }
            set
            {
                if ( value > 0 )
                {
                    _mass = value;
                }
            }
        }

        public float MaxForce
        {
            get{ return _maxForce; }
            set
            {
                if ( value > 0 )
                {
                    _maxForce = value;
                }
            }
        }

        public Vector2 Velocity {get; protected set;}

        /// <summary>
        /// Direction heading (normalized velocity)
        /// Update perpendicular vector when heading change
        /// </summary>
        public Vector2 Heading
        {
            get {return _heading;}
            protected set
            {
                _heading = value;
                _perp = _heading.PerpExt();
            }
        }

        /// <summary>
        /// Perpendicular vector of heading direction.
        /// </summary>
        public Vector2 Perp
        {
            get {return _perp;}
        }

        public float Speed
        {
            get { return Velocity.Length(); }
        }

        /// <summary>
        /// Braking radius used to define when agent is close enough to destination
        /// </summary>
        public int BrakeRadius {get; set;}
    }
}


