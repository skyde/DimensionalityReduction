using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualizePoint
{
	public Color Color;
	public double[] Distances;
	public Entry Entry;
	public Vector3 Position;
	public Vector3 Velocity;
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

				// Caculate distance
				var v = 0.0;
				for (int x = 0; x < point.Entry.Values.Length; x++) 
				{
					var d = (point.Entry.Values[x] - p.Entry.Values[x]);
					v += d * d;
				}

				v = System.Math.Sqrt(v);
				point.Distances[i] = v;

//				if(i == 0)
//				print(v);
			}
		}
	}

	public float Stiffness = 1F;
	public float Damping = 0.96F;
	public float DistanceScalar = 1;

	public void Update () 
	{
		for (int a = 0; a < Points.Length; a++) 
		{
			for (int b = 0; b < Points.Length; b++)
			{
				if(a == b)
				{
					continue;
				}

				var desired = (float) Points[a].Distances[b] * DistanceScalar;

				var diff = Points[b].Position - Points[a].Position;
				var d = diff.magnitude;
				d -= desired;

				var norm = diff.normalized;//d == 0 ? Vector3.zero : diff / d;

				var impulse = norm * d * Stiffness * 0.0001F;

				Points[a].Velocity += impulse;
			}
		}
//		foreach (var point in Points) 
//		{
//			for (int i = 0; i < point.Distances; i++) 
//			{
//				
//			}
//		}

		foreach (var item in Points) 
		{
			item.Velocity *= Damping;
			item.Position += item.Velocity;
		}
	}

	public float PointSize = 0.1F;

	public void OnDrawGizmos()
	{
		foreach (var item in Points) 
		{
			Gizmos.color = item.Color;
			Gizmos.DrawSphere(item.Position, PointSize);
		}
	}
}
