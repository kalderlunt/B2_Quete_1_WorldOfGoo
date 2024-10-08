using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooBuilder : MonoBehaviour
{
    [SerializeField] private GameObject gooPrefab;
    [SerializeField] private GameObject GooHolder;
    [SerializeField] private int gooCount = 10; 
    [SerializeField] private float spacing = 1.0f;

    private GameObject previousGoo;

    void Start()
    {
        CreateGooStructures();
    }

    void CreateGooStructures()
    {
        for (int i = 0; i < gooCount; i++)
        {
            Vector2 position = new Vector2(i * spacing, Random.Range(2, 5));
            GameObject goo = Instantiate(gooPrefab, position, Quaternion.identity, GooHolder.transform);

            if (previousGoo != null)
                goo.GetComponent<Goo>().AttachTo(previousGoo.GetComponent<Goo>());

            previousGoo = goo;
        }
    }
}