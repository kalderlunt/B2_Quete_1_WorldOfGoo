using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxLineRenderer : MonoBehaviour
{
    [SerializeField] private int    numberOfJoints  = 2;
    [SerializeField] private float  amplitude       = 0.5f;
    [SerializeField] private float duration    = 1f;
    private float elapsedTime = 0f;

    private List<GameObject> JointConnected = new();
    private GameObject objJointA;
    private GameObject objJointB;


    public void Initialize(GameObject jointA, GameObject jointB)
    {
        JointConnected.Add(jointA);
        JointConnected.Add(jointB);

        objJointA = jointA.gameObject;
        objJointB = jointB.gameObject;
    }


    public void UpdateLink()
    {
        if (objJointA != null && objJointB != null)
        {
            Vector2 origin = objJointA.transform.position;
            Vector2 destination = objJointB.transform.position;

            /*
             * Gauche Droite not Smouth
             * 
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.PingPong(elapsedTime / duration, 1.0f);

            transform.position = Vector2.Lerp(origin, destination, lerpFactor);*/

            // Ping Pong Mode
            //
            // Gauche Droite Smouth
            
            float lerpFactor = Mathf.PingPong(elapsedTime / duration, 1.0f);
            Vector2 interpolatedPosition = Vector2.Lerp(origin, destination, lerpFactor);

            float sineOffset = Mathf.Sin(elapsedTime * Mathf.PI * 2 / duration) * amplitude;
            
            transform.position = interpolatedPosition + new Vector2(0, sineOffset);
            elapsedTime += Time.deltaTime;
            
            //if (elapsedTime > duration) elapsedTime = 0f;
        }
    }
}