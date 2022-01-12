using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransitionHandler : MonoBehaviour
{
    public SpawnPoints spawnPoint;
    [HideInInspector] public SpawnManager spawnManager;
    public GameObject crossFadePrefab;
    public string animationFadeStart = "Crossfade_Start";
    public string animationFadeEnd = "Crossfade_End";

    public static SceneTransitionHandler instance;

    public string currentScene { get; private set; }
    //private string currentScene = null;
    private Animator fadeAnimation;
    private bool inTransition = false;
    private WaitForSeconds clipSeconds;
    private string sceneGoto;

    public delegate void SceneEvent(string sceneName, SpawnPoints point);
    public event SceneEvent OnBeginTransitionToNewScene = delegate { };

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    //Create CrossFade Effect
        GameObject newGO = Instantiate(crossFadePrefab, transform, false);
        fadeAnimation = newGO.GetComponentInChildren<Animator>();
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
        //Fade In
            UI.SetControlState(false, instance.gameObject);
            fadeAnimation.Play(animationFadeEnd); 
            var _clip = fadeAnimation.GetCurrentAnimatorClipInfo(0); //Find current clicp
            clipSeconds = new WaitForSeconds(_clip.Length); //Get the length of the clip animation
            StartCoroutine(ResetTransitionAfterFadeIn());
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
        instance.InstanceSceneGoto(sceneName, point);
    }

    public static GameObject GetPlayer() {
        return instance.spawnManager.player;
    }

    public static string CurrentScene() {
        return instance.currentScene;
    }

    private void InstanceSceneGoto(string sceneName, SpawnPoints point) {
        if (!inTransition) {
            UI.SetControlState(true, instance.gameObject);
            inTransition = true;
            spawnPoint = point;
            sceneGoto = sceneName;
            OnBeginTransitionToNewScene(sceneName, point);
        //Crossfade
            fadeAnimation.Play(animationFadeStart); //Start Animation
            var _clip = fadeAnimation.GetCurrentAnimatorClipInfo(0); //Find current clicp
            clipSeconds = new WaitForSeconds(_clip.Length); //Get the length of the clip animation
            StartCoroutine(GoToSceneAfterAnimation());
        }
    }

    private IEnumerator GoToSceneAfterAnimation() {
        yield return clipSeconds;
        SceneManager.LoadScene(sceneGoto);
    }

    private IEnumerator ResetTransitionAfterFadeIn() {
        yield return clipSeconds;
        inTransition = false;
    }
}