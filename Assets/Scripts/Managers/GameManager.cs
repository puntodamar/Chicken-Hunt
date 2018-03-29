using System;
using System.Collections;
using Scriptables;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Singleton;
		public static System.Action OnGameOver;
		public int TotalChickenToEat = 0;
		
		private int _caughtChickens = 0;		
		private bool _goToGameScene = false;		
		
		private void Start()
		{
			if (Singleton == null)
			{
				Singleton = this;
				DontDestroyOnLoad(this);
			}
				
			else if (Singleton != this)
				Destroy(this);
			SceneManager.sceneLoaded += OnGameSceneLoaded;						
			//Debug.Log(Resources.Load("LevelDatas/Level1") as LevelData);
		}



		void ShowGameOverDialog()
		{
		
		}



		void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			int currentLevel = LevelManager.Singleton.CurrentLevel;
			LevelData levelData = Resources.Load("LevelDatas/Level"+currentLevel) as LevelData;
			TotalChickenToEat = levelData.TotalChickenToEat;
		}

		public void ChickenCaught()
		{
			_caughtChickens++;

			if (_caughtChickens >= TotalChickenToEat)
			{
				if (OnGameOver != null)
					OnGameOver();
			}
		}
		
		
	
	}
}
