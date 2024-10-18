using System.Collections.Generic;
using UnityEngine;

public class GooMovement : MonoBehaviour
{
    public float speed = 2f;
    [SerializeField] private float speedEndLevel = 3f;

    [HideInInspector] public GameObject CollisionObj;
    private GameObject lastNode;
    private GameObject nextNode;

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

        GameManager.Instance.GooOnMovements.Add(this.gameObject);
    }

    private void OnDisable()
    {
        GameManager.Instance.GooOnMovements.Remove(this.gameObject);
    }

    private void OnMouseDown()
    {
        ourController.RemoveGooMovementScript();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsEndLevel)
            MoveToNextBase();
        else 
            PastFindingEndLevel();
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



    private void PastFindingEndLevel()
    {
        if (rb.bodyType != RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Kinematic;

        nextNode = FindHighestIDGoo();

        if (nextNode != null)
            transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position, speedEndLevel * Time.fixedDeltaTime);
    }


    private GameObject FindHighestIDGoo()
    {
        GameObject highestIDGoo = nextNode;
        int highestID = -1;

        if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.01f && nextNode.TryGetComponent(out GooController link))
        {
            lastNode = nextNode;

            int currentID = link.Id;
            if (currentID > highestID)
            {
                highestID = currentID;
                highestIDGoo = nextNode;
            }

            List<SpringJoint2D> nextLinks = link.ConnectedGoos;

            if (nextLinks.Count > 0)
            {
                foreach (SpringJoint2D springLink in nextLinks)
                {
                    GameObject connectedGoo = springLink.gameObject;
                    int connectedID = connectedGoo.GetComponent<GooController>().Id;

                    if (connectedID > highestID)
                    {
                        highestID = connectedID;
                        highestIDGoo = connectedGoo;
                    }
                }
            }
        }

        return highestIDGoo;
    }

    /*private GameObject FindHighestIDGoo()
    {
        GameObject highestIDGoo = null;
        int highestID = -1;


        int gooID = goo.GetComponent<GooController>().Id;

        if (gooID > highestID)
        {
            highestID    = gooID;
            highestIDGoo = goo;
        }

        if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.01f && nextNode.TryGetComponent(out GooController link))
        {
            List<SpringJoint2D> nextLinks = link.ConnectedGoos;


            foreach (SpringJoint2D springLink in nextLinks)
            {
                lastNode     = nextNode;
                highestIDGoo = nextNode;

                int idTemp = springLink.GetComponent<GooController>().Id;

                if (idTemp > highestID)
                {
                    highestID = idTemp;
                }
            }

            return highestIDGoo;
        }

        return null;
    }*/
}