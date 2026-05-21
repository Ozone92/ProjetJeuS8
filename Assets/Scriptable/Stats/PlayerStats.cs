using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject
{
    private Dictionary<string, float> Stats { get; set; } = new();

    public float Get(string key)
    {
        // Debug.Log($"Stats Get: {key}: {Stats.GetValueOrDefault(key, 0f)}");

        return Stats.GetValueOrDefault(key, 0f);
    }

    public void Add(string key, float value)
    {
        // Debug.Log($"Stats Add: {key}: {value}");
        if (!Stats.TryAdd(key, value))
        {
            if (Stats[key] + value > 100)
            {
                Stats[key] = 100;
            }
            else
            {
                Stats[key] += value;
            }
        }
    }

    public void Reset()
    {
        Stats.Clear();
    }

// Debug purpose
    public void Print()
    {
        foreach (var s in Stats)
        {
            Debug.Log($"{s.Key}: {s.Value}");
        }
    }
}
