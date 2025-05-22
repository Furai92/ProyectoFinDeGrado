using UnityEngine;
using System.Collections.Generic;

public class MainMenuTravellingCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CanvasGroup fadeCG;

    private MainMenuTravellingCameraState currentState;
    private ProceduralStageData backgroundStage;

    public void SetUp(ProceduralStageData stage)
    {
        backgroundStage = stage;
        SetNewState();
        gameObject.SetActive(true);
    }

    void Update()
    {
        currentState.UpdateState();
        if (currentState.GetStatePercentDuration() < 0.1f) { fadeCG.alpha = 1 - currentState.GetStatePercentDuration() * 10; }
        else if (currentState.GetStatePercentDuration() > 0.9f) { fadeCG.alpha = (currentState.GetStatePercentDuration() * 10) - 9; }
        else { fadeCG.alpha = 0; }



        if (currentState.GetStatePercentDuration() >= 1) 
        {
            SetNewState();
        }
    }

    private void SetNewState() 
    {
        int rng = Random.Range(0, 3);
        switch (rng) 
        {
            case 0: 
                {
                    currentState = new HighFlyCamState();
                    break;
                }
            case 1: 
                {
                    currentState = new ForwardMovementState();
                    break;
                }
            default: 
                {
                    currentState = new SlowPanningState();
                    break;
                }
        }
        currentState.Initialize(backgroundStage, cam);
    }

    private abstract class MainMenuTravellingCameraState 
    {
        public abstract float GetStatePercentDuration();
        public abstract void UpdateState();
        public abstract void Initialize(ProceduralStageData stage, Camera c);
    }
    private class HighFlyCamState : MainMenuTravellingCameraState
    {
        private Vector3 startPos;
        private Vector3 endPos;
        private Camera cameraManaged;
        private float t;

        private const float STATE_DURATION = 15f;
        private const float DISTANCE_FROM_CENTER = 100f;
        private const float CAM_INCLINATION_ANGLE = 30f;
        private const float HEIGHT = 30f;

        public override void Initialize(ProceduralStageData stage, Camera c)
        {
            // Calculate the center of the stage averaging out each node position
            Vector3 combinedPositions = Vector3.zero;
            for (int i = 0; i < stage.GetLayoutList().Count; i++) 
            {
                combinedPositions += stage.GetLayoutList()[i].piece.transform.position;
            }
            combinedPositions /= stage.GetLayoutList().Count;
            float flyAngle = Random.Range(0, 361);
            startPos = new Vector3(combinedPositions.x + Mathf.Sin(flyAngle * Mathf.Deg2Rad) * DISTANCE_FROM_CENTER, HEIGHT, combinedPositions.z + Mathf.Cos(flyAngle * Mathf.Deg2Rad) * DISTANCE_FROM_CENTER);
            endPos = new Vector3(combinedPositions.x + Mathf.Sin((flyAngle - 180) * Mathf.Deg2Rad) * DISTANCE_FROM_CENTER, HEIGHT, combinedPositions.z + Mathf.Cos((flyAngle - 180) * Mathf.Deg2Rad) * DISTANCE_FROM_CENTER);
            c.transform.position = startPos;
            c.transform.rotation = Quaternion.Euler(CAM_INCLINATION_ANGLE, GameTools.AngleBetween(startPos, endPos), 0);
            cameraManaged = c;
            t = 0;
        }
        public override void UpdateState()
        {
            t += Time.deltaTime / STATE_DURATION;
            cameraManaged.transform.position = Vector3.Lerp(startPos, endPos, t);
        }


        public override float GetStatePercentDuration()
        {
            return t;
        }
    }

    private class SlowPanningState : MainMenuTravellingCameraState
    {
        private float angleStart;
        private float angleEnd;
        private Camera cameraManaged;
        private float t;

        private const float CAM_HEIGHT = 1f;
        private const float STATE_DURATION = 10f;
        private const float CAM_INCLINATION_ANGLE = -10f;

        public override void Initialize(ProceduralStageData stage, Camera c)
        {
            cameraManaged = c;
            cameraManaged.transform.position = stage.GetLayoutList()[Random.Range(0, stage.GetLayoutList().Count)].piece.transform.position + new Vector3(0, CAM_HEIGHT, 0);
            angleStart = Random.Range(0, 361);
            angleEnd = angleStart + 180;
            t = 0;
        }
        public override void UpdateState()
        {
            t += Time.deltaTime / STATE_DURATION;
            cameraManaged.transform.rotation = Quaternion.Euler(CAM_INCLINATION_ANGLE, Mathf.Lerp(angleStart, angleEnd, t), 0);
        }


        public override float GetStatePercentDuration()
        {
            return t;
        }
    }
    private class ForwardMovementState : MainMenuTravellingCameraState
    {
        private Vector3 startPos;
        private Vector3 endPos;
        private float camAngleY;
        private Camera cameraManaged;
        private float t;

        private const float CAM_HEIGHT = 1f;
        private const float STATE_DURATION = 10f;
        private const float CAM_INCLINATION_ANGLE_START = 5f;
        private const float CAM_INCLINATION_ANGLE_END = -30f;

        public override void Initialize(ProceduralStageData stage, Camera c)
        {
            cameraManaged = c;
            MapNode nodeSelected = stage.GetLayoutList()[Random.Range(0, stage.GetLayoutList().Count)];
            List<Vector3> validEndPositions = new List<Vector3>();
            if (nodeSelected.con_up != null) { validEndPositions.Add(nodeSelected.con_up.piece.transform.position); }
            if (nodeSelected.con_down != null) { validEndPositions.Add(nodeSelected.con_down.piece.transform.position); }
            if (nodeSelected.con_left != null) { validEndPositions.Add(nodeSelected.con_left.piece.transform.position); }
            if (nodeSelected.con_right != null) { validEndPositions.Add(nodeSelected.con_right.piece.transform.position); }

            startPos = nodeSelected.piece.transform.position + new Vector3(0, CAM_HEIGHT, 0);
            if (validEndPositions.Count > 0) { endPos = validEndPositions[Random.Range(0, validEndPositions.Count)] + new Vector3(0, CAM_HEIGHT, 0); } else { endPos = startPos; }

            cameraManaged.transform.position = startPos;
            camAngleY = GameTools.AngleBetween(startPos, endPos);
            t = 0;
        }
        public override void UpdateState()
        {
            t += Time.deltaTime / STATE_DURATION;
            cameraManaged.transform.position = Vector3.Lerp(startPos, endPos, t);
            cameraManaged.transform.rotation = Quaternion.Euler(Mathf.Lerp(CAM_INCLINATION_ANGLE_START, CAM_INCLINATION_ANGLE_END, t), camAngleY, 0);
        }


        public override float GetStatePercentDuration()
        {
            return t;
        }
    }
}
