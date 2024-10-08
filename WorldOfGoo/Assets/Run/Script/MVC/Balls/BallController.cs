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
        model.Velocity += UnityEngine.Physics2D.gravity * UnityEngine.Time.deltaTime;
        
        foreach (SpringModel spring in model.Connections)
        {
            ApplySpringForce(spring);
        }

        model.Position += model.Velocity * UnityEngine.Time.deltaTime;
    }

    private void ApplySpringForce(SpringModel spring)
    {
        UnityEngine.Vector2 direction = spring.BallB.Position - spring.BallA.Position;
        float currentLength = direction.magnitude;

        float stretch = currentLength - spring.NaturalLength;
        UnityEngine.Vector2 force = direction.normalized * stretch * 0.2f; // 0.5f constante de ressort

        spring.BallA.Velocity += force / spring.BallA.Mass;
        spring.BallB.Velocity -= force / spring.BallB.Mass;
    }
}