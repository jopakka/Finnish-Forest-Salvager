using UnityEngine;

namespace Weapons
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private int capacity = 3;
        [SerializeField] private Weapon[] startingWeapons;
        
        public Inventory<Weapon> Inventory { get; private set; }

        private void Awake()
        {
            Inventory = new Inventory<Weapon>(capacity);

            foreach (Weapon weapon in startingWeapons)
            {
                Inventory.AddItem(weapon);
            }
        }
    }
}