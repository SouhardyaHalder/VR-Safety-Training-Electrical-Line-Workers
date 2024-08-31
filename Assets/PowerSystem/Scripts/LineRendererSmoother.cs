using UnityEngine;
using MrCryptographic.PowerSystem;

namespace MrCryptographic.PowerSystem
{

	public static class LineRendererSmoother
	{
		static public void SmoothLine(this LineRenderer lineRenderer, Vector3[] points, int subDivisions, float smoothingLength)

		{

			if (points == null || points.Length < 3)

				return;



			subDivisions = Mathf.Max(subDivisions, 2);



			BezierCurve[] curves = new BezierCurve[points.Length - 1];

			for (int i = 0; i < curves.Length; ++i)

				curves[i] = new BezierCurve();



			Vector3 tangent = (points[1] - points[0]).normalized * -smoothingLength;

			for (int i = 0; i < curves.Length; ++i)

			{

				curves[i].Points[0] = points[i];

				curves[i].Points[1] = points[i] - tangent;

				curves[i].Points[3] = points[i + 1];



				if (i == (curves.Length - 1))

				{

					tangent = (points[i + 1] - points[i]).normalized * -smoothingLength;

				}

				else

				{

					tangent = ((points[i + 2] - points[i + 1]).normalized + (points[i + 1] - points[i]).normalized) * -smoothingLength;

				}



				curves[i].Points[2] = points[i + 1] + tangent;

			}



			Vector3[] smoothedPoints = new Vector3[(curves.Length * subDivisions) + 1];

			for (int i = 0; i < curves.Length; ++i)

			{

				Vector3[] segments = curves[i].GetSegments(subDivisions);

				for (int j = 0; j < segments.Length; ++j)

					smoothedPoints[(i * subDivisions) + j] = segments[j];

			}

			smoothedPoints[smoothedPoints.Length - 1] = curves[curves.Length - 1].EndPosition;



			lineRenderer.positionCount = smoothedPoints.Length;

			lineRenderer.SetPositions(smoothedPoints);

		}
	}

}