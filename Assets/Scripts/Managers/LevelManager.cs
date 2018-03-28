using System.Collections;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Singleton;
    private int _currentLevel;
    public int TotalLevel;

    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }

    private void Start()
    {
        if (Singleton == null)
        {
            DontDestroyOnLoad(this);
            Singleton = this;
        }
            
        else if(Singleton != this)
            Destroy(this);
    }
    
    public void GoToLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);			
    }
}
