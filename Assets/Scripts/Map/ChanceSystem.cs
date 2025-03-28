using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChanceSystem<T> 
{
    private LinkedList<int> probabilities;
    private LinkedList<T> labels;
    private int sum;

    public ChanceSystem() {
        probabilities = new LinkedList<int>();
        labels = new LinkedList<T>();
    }

    public void AddItem(T label, int probability) {
        labels.AddLast(label);
        probabilities.AddLast(probability);
        sum += probability;
    }

    public T Generate() {
        int item = Random.Range(0, sum);
        var iterator = labels.GetEnumerator();
        iterator.MoveNext();
        while (item > 0) {
            foreach(int prob in probabilities) {
                if (item - prob <= 0) {
                    goto finished;
                }
                item -= prob;
                iterator.MoveNext();
            }
        }
    finished:
        return iterator.Current;
    }
}
