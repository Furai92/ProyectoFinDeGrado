using UnityEngine;
using System.Collections.Generic;

public class IngameCamera : MonoBehaviour
{

    private enum CameraMode { StandBy, Intro, Playable, Outro }

    private CameraMode currentCameraMode;
    private LayerMask camTerrainMask;

    private float modeT;
    private float introRotationStart;
    private float introRotationEnd;
    private Vector3 introPosStart;
    private Vector3 introPosEnd;

    private const float INTRO_PLANE_HEIGHT = 0.5f;
    private const float INTRO_PLANE_DURATION = 2.5f;
    private const float WALL_SEPARATION = 0.2f;

    private void OnEnable()
    {
        SelectNewCameraMode();
        camTerrainMask = LayerMask.GetMask("Walls", "Ground");
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.PlayerSpawnedEvent += OnPlayerSpawned;
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.PlayerSpawnedEvent -= OnPlayerSpawned;
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        SelectNewCameraMode();
    }
    private void OnPlayerSpawned(PlayerEntity p) 
    {
        SelectNewCameraMode();
    }
    private void SelectNewCameraMode() 
    {
        switch (StageManagerBase.GetCurrentStateType()) 
        {
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.BossFight: 
                {
                    currentCameraMode = PlayerEntity.ActiveInstance == null ? currentCameraMode = CameraMode.StandBy : CameraMode.Playable;
                    break;
                }
            case StageStateBase.GameState.Intro: { currentCameraMode = CameraMode.Intro; modeT = 1; break; }
            case StageStateBase.GameState.Victory:
            case StageStateBase.GameState.GameOver: 
                {
                    currentCameraMode = CameraMode.Outro;
                    break;
                }
        }
    }

    void Update()
    {
        switch (currentCameraMode) 
        {
            case CameraMode.Playable: 
                {
                    RaycastHit h;
                    Physics.Raycast(PlayerEntity.ActiveInstance.transform.position, 
                        PlayerEntity.ActiveInstance.CamTargetTransform.position - PlayerEntity.ActiveInstance.transform.position, out h,
                        (PlayerEntity.ActiveInstance.transform.position - PlayerEntity.ActiveInstance.CamTargetTransform.position).magnitude, camTerrainMask);

                    if (h.collider == null)
                    {
                        transform.SetPositionAndRotation(PlayerEntity.ActiveInstance.CamTargetTransform.position, PlayerEntity.ActiveInstance.CamTargetTransform.rotation);
                    }
                    else
                    {
                        transform.SetPositionAndRotation(Vector3.MoveTowards(h.point, PlayerEntity.ActiveInstance.transform.position, WALL_SEPARATION)
                            , PlayerEntity.ActiveInstance.CamTargetTransform.rotation);
                    }
                    break;
                }
            case CameraMode.Intro: 
                {
                    if (modeT >= 1)
                    {
                        List<MapNode> stageLayout = StageManagerBase.GetStageLayout();
                        MapNode selectedStartNode = stageLayout[Random.Range(0, stageLayout.Count)];

                        introPosStart = new Vector3(selectedStartNode.piece.transform.position.x, INTRO_PLANE_HEIGHT, selectedStartNode.piece.transform.position.z);
                        List<MapNode> possibleIntroEndNodes = new List<MapNode>();
                        if (selectedStartNode.con_down != null) { possibleIntroEndNodes.Add(selectedStartNode.con_down); }
                        if (selectedStartNode.con_left != null) { possibleIntroEndNodes.Add(selectedStartNode.con_left); }
                        if (selectedStartNode.con_right != null) { possibleIntroEndNodes.Add(selectedStartNode.con_right); }
                        if (selectedStartNode.con_up != null) { possibleIntroEndNodes.Add(selectedStartNode.con_up); }

                        MapNode selectedEndNode = possibleIntroEndNodes[Random.Range(0, possibleIntroEndNodes.Count)];
                        introPosEnd = new Vector3(selectedEndNode.piece.transform.position.x, INTRO_PLANE_HEIGHT, selectedEndNode.piece.transform.position.z);

                        transform.position = introPosStart;
                        introRotationStart = Random.Range(0, 361);
                        introRotationEnd = introRotationStart + Random.Range(-40, 40);
                        modeT = 0;
                    }
                    else 
                    {
                        modeT += Time.deltaTime / INTRO_PLANE_DURATION;
                        transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(introRotationStart, introRotationEnd, modeT), 0);
                        transform.position = Vector3.Lerp(introPosStart, introPosEnd, modeT);
                    }
                    break;
                }
            case CameraMode.Outro: 
                {
                    break;
                }
            default: 
                {
                    break;
                }
        }
    }
}
