using UnityEngine;
using System.Collections.Generic;

public class IngameCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;


    private ICamState currentCamState;
    
    private float fovMult;
    private float settingsFovMult;

    private const float FOV_MULT_MIN = 0.75f;
    private const float FOV_MULT_MAX = 1.35f;
    private const float SETTINGS_FOV_MIN = 60f;
    private const float SETTINGS_FOV_MAX = 100f;


    private void OnEnable()
    {
        fovMult = 0.5f;
        OnSettingsChanged();
        SelectNewCameraMode();
        
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.PlayerSpawnedEvent += OnPlayerSpawned;
        EventManager.GameSettingsChangedEvent += OnSettingsChanged;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.PlayerSpawnedEvent -= OnPlayerSpawned;
        EventManager.GameSettingsChangedEvent -= OnSettingsChanged;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;

        currentCamState?.EndState(this);
    }
    private void Update()
    {
        currentCamState?.UpdateState(this);
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        SelectNewCameraMode();
    }
    private void OnPlayerSpawned(PlayerEntity p) 
    {
        SelectNewCameraMode();
    }
    private void OnPlayerDefeated() 
    {
        ChangeCamState(new DefeatCamState());
    }
    private void SelectNewCameraMode() 
    {
        switch (StageManagerBase.GetCurrentStateType()) 
        {
            case StageStateBase.GameState.Victory:
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.BossFight:
            case StageStateBase.GameState.Delay:
                {
                    ChangeCamState(PlayerEntity.ActiveInstance == null ? new StandByCamState() : new PlaymodeCamState());
                    break;
                }
            case StageStateBase.GameState.Intro: { ChangeCamState(new IntroCamState()); break; }
        }
    }
    private void ChangeCamState(ICamState cs) 
    {
        if (currentCamState != null && cs.GetCamStateName() == currentCamState.GetCamStateName()) { return; }

        currentCamState?.EndState(this);
        currentCamState = cs;
        cs.StartState(this);
    }
    private void OnSettingsChanged()
    {
        settingsFovMult = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.CamFov);
        UpdateCameraFov();
    }

    private void ChangeFovMult(float m) 
    {
        fovMult = Mathf.Clamp01(m);
        UpdateCameraFov();
    }
    private void UpdateCameraFov() 
    {
        cam.fieldOfView = Mathf.Lerp(FOV_MULT_MIN, FOV_MULT_MAX, fovMult) * Mathf.Lerp(SETTINGS_FOV_MIN, SETTINGS_FOV_MAX, settingsFovMult);
    }
    #region States
    private interface ICamState 
    {
        public void StartState(IngameCamera c);
        public void EndState(IngameCamera c);
        public void UpdateState(IngameCamera c);
        public string GetCamStateName();
    }
    private class StandByCamState : ICamState
    {

        public void EndState(IngameCamera c)
        {

        }

        public void StartState(IngameCamera c)
        {
            c.ChangeFovMult(0.5f);
        }

        public void UpdateState(IngameCamera c)
        {
            
        }

        public string GetCamStateName()
        {
            return "STANDBY";
        }

    }
    private class DefeatCamState : ICamState
    {
        private float lookAngle;
        private LayerMask camTerrainMask;
        private float camAngleDurationRemaining;

        private const float WALL_SEPARATION = 0.2f;
        private const float DISTANCE_TO_PLAYER = 12f;
        private const float CAM_ANGLE_DURATION = 2f;

        public void EndState(IngameCamera c)
        {

        }

        public void StartState(IngameCamera c)
        {
            c.ChangeFovMult(0f);
            lookAngle = Random.Range(0, 361);
            camTerrainMask = LayerMask.GetMask("Walls", "Ground");
            camAngleDurationRemaining = CAM_ANGLE_DURATION;
        }

        public void UpdateState(IngameCamera c)
        {
            RaycastHit h;
            Vector3 offset = GameTools.OffsetFromAngle(lookAngle, DISTANCE_TO_PLAYER);

            Physics.Raycast(PlayerEntity.ActiveInstance.transform.position,
                offset, out h,
                DISTANCE_TO_PLAYER, camTerrainMask);

            Quaternion camRotation = Quaternion.Euler(0, GameTools.AngleBetween(PlayerEntity.ActiveInstance.transform.position + offset, PlayerEntity.ActiveInstance.transform.position), 0);
            if (h.collider == null)
            {
                c.transform.SetPositionAndRotation(PlayerEntity.ActiveInstance.transform.position + offset, camRotation);
            }
            else
            {
                c.transform.SetPositionAndRotation(Vector3.MoveTowards(h.point, PlayerEntity.ActiveInstance.transform.position, WALL_SEPARATION), camRotation);
            }

            camAngleDurationRemaining -= Time.unscaledDeltaTime;
            if (camAngleDurationRemaining <= 0) 
            {
                camAngleDurationRemaining = CAM_ANGLE_DURATION;
                lookAngle += 100f;
            }
        }

        public string GetCamStateName()
        {
            return "DEFEAT";
        }
    }
    private class PlaymodeCamState : ICamState
    {
        private LayerMask camTerrainMask;
        private float higherFovEndTime;
        private float dashAnimFovMult;

        private const float WALL_SEPARATION = 0.2f;
        private const float DASH_FOV_DURATION = 0.25f;
        private const float FOV_CHANGE_SPEED = 3f;

        public void EndState(IngameCamera c)
        {
            
            EventManager.PlayerDashStartedEvent -= OnPlayerDashStarted;
        }

        public void StartState(IngameCamera c)
        {
            c.ChangeFovMult(0.5f);
            camTerrainMask = LayerMask.GetMask("Walls", "Ground");
            EventManager.PlayerDashStartedEvent += OnPlayerDashStarted;
        }

        public void UpdateState(IngameCamera c)
        {
            RaycastHit h;
            Physics.Raycast(PlayerEntity.ActiveInstance.transform.position,
                PlayerEntity.ActiveInstance.CamTargetTransform.position - PlayerEntity.ActiveInstance.transform.position, out h,
                (PlayerEntity.ActiveInstance.transform.position - PlayerEntity.ActiveInstance.CamTargetTransform.position).magnitude, camTerrainMask);

            if (h.collider == null)
            {
                c.transform.SetPositionAndRotation(PlayerEntity.ActiveInstance.CamTargetTransform.position, PlayerEntity.ActiveInstance.CamTargetTransform.rotation);
            }
            else
            {
                c.transform.SetPositionAndRotation(Vector3.MoveTowards(h.point, PlayerEntity.ActiveInstance.transform.position, WALL_SEPARATION)
                    , PlayerEntity.ActiveInstance.CamTargetTransform.rotation);
            }

            // Dash fov Anim
            float targetFov = Time.time > higherFovEndTime ? 0.5f : 1;
            dashAnimFovMult = Mathf.MoveTowards(dashAnimFovMult, targetFov, Time.deltaTime * FOV_CHANGE_SPEED);
            c.ChangeFovMult(dashAnimFovMult);
        }

        private void OnPlayerDashStarted(Vector3 pos, float dir)
        {
            higherFovEndTime = Time.time + DASH_FOV_DURATION;
        }

        public string GetCamStateName()
        {
            return "PLAYMODE";
        }
    }
    private class IntroCamState : ICamState
    {
        private float t;
        private float introRotationStart;
        private float introRotationEnd;
        private Vector3 introPosStart;
        private Vector3 introPosEnd;

        private const float INTRO_PLANE_HEIGHT = 0.5f;
        private const float INTRO_PLANE_DURATION = 2.5f;

        public void EndState(IngameCamera c)
        {

        }

        public void StartState(IngameCamera c)
        {
            t = 1;
            c.ChangeFovMult(0.5f);
        }

        public void UpdateState(IngameCamera c)
        {
            if (t >= 1)
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

                c.transform.position = introPosStart;
                introRotationStart = Random.Range(0, 361);
                introRotationEnd = introRotationStart + Random.Range(-40, 40);
                t = 0;
            }
            else
            {
                t += Time.deltaTime / INTRO_PLANE_DURATION;
                c.transform.SetPositionAndRotation(Vector3.Lerp(introPosStart, introPosEnd, t), Quaternion.Euler(0, Mathf.LerpAngle(introRotationStart, introRotationEnd, t), 0));
            }
        }

        public string GetCamStateName()
        {
            return "INTRO";
        }
    }
    #endregion
}
