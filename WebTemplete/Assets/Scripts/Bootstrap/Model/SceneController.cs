using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;

    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneController>();

                if (_instance == null)
                {
                    throw new NotImplementedException("SceneController not found!");
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(SceneList scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}