using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<BallController> BallControllers;
    public List<SpringController> SpringControllers;

    [SerializeField] private GameObject prefabBall;
    [SerializeField] private GameObject prefabSpring;

    private void Awake()
    {
        BallControllers     = new ();
        SpringControllers   = new ();
    }


    private void Start()
    {
        CreateBallsAndSprings();
    }

    private void Update()
    {
        foreach (BallController ball in BallControllers)
        {
            ball.UpdateBall();
        }

        foreach (SpringController spring in SpringControllers)
        {
            spring.UpdateSpring();
        }
    }




    public void AddBallController(BallController ballController)
    {
        BallControllers.Add(ballController);
    }

    public void AddSpringController(SpringController springController)
    {
        SpringControllers.Add(springController);
    }

    private void CreateBallsAndSprings()
    {
        // Instantiate ball 1
        GameObject      ball1Object     = Instantiate(prefabBall);
        BallModel       ball1Model      = new (ball1Object.GetComponent<Rigidbody2D>(), 1.0f);
        BallView        ball1View       = ball1Object.GetComponent<BallView>();
        ball1View.Initialize(ball1Model);
        
        BallController  ball1Controller = new (ball1Model, ball1View);
        BallControllers.Add(ball1Controller);



        // Instantiate ball 2
        GameObject      ball2Object     = Instantiate(prefabBall);
        BallModel       ball2Model      = new (ball2Object.GetComponent<Rigidbody2D>(), 1.0f);
        BallView        ball2View       = ball2Object.GetComponent<BallView>();
        ball2View.Initialize(ball2Model);
        
        BallController  ball2Controller = new (ball2Model, ball2View);
        BallControllers.Add(ball2Controller);


        // Instantiate spring
        GameObject          springObject        = Instantiate(prefabSpring);
        SpringModel         springModel         = new (ball1Model, ball2Model, 1.5f);
        SpringView          springView          = springObject.GetComponent<SpringView>();
        springView.Initialize(springModel, springObject.GetComponent<LineRenderer>());

        SpringController    springController    = new (springModel, springView);
        SpringControllers.Add(springController);
    }
}