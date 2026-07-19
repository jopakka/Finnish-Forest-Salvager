using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    public delegate void OnHealthChangeDelegate(int health);

    public OnHealthChangeDelegate OnHealthChange;

    public int Health => health;
    public int MaxHealth => maxHealth;

    public void Initialize(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        UpdateHealthValue(-damage);
    }

    public void Heal(int heal)
    {
        UpdateHealthValue(heal);
    }

    private void UpdateHealthValue(int value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);
        OnHealthChange?.Invoke(health);
    }
}