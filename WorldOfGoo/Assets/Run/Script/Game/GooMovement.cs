using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GooMovement : MonoBehaviour
{
    public float speed = 2f;
    
    public  GameObject collisionObj;
    private GameObject currentNode;

    private GooController collisionController;

    private Rigidbody2D rb;
    private Rigidbody2D originRb;

    private int originalLayer;
    private int newLayer = 9;

    private void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        originRb    = rb;
    }

    private void Start()
    {
        collisionController = collisionObj.GetComponent<GooController>();
        List<SpringJoint2D> connectedGoos = collisionController.ConnectedGoos;
        
        if (connectedGoos.Count > 0)
            currentNode = connectedGoos[Random.Range(0, connectedGoos.Count)].gameObject;
        else
            RemoveGooMovementScript();
    }

    private void OnEnable()
    {
        rb.bodyType = RigidbodyType2D.Static;

        originalLayer = gameObject.layer;
        gameObject.layer = newLayer;
    }

    private void OnDisable()
    {
        rb = originRb;
        gameObject.layer = originalLayer;
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
            RemoveGooMovementScript();
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, currentNode.transform.position) < 0.01f && currentNode.TryGetComponent(out GooController link))
        {
            List<SpringJoint2D> listLinks = link.ConnectedGoos;
            if (listLinks.Count > 0)
                currentNode = listLinks[Random.Range(0, listLinks.Count)].gameObject;
            else
                RemoveGooMovementScript();
        }
    }

    private void RemoveGooMovementScript()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (TryGetComponent<GooMovement>(out GooMovement gooMovement))
            Destroy(gooMovement);
    }
}