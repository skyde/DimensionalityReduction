using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Connection
{
	public Connection(int index, double distance)
	{
		this.Index = index;
		this.Distance = distance;
	}
	
	public int Index;
	public double Distance;
}

public class VisualizePoint
{
	public Color Color;
	public Connection[] Connections;
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

	public int Connections = 3;

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
				var c = p.Visualize.AddComponent<EntryContainer>();
				c.Entry = p.Entry;

				points.Add(p);
			}
		}

		Points = points.ToArray();

		foreach (var point in Points) 
		{
			point.Position = Random.insideUnitSphere;
			 
			point.Connections = new Connection[Connections];

			for (int p = 0; p < point.Connections.Length; p++) 
			{
				point.Connections[p].Distance = double.MaxValue;
				point.Connections[p].Index = -1;
			}

			for (int i = 0; i < Points.Length; i++) 
			{
				var p = Points[i];

				if(p == point)
				{
					continue;
				}

				// Caculate distance
				var v = 0.0;
				for (int x = 0; x < point.Entry.Values.Length; x++) 
				{
					var d = (point.Entry.Values[x] - p.Entry.Values[x]);
					v += d * d;
				}

				v = System.Math.Sqrt(v);

				var highest = -1.0;
				var highestIndex = -1;
				for (int c = 0; c < point.Connections.Length; c++) 
				{
					if(point.Connections[c].Distance > highest)
					{
						highest = point.Connections[c].Distance;
						highestIndex = c;
					}
				}

				if(highest > v)
				{
					point.Connections[highestIndex] = new Connection(i, v);
				}

//				point.Connections[i] = new Connection(;
			}
		}
	}

	public bool Simulate = true;
	public float Stiffness = 1F;
	public float Damping = 0.96F;
	public float DistanceScalar = 1;

	public float Repulse = 1F;

	public void Update () 
	{
		if(!Simulate)
		{
			return;
		}

		if(Repulse != 0)
		{
			for (int a = 0; a < Points.Length; a++) 
			{
				for (int b = a; b < Points.Length; b++) 
				{
					if(a == b)
					{
						continue;
					}

					var aP = Points[a];
					var bP = Points[b];

					var diff = aP.Position - bP.Position;
					var d = diff.magnitude;
					var dir = diff / d;//diff.normalized;

					if(d == 0)
					{
						continue;
					}
					// 
					var impulse = dir * (1F / (Mathf.Sqrt(d))) * Repulse * 0.0001F;

					aP.Velocity += impulse;
					bP.Velocity -= impulse;
				}
			}
		}

		for (int a = 0; a < Points.Length; a++) 
		{
			for (int b = 0; b < Points[a].Connections.Length; b++)
			{
				var connection = Points[a].Connections[b];
				var targetPoint = Points[connection.Index];

				var desired = (float) connection.Distance * DistanceScalar;

				var diff = targetPoint.Position - Points[a].Position;
				var d = diff.magnitude;
				d -= desired;

				var norm = diff.normalized;//d == 0 ? Vector3.zero : diff / d;

				var impulse = norm * d * Stiffness * 0.0001F;

				Points[a].Velocity += impulse;
				Points[connection.Index].Velocity -= impulse;
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
	[Range(0, 1)]
	public float ConnectionAlpha = 0.5F;

	public void OnDrawGizmos()
	{
//		var a = new Color(0, 0, 1, PointAlpha);
//		var b = new Color(1, 0, 0, PointAlpha);


		foreach (var point in Points) 
		{
			if(DisplayConnections)
			{
				for (int i = 0; i < point.Connections.Length; i++)
				{
					var c = point.Connections[i];
					var p = Points[c.Index];
					var dist = (point.Position - p.Position).magnitude;
//					var t = 0.5F + ((float) c.Distance - dist) / PointDistanceScale;

					var col = Color.Lerp(point.Color, p.Color, ConnectionAlpha);

					col = new Color(col.r, col.g, col.b, ConnectionAlpha);

					Gizmos.color = col;



//					var tar = Vector3.Lerp(point.Position, p.Position, 0.5F);
					Gizmos.DrawLine(point.Position, p.Position);
				}
			}

//			Gizmos.color = point.Color;
//			Gizmos.DrawSphere(point.Position, PointSize);
		}
	}
}
