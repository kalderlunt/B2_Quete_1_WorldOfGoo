using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class GooController : MonoBehaviour
{
    [SerializeField] private float springDistance = 2f;
    [SerializeField] private float springDampingRatio = 0.6f;
    [SerializeField] private float springFrequency = 2f;

    [SerializeField] private List<Collider2D> hitColliders;
    [SerializeField] private List<SpringJoint2D> connectedGoos;
    [SerializeField] private List<LineRenderer> lineRenderers;

    [SerializeField] private AnimationCurve modelCurve;
    [SerializeField] private Material baseMaterial;

    private Rigidbody2D rb;
    private bool isBeingDragged = false;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Color connectedColor = Color.green;
    private Color originalColor;


    public float smoothSpeedGooToMouse = 1.0f;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        lineRenderers   = new();
        connectedGoos   = GetComponents<SpringJoint2D>().ToList();

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        //DetectGoo();
    }

    private void ApplyPhysics()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, springDistance);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Goo"))
            {
                /*connections.Add
                collider.gameObject*/
            }
        }
    }


    private void DetectGoo()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        hitColliders = Physics2D.OverlapCircleAll(position, springDistance, 1 << LayerMask.NameToLayer("Goo")).ToList();

        foreach (Collider2D other in hitColliders)
        {
            Debug.Log(hitColliders.Count);

            if (other.gameObject != this.gameObject) // Évite de lier à soi-même
            {
                CreateView(transform.position, other.transform.position);
                //CreateView(other.transform.position, transform.position);
            }
        }
    }

    public void CreateView(Vector2 origin, Vector2 destination)
    {
        lineRenderers.Add(gameObject.AddComponent<LineRenderer>());
        float lineWidth = 0.1f;

        foreach (LineRenderer line in lineRenderers)
        {
            line.material = baseMaterial != null ? baseMaterial : new Material(Shader.Find("Sprites/Default"));
            // Linerenderer de l'object a l'autre et de l'autre a lui creer dans ce gameobject

            if (modelCurve != null)
            {
                line.widthCurve = modelCurve;
            }
            else
            {
                // Si aucune courbe n'est définie, on garde une largeur constante
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
            }
            line.positionCount = 2;
            line.SetPosition(0, origin); // Position de cet objet
            line.SetPosition(1, destination);
        }
    }

    public void AttachTo(GameObject otherGoo)
    {
        if (otherGoo == null) return;

        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = otherGoo.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.distance = springDistance;
        joint.dampingRatio = springDampingRatio;
        joint.frequency = springFrequency;

        connectedGoos.Add(joint);

        //linerenderer 


        //spriteRenderer.color = connectedColor;
    }

    /*public void Detach(GameObject otherGoo)
    {
        if (otherGoo == null || !connectedGoos.Contains(GetComponent<SpringJoint2D>())) return;

        SpringJoint2D otherSpring = otherGoo.gameObject.GetComponent<SpringJoint2D>();

        SpringJoint2D joint = GetComponent<SpringJoint2D>();
        if (joint != null && joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>())
        {
            Destroy(joint);
        }

        connectedGoos.Remove(otherSpring);

        //spriteRenderer.color = originalColor;
    }*/

    public void DetachAllGoo()
    {
        for (int i = 0; i < connectedGoos.Count;)
        {
            Destroy(connectedGoos[i]);
            connectedGoos.Remove(connectedGoos[i]);
        }
    }

    public void MouseDragSystem()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPosition = Vector2.Lerp(transform.position, mousePosition, smoothSpeedGooToMouse);
        transform.position = smoothedPosition;
    }

    public void OnMouseDown()
    {
        isBeingDragged = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        gameObject.layer = 0 << layerMask.value;
        DetachAllGoo();
    }

    private void OnMouseDrag()
    {
        MouseDragSystem();
    }

    public void OnMouseUp()
    {
        isBeingDragged = false;
        rb.isKinematic = false;

        gameObject.layer = 6 << layerMask.value;

        foreach (Collider2D other in hitColliders)
        {
            AttachTo(other.gameObject);
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = connectedColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = originalColor;
    }
}