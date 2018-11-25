using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AutonomousAgent
{
    // Enable access to extensions methods
    using Tools;
    using Tools.Vector;

    /// <summary>
    /// Steering Behavior class
    /// </summary>
    public class SteeringBehavior
    {
    // Globals
        // Reference to agent link to this behavior
        private Vehicle _agent;
        // A binary code used to store active behavior
        private int _activeBehaviors;
        // How far the agent can 'see'
        int _viewDistance;

    // Wandering parameters
        // Wandering strength (capacity to get more orientation)
        private float _wanderingRadius;
        // Projection of wander circle in front of agent
        private float _wanderingDistance;
        // Max amount of random displacement that can be add each second
        private double _wanderingJitter;
        // Wander circle around target
        private Vector2 _wanderingTarget;

    // Debug purpose
        public Texture2D wanderTarget;
        public Vector2 wanderTargetPos;
        public Vector2 wanderCirclePos;


    // Group behavior purpose
        private MovingEntity[] _neighbors;


    // // Obstacle avoidance parameters
    //     private float _minDectectionBoxLength;


    // Offset used for offsetPursuit
        Vector2 _offset = new Vector2(20, 20);

    // Weights for Weight sum calculation
        Dictionary<BehaviorType, float> _weights = new Dictionary<BehaviorType, float>
            {
                {BehaviorType.Seek, 1 },
                {BehaviorType.Flee, 1 },
                {BehaviorType.Arrive, 1 },
                {BehaviorType.Pursuit, 1 },
                {BehaviorType.Evade, 1 },
                {BehaviorType.Wander, 1 },
                {BehaviorType.Separation, 1 },
                {BehaviorType.Alignment, 1 },
                {BehaviorType.Cohesion, 1 }
            };



    // Properties
        /// <summary>
        /// Get target of steering behavior agent
        /// </summary>
        public Vehicle Target { get; set; }
        public Vehicle ToAvoid { get; set; }

        /// <summary>
        /// Property for steering force attribute
        /// </summary>
        public Vector2 SteeringForce { get; private set; }

        /// <summary>
        /// Summing method used to calculate the steering force.
        /// </summary>
        public CombinaisonMethod CombinaisonMethodUsed  { get; set; }

        /// <summary>
        /// Get steering behavior agent
        /// </summary>
        public Vehicle Agent
        {
            get {return _agent;}
        }

        /// <summary>
        /// Accessor to wandering radius
        /// </summary>
        public float WanderingRadius
        {
            get { return _wanderingRadius; }
            set { _wanderingRadius = value; }
        }

        /// <summary>
        /// Accessor to wandering jitter
        /// </summary>
        public double WanderingJitter
        {
            get { return _wanderingJitter; }
            set { _wanderingJitter = value; }
        }

        /// <summary>
        /// Accessor to wandering distance
        /// </summary>
        public float WanderingDistance
        {
            get { return _wanderingDistance; }
            set { _wanderingDistance = value; }
        }

        /// <summary>
        /// Accessor to wandering distance
        /// </summary>
        private Vector2 WanderingTarget
        {
            get { return _wanderingTarget; }
            set { _wanderingTarget = value; }
        }

        // /// <summary>
        // /// Accessor to minimal length of detection box.
        // /// </summary>
        // public float MinDetectionBoxLength
        // {
        //     get { return _minDectectionBoxLength; }
        // }

        /// <summary>
        /// Accessor to maximal view for the agent
        /// </summary>
        public int ViewDistance
        {
            get { return _viewDistance; }
            set
            {
                if ( value > 0 )
                {
                    _viewDistance = value;
                }
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public SteeringBehavior (Vehicle agent)
        {
            // Assign a Vehicle to it
            _agent = agent;

            // Default values for wandering
            _wanderingRadius = 25f;
            _wanderingJitter = 1000.0f;
            _wanderingDistance = 150.0f;

            // Initialize a random position of wander circle
            double theta = RandomHelper.NextDouble() * MathHelper.TwoPi;
            WanderingTarget = new Vector2((float)(WanderingRadius * Math.Cos(theta)),
                                          (float)(WanderingRadius * Math.Sin(theta)));
        }


        /// <summary>
        /// Reset all active behavior
        /// </summary>
        public void CleanAllBehavior ()
        {
            _activeBehaviors = (int)BehaviorType.None;
        }

        // Test if a property is active
        public bool IsFleeOn          {get {return IsOn(BehaviorType.Flee); }}
        public bool IsSeekOn          {get {return IsOn(BehaviorType.Seek); }}
        public bool IsArriveOn        {get {return IsOn(BehaviorType.Arrive); }}
        public bool IsPursuitOn       {get {return IsOn(BehaviorType.Pursuit); }}
        public bool IsEvadeOn         {get {return IsOn(BehaviorType.Evade); }}
        public bool IsWanderOn        {get {return IsOn(BehaviorType.Wander); }}
        public bool IsAlignmentOn     {get {return IsOn(BehaviorType.Alignment); }}
        public bool IsCohesionOn      {get {return IsOn(BehaviorType.Cohesion); }}
        public bool IsSeparationOn    {get {return IsOn(BehaviorType.Separation); }}
        public bool IsOffsetPursuitOn {get {return IsOn(BehaviorType.OffsetPursuit); }}



        /// <summary>
        /// Set a specific behavior weight (value must be greater than 0)
        /// Return true if behavior has been set else false.
        /// </summary>
        public bool AssignWeight(BehaviorType behavior, float value)
        {
            if (_weights.ContainsKey(behavior) && value > 0 )
            {
                _weights[behavior] = value;
                return true;
            }
            return false;
        }



        /// <summary>
        /// Calculate the steering force according to states (not thread safe at all).
        /// </summary>
        public Vector2 Calculate(double timeElapsed)
        {
            // Reinit steering force at each call to calculate
            SteeringForce = Vector2.Zero;

            // Only check neighbors if needed
            if ( IsSeparationOn || IsAlignmentOn || IsCohesionOn)
            {
                // Tag once for all group behaviors
                // Agent.World.SceneManager.Current.Entities.TagEntities(Agent, ViewDistance);
                _neighbors = Agent.World.SceneManager.Current.Entities.GetClosestMovingEntities(Agent, ViewDistance).ToArray();
            }

            switch ( CombinaisonMethodUsed )
            {
                case CombinaisonMethod.Weighted:
                    // Calculate using weighted sum
                    CalculateWeightsSum(timeElapsed);
                    break;
                case CombinaisonMethod.PrioritizedAndWeighted:
                    //Calculate using prioritized sum
                    CalculatePrioritizedWithWeights(timeElapsed);
                    break;
            }

            return SteeringForce;
        }


        /// <summary>
        /// Calculate forces using weights map to each behavior.
        /// Currently all behavior have the same weight.
        /// </summary>
        private void CalculateWeightsSum(double timeElapsed)
        {
            // Check for each behavior
            if ( IsSeekOn )       { SteeringForce += Seek(Target.Pos) * _weights[BehaviorType.Seek]; }
            if ( IsFleeOn )       { SteeringForce += Flee(ToAvoid.Pos) * _weights[BehaviorType.Flee]; }
            if ( IsArriveOn )     { SteeringForce += Arrive(Target.Pos) * _weights[BehaviorType.Arrive]; }
            if ( IsPursuitOn )    { SteeringForce += Pursuit(Target) * _weights[BehaviorType.Pursuit]; }
            if ( IsEvadeOn )      { SteeringForce += Evade(ToAvoid) * _weights[BehaviorType.Evade]; }
            if ( IsWanderOn )     { SteeringForce += Wander(timeElapsed) * _weights[BehaviorType.Wander]; }
            if ( IsSeparationOn ) { SteeringForce += Separation(_neighbors) * _weights[BehaviorType.Separation]; }
            if ( IsAlignmentOn )  { SteeringForce += Alignment(_neighbors) * _weights[BehaviorType.Alignment]; }
            if ( IsCohesionOn )   { SteeringForce += Cohesion(_neighbors) * _weights[BehaviorType.Cohesion]; }
            // if ( IsOffsetPursuitOn )   { SteeringForce += OffsetPursuit(Target, new Vector2(30f, 30f)) * _weights["Cohesion"]; }

            // Make sure force does not exceed max agent Force
            SteeringForce = SteeringForce.TruncateExt(Agent.MaxForce);
        }

        /// <summary>
        /// Calculate forces using weights and prioritization.
        /// Method return when max force is reached.
        /// </summary>
        private void CalculatePrioritizedWithWeights (double timeElapsed)
        {
            Vector2 force;

            // Check for each behavior in a defined order
            if ( IsEvadeOn )
            {
                force = Evade(ToAvoid) * _weights[BehaviorType.Evade];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsFleeOn )
            {
                force = Flee(ToAvoid.Pos) * _weights[BehaviorType.Flee];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsSeparationOn )
            {
                force = Separation(_neighbors) * _weights[BehaviorType.Separation];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsAlignmentOn )
            {
                force = Alignment(_neighbors) * _weights[BehaviorType.Alignment];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsCohesionOn )
            {
                force = Cohesion(_neighbors) * _weights[BehaviorType.Cohesion];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsSeekOn )
            {
                force = Seek(Target.Pos) * _weights[BehaviorType.Seek];
                if ( !AccumulateForce(force) ) { return; }
            }

            // if ( IsOffsetPursuitOn )
            // {
            //     force = OffsetPursuit(Target, _offset) * _weights["Cohesion"];
            //     if ( !AccumulateForce(force) ) { return; }
            // }

            if ( IsArriveOn )
            {
                force = Arrive(Target.Pos) * _weights[BehaviorType.Arrive];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsWanderOn )
            {
                force = Wander(timeElapsed) * _weights[BehaviorType.Wander];
                if ( !AccumulateForce(force) ) { return; }
            }
            if ( IsPursuitOn )
            {
                force = Pursuit(Target) * _weights[BehaviorType.Pursuit];
                if ( !AccumulateForce(force) ) { return; }
            }

        }



        /// <summary>
        /// Calculate remaining steering force and return false if no more can
        /// be added. Return true if still remaining force that can be added.
        /// </summary>
        private bool AccumulateForce (Vector2 forceToAdd)
        {
            // Get steering force  remaining
            double remainingStrength = Agent.MaxForce - SteeringForce.Length();

            // Max exceeed add nothing and return false to stop steering computation
            if ( remainingStrength <= 0 )
            {
                return false;
            }

            // We can add new steering force as it
            if ( forceToAdd.Length() < remainingStrength )
            {
              SteeringForce += forceToAdd;
            }
            // We can add new steering force but we need to limit it
            else
            {
                SteeringForce += Vector2.Multiply(Vector2.Normalize(forceToAdd), (float)remainingStrength);
            }

            return true;
        }


        /// <summary>
        /// Return true if behavior active in the steering behavior
        /// </summary>
        private bool IsOn (BehaviorType behavior)
        {
            return (_activeBehaviors & (int)behavior) == (int)behavior;
        }

        /// <summary>
        /// Return True if a behavior is active.
        /// </summary>
        public bool HasActiveBehavior ()
        {
            return _activeBehaviors != (int)BehaviorType.None;
        }

        // Activator for each behavior hiding behavior type
        public void FleeOn ()          { _activeBehaviors |= (int)BehaviorType.Flee; }
        public void SeekOn ()          { _activeBehaviors |= (int)BehaviorType.Seek; }
        public void ArriveOn ()        { _activeBehaviors |= (int)BehaviorType.Arrive; }
        public void PursuitOn ()       { _activeBehaviors |= (int)BehaviorType.Pursuit; }
        public void EvadeOn ()         { _activeBehaviors |= (int)BehaviorType.Evade; }
        public void WanderOn ()        { _activeBehaviors |= (int)BehaviorType.Wander; }
        public void CohesionOn ()      { _activeBehaviors |= (int)BehaviorType.Cohesion; }
        public void SeparationOn ()    { _activeBehaviors |= (int)BehaviorType.Separation; }
        public void AlignmentOn ()     { _activeBehaviors |= (int)BehaviorType.Alignment; }
        public void OffsetPursuitOn () { _activeBehaviors |= (int)BehaviorType.OffsetPursuit; }

        // De-Activator for each behavior hiding behavior type
        public void FleeOff ()          { if ( IsOn(BehaviorType.Flee) ) _activeBehaviors ^= (int)BehaviorType.Flee; }
        public void SeekOff ()          { if ( IsOn(BehaviorType.Seek) ) _activeBehaviors ^= (int)BehaviorType.Seek; }
        public void ArriveOff ()        { if ( IsOn(BehaviorType.Arrive) ) _activeBehaviors ^= (int)BehaviorType.Arrive; }
        public void PursuitOff ()       { if ( IsOn(BehaviorType.Pursuit) ) _activeBehaviors ^= (int)BehaviorType.Pursuit; }
        public void EvadeOff ()         { if ( IsOn(BehaviorType.Evade) ) _activeBehaviors ^= (int)BehaviorType.Evade; }
        public void WanderOff ()        { if ( IsOn(BehaviorType.Wander) ) _activeBehaviors ^= (int)BehaviorType.Wander; }
        public void CohesionOff ()      { if ( IsOn(BehaviorType.Cohesion) ) _activeBehaviors ^= (int)BehaviorType.Cohesion; }
        public void SeparationOff ()    { if ( IsOn(BehaviorType.Separation) ) _activeBehaviors ^= (int)BehaviorType.Separation; }
        public void AlignmentOff ()     { if ( IsOn(BehaviorType.Alignment) ) _activeBehaviors ^= (int)BehaviorType.Alignment; }
        public void OffsetPursuitOff () { if ( IsOn(BehaviorType.OffsetPursuit) ) _activeBehaviors ^= (int)BehaviorType.OffsetPursuit; }


        /// <summary>
        /// Make orientation of agent factor by some time.
        /// A coefficient to scale the returned value
        /// The higher the max turn rate of the vehicle, the higher `coefDelay` value should be.
        /// If the vehicle is heading in the opposite direction to its target position,
        /// a `coefDelay` of 0.5 return a delay of 1 second.
        /// </summary>
        /// <param name="entity">The entity direction that must be delay.</param>
        /// <param name="targetPos">The position that the target must reached.</param>
        /// <param name="coefDelay">A coefficient to delay the modification of direction.</param>
        public double TurnArroundTime (MovingEntity entity, Vector2 targetPos, double coefDelay=0.5)
        {
            Vector2 desiredDirection = Vector2.Normalize(targetPos - entity.Pos);
            // Give a value in [-1, 1] where 1 means ahead target and -1 behind
            float cos_angle = Vector2.Dot(entity.Heading, desiredDirection);
            // Substrate 1 to add no change when already facing desired orientation
            // Multiply by coef to define the scaling
            // Multiply by -1 to get a positive value
            return (cos_angle - 1) * coefDelay * -1;
        }


        /// <summary>
        /// Seek method
        /// </summary>
        private Vector2 Seek (Vector2 targetPos)
        {
            Vector2 desiredDirection = targetPos - Agent.Pos;  // Opposite to Flee
            // Always at max speed
            desiredDirection.Normalize();

            // Return steering force
            return Vector2.Multiply(desiredDirection, Agent.MaxSpeed) - Agent.Velocity;

        }

        /// <summary>
        /// Flee method
        /// </summary>
        private Vector2 Flee (Vector2 toAvoidPos)
        {
            // Only flee if the target is within 'panic distance'
            // Avoid square root using squared distance
			if (Vector2.DistanceSquared(Agent.Pos, toAvoidPos) > (_viewDistance * _viewDistance))
            {
                return Vector2.Zero;
            }
            Vector2 desiredDirection = Agent.Pos - toAvoidPos;  // Opposite to Seek
            desiredDirection.Normalize();

            // Return steering force
            return Vector2.Multiply(desiredDirection, Agent.MaxSpeed) - Agent.Velocity;
        }

        /// <summary>
        /// Arrive method using radius with decreasing speed
        /// </summary>
        private Vector2 Arrive (Vector2 targetPos)
        {
            Vector2 desiredDirection = targetPos - Agent.Pos;
            float distance = desiredDirection.Length();
            desiredDirection.Normalize();
            Vector2 desiredVelocity = Vector2.Multiply(desiredDirection, Agent.MaxSpeed);

            // If close enough to zero assume it’s zero
            if (distance <= 0.000001)
            {
                desiredVelocity = Vector2.Zero;
            }

            else if (distance < Agent.BrakeRadius)
            {
                // Reduce velocity according to target distance
                desiredVelocity *= (float)(Math.Exp(-Agent.BrakeRadius / distance));
            }

            return desiredVelocity - Agent.Velocity;
        }


        /// <summary>
        /// Pursuit account for target next position to intercept it.
        /// </summary>
        private Vector2 Pursuit (MovingEntity target)
        {
            Vector2 targetPos = target.Pos;
            Vector2 toTarget = targetPos - Agent.Pos;
            float cos_angle = Vector2.Dot(Agent.Heading, target.Heading);
            // Face if angle between agents head is lower than 20°
            // (180 - 20 used to get angle when both facing each other)
            if ( cos_angle < Math.Cos(MathHelper.ToRadians(180 - 20)) &&
                 Vector2.Dot(toTarget, Agent.Heading) > 0 )
            {
                // Target is ahead and facing the agent
                return Seek(targetPos);
            }
            // Target is not ahead, find next position
            // Assuming MaxSpeed for Agent and current speed for target
            double timeNextPosition = toTarget.Length() /
                                      (Agent.MaxSpeed + target.Speed);

            // Delay orientation
            timeNextPosition += TurnArroundTime(Agent, target.Pos, 0.05);

            return Seek(target.Pos + Vector2.Multiply(target.Velocity, (float)timeNextPosition));
        }

        /// <summary>
        /// Evade from another agent. Opposite to pursuit. This time predicted
        /// next position is used to Flee from agent.
        /// </summary>
        private Vector2 Evade (MovingEntity pursuer)
        {
            Vector2 pursuerPos = pursuer.Pos;
            Vector2 notDesiredDirection = pursuerPos - Agent.Pos;
            // Assuming MaxSpeed for Agent and current speed for pursuer
            float timeNextPosition = notDesiredDirection.Length() /
                (Agent.MaxSpeed + pursuer.Speed);
            // Flee predicted position
            return Flee(pursuerPos + Vector2.Multiply(pursuer.Velocity, timeNextPosition));
        }


        /// <summary>
        /// Wander behavior.
        /// This behavior is defined by the values of:
        ///     - WanderingRadius    --> Strength of wander
        ///     - WanderingDistance  --> Distance ahead the agent where wander circle is drawn
        ///     - WanderingJitter    --> Max amount of random displacement that can be add each second
        /// Can also be compute using Perlin noise.
        /// </summary>
        private Vector2 Wander (double timeElapsed)
        {
            // Jitter scale according to time elapsed
            double jitterTimeSliced = timeElapsed * WanderingJitter;
            // Create wander target direction using a binomial distribution (-1 ; 1)
            WanderingTarget += new Vector2((float) (RandomHelper.NextBinomial() * jitterTimeSliced),
                                           (float) (RandomHelper.NextBinomial() * jitterTimeSliced));

            // Get normalized direction (projection on a unit circle)
            // Be aware that we cannot normalize using the property because only the copy will be normalized
            // Indeed the Vector is a structure not a class
            _wanderingTarget.Normalize();

            // Scale based on circle radius
            WanderingTarget *= WanderingRadius;
            // Project target ahead of agent
            Vector2 targetLocal = WanderingTarget + new Vector2(WanderingDistance, 0);

            // Get world position based on local basis
            // Vector2 targetWorld = TransformHelper.LocalToWorld(targetLocal, Agent.Heading, Agent.Perp, Agent.Pos);
            Vector2 targetWorld = TransformHelper.LocalToWorldByMatrix(targetLocal, Agent.Heading, Agent.Perp, Agent.Pos);

            // Only used for debug purpose
            wanderTargetPos = targetWorld;
            wanderCirclePos = TransformHelper.LocalToWorldByMatrix(new Vector2(WanderingDistance, 0), Agent.Heading, Agent.Perp, Agent.Pos);

            // Return a force of attraction to random target
            return Seek(targetWorld);
        }


        /// <summary>
        /// Group behavior: Separation make agent kept a separation distance
        /// between all agents. This minimal distance is computed based on Agent BRadius.
        /// </summary>
        private Vector2 Separation (MovingEntity[] vehicles)
        {
            // Minimal squared distance between vehicles (size dependant)
            double desiredSquaredDistance = Math.Pow(Agent.BRadius * 2, 2);

            // Counter used to average vehicles forces
            int neighborsCount = 0;
            Vector2 desiredDirection = Vector2.Zero;

            foreach (MovingEntity neighbor in vehicles)
            {
                // Make sure it’s not the current agent and it’s different from the target to avoid
                if (neighbor != Agent && neighbor != ToAvoid)
                {
                    Vector2 targetPos = neighbor.Pos;
                    double distanceSquared = Vector2.DistanceSquared(Agent.Pos, targetPos);
                    // Avoid divide by 0 and only keep close enough entities
                    if ( (distanceSquared > 0) && (distanceSquared < desiredSquaredDistance) )
                    {
                        // Get difference between vehicles using updated position
                        Vector2 force = Agent.Pos - targetPos;
                        force.Normalize();
                        // Scale force by distanceSquared (inversely proportional)
                        force /= (float)distanceSquared;
                        desiredDirection += force;
                        neighborsCount++;
                    }
                }
            }
            if ( neighborsCount > 0 )
            {
                // Average force
                desiredDirection /= neighborsCount;
                // Then normalize it
                Vector2 desiredVelocity = Vector2.Multiply(Vector2.Normalize(desiredDirection), Agent.MaxSpeed);
                // Return steering force
                return desiredVelocity - Agent.Velocity;
            }
            return Vector2.Zero;
        }


        /// <summary>
        /// Group behavior: Alignment try to match vehicles average heading
        /// </summary>
        private Vector2 Alignment (MovingEntity[] vehicles)
        {
            Vector2 desiredVelocity = Vector2.Zero;
            int neighborsCount = 0;

            foreach (MovingEntity neighbor in vehicles)
            {
                // Make sure it’s not the current agent and it’s different from the target to avoid
                if (neighbor != Agent && neighbor != ToAvoid)
                {
                    desiredVelocity += neighbor.Velocity;
                    neighborsCount ++;
                }
            }

            if (neighborsCount > 0)
            {
                desiredVelocity /= neighborsCount;
                desiredVelocity = Vector2.Multiply(Vector2.Normalize(desiredVelocity), Agent.MaxSpeed);
                return desiredVelocity - Agent.Velocity;
            }

            return desiredVelocity;
        }

        /// <summary>
        /// Group behavior: Cohesion attract vehicles.
        /// </summary>
        private Vector2 Cohesion (MovingEntity[] vehicles)
        {
            Vector2 centerOfFlock = Vector2.Zero;
            Vector2 force = Vector2.Zero;
            int neighborsCount = 0;

            foreach (MovingEntity neighbor in vehicles)
            {
                // Make sure it’s not the current agent and it’s different from the target to avoid
                if (neighbor != Agent && neighbor != ToAvoid)
                {
                    // Get correct position in a toroidal world
                    centerOfFlock += neighbor.Pos;
                    neighborsCount++;
                }
            }
            if (neighborsCount > 0)
            {
                centerOfFlock /= neighborsCount;
                force = Seek(centerOfFlock);
            }
            return force;
        }

    //     /// <summary>
    //     /// OffsetPursuit try to follow a leader with a specific offset between them.
    //     /// Avoid being in front of leader and avoid blocking other entities field
    //     /// of view.
    //     /// </summary>
    //     private Vector2 OffsetPursuit (MovingEntity leader, Vector2 offset)
    //     {
    //         // Get offset in world coordinates
    //         Vector2 offsetPos = LocalToWorld(offset, leader.Orientation, leader.Pos);
    //         // Get correct position in a toroidal world
    //         offsetPos = Agent.World.ClosestPos(Agent.Pos, offsetPos);
    //         Vector2 desiredDirection = offsetPos - Agent.Pos;
    //         // Get time using current speed of both entities
    //         float timeNextPosition = desiredDirection.Length() /
    //             (Agent.MaxSpeed + leader.Speed);

    //         // Send event
    //         OnOffsetPosCall(offsetPos);

    //         // Arrive used to always stay behind
    //         return Arrive(offsetPos + Vector2.Multiply(leader.Velocity, timeNextPosition));

    //     }

    //     /// <summary>
    //     /// Obstacle avoiding behavior.
    //     /// </summary>
    //     /// <param name="obstacles">A list of obstacles to avoid.</param>
    //     private Vector2 ObstacleAvoidance (GameObject[] obstacles)
    //     {
    //         // Detection box proportional to velocity of agent
    //         float boxLength = MinDetectionBoxLength +
    //             MinDetectionBoxLength * ( Agent.Speed / Agent.MaxSpeed);

    //         GameObject closerObstacle;

    //         // Select obstacles in the circle of box length radius

    //         // Convert selected obstacles to agent coordinates

    //         // Loop over selected obstacles

    //             // Select obstacles with positive x values (ahead of target)

    //             // Select obstacles where absolute y value is less than agent circle

    //             // Test for intersection between agent box and obstacle

    //             // If closer than current obstacle to avoid replace it

    //         // If an obstacle to avoid

    //             // Calculate Steering force

    //         // Apply no force if no obstacle as target
    //         return Vector2.Zero;

    //     }

    //     /// <summary>
    //     /// Return world coordinates from local ones (agent).
    //     /// </summary>
    //     /// <param name="position">Local coordinate to transform to world one.</param>
    //     /// <param name="orientation">Reference object orientation in world.</param>
    //     /// <param name="reference">Reference object position in world.</param>
    //     public Vector2 LocalToWorld (Vector2 position, double orientation, Vector2 reference)
    //     {
    //         // Create the matrix transform
    //         // Inverse rotation matrix because we are going from local to world space ?
    //         Matrix toWorldMatrix = Matrix.Invert(Matrix.CreateRotationZ((float)orientation)) *
    //                                Matrix.CreateTranslation(new Vector3(reference.X, reference.Y, 0));
    //         // Matrix toWorldMatrix = Matrix.CreateTranslation(new Vector3(reference.X, reference.Y, 0));
    //             // Matrix.Invert(Matrix.CreateRotationZ((float)orientation));
    //         return Vector2.Transform(position, toWorldMatrix);
    //     }

    //     /// <summary>
    //     /// Return local coordinates (agent) using world ones.
    //     /// </summary>
    //     /// <param name="position">World coordinate to transform to local one.</param>
    //     /// <param name="orientation">Reference object orientation in world.</param>
    //     /// <param name="reference">Reference object position in world.</param>
    //     public Vector2 WorldToLocal (Vector2 position, double orientation, Vector2 reference)
    //     {
    //         // Create the matrix transform
    //         Matrix toWorldMatrix = Matrix.Invert(Matrix.CreateRotationZ((float)orientation)) *
    //                                Matrix.CreateTranslation(new Vector3(reference.X, reference.Y, 0));
    //         return Vector2.Transform(position, Matrix.Invert(toWorldMatrix));
    //     }


    //     /// <summary>
    //     /// Wrap an angle in radians to [-Pi, Pi).
    //     /// Not needed when using Math.Atan2 because it already compute the
    //     /// angle in that range.
    //     /// </summary>
    //     /// <param name="angle">Angle to wrap to the range [-Pi, Pi).</param>
    //     public static double WrapAngle (double angle)
    //     {
    //         return (angle + Math.PI) % (MathHelper.TwoPi) - Math.PI;
    //     }



        // /// <summary>
        // /// Accessor to minimal length of detection box.
        // /// </summary>
        // public float MinDetectionBoxLength
        // {
        //     get { return _minDectectionBoxLength; }
        // }

        // /// <summary>
        // /// Accessor to maximal view for the agent
        // /// </summary>
        // public double ViewDistance
        // {
        //     get { return _viewDistance; }
        //     set
        //     {
        //         if ( value > 0 )
        //         {
        //             _viewDistance = value;
        //             OnDistanceViewUpdate();
        //         }
        //     }
        // }








    // // Events
    //     public event Action FieldOfViewUpdated;
    //     public event Action<Vector2> ArriveDestination;
    //     public event Action<Vector2> OffsetPos;

    //     /// <summary>
    //     /// Call when updating the field of view
    //     /// </summary>
    //     private void OnDistanceViewUpdate ()
    //     {
    //         if ( FieldOfViewUpdated != null )
    //         {
    //             FieldOfViewUpdated.Invoke();
    //         }
    //     }

    //     /// <summary>
    //     /// Call when arrive define a target to reach
    //     /// </summary>
    //     private void OnArriveBehaviorCall (Vector2 target)
    //     {
    //         if ( ArriveDestination != null )
    //         {
    //             ArriveDestination.Invoke(target);
    //         }
    //     }

    //     /// <summary>
    //     /// Call when arrive define a target to reach
    //     /// </summary>
    //     private void OnOffsetPosCall (Vector2 target)
    //     {
    //         if ( OffsetPos != null )
    //         {
    //             OffsetPos.Invoke(target);
    //         }
    //     }

    }
}
