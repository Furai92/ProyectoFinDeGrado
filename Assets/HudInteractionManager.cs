using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudInteractionManager : MonoBehaviour
{
    [SerializeField] private Transform interactionPanelParent;
    [SerializeField] private TextMeshProUGUI interactionNameText;
    [SerializeField] private TextMeshProUGUI interactionActionText;

    private Camera mcam;
    private Interactable targetInteractable;
    private static HudInteractionManager instance;
    private Vector3 wposOffset;

    private const float WORLD_TO_HUD_VERTICAL_OFFSET = 1f;


    private void OnEnable()
    {
        instance = this;

        instance.UpdatePanelVisuals();
        instance.UpdatePanelPosition();
        wposOffset = new Vector3(0, WORLD_TO_HUD_VERTICAL_OFFSET, 0);
    }

    public void SetUp() 
    {
        mcam = Camera.main;
    }

    void Update()
    {
        UpdatePanelPosition();
    }
    private void UpdatePanelVisuals() 
    {
        if (targetInteractable == null)
        {
            interactionPanelParent.gameObject.SetActive(false);
        }
        else 
        {
            interactionPanelParent.gameObject.SetActive(true);
            interactionNameText.text = targetInteractable.GetInfo().name;
            interactionActionText.text = targetInteractable.GetInfo().desc;
        }
    }
    private void UpdatePanelPosition() 
    {
        if (targetInteractable == null) { return; }

        interactionPanelParent.position = mcam.WorldToScreenPoint(targetInteractable.GetInfo().gotransform.position + wposOffset);
    }
    public static void UpdateTargetInteractable(Interactable inter) 
    {
        if (instance == null) { return; }

        instance.targetInteractable = inter;
        instance.UpdatePanelVisuals();
        instance.UpdatePanelPosition();
    }
}
