using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public enum GizmoType { Never, SelectedOnly, Always };

    public Boid Prefab;
    public float Radius = 10;
    public int Count = 10;
    public Color DebugColor;
    public GizmoType ShowSpawnRegion;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * Radius;
            Boid boid = Instantiate(Prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;
            boid.spawner = this;
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (ShowSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {

        Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 0.3f);
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
