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
	public GameObject Visualize;
}

public class VisualizeData : MonoBehaviour 
{
	public int DisplayPerCategory = 100;
	public ReadMNISTData Data;
	public VisualizePoint[] Points = new VisualizePoint[0];
	public GameObject VisualizePoint;

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

				p.Visualize = Instantiate(VisualizePoint) as GameObject;
				p.Visualize.GetComponent<MeshRenderer>().material.color = category.Color;
				p.Visualize.transform.parent = transform;

				points.Add(p);
			}
		}

		Points = points.ToArray();

		foreach (var point in Points) 
		{
			point.Distances = new double[Points.Length];

			for (int i = 0; i < point.Distances.Length; i++) 
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

//				v = Mathf.;//System.Math.Log(v);
//				v = System.Math.Sqrt(v);
//				v = System.Math.Sqrt(v);
//				v = System.Math.Sqrt(v);
				point.Distances[i] = v;
			}
		}
	}

	public bool Simulate = true;
	public float Stiffness = 1F;
	public float Damping = 0.96F;
	public float DistanceScalar = 1;

	public void Update () 
	{
		if(!Simulate)
		{
			return;
		}

		for (int a = 0; a < Points.Length; a++) 
		{
			for (int b = 0; b < Points[a].Distances.Length; b++)
			{
				var desired = (float) Points[a].Distances[b] * DistanceScalar;

				var diff = Points[b].Position - Points[a].Position;
				var d = diff.magnitude;
				d -= desired;

				var norm = diff.normalized;//d == 0 ? Vector3.zero : diff / d;

				var impulse = norm * d * Stiffness * 0.0001F;

				Points[a].Velocity += impulse;
			}

			Points[a].Visualize.transform.position = Points[a].Position;
			Points[a].Visualize.transform.localScale = new Vector3(PointScale, PointScale, PointScale);

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

	public float PointScale = 1F;

	public bool DisplayConnections = false;
	public float PointAlpha = 0.02F;
	public float PointDistanceScale = 5;

	public void OnDrawGizmos()
	{
		var a = new Color(0, 0, 1, PointAlpha);
		var b = new Color(1, 0, 0, PointAlpha);

		foreach (var point in Points) 
		{
			if(DisplayConnections)
			{
				for (int i = 0; i < point.Distances.Length; i++)
				{
					var p = Points[i];
					var dist = (point.Position - p.Position).magnitude;
					var t = 0.5F + ((float) point.Distances[i] - dist) / PointDistanceScale;

					Gizmos.color = Color.Lerp(a, b, t);

					var tar = Vector3.Lerp(point.Position, p.Position, 0.5F);
					Gizmos.DrawLine(point.Position, tar);
				}
			}

//			Gizmos.color = point.Color;
//			Gizmos.DrawSphere(point.Position, PointSize);
		}
	}
}
