using UnityEngine;

public class HudDamageNotificationsManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    private MonoBehaviourPool<HudDamageNotificationsElement> notificationPool;
    private Camera mcam;

    public void SetUp(Camera c) 
    {
        mcam = c;
    }
    private void OnEnable()
    {
        notificationPool = new MonoBehaviourPool<HudDamageNotificationsElement>(notificationPrefab, transform);
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }
    private void OnEnemyDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        notificationPool.GetCopyFromPool().SetUp(mcam, magnitude, critlevel, elem, target);
    }
}
