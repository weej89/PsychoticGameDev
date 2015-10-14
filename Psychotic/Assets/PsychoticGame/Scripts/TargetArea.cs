using UnityEngine;
using System.Collections;

public class TargetArea {

	Vector3 center;
	float radius;
	double area;

	public TargetArea(Vector3 _center, float _radius)
	{
		center = _center;
		radius = _radius;

		this.area = Mathf.PI * Mathf.Pow(radius, 2);
	}

	public Vector3 GetPointInTargetArea()
	{
		float x, z;
		float xRange = Mathf.Abs(center.x - radius);
		float zRange = Mathf.Abs(center.z - radius);
		Vector3 point;

		x = Random.Range(center.x - xRange, center.x + xRange);
		z = Random.Range(center.z - zRange, center.z + zRange);

		point = new Vector3(x, 0, z);
		return point;
	}
}
