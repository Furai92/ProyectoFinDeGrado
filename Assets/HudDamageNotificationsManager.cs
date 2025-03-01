using UnityEngine;

public class HudDamageNotificationsManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    private MonoBehaviourPool<HudDamageNotificationsElement> notificationPool;

    private void OnEnable()
    {
        notificationPool = new MonoBehaviourPool<HudDamageNotificationsElement>(notificationPrefab, transform);
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }
    private void OnEnemyDamageTaken(Vector3 wpos, float magnitude, int critlevel, GameEnums.DamageElement elem) 
    {
        notificationPool.GetCopyFromPool().SetUp(wpos, magnitude, critlevel, elem);
    }
}
