using System;
using System.Collections.Generic;

public class MaxHeap<T>
{
    private List<T> heap;
    private Func<T, float> valueSelector;

    public int Count => heap.Count;

    public MaxHeap(Func<T, float> valueSelector)
    {
        if (valueSelector == null)
            throw new ArgumentNullException(nameof(valueSelector));

        this.valueSelector = valueSelector;
        heap = new List<T>();
    }
    public MaxHeap(T[] initialValues, Func<T, float> valueSelector)
        : this(valueSelector)
    {
        heap.AddRange(initialValues);
        Heapify();
    }

    public void Push(T item)
    {
        heap.Add(item);
        SiftUp(heap.Count - 1);
    }

    public T Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        return heap[0];
    }

    public T Pull()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        T max = heap[0];
        int lastIndex = heap.Count - 1;

        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 0)
            SiftDown(0);

        return max;
    }

    public void Clear()
    {
        heap.Clear();
    }

    private void Heapify()
    {
        for (int i = ParentIndex(heap.Count - 1); i >= 0; i--)
        {
            SiftDown(i);
        }
    }

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            int parent = ParentIndex(index);

            if (GetValue(index) <= GetValue(parent))
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        while (true)
        {
            int left = LeftChildIndex(index);
            int right = RightChildIndex(index);
            int largest = index;

            if (left < heap.Count && GetValue(left) > GetValue(largest))
                largest = left;

            if (right < heap.Count && GetValue(right) > GetValue(largest))
                largest = right;

            if (largest == index)
                break;

            Swap(index, largest);
            index = largest;
        }
    }

    private float GetValue(int index)
    {
        return valueSelector(heap[index]);
    }

    private void Swap(int a, int b)
    {
        T temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }

    private int ParentIndex(int index) => (index - 1) / 2;
    private int LeftChildIndex(int index) => index * 2 + 1;
    private int RightChildIndex(int index) => index * 2 + 2;
}