using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float globalGravity = -9.81f;

    private void Start()
    {
        Physics2D.gravity = new Vector2(0, globalGravity);
    }
}