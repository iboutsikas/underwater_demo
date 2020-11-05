using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boid System Settings", menuName = "ScriptableObjects/Boid System Settings", order = 1)]
public class BoidSettings : ScriptableObject
{
    public float MinSpeed = 2;
    public float MaxSpeed = 5;
    public float PerceptionRadius = 2.5f;
    public float AvoidanceRadius = 1;
    public float MaxSteerForce = 3;

    public float AlignmentWeight = 1;
    public float CohesionWeight = 1;
    public float SeparationWeight = 1;

    public float TargetWeight = 1;
#if USE_LEDERSHIP
    public float LeadershipWeight = 1;
    public float LeadershipVelocity = 10.0f;
#endif
    public float CurlWeight = 1.0f;

    [Header("Collisions")]
    public LayerMask ObstacleMask;
    public float BoundsRadius = .27f;
    public float AvoidCollisionWeight = 10;
    public float CollisionAvoidDst = 5;

}
