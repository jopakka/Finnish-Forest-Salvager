using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponData weaponData;

        private bool _isAttacking = false;
        private float _timeSinceLastAttack = float.MaxValue;

        public abstract void Attack();

        private bool CanAttack() => !_isAttacking && _timeSinceLastAttack >= weaponData.cooldown;

        protected bool StartAttackOrCalculate()
        {
            if (CanAttack())
            {
                _isAttacking = true;
                return true;
            }

            _timeSinceLastAttack += Time.deltaTime;

            return false;
        }

        protected void ResetAttack()
        {
            _timeSinceLastAttack = float.MaxValue;
            _isAttacking = false;
        }
    }
}