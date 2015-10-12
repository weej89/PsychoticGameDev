#region Using
using System;
using UnityEngine;
using System.Collections;
#endregion

public class Heap<T> where T : IHeapItem<T>
{
	#region Private Variables
	T[] items;
	int currentItemCount;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the Heap class class
	/// using an array of maxSize.
	/// </summary>
	/// <param name="maxHeapSize">Max heap size.</param>
	public Heap(int maxHeapSize)
	{
		items = new T[maxHeapSize];
	}
	#endregion

	#region Add
	/// <summary>
	/// Adds an item of specified Heap type and
	/// swaps the item down the heap if its priority
	/// is greater than its children.
	/// </summary>
	/// <param name="item">Item.</param>
	public void Add(T item)
	{
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}
	#endregion

	#region RemoveFirst
	/// <summary>
	/// Removes the first element of the heap and replaces
	/// it with the one at the end.  Then sorts the element
	/// down to its new child/parent position based on priority
	/// </summary>
	/// <returns>The first element</returns>
	public T RemoveFirst()
	{
		T firstItem = items[0];
		currentItemCount--;

		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}
	#endregion

	#region UpdateItem
	/// <summary>
	/// Sorts an item up the heap if its priority has changed
	/// In this case it priority will never decrease so item will
	/// only move up
	/// </summary>
	/// <param name="item">Item.</param>
	public void UpdateItem(T item)
	{
		SortUp(item);
	}
	#endregion

	#region Count
	/// <summary>
	/// Gets the current item count of heap
	/// </summary>
	/// <value>The count.</value>
	public int Count
	{
		get{return currentItemCount;}
	}
	#endregion

	#region Contains
	/// <summary>
	/// Returns true if the heap contains the item in parameter
	/// </summary>
	/// <param name="item">Item.</param>
	public bool Contains(T item)
	{
		return Equals(items[item.HeapIndex], item);
	}
	#endregion

	#region SortDown
	/// <summary>
	/// Sorts an item down the heap using swaps
	/// for child index.  If item priority is less
	/// than children then it will be swapped with the highest
	/// one.
	/// </summary>
	/// <param name="item">Item.</param>
	void SortDown(T item)
	{
		while(true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if(childIndexLeft < currentItemCount)
			{
				swapIndex = childIndexLeft;

				if(childIndexRight < currentItemCount)
				{
					if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}

				if(item.CompareTo(items[swapIndex]) < 0)
				{
					Swap(item, items[swapIndex]);
				}
				else
					return;
			}
			else
				return;
		}
	}
	#endregion

	#region SortUp
	/// <summary>
	/// Sorts an item of specified type up the heap
	/// based on priority by checking if its priority is
	/// higher than its parent.  If it is then it is swapped.
	/// </summary>
	/// <param name="item">Item.</param>
	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1)/2;

		while (true)
		{
			T parentItem = items[parentIndex];
			if(item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}
			else
				break;

			parentIndex = (item.HeapIndex-1)/2;
		}
	}
	#endregion

	#region Swap
	/// <summary>
	/// Swap two items in the heap
	/// </summary>
	/// <param name="itemA">Item a.</param>
	/// <param name="itemB">Item b.</param>
	void Swap(T itemA, T itemB)
	{
		int itemAIndex = itemA.HeapIndex;

		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
	#endregion
}

public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex
	{
		get;
		set;
	}
}