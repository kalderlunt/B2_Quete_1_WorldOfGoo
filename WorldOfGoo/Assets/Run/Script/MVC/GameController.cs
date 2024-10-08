using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<BallController> BallControllers;
    public List<SpringController> SpringControllers;

    private void Awake()
    {
        BallControllers = new List<BallController>(FindObjectsOfType<BallController>());
        SpringControllers = new List<SpringController>(FindObjectsOfType<SpringController>());
    }


    private void Start()
    {
        //CreateBallsAndSprings();
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
        BallModel ball1 = new(new Vector2(0, 0), 1.0f);
        BallModel ball2 = new(new Vector2(2, 0), 1.0f);

        SpringModel spring = new(ball1, ball2, 1.5f);

        BallController ballController1 = gameObject.AddComponent<BallController>();
        BallController ballController2 = gameObject.AddComponent<BallController>();
        SpringController springController = gameObject.AddComponent<SpringController>();

        BallControllers.Add(ballController1);
        BallControllers.Add(ballController2);
        SpringControllers.Add(springController);
    }
}