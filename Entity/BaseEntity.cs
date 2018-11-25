using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AutonomousAgent
{

    /// <summary>
    /// Base class used to construct all game object
    /// </summary>
    public class BaseEntity
    {

    // STATIC
        // Used to define unique id for new game entities
        static int currentID;

        /// <summary>
        /// Used to set a unique id to each entity
        /// </summary>
        static private int NextValidID ()
        {
          return BaseEntity.currentID++;
        }

    // OBJECT
        // A flag that can be used to whatever you want
        private bool _tag;

        /// <summary>
        /// Base Constructor.
        /// </summary>
        public BaseEntity (EntityType entityType, Vector2 pos, Vector2 scale,
                           float orientation, float radius)
        {
            Id = BaseEntity.NextValidID();
            Type = entityType;
            Pos = pos;
            Scale = scale;
            Orientation = orientation;
            BRadius = radius;
            Active = false;
        }

        /// <summary>
        /// Auto constructor with default values.
        /// </summary>
        public BaseEntity ()
            : this(EntityType.Default, Vector2.Zero, Vector2.One, 0, 0)
        {}

        /// <summary>
        /// Update entity according to gameTime
        /// </summary>
        public virtual void Update (GameTime gameTime){}

        /// <summary>
        /// Update entity according to gameTime
        /// </summary>
        public virtual void Draw (SpriteBatch spriteBatch, bool addDebug){}

        /// <summary>
        /// Set tag to true.
        /// </summary>
        public void Tag () { _tag = true; }

        /// <summary>
        /// Set tag to false
        /// </summary>
        public void UnTag () { _tag = false; }

        // Properties
        public bool Active {get; set;}

        /// <summary>
        /// Return agent unique ID.
        /// </summary>
        public int Id {get;}

        /// <summary>
        /// Return agent scale.
        /// </summary>
        public Vector2 Scale {get; set;}

        /// <summary>
        /// Return current position.
        /// </summary>
        public Vector2 Pos {get; set;}

        /// <summary>
        /// Return entity type.
        /// </summary>
        public EntityType Type {get; set;}

        /// <summary>
        /// Return agent orientation.
        /// </summary>
        public double Orientation {get; protected set;}

        /// <summary>
        /// Return tag state.
        /// </summary>
        public bool IsTagged
        {
            get { return _tag; }
        }

        /// <summary>
        /// Length of circle approximation used for collision detection
        /// </summary>
        public double BRadius {get; protected set;}
    }
}
