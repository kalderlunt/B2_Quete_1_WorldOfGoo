using UnityEngine;

public class BallView : MonoBehaviour
{
    private BallModel model;
    private SpriteRenderer spriteRenderer;

    public void Initialize(BallModel model, SpriteRenderer spriteRenderer)
    {
        this.model = model;
        this.spriteRenderer = spriteRenderer;
    }

    public void UpdateView()
    {
        transform.position = model.Position;
    }
}