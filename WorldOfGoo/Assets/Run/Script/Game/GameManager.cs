using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> GooOnMovements = new();
    public List<GameObject> GooPlaced      = new();
    public bool EndLevel = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(this);
    }

    public void LevelFinished()
    {
        if (EndLevel)
            return;

        EndLevel = true;

        foreach (GameObject goo in GooOnMovements)
        {
            goo.tag = "Untouchable";
            goo.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (GameObject goo in GooPlaced)
        {
            goo.tag = "Untouchable";
        }


        StartCoroutine(CoroutineEndLevel());
    }

    private IEnumerator CoroutineEndLevel()
    {
        while (GooOnMovements.Count > 0)
        {
            yield return null;
        }
        LevelManager.Instance.LoadNextLevel();
    }
}