using UnityEngine;

public class PlayerAttackImpactEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ColorDatabaseSO cdb;

    private float removeTime;

    private const float LIFETIME = 1f;

    public void SetUp(Vector3 pos, float dir, GameEnums.DamageElement elem) 
    {
        gameObject.SetActive(true);
        transform.SetPositionAndRotation(pos, Quaternion.Euler(0, dir, 0));
        removeTime = Time.time + LIFETIME;
        ParticleSystem.MainModule m = ps.main; m.startColor = cdb.ElementToColor(elem);
        ps.Stop();
        ps.Play();
    }
    
    void Update()
    {
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
