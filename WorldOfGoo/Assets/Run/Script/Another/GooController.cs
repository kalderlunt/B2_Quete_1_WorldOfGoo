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
    public List<SpringJoint2D> ConnectedGoos;
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
        ConnectedGoos   = GetComponents<SpringJoint2D>().ToList();

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        //DetectGoo();
    }


    private void DetectGoo()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        hitColliders = Physics2D.OverlapCircleAll(position, springDistance, 1 << LayerMask.NameToLayer("Goo")).ToList();

        foreach (Collider2D other in hitColliders)
        {
            Debug.Log(hitColliders.Count);

            
            if (other.gameObject != this.gameObject && other.GetComponent<SpringJoint2D>().connectedBody != this.gameObject) // Évite de lier à soi-même
            {
                //CreateView(transform.position, other.transform.position);
                //CreateView(other.transform.position, transform.position);
            }
        }
    }

    private void CreateView(Vector2 origin, Vector2 destination)
    {
        lineRenderers.Add(gameObject.AddComponent<LineRenderer>());
        float lineWidth = 0.1f;

        foreach (LineRenderer line in lineRenderers)
        {
            line.material = baseMaterial != null ? baseMaterial : new Material(Shader.Find("Sprites/Default"));

            if (modelCurve != null)
            {
                line.widthCurve = modelCurve;
            }
            else
            {
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
            }
            line.positionCount = 2;
            line.SetPosition(0, origin);
            line.SetPosition(1, destination);
        }
    }

    private void AttachTo(GameObject otherGoo)
    {
        if (otherGoo == null) return;

        // other goo 
        SpringJoint2D jointOtherGoo = otherGoo.AddComponent<SpringJoint2D>();
        jointOtherGoo.connectedBody = this.rb;
        jointOtherGoo.autoConfigureDistance = false;
        jointOtherGoo.distance = springDistance;
        jointOtherGoo.dampingRatio = springDampingRatio;
        jointOtherGoo.frequency = springFrequency;

        otherGoo.GetComponent<GooController>().ConnectedGoos.Add(jointOtherGoo);

        // this goo
        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = otherGoo.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.distance = springDistance;
        joint.dampingRatio = springDampingRatio;
        joint.frequency = springFrequency;

        ConnectedGoos.Add(joint);
        //spriteRenderer.color = connectedColor;
    }

    /*private void Detach(GameObject otherGoo)
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

    private void DetachAllGoo()
    {
        for (int i = 0; i < ConnectedGoos.Count;)
        {
            Destroy(ConnectedGoos[i]);
            ConnectedGoos.Remove(ConnectedGoos[i]);
        }
    }

    private void MouseDragSystem()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPosition = Vector2.Lerp(transform.position, mousePosition, smoothSpeedGooToMouse);
        transform.position = smoothedPosition;
    }

    private void OnMouseDown()
    {
        isBeingDragged = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        gameObject.layer = 0 << layerMask.value;
        DetachAllGoo();

        foreach (Collider2D other in hitColliders)
        {
            AttachTo(other.gameObject);
        }
    }

    private void OnMouseDrag()
    {
        MouseDragSystem();
        DetectGoo();
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