using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogs/CameraTransformMapper")]
public class CameraTransformMapper : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        public string key;
        public Vector3 position;
        public Vector3 rotation;
    }
    
    public List<Item> items;

    public bool TryGet(string key, out Tuple<Vector3, Vector3> transform)
    {
        foreach (var item in items)
        {
            if (item.key == key)
            {
                transform = new Tuple<Vector3, Vector3>(item.position, item.rotation);
                return true;
            }
        }

        transform = null;
        return false;
    }
}
