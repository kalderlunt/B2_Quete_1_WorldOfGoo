using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private BallModel model;
    private BallView view;

    private BallController(BallModel model, BallView view)
    {
        this.model = model;
        this.view = view;
    }

    public void UpdateBall()
    {
        UpdatePhysics();
        view.UpdateView();
    }

    private void UpdatePhysics()
    {
        model.Velocity += Physics2D.gravity * Time.deltaTime;
        
        foreach (SpringModel spring in model.Connections)
        {
            ApplySpringForce(spring);
        }

        model.Position += model.Velocity * Time.deltaTime;
    }

    private void ApplySpringForce(SpringModel spring)
    {
        Vector2 direction = spring.BallB.Position - spring.BallA.Position;
        float currentLength = direction.magnitude;

        float stretch = currentLength - spring.NaturalLength;
        Vector2 force = direction.normalized * stretch * 0.5f; // 0.5f constante de ressort

        spring.BallA.Velocity += force / spring.BallA.Mass;
        spring.BallB.Velocity -= force / spring.BallB.Mass;
    }
}