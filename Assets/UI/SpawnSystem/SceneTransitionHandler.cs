using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransitionHandler : MonoBehaviour
{
    public SpawnPoints spawnPoint;
    [HideInInspector] public SpawnManager spawnManager;

    public static SceneTransitionHandler instance;

    public string currentScene { get; private set; }
    //private string currentScene = null;

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (currentScene == null) {
            //Debug.Log(mode);
        //Find SpawnManager
            spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager == null) {
                Debug.Log("SpawnManager not found in: " + scene.name);
            } else {
                currentScene = scene.name;
                spawnManager.SpawnPlayer(spawnPoint);
            }
        }
    }

    private void SceneManager_sceneUnloaded(Scene scene) {
        if (currentScene == scene.name) {
            currentScene = null;
        }
    }

    public static void SceneGoto(string sceneName, SpawnPoints point) {
        SceneManager.LoadScene(sceneName);
        instance.spawnPoint = point;
    }

    public static GameObject GetPlayer() {
        return instance.spawnManager.player;
    }

    public static string CurrentScene() {
        return instance.currentScene;
    }
}