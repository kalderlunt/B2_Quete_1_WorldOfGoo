using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MagnetDetection : MonoBehaviour
{
    [SerializeField] private float moveSpeed        = 1.0f;
    [SerializeField] private float springDistance   = 2f;

    private List<Collider2D> hitGooColliders;
    private Transform targetPoint;                           // Target position for movement
    private bool isMoving = false;

    private void Update()
    {
        if (isMoving)
        {
            UpdateMovement();
        }
    }

    public void UpdateMagnet()
    {
        Vector2 position = transform.position;
        hitGooColliders = new List<Collider2D>(Physics2D.OverlapCircleAll(position, springDistance, LayerMask.GetMask("Goo")));
        
        if (hitGooColliders.Count > 0)
        {
            targetPoint = hitGooColliders[0].transform;
            isMoving = true;
        }
    }

    public void UpdateMovement()
    {
        if (targetPoint != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                isMoving = false;
                PerformRandomActionOnConnectedGoos();
            }
        }
    }

    public Transform ChoiceNextPoint()
    {
        GooController newTarget = targetPoint.GetComponent<GooController>();
        int randomIndex = Random.Range(0, newTarget.ConnectedGoos.Count);
        return newTarget.ConnectedGoos[randomIndex].transform;
    }

    private void PerformRandomActionOnConnectedGoos()
    {
        Transform nextPoint = ChoiceNextPoint();

        if (nextPoint != null)
        {
            Debug.Log($"Random next point selected: {nextPoint.gameObject.name}");

            targetPoint = nextPoint;
            isMoving = true;
        }
    }
}