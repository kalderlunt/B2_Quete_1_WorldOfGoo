using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GooMovement : MonoBehaviour
{
    public float speed = 2f;
    
    public GameObject CollisionObj;

    [SerializeField] private GameObject lastNode;
    [SerializeField] private GameObject nextNode;

    private GooController ourController;
    private GooController lastController;

    private Rigidbody2D rb;

    private int GooOnMovement      = 9;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        ourController   = GetComponent<GooController>(); 
    }

    private void Start()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.bodyType = RigidbodyType2D.Static;

        gameObject.layer = GooOnMovement;

        lastNode = CollisionObj; 
        lastController = lastNode.GetComponent<GooController>();

        List<SpringJoint2D> connectedGoos = lastController.ConnectedGoos;
        if (connectedGoos.Count > 0)
            nextNode = connectedGoos[0].connectedBody.gameObject;
        else
            ourController.RemoveGooMovementScript();
    }

    private void OnMouseDown()
    {
        ourController.RemoveGooMovementScript();
    }

    private void FixedUpdate()
    {
        MoveToNextBase();
    }

    private void MoveToNextBase()
    {

        if (nextNode == null || lastNode == null)
        {
            ourController.RemoveGooMovementScript();
            return;
        }

        GooController nextNodeController = nextNode.GetComponent<GooController>();
        
        if (nextNodeController.ConnectedGoos.Count == 0)
        {
            ourController.RemoveGooMovementScript();
            return;
        }


        foreach (SpringJoint2D joint in nextNodeController.ConnectedGoos)
        {
            if (joint != null)
                continue;

            if (joint.connectedBody != null)
                continue;

            if (joint.connectedBody.gameObject == lastNode)
            {
                ourController.RemoveGooMovementScript();
                return;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.01f && nextNode.TryGetComponent(out GooController link))
        {
            lastNode = nextNode;

            List<SpringJoint2D> nextLinks = link.ConnectedGoos;
            if (nextLinks.Count > 0)
                nextNode = nextLinks[Random.Range(0, nextLinks.Count)].gameObject;
            else
            {
                ourController.RemoveGooMovementScript();
                return;
            }
        }
    }
}