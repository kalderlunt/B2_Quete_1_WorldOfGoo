using UnityEngine;

public class SpringView : MonoBehaviour
{
    public readonly SpringModel model;
    public readonly LineRenderer lineRenderer;

    private SpringView(SpringModel model, LineRenderer lineRenderer)
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