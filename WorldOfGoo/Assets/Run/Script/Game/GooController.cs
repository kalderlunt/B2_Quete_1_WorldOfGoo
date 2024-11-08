using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GooController : MonoBehaviour
{
    public static int IdGeneral = 0;
    public int Id = 0;
    
    private const float G = 0.1f;

    [SerializeField] private float springMaxDistance = 2f;
    [SerializeField] private float springMinDistance = 2f;
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

    [SerializeField] private GameObject previewLinkPrefab;
    [SerializeField] private GameObject linkPrefab;

    private Dictionary<GameObject, GameObject> activePreviewslinks = new ();
    private Dictionary<GameObject, GameObject> activeLink          = new ();
    private float duration      = 1f;
    private float elapsedTime   = 0f;

    private int FixGooLayer        = 6;
    private int FreeGooLayer       = 7;
    private int GooOnMovementLayer = 9;
    private int FixPlatormLayer    = 10;

    private bool isLinked   = false;
    private bool isClicked  = false;

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

        GameManager.Instance.AllGoos.Add(gameObject);

        if (hitColliders.Count > 0)
        {
            foreach (Collider2D other in hitColliders)
            {
                if (other != null)
                    gameObject.layer = FixPlatormLayer;

                AttachTo(other.gameObject);
            }

            RemoveBrokenPreviewLinks(activePreviewslinks);

            CreateLinkIfConnected();
        }
    }


    private void DetectGoo()
    {
        // detection d'autre goo (tag - overlapshepere)
        Vector2 position = transform.position;

        hitColliders = Physics2D.OverlapCircleAll(position, springMaxDistance, 1 << FixPlatormLayer | 1 << FixGooLayer).ToList();


        
        foreach (Collider2D other in hitColliders)
        {
            //Debug.Log($"Number of collision : {hitColliders.Count}");
            //Debug.Log($"Distance between collisions : {Vector2.Distance(position, other.transform.position)}");

            if (other != null && !activePreviewslinks.ContainsKey(other.gameObject))
            {
                GameObject previewInstance = Instantiate(previewLinkPrefab, position, Quaternion.identity);

                VfxLineRenderer vfxLineRenderer = previewInstance.GetComponent<VfxLineRenderer>();
                GameObject jointA = this.gameObject;
                GameObject jointB = other.gameObject;
                vfxLineRenderer.Initialize(jointA, jointB);

                activePreviewslinks.Add(other.gameObject, previewInstance);
            }
        }

        foreach (var entry in activePreviewslinks.ToList())
        {
            GameObject otherGoo = entry.Key;

            float distance = Vector2.Distance(transform.position, otherGoo.transform.position) - 0.5f;                  //// JE SAIS PAS POURQUOI IL FAUT METTRE - 0.5f  Nombre Magique du radius de la balle    ////
            if (distance > springMaxDistance)
            {
                RemoveBrokenPreviewLink(otherGoo, activePreviewslinks);
            }
        }
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

        // Mise � jour des liens cr��s
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
        GameManager.Instance.GooPlaced.Remove(gameObject);


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

        // this intantiate to delete other
        SpringJoint2D[] otherSpringsInThisGoo = otherGoo.GetComponents<SpringJoint2D>();
        foreach (SpringJoint2D otherJoint in otherSpringsInThisGoo)
        {
            if (otherJoint.connectedBody == rb)
            {
                Destroy(otherJoint);
                ConnectedGoos.Remove(otherJoint);
                if (tag != "Untouchable")
                    rb.bodyType = RigidbodyType2D.Dynamic;
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
                
                if (otherGoo.tag != "Untouchable")
                    otherGoo.GetComponent<GooController>().rb.bodyType = RigidbodyType2D.Dynamic;
            }
            //Debug.Log($"Le SpringJoint {otherGoo.name} � �t� d�tach� avec succ�s du {this.name}");
            
            //ConnectedGoos.RemoveAll(joint => joint == null || joint.connectedBody == null);
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

        if (otherController.tag != "Untouchable")
            otherController.rb.bodyType = RigidbodyType2D.Dynamic;
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
        if (tag == "Untouchable") return;


        RemoveGooMovementScript();
        isClicked = true;
        rb.isKinematic  = true;
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.velocity     = Vector2.zero;

        Id = 0;
        gameObject.layer = FreeGooLayer;
        GameManager.Instance.GooPlaced.Remove(gameObject);

        DetachAllLink();
    }

    private void OnMouseDrag()
    {
        if (tag == "Untouchable") return;
        
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.velocity = Vector2.zero;

        DetachAllLink();
        MouseDragSystem();
        DetectGoo();
        UpdateLinks();
    }

    public void OnMouseUp()
    {

        if (tag == "Untouchable") return;

        RemoveGooMovementScript();

        isClicked = false;
        rb.isKinematic = false;
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.velocity     = Vector2.zero;
            

        foreach (Collider2D other in hitColliders)
        {
            if (other != null)
                gameObject.layer = FixGooLayer;

            AttachTo(other.gameObject);
        }

        RemoveBrokenPreviewLinks(activePreviewslinks);

        CreateLinkIfConnected();

        AssignId();
    }

    public void AssignId()
    {
        IdGeneral++;
        Id = IdGeneral;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (isClicked)
            return;
        
        if (collision.gameObject.layer      == gameObject.layer 
            || gameObject.layer             == FixPlatormLayer 
            && collision.gameObject.layer   == FixGooLayer 
            || gameObject.layer             == FixGooLayer
            && collision.gameObject.layer   == FixPlatormLayer)
            return;


        if (collision.gameObject.layer == FixPlatormLayer || collision.gameObject.layer == FixGooLayer)
        {
            if (collision.gameObject.GetComponent<ParticleSystem>() ||
                collision.gameObject.GetComponent<GooController>())
            {
                if (!gameObject.TryGetComponent<GooMovement>(out _))
                {
                    GooMovement gooMovement = gameObject.AddComponent<GooMovement>();
                    gooMovement.speed = 2f;
                    gooMovement.CollisionObj = collision.gameObject;
                    //Debug.Log("Combien de fois il passe dedans :");
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


    public void RemoveGooMovementScript()
    {
        gameObject.layer = FreeGooLayer;


        if (TryGetComponent<GooMovement>(out GooMovement gooMovement))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gooMovement);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsEndLevel && gameObject.layer != GooOnMovementLayer)
                ApplyGravitationalForces();
    }

    private void ApplyGravitationalForces()
    {
        Vector2 totalForce = Vector2.zero;

        foreach (GameObject otherGoo in GameManager.Instance.AllGoos)
        {
            if (otherGoo.layer != GooOnMovementLayer)
            {
                Vector2 direction = otherGoo.transform.position - transform.position;
                float distance = direction.magnitude;

                if (distance > 0f)
                {
                    float mass1 = rb.mass;
                    float mass2 = otherGoo.GetComponent<Rigidbody2D>().mass;
                    float forceMagnitude = G * (mass1 * mass2) / Mathf.Pow(distance, 2);

                    totalForce += direction.normalized * forceMagnitude;
                }
            }
        }

        rb.AddForce(totalForce);
    }
}