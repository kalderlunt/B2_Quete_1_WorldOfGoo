using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallModel
{
    public Rigidbody2D Rigidbody { get;  set; }
    public float Mass { get; private set; }
    public List<SpringModel> Connections { get; set; }

    public BallModel(Rigidbody2D rigidbody, float mass)
    {
        this.Rigidbody = rigidbody;
        this.Rigidbody.mass = mass;
        Mass = mass;
        Connections = new();
    }

    public void AddConnection(SpringModel spring)
    {
        Connections.Add(spring);
    }
}