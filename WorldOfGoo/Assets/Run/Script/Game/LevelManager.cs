using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float globalGravity = -9.81f;
    public static LevelManager Instance { get; private set; }

    [Header("Level")]
    private int currentLevelIndex;

    [Header("Creation de Goo sur le terrain")]
    [SerializeField] private GameObject prefabGoo;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform parentHolderGoos;

    [Header("Parameters")]
    [SerializeField] private int numberOfInstances = 5;
    [SerializeField] private float scaleDuration = 1f;
    [SerializeField] private float waitBetweenInstantiations = 1f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (Physics2D.gravity != new Vector2(0, globalGravity))
            Physics2D.gravity = new Vector2(0, globalGravity);


        StartCoroutine(InstantiatePrefabs());
    }

    private IEnumerator InstantiatePrefabs()
    {
        for (int i = 0; i < numberOfInstances; i++)
        {
            GameObject instance = Instantiate(prefabGoo, spawnPoint.position, Quaternion.identity, parentHolderGoos);

            StartCoroutine(ScaleObject(instance));

            yield return new WaitForSeconds(waitBetweenInstantiations);
        }
    }

    private IEnumerator ScaleObject(GameObject obj)
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }
}