using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    protected WeaponAttackSetupData setupData;
    public abstract void SetUp(Vector3 pos, float direction, WeaponAttackSetupData sd, PlayerAttackBase parentAttack);

    protected void Explode(Vector3 center)
    {
        WeaponAttackSetupData explosionsd = new WeaponAttackSetupData()
        {
            element = setupData.element,
            magnitude = setupData.magnitude,
            critchance = setupData.critchance,
            critdamage = setupData.critdamage,
            sizemult = setupData.splash,
            builduprate = setupData.builduprate,
        };
        StageManagerBase.GetObjectPool().GetPlayerAttackFromPool("EXPLOSION").SetUp(center, 0, explosionsd, this);
    }
}
