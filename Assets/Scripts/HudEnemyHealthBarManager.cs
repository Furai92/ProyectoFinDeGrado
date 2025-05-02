using UnityEngine;
using System.Collections.Generic;

public class HudEnemyHealthBarManager : MonoBehaviour
{
    [SerializeField] private GameObject elementPrefab;
    [SerializeField] private Camera mCamRef;

    private Dictionary<int, HudEnemyHealthBarElement> activeElements;
    private MonoBehaviourPool<HudEnemyHealthBarElement> elementPool;

    private void OnEnable()
    {
        elementPool = new MonoBehaviourPool<HudEnemyHealthBarElement>(elementPrefab, transform);
        activeElements = new Dictionary<int, HudEnemyHealthBarElement>();

        EventManager.UiEnemyHealthBarDisabledEvent += OnUiEnemyHealthBarDisabled;
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
    }
    private void OnDisable()
    {
        EventManager.UiEnemyHealthBarDisabledEvent -= OnUiEnemyHealthBarDisabled;
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
    }
    private void OnUiEnemyHealthBarDisabled(HudEnemyHealthBarElement e) 
    {
        if (activeElements.ContainsKey(e.TrackedEnemy.EnemyInstanceID)) { activeElements.Remove(e.TrackedEnemy.EnemyInstanceID); }
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target) 
    {
        if (activeElements.ContainsKey(target.EnemyInstanceID)) { return; }

        HudEnemyHealthBarElement hpbar = elementPool.GetCopyFromPool();
        hpbar.SetUp(target, mCamRef);
        activeElements.Add(target.EnemyInstanceID, hpbar);
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity target)
    {
        if (activeElements.ContainsKey(target.EnemyInstanceID)) { return; }

        HudEnemyHealthBarElement hpbar = elementPool.GetCopyFromPool();
        hpbar.SetUp(target, mCamRef);
        activeElements.Add(target.EnemyInstanceID, hpbar);
    }
}

