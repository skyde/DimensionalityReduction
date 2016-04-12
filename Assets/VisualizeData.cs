using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class VisualizePoint
{
	public Color Color;
	public double[] Distances;
	public Entry Entry;
	public Vector2 Position;
	public Vector2 Velocity;
}

public class VisualizeData : MonoBehaviour 
{
	public int DisplayPerCategory = 100;
	public ReadMNISTData Data;
	public VisualizePoint[] Points = new VisualizePoint[0];

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

		foreach (var point in Points) 
		{
			point.Distances = new double[Points.Length];

			for (int i = 0; i < Points.Length; i++) 
			{
				var p = Points[i];

				if(p == point)
				{
					p.Distances[i] = 0;
				}

				p.Position = Random.insideUnitSphere;
			}
		}
	}

	public void Update () 
	{
	
	}

	public void OnDrawGizmos()
	{
		foreach (var item in Points) 
		{
			Gizmos.color = item.Color;
			Gizmos.DrawSphere(item.Position, 0.02F);
		}
	}
}
