using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Assertions;

//[System.Serializable]
using UnityEditor;


public class Entry
{
	public double[] Values;
}

//[System.Serializable]
public class Category
{
	public Color Color = Color.white;
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
			var c = LoadData(Datas[i]);

			c.Color = new HSBColor(i / (float) Datas.Length, 1, 1).ToColor();

			category.Add(c);
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

	public bool PreviewEnabled = true;
	public int PreviewCategory = 0;
	public int PreviewEntry = 0;

	public void OnDrawGizmos()
	{
		Entry entry = null;
		if(Selection.activeGameObject)//!PreviewEnabled || Category == null || Category.Length == 0)
		{
			var c = Selection.activeGameObject.GetComponent<EntryContainer>();

			if(c)
			{
				entry = c.Entry;
			}
		}

		if(entry == null)
		{
			return;
		}

		var values = entry.Values;

		var iter = 28;
		float step = 0.1F;
		for (int x = 0; x < iter; x++) 
		{
			for (int y = 0; y < iter; y++) 
			{
				var v = 1F - (float) values[y * iter + x];
				Gizmos.color = new Color(v, v, v, 1);

				Gizmos.DrawCube(new Vector2(x * step, -y * step), new Vector2(step, step));
			}
		}
	}
}
















