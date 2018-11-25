namespace AutonomousAgent
{
    /// <summary>
    /// Use base 2 multiple to make each combinaisons uniques
    /// bin(0)  == b00000
    /// bin(2)  == b00010
    /// bin(4)  == b00100
    /// bin(8)  == b01000
    /// bin(16) == b10000
    /// ...
    /// </summary>
    public enum  BehaviorType
    {
        None=0,
        Seek=2,
        Flee=4,
        Arrive=8,
        Wander=16,
        Cohesion=32,
        Separation=64,
        Alignment=128,
        ObstacleAvoidance=256,
        WallAvoidance=512,
        FollowPath=1024,
        Pursuit=2048,
        Evade=4096,
        Interpose=8192,
        Hide=16384,
        Flock=32768,
        OffsetPursuit=65536,
    }
}
