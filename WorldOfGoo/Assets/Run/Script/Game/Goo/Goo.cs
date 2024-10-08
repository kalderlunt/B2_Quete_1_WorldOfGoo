using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour
{
    [SerializeField] private float springDistance = 0.05f;
    [SerializeField] private float springDampingRatio = 0.6f;
    [SerializeField] private float springFrequency = 2f;
    
    [SerializeField] private Color connectedColor = Color.green;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    
    private Rigidbody2D rb;
    private bool isDragging = false;
    [SerializeField] private float distGoo = 1.0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    public void StartDragging()
    {
        isDragging = true;
        rb.isKinematic = true;
    }

    public void StopDragging()
    {
        isDragging = false;
        rb.isKinematic = false; 

        Goo nearestGoo = FindNearestGoo();
        if (nearestGoo != null)
        {
            AttachTo(nearestGoo);
        }
    }

    public void AttachTo(Goo otherGoo)
    {
        if (otherGoo == null) return;

        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = otherGoo.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.distance = springDistance;
        joint.dampingRatio = springDampingRatio;
        joint.frequency = springFrequency;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = connectedColor;
    }

    public void Detach()
    {
        SpringJoint2D[] joints = GetComponents<SpringJoint2D>();
        foreach (var joint in joints)
        {
            Destroy(joint);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = originalColor;
    }

    public Goo FindNearestGoo()
    {
        float minDistance = Mathf.Infinity;
        Goo nearestGoo = null;
        Goo[] allGoos = FindObjectsOfType<Goo>();

        foreach (Goo goo in allGoos)
        {
            if (goo == this) continue; // Ne pas se connecter à soi-même

            float distance = Vector2.Distance(transform.position, goo.transform.position);
            if (distance < minDistance && distance < distGoo) // Ajustez la portée à 1.0f
            {
                minDistance = distance;
                nearestGoo = goo;
            }
        }

        return nearestGoo;
    }
    // logique de vie, des collisions, etc.
}