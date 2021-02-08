using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransitionHandler : MonoBehaviour
{
    public SpawnPoints spawnPoint;
    [HideInInspector] public SpawnManager spawnManager;

    public static SceneTransitionHandler instance;

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
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
    //Find SpawnManager
        spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager == null) {
            Debug.Log("SpawnManager not found in: "+scene.name);
        } else {
            spawnManager.SpawnPlayer(spawnPoint);
        }
    }

    public static void SceneGoto(string sceneName, SpawnPoints point) {
        SceneManager.LoadScene(sceneName);
        instance.spawnPoint = point;
    }

    public static GameObject GetPlayer() {
        return instance.spawnManager.player;
    }
}