using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCondition : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {   
        if (collision.gameObject.layer == LayerMask.NameToLayer("FixGoo") && !GameManager.Instance.IsEndLevel)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            //LevelManager.Instance.LoadNextLevel();
            GameManager.Instance.LevelFinished();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("GooOnMovement") && GameManager.Instance.IsEndLevel)
        {
            Destroy(collision.gameObject);
            //Debug.Log($"GooOnMovement Destroy ?  {collision.gameObject} ");
        }
    }
}