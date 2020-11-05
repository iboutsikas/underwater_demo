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
    
    Boid[] boids;

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
}
