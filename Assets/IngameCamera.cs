using UnityEngine;

public class IngameCamera : MonoBehaviour
{

    private enum CameraMode { StandBy, Intro, Playable, Outro }

    private CameraMode currentCameraMode;
    private LayerMask camTerrainMask;

    private const float WALL_SEPARATION = 0.2f;

    private void OnEnable()
    {
        UpdateCameraMode();
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
        UpdateCameraMode();
    }
    private void OnPlayerSpawned(PlayerEntity p) 
    {
        UpdateCameraMode();
    }
    private void UpdateCameraMode() 
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
            case StageStateBase.GameState.Intro: { currentCameraMode = CameraMode.Intro; break; }
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
