using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudInteractionManager : MonoBehaviour
{
    [SerializeField] private Transform interactionPanelParent;
    [SerializeField] private HudWeaponCard wpnCard;
    [SerializeField] private Transform interactableNamePanel;
    [SerializeField] private Transform visibilityParent;
    [SerializeField] private TextMeshProUGUI interactableNameText;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private Camera mCamRef;

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

        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        OnUiFocusChanged(IngameMenuManager.GetActiveMenu());
    }
    private void OnDisable()
    {
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
    }
    private void OnUiFocusChanged(IGameMenu m) 
    {
        visibilityParent.gameObject.SetActive(m == null);
    }

    void Update()
    {
        UpdatePanelPosition();
        if (InputManager.Instance.GetInteractInput() && targetInteractable != null) { targetInteractable.OnInteract(0); }
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

            if (targetInteractable.GetInfo().weapon == null)
            {
                interactableNameText.text = targetInteractable.GetInfo().name;
                wpnCard.gameObject.SetActive(false);
                interactableNamePanel.gameObject.SetActive(true);
            }
            else 
            {
                wpnCard.gameObject.SetActive(true);
                wpnCard.SetUp(targetInteractable.GetInfo().weapon);
                interactableNamePanel.gameObject.SetActive(false);
            }
            actionNameText.text = targetInteractable.GetInfo().desc;
        }
    }
    private void UpdatePanelPosition() 
    {
        if (targetInteractable == null) { return; }

        interactionPanelParent.position = mCamRef.WorldToScreenPoint(targetInteractable.GetInfo().gotransform.position + wposOffset);
    }
    public static void UpdateTargetInteractable(Interactable inter) 
    {
        if (instance == null) { return; }

        instance.targetInteractable = inter;
        instance.UpdatePanelVisuals();
        instance.UpdatePanelPosition();
    }
}
