using Ui;
using UnityEngine;

namespace TerrainSystem
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private HealthSystem healthSystem;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private float maxHealthBarDrawDistance = 10f;
        
        private Transform _target;

        private void Awake()
        {
            healthSystem.OnHealthChange += OnHealthChange;
            ResetHealthBar();
        }

        public void Initialize(Transform target, Camera camera)
        {
            _target = target;
            healthBar.Initialize(camera);
        }

        private void OnHealthChange(int health)
        {
            healthBar.SetValue(health);
        }

        private void ResetHealthBar()
        {
            healthBar.SetMaxValue(healthSystem.MaxHealth);
            healthBar.SetValue(healthSystem.Health);
        }

        private void LateUpdate()
        {
            float distance = Vector3.Distance(_target.transform.position, healthBar.WantedPosition);
            healthBar.gameObject.SetActive(distance <= maxHealthBarDrawDistance);
        }
    }
}