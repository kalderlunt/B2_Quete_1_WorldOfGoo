using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GooMovement : MonoBehaviour
{
    public float speed = 2f;
    
    public  GameObject collisionObj;
    private GameObject currentNode;

    private GooController ourController;
    private GooController collisionController;

    private Rigidbody2D rb;

    private int FixGooLayer        = 6;
    private int FreeGooLayer       = 7;
    private int GooOnMovement      = 9;
    private int FixPlatormLayer    = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.bodyType = RigidbodyType2D.Static;
        
        gameObject.layer = GooOnMovement;

        ourController       = GetComponent<GooController>(); 
        collisionController = collisionObj.GetComponent<GooController>();

        List<SpringJoint2D> connectedGoos = collisionController.ConnectedGoos;
        
        if (connectedGoos.Count > 0)
            currentNode = connectedGoos[0].connectedBody.gameObject;
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
        collisionController = currentNode.GetComponent<GooController>();

        if (currentNode == null || collisionController.ConnectedGoos.Count == 0)
        {
            ourController.RemoveGooMovementScript();
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, currentNode.transform.position) < 0.01f && currentNode.TryGetComponent(out GooController link))
        {
            List<SpringJoint2D> listLinks = link.ConnectedGoos;
            if (listLinks.Count > 0)
                currentNode = listLinks[Random.Range(0, listLinks.Count)].gameObject;
            else
                ourController.RemoveGooMovementScript();
        }
    }
}