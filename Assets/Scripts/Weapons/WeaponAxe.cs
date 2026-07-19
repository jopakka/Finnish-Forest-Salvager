using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class WeaponAxe : Weapon
    {
        public WeaponData WeaponData => weaponData;

        public override void Attack()
        {
            if (!StartAttackOrCalculate()) return;

            StartCoroutine(DoAttack());
        }

        private IEnumerator DoAttack()
        {
            Debug.Log("Attacked with Axe");
            yield return new WaitForSeconds(weaponData.cooldown);
            ResetAttack();
        }
    }
}