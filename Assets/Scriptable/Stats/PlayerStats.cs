using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject
{
    private Dictionary<string, float> Stats { get; set; }

    public float get(string key)
    {
        return Stats.GetValueOrDefault(key, 0f);
    }

    public void add(string key, float value)
    {
        if (!Stats.TryAdd(key, value))
        {
            Stats[key] += value;
        }
    }

    // Debug purpose
    public void print()
    {
        foreach (var s in Stats)
        {
            Debug.Log($"{s.Key}: {s.Value}");
        }
    }
}
