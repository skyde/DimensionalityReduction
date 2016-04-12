using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class ReadMNISTData : MonoBehaviour
{
	public TextAsset[] Datas;

	public int EntryLength = 28 * 28;
	
	public void Awake () 
	{
		LoadData(Datas[0]);
	}

	public List<double[]> LoadData(TextAsset asset)
	{
		var bytes = asset.bytes;

		Assert.IsTrue(bytes.Length % EntryLength == 0);

		var entries = new List<double[]>();
		for (int i = 0; i < bytes.Length; i += EntryLength)
		{
			var entry = new double[EntryLength];

			for (int e = 0; e < EntryLength; e++) 
			{
				entry[e] = (double) bytes[i + e] / 256.0;
			}

			entries.Add(entry);
		}

		return entries;
	}
}
