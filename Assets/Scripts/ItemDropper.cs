using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemDropWeight{

    public int weight;
    public GameObject prefab;
}
public class ItemDropper : MonoBehaviour
{
    public List<ItemDropWeight> health_drop_weights = new List<ItemDropWeight>();
    public List<ItemDropWeight> item_drop_weights = new List<ItemDropWeight>();

    public void DropHealth() {
        if(health_drop_weights.Count > 0) {
            var result = GetDrop(health_drop_weights);
            var item_instance = Instantiate(result);
            item_instance.transform.position = transform.position;
        }
    }

    public void DropItem() {
        if(item_drop_weights.Count > 0) {
            var result = GetDrop(item_drop_weights);
            var item_instance = Instantiate(result);
            item_instance.transform.position = transform.position;
        }
    }

    private GameObject GetDrop(List<ItemDropWeight> item_weights) {
        Dictionary<GameObject, int> weights = new Dictionary<GameObject, int>();
        item_weights.ForEach(x => weights.Add(x.prefab, x.weight));
        return WeightedRandomizer.From(weights).TakeOne();
    }
}
