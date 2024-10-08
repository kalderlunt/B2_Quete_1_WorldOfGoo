using UnityEngine;

public class BallController
{
    private BallModel model;
    private BallView view;

    public BallController(BallModel model, BallView view)
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
        foreach (SpringModel spring in model.Connections)
        {
            ApplySpringForce(spring);
        }
    }

    private void ApplySpringForce(SpringModel spring)
    {
        Vector2 direction = spring.BallB.Rigidbody.position - spring.BallA.Rigidbody.position;
        float currentLength = direction.magnitude;

        float stretch = currentLength - spring.NaturalLength;

        float springConstant = 0.5f;
        UnityEngine.Vector2 force = direction.normalized * stretch * springConstant; // 0.5f constante de ressort

        spring.BallA.Rigidbody.AddForce(force);
        spring.BallB.Rigidbody.AddForce(force);
    }
}