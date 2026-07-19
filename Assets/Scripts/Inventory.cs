using System.Collections.Generic;

public class Inventory<T> where T : class
{
    private readonly List<T> _items = new();
    
    public IReadOnlyList<T> Items => _items;
    public int Count => _items.Count;
    public int Capacity { get; }
    public bool IsFull => Count >= Capacity;
    
    public event System.Action<T> ItemAdded;
    public event System.Action<T> ItemRemoved;
    
    public Inventory(int capacity)
    {
        Capacity = capacity;
    }

    public bool AddItem(T item)
    {
        if (IsFull || _items.Contains(item))
        {
            return false;
        }
        _items.Add(item);
        ItemAdded?.Invoke(item);
        return true;
    }

    public bool RemoveItem(T item)
    {
        if (!_items.Remove(item))
        {
            return false;
        }
        ItemRemoved?.Invoke(item);
        return true;
    }

    public bool Contains(T item) => _items.Contains(item);
    
    public T GetItem(int index) => _items[index];
    
    public void Clear() => _items.Clear();
}