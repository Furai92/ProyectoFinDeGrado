using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private ProceduralStagePropertiesSO backgroundStageProperties;
    [SerializeField] private Transform backgroundStageParent;
    [SerializeField] private MainMenuTravellingCamera travellingCam;

    private ProceduralStageData backgroundStage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundStage = new ProceduralStageData(0, 30, backgroundStageProperties, backgroundStageParent);
        travellingCam.SetUp(backgroundStage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
