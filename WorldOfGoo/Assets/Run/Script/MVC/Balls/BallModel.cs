using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallModel
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Mass { get; set; }
    public List<SpringModel> Connections { get; set; }

    public BallModel(Vector2 position, float mass)
    {
        Position = position;
        Mass = mass;
        Velocity = Vector2.zero;
        Connections = new();
    }

    public void AddConnection(SpringModel spring)
    {
        Connections.Add(spring);
    }
}