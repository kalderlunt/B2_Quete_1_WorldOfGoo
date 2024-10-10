using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBuilder : MonoBehaviour
{
    public GameObject gooPrefab;
    public GameObject springPrefab;

    public void BuildStructure(Vector2[] positions)
    {
/*        GooModel[] gooModels = new GooModel[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject goo = Instantiate(gooPrefab, positions[i], Quaternion.identity);
            gooModels[i] = goo.GetComponent<GooModel>();
        }

        for (int i = 0; i < positions.Length - 1; i++)
        {
            GameObject spring = Instantiate(springPrefab);
            GooSpring gooSpring = spring.GetComponent<GooSpring>();
            gooSpring.gooA = gooModels[i];
            gooSpring.gooB = gooModels[i + 1];
        }*/
    }
}