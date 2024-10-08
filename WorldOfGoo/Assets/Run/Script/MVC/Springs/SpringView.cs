using UnityEngine;

public class SpringView : MonoBehaviour
{
    private SpringModel model;
    private LineRenderer lineRenderer;

    public void Initialize(SpringModel model, LineRenderer lineRenderer)
    {
        this.model = model;
        this.lineRenderer = lineRenderer;
    }

    public void UpdateView()
    {
        lineRenderer.SetPosition(0, model.BallA.Position);
        lineRenderer.SetPosition(1, model.BallB.Position);
    }
}