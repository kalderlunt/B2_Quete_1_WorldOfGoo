using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GooController : MonoBehaviour
{
    [SerializeField] private float springDistance = 2f;
    [SerializeField] private float springDampingRatio = 0.6f;
    [SerializeField] private float springFrequency = 2f;

    [SerializeField] private List<Collider2D> hitColliders;
    public List<SpringJoint2D> ConnectedGoos;

    [SerializeField] private AnimationCurve modelCurve;
    [SerializeField] private Material baseMaterial;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isBeingDragged = false;

    [SerializeField] private Color connectedColor = Color.green;
    private Color originalColor;


    public float smoothSpeedGooToMouse = 1.0f;
    [SerializeField] private LayerMask layerMask;

    private Dictionary<GameObject, GameObject> activePreviews = new ();
    [SerializeField] private GameObject previewLinkPrefab;
    
    private float duration      = 1f;
    private float elapsedTime   = 0f;


    private void Start()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        ConnectedGoos   = GetComponents<SpringJoint2D>()
            .Select(joint => joint.connectedBody.gameObject.GetComponent<SpringJoint2D>())
            .ToList();

        originalColor = spriteRenderer.color;
    }


    private void DetectGoo()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        hitColliders = Physics2D.OverlapCircleAll(position, springDistance, 1 << LayerMask.NameToLayer("Goo")).ToList();

        foreach (Collider2D other in hitColliders)
        {
            Debug.Log($"Number of collision : {hitColliders.Count}");
            Debug.Log($"Distance between collisions : {Vector2.Distance(transform.position, other.transform.position)}");

            if (other != null && !activePreviews.ContainsKey(other.gameObject))
            {
                GameObject previewInstance = Instantiate(previewLinkPrefab, transform.position, Quaternion.identity);

                VfxLineRenderer vfxLineRenderer = previewInstance.GetComponent<VfxLineRenderer>();
                GameObject jointA = this.gameObject;
                GameObject jointB = other.gameObject;
                vfxLineRenderer.Initialize(jointA, jointB);

                activePreviews.Add(other.gameObject, previewInstance);
            }
        }

        foreach (var entry in activePreviews.ToList())
        {
            GameObject otherGoo = entry.Key;

            float distance = Vector2.Distance(transform.position, otherGoo.transform.position) - 0.5f;                  //// JE SAIS PAS POURQUOI IL FAUT METTRE - 0.5f////
            if (distance > springDistance)
            {
                RemoveBrokenPreviewLink(otherGoo);
            }
        }
    }

    private void RemoveBrokenPreviewLinks()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var entry in activePreviews)
        {
            GameObject otherGoo         = entry.Key;
            GameObject previewInstance  = entry.Value;

            if (ConnectedGoos.All(joint => joint.connectedBody != otherGoo.GetComponent<Rigidbody2D>()))
            {
                Destroy(previewInstance);
                toRemove.Add(otherGoo);
            }
        }

        foreach (var goo in toRemove)
        {
            activePreviews.Remove(goo);
        }
    }

    private void RemoveBrokenPreviewLink(GameObject otherGoo)
    {
        if (activePreviews.TryGetValue(otherGoo, out GameObject previewInstance))
        {
            Destroy(previewInstance);
            activePreviews.Remove(otherGoo);
        }
    }


    private void UpdatePreviewsLink()
    {
        foreach (var entry in activePreviews)
        {
            GameObject otherGoo = entry.Key;
            GameObject previewInstance = entry.Value;

            if (previewInstance != null)
            {
                VfxLineRenderer vfxLineRenderer = previewInstance.GetComponent<VfxLineRenderer>();
                if (vfxLineRenderer != null)
                {
                    vfxLineRenderer.UpdateLink();
                }
            }
        }
    }

    private void AttachTo(GameObject otherGoo)
    {
        if (otherGoo == null) return;

/*        if (ConnectedGoos.Any(joint => joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>()))
        {
            Debug.Log($"{otherGoo.name} est d�j� connect� � l'object {this.name}");
            return;
        }

        if (otherGoo.GetComponent<GooController>().ConnectedGoos.Any(joint => joint.connectedBody == rb))
        {
            Debug.Log($"{this.name} est d�j� connect� � l'object {otherGoo.name}");
            return;
        }

        if (ConnectedGoos.Any(joint => joint.connectedBody.gameObject.GetComponent<SpringJoint2D>()))
            return;
*/

        if (ConnectedGoos.Any(joint => joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>()))
            return;

        if (otherGoo.GetComponent<GooController>().ConnectedGoos.Any(joint => joint.connectedBody == this.rb))
            return;














        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA                       REVOIR LES RETURN                  AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA








        float distance = Vector2.Distance(transform.position, otherGoo.transform.position);

        // other goo 
        SpringJoint2D jointOtherGoo         = otherGoo.AddComponent<SpringJoint2D>();
        jointOtherGoo.connectedBody         = this.rb;
        jointOtherGoo.autoConfigureDistance = false;
        jointOtherGoo.distance              = distance;
        jointOtherGoo.dampingRatio          = springDampingRatio;
        jointOtherGoo.frequency             = springFrequency;

        // this goo
        SpringJoint2D joint         = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody         = otherGoo.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.distance              = distance;
        joint.dampingRatio          = springDampingRatio;
        joint.frequency             = springFrequency;

        otherGoo.GetComponent<GooController>().ConnectedGoos.Add(joint);
        ConnectedGoos.Add(jointOtherGoo);
        
        
        //spriteRenderer.color = connectedColor;
    }

    private void DetachAllLink()
    {
        foreach (var other in hitColliders)
        {
            Detach(other.gameObject);
        }
    }

    private void Detach(GameObject otherGoo)
    {
        //if (otherGoo == null) return;

        // this intantiate to delete othter
        SpringJoint2D[] otherSpringsInThisGoo = otherGoo.GetComponents<SpringJoint2D>();
        foreach (SpringJoint2D otherJoint in otherSpringsInThisGoo)
        {
            if (otherJoint.connectedBody == rb)
            {
                Destroy(otherJoint);
                ConnectedGoos.Remove(otherJoint);
            }
            //Debug.Log($"Le SpringJoint {this.name} � �t� d�tach� avec succ�s du {otherGoo.name}");
        }

        
        // other references to delete this instance
        SpringJoint2D[] thisSprings = gameObject.GetComponents<SpringJoint2D>();
        foreach (SpringJoint2D thisJoint in thisSprings)
        {
            if (thisJoint.connectedBody == otherGoo.GetComponent<Rigidbody2D>())
            {
                Destroy(thisJoint);
                otherGoo.GetComponent<GooController>().ConnectedGoos.Remove(thisJoint);
            }
            //Debug.Log($"Le SpringJoint {otherGoo.name} � �t� d�tach� avec succ�s du {this.name}");
        }
    }



    private void MouseDragSystem()
    {
        Vector2 mousePosition       = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPosition    = Vector2.Lerp(transform.position, mousePosition, smoothSpeedGooToMouse);
        transform.position          = smoothedPosition;
    }

    private void OnMouseDown()
    {
        isBeingDragged  = true;
        rb.isKinematic  = true;
        rb.velocity     = Vector2.zero;

        gameObject.layer = 0 << layerMask.value;
    }

    private void OnMouseDrag()
    {
        MouseDragSystem();
        DetectGoo();
        DetachAllLink();
        UpdatePreviewsLink();
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

        RemoveBrokenPreviewLinks();
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