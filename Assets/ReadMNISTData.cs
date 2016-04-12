using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Assertions;

//[System.Serializable]
public class Entry
{
	public double[] Values;
}

//[System.Serializable]
public class Category
{
	public Entry[] Entry;
}

public class ReadMNISTData : MonoBehaviour
{
	public TextAsset[] Datas;
	public Category[] Category;

	public int EntryLength = 28 * 28;
	
	public void Awake () 
	{
		Category = new Category[Datas.Length];

		var category = new List<Category>();

		for (int i = 0; i < Datas.Length; i++)
		{
			category.Add(LoadData(Datas[0]));
		}

		Category = category.ToArray();
	}

	public Category LoadData(TextAsset asset)
	{
		var bytes = asset.bytes;

		Assert.IsTrue(bytes.Length % EntryLength == 0);

		var entries = new List<Entry>();
		for (int i = 0; i < bytes.Length; i += EntryLength)
		{
			var entry = new Entry();
			entry.Values = new double[EntryLength];

			for (int e = 0; e < EntryLength; e++) 
			{
				entry.Values[e] = (double) bytes[i + e] / 256.0;
			}

			entries.Add(entry);
		}

		var category = new Category();
		category.Entry = entries.ToArray();

		return category;
	}
}
