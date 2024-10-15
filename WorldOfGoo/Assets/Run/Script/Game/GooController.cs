using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GooController : MonoBehaviour
{
    public int ID = 0;

    [SerializeField] private float springDistance = 2f;
    [SerializeField] private float springDampingRatio = 0.6f;
    [SerializeField] private float springFrequency = 2f;

    [SerializeField] private List<Collider2D> hitColliders;
    public List<SpringJoint2D> ConnectedGoos;

    [SerializeField] private AnimationCurve modelCurve;
    [SerializeField] private Material baseMaterial;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Color connectedColor = Color.green;
    private Color originalColor;


    public float smoothSpeedGooToMouse = 1.0f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject previewLinkPrefab;
    [SerializeField] private GameObject linkPrefab;

    private Dictionary<GameObject, GameObject> activePreviewslinks = new ();
    private Dictionary<GameObject, GameObject> activeLink           = new ();
    private bool  isLinked      = false;
    private float duration      = 1f;
    private float elapsedTime   = 0f;

    private readonly int indexLayerFixGoo    = 6;
    private readonly int indexLayerFreeGoo   = 7;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        ConnectedGoos   = GetComponents<SpringJoint2D>()
            .Select(joint => joint.connectedBody.gameObject.GetComponent<SpringJoint2D>())
            .ToList();
        
    }

    private void Start()
    {
        originalColor = spriteRenderer.color;
    }


    private void DetectGoo()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        hitColliders = Physics2D.OverlapCircleAll(position, springDistance, 1 << LayerMask.NameToLayer("FixGoo")).ToList();

        
        foreach (Collider2D other in hitColliders)
        {
            //Debug.Log($"Number of collision : {hitColliders.Count}");
            //Debug.Log($"Distance between collisions : {Vector2.Distance(position, other.transform.position)}");

/*            if (IsLinked)
            {
                if (other != null && !activeLink.ContainsKey(other.gameObject))
                {
                    GameObject previewInstance = Instantiate(previewLinkPrefab, position, Quaternion.identity);

                    VfxLineRenderer vfxLineRenderer = previewInstance.GetComponent<VfxLineRenderer>();
                    GameObject jointA = this.gameObject;
                    GameObject jointB = other.gameObject;
                    vfxLineRenderer.Initialize(jointA, jointB);

                    activeLink.Add(other.gameObject, previewInstance);
                }
            }
            else
            {*/ 
                if (other != null && !activePreviewslinks.ContainsKey(other.gameObject))
                {
                    GameObject previewInstance = Instantiate(previewLinkPrefab, position, Quaternion.identity);

                    VfxLineRenderer vfxLineRenderer = previewInstance.GetComponent<VfxLineRenderer>();
                    GameObject jointA = this.gameObject;
                    GameObject jointB = other.gameObject;
                    vfxLineRenderer.Initialize(jointA, jointB);

                    activePreviewslinks.Add(other.gameObject, previewInstance);
                }
            //}
        }

        foreach (var entry in activePreviewslinks.ToList())
        {
            GameObject otherGoo = entry.Key;

            float distance = Vector2.Distance(transform.position, otherGoo.transform.position) - 0.5f;                  //// JE SAIS PAS POURQUOI IL FAUT METTRE - 0.5f////
            if (distance > springDistance)
            {
                RemoveBrokenPreviewLink(otherGoo, activePreviewslinks);
            }
        }
    }

    public Transform ChoiceNextPoint()
    {
        int randomIndex = Random.Range(0, ConnectedGoos.Count);
        return ConnectedGoos[randomIndex].transform;
    }


    private void RemoveBrokenPreviewLinks(Dictionary<GameObject, GameObject> link)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var entry in link)
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
            link.Remove(goo);
        }
    }

    private void RemoveBrokenPreviewLink(GameObject otherGoo, Dictionary<GameObject, GameObject> link)
    {
        if (link.TryGetValue(otherGoo, out GameObject previewInstance))
        {
            Destroy(previewInstance);
            link.Remove(otherGoo);
        }
    }


    private void UpdateLinks()
    {
        foreach (var entry in activePreviewslinks)
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

        // Mise à jour des liens créés
        foreach (var entry in activeLink)
        {
            GameObject otherGoo = entry.Key;
            GameObject linkInstance = entry.Value;

            if (linkInstance != null)
            {
                VfxLineRenderer vfxLineRenderer = linkInstance.GetComponent<VfxLineRenderer>();
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

        if (activeLink.ContainsKey(otherGoo) || activeLink.ContainsKey(this.gameObject))
            return;

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
        //jointOtherGoo.distance              = distance;
        jointOtherGoo.dampingRatio          = springDampingRatio;
        jointOtherGoo.frequency             = springFrequency;

        // this goo
        SpringJoint2D joint         = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody         = otherGoo.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        //joint.distance              = distance;
        joint.dampingRatio          = springDampingRatio;
        joint.frequency             = springFrequency;

        otherGoo.GetComponent<GooController>().ConnectedGoos.Add(joint);
        ConnectedGoos.Add(jointOtherGoo);

        isLinked = true;
        if (!activeLink.ContainsKey(otherGoo))
        {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity, transform);
            VfxLineRenderer vfxLineRenderer = linkInstance.GetComponent<VfxLineRenderer>();

            if (vfxLineRenderer != null)
            {
                vfxLineRenderer.Initialize(this.gameObject, otherGoo);
                activeLink.Add(otherGoo, linkInstance);
            }
        }
    }

    private void DetachAllLink()
    {
        foreach (var other in hitColliders)
        {
            Detach(other.gameObject);
            DetachLink(other.gameObject);
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

    private void DetachLink(GameObject otherGoo)
    {
        if (otherGoo == null) return;

        // Suppression du lien visuel dans les deux sens
        if (activeLink.ContainsKey(otherGoo))
        {
            Destroy(activeLink[otherGoo]);
            activeLink.Remove(otherGoo);
        }

        GooController otherController = otherGoo.GetComponent<GooController>();
        if (otherController.activeLink.ContainsKey(this.gameObject))
        {
            Destroy(otherController.activeLink[this.gameObject]);
            otherController.activeLink.Remove(this.gameObject);
        }

        isLinked = ConnectedGoos.Count > 0;
        otherController.isLinked = otherController.ConnectedGoos.Count > 0;
    }


    private void MouseDragSystem()
    {
        Vector2 mousePosition       = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPosition    = Vector2.Lerp(transform.position, mousePosition, smoothSpeedGooToMouse);
        transform.position          = smoothedPosition;
    }

    private void CreateLinkIfConnected()
    {
        isLinked = ConnectedGoos.Count > 0;

        if (isLinked)
        {
            foreach (var otherGoo in hitColliders)
            {
                if (!activeLink.ContainsKey(otherGoo.gameObject))
                {
                    GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity, otherGoo.gameObject.transform);

                    VfxLineRenderer vfxLineRenderer = linkInstance.GetComponent<VfxLineRenderer>();
                    if (vfxLineRenderer != null)
                    {
                        GameObject jointA = this.gameObject;
                        GameObject jointB = otherGoo.gameObject;
                        vfxLineRenderer.Initialize(jointA, jointB);

                        activeLink.Add(otherGoo.gameObject, linkInstance);
                    }
                }
            }
        }
    }



    private void OnMouseDown()
    {
        if (tag != "Untouchable")
        {
            rb.isKinematic  = true;
            rb.velocity     = Vector2.zero;

            gameObject.layer = indexLayerFreeGoo << layerMask.value;

            DetachAllLink();

            if (gameObject.TryGetComponent<GooMovement>(out GooMovement gooMovement))
                Destroy(gooMovement);
        }
    }

    private void OnMouseDrag()
    {
        if (tag != "Untouchable")
        { 
            MouseDragSystem();
            DetectGoo();
            DetachAllLink();
            UpdateLinks();
        }
    }

    public void OnMouseUp()
    {
        if (tag != "Untouchable")
        { 
            rb.isKinematic = false;
            

            foreach (Collider2D other in hitColliders)
            {
                if (other != null)
                    gameObject.layer = indexLayerFixGoo << layerMask.value;
                
                AttachTo(other.gameObject);
            }

            RemoveBrokenPreviewLinks(activePreviewslinks);

            CreateLinkIfConnected();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.layer == LayerMask.NameToLayer("FixGoo"))
        {
            if (collision.gameObject.GetComponent<ParticleSystem>() ||
                collision.gameObject.GetComponent<GooController>())
            {
                if (!collision.gameObject.TryGetComponent<GooMovement>(out _))
                {
                    GooMovement gooMovement = collision.gameObject.AddComponent<GooMovement>();
                    gooMovement.speed = 2f;
                    Debug.Log("Combien de fois il passe dedans :");
                }

                /*if (collision.gameObject.TryGetComponent<GooMovement>(out GooMovement gooMovement))
                {
                    gooMovement.enabled = true; 
                }*/
            }
            /*
             if (collision.gameObject.layer == LayerMask.NameToLayer("FixGoo") || 
                collision.gameObject.layer == LayerMask.NameToLayer("Link"))
            {
                if (!collision.gameObject.TryGetComponent<GooMovement>(out _))
                {
                    collision.gameObject.AddComponent<GooMovement>();
                }
            }
            */
        }
    }   

    private void Update()
    {
        if (activeLink.Count > 0)
        {
            foreach (var link in activeLink.Values)
            {
                VfxLineRenderer vfxLineRenderer = link.GetComponent<VfxLineRenderer>();
                if (vfxLineRenderer != null)
                {
                    vfxLineRenderer.UpdateLink();
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (tag != "Untouchable")
            spriteRenderer.color = connectedColor;
    }

    private void OnMouseExit()
    {
        if (tag != "Untouchable")
            spriteRenderer.color = originalColor;
    }
}