using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BoidSystem : MonoBehaviour
{
    public BoidSettings SystemSettings;
    public ComputeShader ComputeShader;
    public float Width = 20;
    public float Height = 20;
    public float Depth = 20;
    public GameObject WallPrefub;

    Boid[] boids;

    private void Awake() {
        // + X
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall +X";
            go.transform.localScale = new Vector3(0.1f, Height, Depth);
            var lpos = go.transform.localPosition;
            lpos.x = Width * 0.5f;
            go.transform.localPosition = lpos;
        }

        // - X
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall -X";
            go.transform.localScale = new Vector3(0.1f, Height, Depth);
            var lpos = go.transform.localPosition;
            lpos.x = -Width * 0.5f;
            go.transform.localPosition = lpos;
        }

        // + Y
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall +Y";
            go.transform.localScale = new Vector3(Width, 0.1f, Depth);
            var lpos = go.transform.localPosition;
            lpos.y = Height * 0.5f;
            go.transform.localPosition = lpos;
        }

        // - Y
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall -Y";
            go.transform.localScale = new Vector3(Width, 0.1f, Depth);
            var lpos = go.transform.localPosition;
            lpos.y = -Height * 0.5f;
            go.transform.localPosition = lpos;
        }

        // + Z
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall +Z";
            go.transform.localScale = new Vector3(Width, Height, 0.1f);
            var lpos = go.transform.localPosition;
            lpos.z = Depth * 0.5f;
            go.transform.localPosition = lpos;
        }

        // - Z
        {
            var go = Instantiate(WallPrefub, transform);
            go.name = "Wall -Z";
            go.transform.localScale = new Vector3(Width, Height, 0.1f);
            var lpos = go.transform.localPosition;
            lpos.z = -Depth * 0.5f;
            go.transform.localPosition = lpos;
        }

    }

    private void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid boid in boids)
        {
            boid.Initialize(SystemSettings, null);
        }

    }

    private void Update()
    {
        if (boids == null)
            return;

        var boidData = new BoidData[boids.Length];// new NativeArray<BoidData>(boids.Length, Allocator.TempJob);

        for (int i = 0; i < boids.Length; i++)
        {
            boidData[i].position = boids[i].Position;
            boidData[i].direction = boids[i].Forward;
        }

        ComputeBuffer buffer = new ComputeBuffer(boids.Length, BoidData.Size);

        buffer.SetData(boidData);

        ComputeShader.SetBuffer(0, "boids", buffer);
        ComputeShader.SetInt("numBoids", boids.Length);
        ComputeShader.SetFloat("viewRadius", SystemSettings.PerceptionRadius);
        ComputeShader.SetFloat("avoidRadius", SystemSettings.AvoidanceRadius);

        int threadGroups = Mathf.CeilToInt(boids.Length / (float)1024);

        ComputeShader.Dispatch(0, threadGroups, 1, 1);

        buffer.GetData(boidData);

        //boids[0].UpdateRandom();

        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].UpdateSelf(boidData[i]);
        }

        buffer.Release();
    }

    private void OnDestroy() {
        foreach (var b in boids) {
            Destroy(b);
        }
    }

    private void OnDrawGizmos() {
        var size = new Vector3();
        size.x = Width * transform.localScale.x;
        size.y = Height * transform.localScale.y;
        size.z = Depth * transform.localScale.z;

        Gizmos.DrawWireCube(transform.position, size);
    }
}
