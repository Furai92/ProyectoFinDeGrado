using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudInteractionManager : MonoBehaviour
{
    [SerializeField] private Transform positionParent;
    [SerializeField] private HudWeaponCard wpnCard;
    [SerializeField] private Transform interactableNamePanel;
    [SerializeField] private Transform visibilityParent;
    [SerializeField] private TextMeshProUGUI interactableNameText;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private Camera mCamRef;
    [SerializeField] private Transform currentlyEquippedParent;
    [SerializeField] private HudWeaponCard currentlyEquippedWeapon;

    private Interactable targetInteractable;
    private static HudInteractionManager instance;
    private Vector3 wposOffset;
    private bool playerDefeated;

    private const float WORLD_TO_HUD_VERTICAL_OFFSET = 1f;


    private void OnEnable()
    {
        instance = this;
        playerDefeated = false;
        instance.UpdatePanelVisuals();
        instance.UpdatePanelPosition();
        wposOffset = new Vector3(0, WORLD_TO_HUD_VERTICAL_OFFSET, 0);

        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
        OnUiFocusChanged(IngameMenuManager.GetActiveMenu());
    }
    private void OnDisable()
    {
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
    }
    private void OnPlayerDefeated() 
    {
        playerDefeated = true;
        UpdateVisibility();
    }
    private void OnUiFocusChanged(IGameMenu m) 
    {
        UpdateVisibility();
    }
    private void UpdateVisibility() 
    {
        visibilityParent.gameObject.SetActive(IngameMenuManager.GetActiveMenu() == null && !playerDefeated);
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
            positionParent.gameObject.SetActive(false);
            currentlyEquippedParent.gameObject.SetActive(false);
        }
        else 
        {
            positionParent.gameObject.SetActive(true);

            if (targetInteractable.GetInfo().weapon == null)
            {
                interactableNameText.text = targetInteractable.GetInfo().name;
                wpnCard.gameObject.SetActive(false);
                interactableNamePanel.gameObject.SetActive(true);
                currentlyEquippedParent.gameObject.SetActive(false);
            }
            else 
            {
                wpnCard.gameObject.SetActive(true);
                wpnCard.SetUp(targetInteractable.GetInfo().weapon);
                interactableNamePanel.gameObject.SetActive(false);
                currentlyEquippedParent.gameObject.SetActive(true);
                currentlyEquippedWeapon.SetUp(targetInteractable.GetInfo().weapon.BaseWeapon.Slot == WeaponSO.WeaponSlot.Melee ? PlayerEntity.ActiveInstance.MeleeWeapon : PlayerEntity.ActiveInstance.RangedWeapon);
            }
            actionNameText.text = targetInteractable.GetInfo().desc;
        }
    }
    private void UpdatePanelPosition() 
    {
        if (targetInteractable == null) { return; }

        positionParent.position = mCamRef.WorldToScreenPoint(targetInteractable.GetInfo().gotransform.position + wposOffset);
    }
    public static void UpdateTargetInteractable(Interactable inter) 
    {
        if (instance == null) { return; }

        instance.targetInteractable = inter;
        instance.UpdatePanelVisuals();
        instance.UpdatePanelPosition();
    }
}
