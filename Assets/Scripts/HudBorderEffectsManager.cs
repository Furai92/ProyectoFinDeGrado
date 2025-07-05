using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudBorderEffectsManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup damageCG;
    [SerializeField] private CanvasGroup corruptionCG;

    private bool damageCrActive;
    private bool corruptionCrActive;
    private bool playerHasCorruptionFlag;

    private const float DAMAGE_CR_DURATION = 1.0f;

    private void OnEnable()
    {
        EventManager.PlayerDamageTakenEvent += OnPlayerDamaged;
        EventManager.PlayerStatsUpdatedEvent += OnPlayerStatsChanged;

        damageCrActive = false;
        corruptionCrActive = false;
        damageCG.gameObject.SetActive(false);
        corruptionCG.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        EventManager.PlayerDamageTakenEvent -= OnPlayerDamaged;
        EventManager.PlayerStatsUpdatedEvent -= OnPlayerStatsChanged;
    }
    private void OnPlayerDamaged(float mag) 
    {
        if (damageCrActive) 
        {
            StopCoroutine(DamageFlashCR());
        }
        StartCoroutine(DamageFlashCR());
    }
    private void OnPlayerStatsChanged(PlayerEntity p) 
    {
        playerHasCorruptionFlag = p.GetFlag(PlayerStatGroup.PlayerFlags.NoStopAttackRanged);
        if (corruptionCrActive)
        {
            StopCoroutine(CorruptionFlashCR());
        }
        StartCoroutine(CorruptionFlashCR());
    }
    IEnumerator DamageFlashCR()
    {
        damageCrActive = true;
        damageCG.gameObject.SetActive(true);
        float t = 1;
        while (t > 0) 
        {
            t -= Time.deltaTime / DAMAGE_CR_DURATION;
            damageCG.alpha = t;
            yield return null;
        }
        damageCG.gameObject.SetActive(false);
        damageCrActive = false;
    }
    IEnumerator CorruptionFlashCR() 
    {
        corruptionCrActive = true;
        corruptionCG.gameObject.SetActive(true);
        float t = 0;
        while (playerHasCorruptionFlag)
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime);
            damageCG.alpha = t;
            yield return null;
        }
        corruptionCG.gameObject.SetActive(false);
        corruptionCrActive = false;
    }
}
