using UnityEngine;

public class BallView : MonoBehaviour
{
    private BallModel model;
    public SpriteRenderer spriteRenderer;

    private BallView(BallModel model, SpriteRenderer spriteRenderer)
    {
        this.model = model;
        this.spriteRenderer = spriteRenderer;
    }

    public void UpdateView()
    {
        transform.position = model.Position;
    }
}