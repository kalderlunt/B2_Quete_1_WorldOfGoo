public class SpringModel
{
    public BallModel BallA { get; }
    public BallModel BallB { get; }
    public float NaturalLength { get; } // longueur naturelle a l'etat repos

    public SpringModel(BallModel ballA, BallModel ballB, float naturalLength)
    {
        BallA = ballA;
        BallB = ballB;
        NaturalLength = naturalLength;
    }
}