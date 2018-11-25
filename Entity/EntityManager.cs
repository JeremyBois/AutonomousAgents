using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AutonomousAgent
{
    public class EntityManager : IEnumerable<KeyValuePair<int, BaseEntity>>
    {

        private Dictionary<int, BaseEntity> _map;


        public Dictionary<int, BaseEntity> EntitiesMap
        {
            get {return _map;}
        }

        public EntityManager()
        {
            _map = new Dictionary<int, BaseEntity>();
        }

        public BaseEntity this[int uniqueID]
        {
            get
            {
                return (_map.ContainsKey(uniqueID)) ? _map[uniqueID] : null;
            }

            set
            {
                _map[uniqueID] = value;
            }
        }

        public bool Add(BaseEntity entity)
        {
            _map.Add(entity.Id, entity);
            return true;
        }

        public bool Remove(int uniqueID)
        {
            if (!_map.ContainsKey(uniqueID))
            {
                return false;
            }

            _map.Remove(uniqueID);
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, bool debugOn)
        {
            foreach(BaseEntity entity in _map.Values)
            {
                entity.Draw(spriteBatch, debugOn);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach(BaseEntity entity in _map.Values)
            {
                entity.Update(gameTime);
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }


        IEnumerator<KeyValuePair<int, BaseEntity>> IEnumerable<KeyValuePair<int, BaseEntity>>.GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        public IEnumerable<BaseEntity> Entities
        {
            get{return _map.Values;}
        }


        public void TagEntities(BaseEntity entityRef, int distance)
        {
            foreach (BaseEntity entity in _map.Values)
            {
                // First make sur all entities are untag
                entity.UnTag();

                // Get distance from ref
                Vector2 posDiff = entity.Pos - entityRef.Pos;

                // Account for bounding radius (physic boundaries of the object)
                double radius = distance + entity.BRadius;

                // Tag if different from reference and close enough to reference
                if (entity != entityRef && (posDiff.LengthSquared() < (radius * radius)))
                {
                    entity.Tag();
                }
            }
        }

        /// <summary>
        /// Get closest moving entities based on distance radius.
        /// </summary>
        public List<MovingEntity> GetClosestMovingEntities(MovingEntity entityRef, int distance)
        {
            var _entities = new List<MovingEntity>();

            foreach (BaseEntity entity in _map.Values)
            {
                // Try to cast to MovingEntiy
                var movingEntity = entity as MovingEntity;

                // If not a movingEntity just continue the loop
                if (movingEntity != null)
                {
                    // Get distance from ref
                    Vector2 posDiff = movingEntity.Pos - entityRef.Pos;

                    // Account for bounding radius (physic boundaries of the object)
                    double radius = distance + movingEntity.BRadius;

                    // Tag if different from reference and close enough to reference
                    if (movingEntity != entityRef && (posDiff.LengthSquared() < (radius * radius)))
                    {
                        _entities.Add(movingEntity);
                    }
                }
            }

            // Return all entities closed enough
            return _entities;
        }
    }
}
