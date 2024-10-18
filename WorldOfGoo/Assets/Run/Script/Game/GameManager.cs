using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> GooOnMovements  = new();
    public List<GameObject> GooPlaced       = new();
    public List<GameObject> AllGoos         = new();
    public bool IsEndLevel = false;


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
        if (IsEndLevel)
            return;

        IsEndLevel = true;

        
        foreach (GameObject goo in GooOnMovements)
        {
            goo.tag = "Untouchable";
            goo.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (GameObject goo in GooPlaced)
        {
            goo.tag = "Untouchable";
        }
        
        AllGoos.Clear();
        StartCoroutine(CoroutineEndLevel());
    }

    private IEnumerator CoroutineEndLevel()
    {
        while (GooOnMovements.Count > 0)
        {
            yield return null;
        }
        LoadNextLevel();
    }

    // ----------------------------------------------------------------------------------------------


    public void LoadNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }


    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene($"{levelName}");
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}