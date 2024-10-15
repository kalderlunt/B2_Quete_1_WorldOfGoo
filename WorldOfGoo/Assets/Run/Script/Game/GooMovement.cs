using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GooMovement : MonoBehaviour
{
    public float speed = 2f;
    private Transform currentNode;
    private Rigidbody2D rb;
    private Rigidbody2D originRb;

    private void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        originRb    = rb;
    }

    private void Start()
    {
        GameObject[] listFixGoos = GetAllObjectsInLayer(LayerMask.NameToLayer("FixGoo"));
        currentNode = listFixGoos[Random.Range(0, listFixGoos.Length)].transform;

    }

    private void OnEnable()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void OnDisable()
    {
        rb = originRb;
    }

    private void FixedUpdate()
    {
        MoveToNextBase();
    }

    private void MoveToNextBase()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentNode.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, currentNode.position) < 0.01f && currentNode.TryGetComponent(out GooController link))
        {
            List<SpringJoint2D> listLinks = link.ConnectedGoos;
            if (listLinks.Count > 0)
                currentNode = listLinks[Random.Range(0, listLinks.Count)].transform;
        }
    }

    private GameObject[] GetAllObjectsInLayer(int layer)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> objectsInLayer = new();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layer)
            {
                objectsInLayer.Add(obj);
            }
        }

        return objectsInLayer.ToArray();
    }
}