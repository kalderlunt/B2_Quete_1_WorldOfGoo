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
    private bool isBeingDragged = false;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Color connectedColor = Color.green;
    private Color originalColor;


    public float smoothSpeedGooToMouse = 1.0f;
    [SerializeField] private LayerMask layerMask;


    [SerializeField] private GameObject previewLinkPrefab;
    private Dictionary<Collider2D, GameObject> activePreviews = new ();
    private float duration = 1f;
    private float elapsedTime = 0f;


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
            // gameobject qui fait un aller retour du transform.position a la other.transform.position avec une duree de 1 sec
            Debug.Log(hitColliders.Count);
            GameObject previewInstance = Instantiate(previewLinkPrefab, transform.position, Quaternion.identity);

            if (!activePreviews.Any(other, previewInstance))
                activePreviews.Add(other, previewInstance);
            else if (other != null)
            {
                activePreviews.Remove(other);
                Destroy(previewInstance);
            }
            //RemoveBrokenLinks();
        }
    }

    private void RemoveBrokenLinks()
    {
        List<SpringJoint2D> toRemove = new();

        foreach (var entry in activePreviews)
        {
            var otherGoo = entry.Key;
            GameObject previewInstance = entry.Value;

            if (ConnectedGoos.Any(joint => joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>()))
            {
                Destroy(previewInstance);
                //toRemove.Add(otherGoo);
            }
        }
    }

    private void UpdatePreviewsLink()
    {
        foreach (var entry in activePreviews)
        {
            //GameObject otherGoo = entry.Key;
            GameObject previewInstance = entry.Value;

            if (otherGoo != null)
            {
                Vector2 origin = transform.position;
                Vector2 destination = otherGoo.transform.position;

                elapsedTime += Time.deltaTime;
                float lerpFactor = Mathf.PingPong(elapsedTime / duration, 1.0f);

                previewInstance.transform.position = Vector2.Lerp(origin, destination, lerpFactor);

                if (ConnectedGoos.Any(joint => joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>()))
                {
                    Destroy(previewInstance);
                }
            }
        }
    }

    private void AttachTo(GameObject otherGoo)
    {
        if (otherGoo == null) return;

/*        if (ConnectedGoos.Any(joint => joint.connectedBody == otherGoo.GetComponent<Rigidbody2D>()))
        {
            Debug.Log($"{otherGoo.name} est déjà connecté à l'object {this.name}");
            return;
        }

        if (otherGoo.GetComponent<GooController>().ConnectedGoos.Any(joint => joint.connectedBody == rb))
        {
            Debug.Log($"{this.name} est déjà connecté à l'object {otherGoo.name}");
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
            //Debug.Log($"Le SpringJoint {this.name} à été détaché avec succès du {otherGoo.name}");
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
            //Debug.Log($"Le SpringJoint {otherGoo.name} à été détaché avec succès du {this.name}");
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

        RemoveBrokenLinks();
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