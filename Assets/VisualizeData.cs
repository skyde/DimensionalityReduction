using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VisualizePoint
{
	public Color Color;
	public double[] Distances;
	public Entry Entry;
}

public class VisualizeData : MonoBehaviour 
{
	public int DisplayPerCategory = 100;
	public ReadMNISTData Data;
	public VisualizePoint[] Points;

	public void Start()
	{
		var points = new List<VisualizePoint>();

		foreach (var category in Data.Category)
		{
			for (int i = 0; i < category.Entry.Length && i < DisplayPerCategory; i++)
			{
				var p = new VisualizePoint();

				p.Color = category.Color;
				p.Entry = category.Entry[i];

				points.Add(p);
			}
		}

		Points = points.ToArray();
	}

	public void Update () 
	{
	
	}
}
