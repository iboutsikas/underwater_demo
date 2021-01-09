using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public struct BoidData {
    public Vector3 position;
    public Vector3 direction;

    public Vector3 flockHeading;
    public Vector3 flockCentre;
    public Vector3 avoidanceHeading;
    public int numFlockmates;

    public static int Size {
        get {
            return sizeof(float) * 3 * 5 + sizeof(int);
        }
    }
}

public class Boid : MonoBehaviour {
    static Vector3[] Directions;
    static int NumDirectionsToGenerate = 10;

#if USE_LEADERSHIP
    static bool HaveLeader = false;
#endif

    static Boid() {
        Directions = new Vector3[NumDirectionsToGenerate];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < NumDirectionsToGenerate; i++) {
            float t = (float)i / NumDirectionsToGenerate;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            Directions[i] = new Vector3(x, y, z);
        }
    }

    private SpriteRenderer spriteRenderer;
    private Transform target;
    private BoidSettings settings;
    private Vector3 velocity;
    private Vector3 selectedDir;
    private Vector3 leadershipDirection;
    private Vector3 acceleration;
#if USE_LEADERSHIP
    private float leaderAccumulator = 0;
    public bool IsLeader = false;
#endif
    public BoidSpawner spawner;
    public Vector3 Forward => transform.forward;
    public Vector3 Position {
        get { return transform.position; }
    }

    public void Initialize(BoidSettings settings, Transform target) {
        this.target = target;
        this.settings = settings;

        float startSpeed = (settings.MinSpeed + settings.MaxSpeed) / 2;
        //velocity = transform.forward * startSpeed;
        velocity = Vector3.zero;
    }

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSelf(BoidData update) {
        acceleration = Vector3.zero;

        if (target != null) {
            Vector3 toTarget = (target.position - Position);
            acceleration = SteerTowards(toTarget) * settings.TargetWeight;
        }

        Vector3 toFlockCenter = Vector3.one;

        if (update.numFlockmates != 0) {
            update.flockCentre /= update.numFlockmates;
            toFlockCenter = update.flockCentre - Position;
            var alignment = SteerTowards(update.flockHeading) * settings.AlignmentWeight;
            var cohesion = SteerTowards(toFlockCenter) * settings.CohesionWeight;
            var separation = SteerTowards(update.avoidanceHeading) * settings.SeparationWeight;
            // Look into 3D Curl Noise
            acceleration += alignment;
            acceleration += cohesion;
            acceleration += separation;
        }

        if (IsHeadingForCollision()) {
            Vector3 collisionAvoidDir = ObstacleRays();
            selectedDir = collisionAvoidDir;
            Vector3 avoidanceForce = SteerTowards(collisionAvoidDir) * settings.AvoidCollisionWeight;
            acceleration += avoidanceForce;
        }
#if USE_LEADERSHIP
        if (!IsLeader && !HaveLeader && update.numFlockmates != 0)
        {
            var eccentricity = toFlockCenter.magnitude / settings.PerceptionRadius;
            var isEscaping = Vector3.Dot(toFlockCenter.normalized, acceleration.normalized);
            

            var escapeRadius = MathHelpers.GaussianRandom01(0, 1);

            if (eccentricity < escapeRadius && isEscaping < 0.0f)
            {
                IsLeader = true;
                HaveLeader = true;
                leaderAccumulator = 0;
                renderer.color = Color.red;
                transform.localScale = Vector3.one * 10.0f;
                leadershipDirection = toFlockCenter  * settings.LeadershipVelocity;
            }
        }
        else if (IsLeader)
        {
            Debug.DrawRay(transform.position, toFlockCenter, Color.green);

            if (leaderAccumulator >= 30.0f)
            {
                IsLeader = false;
                HaveLeader = false;
                leaderAccumulator = 0;
                renderer.color = Color.white;
                transform.localScale = Vector3.one;
            }
            else
            {
                acceleration += SteerTowards(leadershipDirection * leaderAccumulator) * settings.LeadershipWeight;
                leaderAccumulator += Time.deltaTime;
                //Debug.DrawRay(transform.position, leadershipDirection * leaderAccumulator);
            }
        }
#endif

        //acceleration = Vector3.ClampMagnitude(acceleration, settings.MaxSteerForce);


        velocity += acceleration * Time.deltaTime;
        velocity += CalculateCurlVelocity().normalized * settings.CurlWeight * Time.deltaTime;

        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.MinSpeed, settings.MaxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = dir;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation,
        //    Quaternion.LookRotation(velocity.normalized, Vector3.up),
        //    10.0f * Time.deltaTime
        //    );

        if (Vector3.SqrMagnitude(transform.position - spawner.transform.position) > 40.0f * 40.0f) {
            transform.position = spawner.transform.position;
        }

    }

    Vector3 CalculateCurlVelocity() {
        //var e = 0.0001f;

        int x = (int)Position.x;
        int y = (int)Position.y;
        int z = (int)Position.z;

        var velocity = Vector3.zero;
        // YZ plane
        var n1 = SimplexNoise.Noise.CalcPixel3D(x, y + 1, z, 1);
        var n2 = SimplexNoise.Noise.CalcPixel3D(x, y - 1, z, 1);
        var avgy = (n1 - n2) / 2.0f;

        n1 = SimplexNoise.Noise.CalcPixel3D(x, y, z + 1, 50);
        n2 = SimplexNoise.Noise.CalcPixel3D(x, y, z - 1, 50);
        var avgz = (n1 - n2) / 2.0f;
        velocity.x = avgy - avgz;

        // XZ plane
        n1 = SimplexNoise.Noise.CalcPixel3D(x, y, z + 1, 50);
        n2 = SimplexNoise.Noise.CalcPixel3D(x, y, z - 1, 50);
        avgz = (n1 - n2) / 2.0f;
        
        n1 = SimplexNoise.Noise.CalcPixel3D(x + 1, y, z, 138);
        n2 = SimplexNoise.Noise.CalcPixel3D(x - 1, y, z, 138);
        var avgx = (n1 - n2) / 2.0f;
        velocity.y = avgz - avgx;

        // XY plane
        n1 = SimplexNoise.Noise.CalcPixel3D(x + 1, y, z, 138);
        n2 = SimplexNoise.Noise.CalcPixel3D(x - 1, y, z, 138);
        avgx = (n1 - n2) / 2.0f;

        n1 = SimplexNoise.Noise.CalcPixel3D(x, y + 1, z, 1);
        n2 = SimplexNoise.Noise.CalcPixel3D(x, y - 1, z, 1);
        avgy = (n1 - n2) / 2.0f;
        velocity.z = avgx - avgy;


        return velocity;
    }

    bool IsHeadingForCollision() {
        RaycastHit hit;
        return Physics.SphereCast(
            Position,
            settings.BoundsRadius,
            Forward,
            out hit,
            settings.CollisionAvoidDst,
            settings.ObstacleMask);

    }

    Vector3 ObstacleRays() {
        for (int i = 0; i < NumDirectionsToGenerate; i++) {
            int index = (int)(Random.value * (NumDirectionsToGenerate - 1));

            Vector3 dir = transform.TransformDirection(Directions[index]);
            Ray ray = new Ray(Position, dir);
            if (!Physics.Raycast(ray, settings.CollisionAvoidDst, settings.ObstacleMask)) {
                return dir;
            }
        }

        return Forward;
    }

    Vector3 SteerTowards(Vector3 vector) {
        Vector3 v = vector.normalized * this.settings.MaxSpeed - this.velocity;
        return Vector3.ClampMagnitude(v, settings.MaxSteerForce);
        //return v;
    }

    private void OnDrawGizmos() {
#if USE_LEADERSHIP
        if (IsLeader) {
            Gizmos.DrawWireSphere(transform.position, settings.PerceptionRadius);
            Gizmos.DrawRay(transform.position, leadershipDirection);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, acceleration * Time.deltaTime);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, velocity);
        }
#endif
#if false
        var dir = transform.forward * settings.CollisionAvoidDst;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dir);
        Gizmos.DrawWireSphere(transform.position + dir, settings.BoundsRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, selectedDir);
#endif

#if false
        Gizmos.color = Color.magenta;

        for (int i = 0; i < NumDirectionsToGenerate; i++)
        {
            dir = transform.TransformDirection(Directions[i]) * settings.CollisionAvoidDst;
            Gizmos.DrawRay(transform.position, dir);
        }
#endif
    }
}
