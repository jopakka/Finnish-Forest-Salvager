using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public WeaponType type;
        public float range;
        public int damage;
        public float cooldown;
    }
}