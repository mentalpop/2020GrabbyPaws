using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCamera;
//using Invector.vCharacterController.vActions;
using Invector.vCharacterController;
using Cinemachine;

public class SpawnManager : MonoBehaviour
{
    public GameObject prefabPlayer;
    public GameObject prefabDialogueSystem;
    public GameObject prefabCinemachine;
    public GameObject prefabTPCamera;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject dSystem;

    /*
    private void Awake() {
        SpawnPlayer(SpawnPoints.UITestRoomA);
    }
    //*/

    public void SpawnPlayer(SpawnPoints point) {
        SpawnPoint pointToSpawnPlayer = spawnPoints[0];
        bool foundSpawnPoint = false;
        foreach (var spawnPoint in spawnPoints) {
            if (spawnPoint.pointID == point) {
                pointToSpawnPlayer = spawnPoint;
                foundSpawnPoint = true;
                break;
            }
        }
        if (foundSpawnPoint) {
            dSystem = Instantiate(prefabDialogueSystem, Vector3.zero, Quaternion.identity);
    //Spawn the player
            player = Instantiate(prefabPlayer, pointToSpawnPlayer.transform.position, pointToSpawnPlayer.transform.localRotation);
            //vGenericAction gAction = player.GetComponent<vGenericAction>();
            PlayerBehaviour _player = player.GetComponent<PlayerBehaviour>();
            //UI.Instance.player = _player;
        //TP Camera Setup
            GameObject _tpCamera = Instantiate(prefabTPCamera, pointToSpawnPlayer.transform.position, Quaternion.identity);
            UI.AssignPlayerAndCamera(_player, _tpCamera.GetComponent<CinemachineBrain>());
            //vThirdPersonCamera tpCamera = _tpCamera.GetComponent<vThirdPersonCamera>();
            //gAction.mainCamera = tpCamera.GetComponent<Camera>();
            _player.vThirdPersonInput.cameraMain = _tpCamera.GetComponent<Camera>();
            //tpCamera.SetTarget(player.transform);
            //UI.Instance.thirdPersonCamera = tpCamera;
        //Spawn Cinemachine Camera
            GameObject _cCamera = Instantiate(prefabCinemachine, pointToSpawnPlayer.transform.position, Quaternion.identity);
            CinemachineFreeLook cinemachineFreeLook = _cCamera.GetComponent<CinemachineFreeLook>();
            UI.Instance.cFreeLook = cinemachineFreeLook;//tpCamera.GetComponent<CinemachineBrain>(); //Cinemachine Brain is on the TP Camera
            cinemachineFreeLook.LookAt = _player.cameraTarget.transform;
            cinemachineFreeLook.Follow = _player.cameraTarget.transform;
        } else {
            Debug.Log("Failed to find SpawnPoint: "+point);
        }
    }
}