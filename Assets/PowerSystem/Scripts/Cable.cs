using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MrCryptographic.PowerSystem
{

	public class Cable : MonoBehaviour
	{
		[Header("This component is only to show with object are connected and when its destroy to tell the systems witch objects must be disconnected")]
		[Header("")]
		[Header("Only to show with object are connected for debugging <- do not set anything here")]
		public GameObject object1;
		public GameObject object2;

		[Header("References")]
		public MeshFilter _meshFilter;
		public MeshRenderer _meshRenderer;
		public MeshCollider _collider;
		public LineRenderer lineRenderer;

		[Header("Options")]
		[SerializeField] int _sides;
		[SerializeField] float radius;
		[SerializeField] bool _useWorldSpace = true;
		[SerializeField] float _falloffByDistance = 0.1f;

		public Vector3[] _positions;
		private Vector3[] _vertices;
		private Vector3 betweenVector;
		private Mesh _mesh;

		public float smoothingLeangth;
		public float distance;

		public Material material
		{
			get { return _meshRenderer.material; }
			set { _meshRenderer.material = value; }
		}

		void Awake()
		{
			_mesh = new Mesh();
			_mesh.name = "Cable";
			_meshFilter.mesh = _mesh;
		}

		private void OnEnable()
		{
			_meshRenderer.enabled = true;
		}

		private void OnDisable()
		{
			_meshRenderer.enabled = false;
		}

		private void OnValidate()
		{
			_sides = Mathf.Max(3, _sides);
		}

		public void SetConnection(GameObject obj1, GameObject obj2)
		{
			//Sets references
			object1 = obj1;
			object2 = obj2;

			//Sets the connectedCable value in the machines to this cable
			if (obj1.GetComponentInParent<Machine>())
				obj1.GetComponentInParent<Machine>().connectedCable = this;
			if (obj2.GetComponentInParent<Machine>())
				obj2.GetComponentInParent<Machine>().connectedCable = this;

			if (obj1.GetComponentInParent<PowerPole>())
				obj1.GetComponentInParent<PowerPole>().connectedCables.Add(this);
			if (obj2.GetComponentInParent<PowerPole>())
				obj2.GetComponentInParent<PowerPole>().connectedCables.Add(this);

			//Resets the position
			transform.position = new Vector3(0, 0, 0);

			//Calculate vector between the two points
			betweenVector = Vector3.Lerp(obj1.transform.position, obj2.transform.position, .5f);
			//Set falloff for hanging cable illusion
			//_falloffByDistance
			_falloffByDistance = .1f * Vector3.Distance(obj1.transform.position, obj2.transform.position);
			betweenVector.y -= _falloffByDistance;
			//Set the position for the calculation
			SetPositions(new Vector3[] {
			new Vector3(obj1.transform.position.x, obj1.transform.position.y, obj1.transform.position.z),
			betweenVector,
			new Vector3(obj2.transform.position.x,obj2.transform.position.y,obj2.transform.position.z)
		});
		}

		public void SetPositions(Vector3[] positions)
		{
			//Set positions
			_positions = positions;
			//Make Smooth
			distance = Vector3.Distance(object1.transform.position, object2.transform.position);
			int subDivision = 2 * (int)distance + 1;
			LineRendererSmoother.SmoothLine(lineRenderer, _positions, subDivision, distance / 10);
			//Get back the smooth line
			Vector3[] newPos = new Vector3[lineRenderer.positionCount];
			lineRenderer.GetPositions(newPos);
			_positions = newPos;
			//Generate the mesh from the smoothed points
			GenerateMesh();
		}

		//Generates the Mesh
		private void GenerateMesh()
		{
			if (_mesh == null || _positions == null || _positions.Length <= 1)
			{
				_mesh = new Mesh();
				return;
			}

			var verticesLength = _sides * _positions.Length;
			if (_vertices == null || _vertices.Length != verticesLength)
			{
				_vertices = new Vector3[verticesLength];

				var indices = GenerateIndices();
				var uvs = GenerateUVs();

				if (verticesLength > _mesh.vertexCount)
				{
					_mesh.vertices = _vertices;
					_mesh.triangles = indices;
					_mesh.uv = uvs;
				}
				else
				{
					_mesh.triangles = indices;
					_mesh.vertices = _vertices;
					_mesh.uv = uvs;
				}
			}

			var currentVertIndex = 0;

			for (int i = 0; i < _positions.Length; i++)
			{
				var circle = CalculateCircle(i);
				foreach (var vertex in circle)
				{
					_vertices[currentVertIndex++] = _useWorldSpace ? transform.InverseTransformPoint(vertex) : vertex;
				}
			}

			_mesh.vertices = _vertices;
			_mesh.RecalculateNormals();
			_mesh.RecalculateBounds();

			_meshFilter.mesh = _mesh;
			_collider.sharedMesh = _mesh;
		}

		//Generates the UVs
		private Vector2[] GenerateUVs()
		{
			var uvs = new Vector2[_positions.Length * _sides];

			for (int segment = 0; segment < _positions.Length; segment++)
			{
				for (int side = 0; side < _sides; side++)
				{
					var vertIndex = (segment * _sides + side);
					var u = side / (_sides - 1f);
					var v = segment / (_positions.Length - 1f);

					uvs[vertIndex] = new Vector2(u, v);
				}
			}

			return uvs;
		}

		//Generates the indices
		private int[] GenerateIndices()
		{
			// Two triangles and 3 vertices
			var indices = new int[_positions.Length * _sides * 2 * 3];

			var currentIndicesIndex = 0;
			for (int segment = 1; segment < _positions.Length; segment++)
			{
				for (int side = 0; side < _sides; side++)
				{
					var vertIndex = (segment * _sides + side);
					var prevVertIndex = vertIndex - _sides;

					// Triangle one
					indices[currentIndicesIndex++] = prevVertIndex;
					indices[currentIndicesIndex++] = (side == _sides - 1) ? (vertIndex - (_sides - 1)) : (vertIndex + 1);
					indices[currentIndicesIndex++] = vertIndex;


					// Triangle two
					indices[currentIndicesIndex++] = (side == _sides - 1) ? (prevVertIndex - (_sides - 1)) : (prevVertIndex + 1);
					indices[currentIndicesIndex++] = (side == _sides - 1) ? (vertIndex - (_sides - 1)) : (vertIndex + 1);
					indices[currentIndicesIndex++] = prevVertIndex;
				}
			}

			return indices;
		}

		//Calculates the circle
		private Vector3[] CalculateCircle(int index)
		{
			var dirCount = 0;
			var forward = Vector3.zero;

			// If not first index
			if (index > 0)
			{
				forward += (_positions[index] - _positions[index - 1]).normalized;
				dirCount++;
			}

			// If not last index
			if (index < _positions.Length - 1)
			{
				forward += (_positions[index + 1] - _positions[index]).normalized;
				dirCount++;
			}

			// Forward is the average of the connecting edges directions
			forward = (forward / dirCount).normalized;
			var side = Vector3.Cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)).normalized;
			var up = Vector3.Cross(forward, side).normalized;

			var circle = new Vector3[_sides];
			var angle = 0f;
			var angleStep = (2 * Mathf.PI) / _sides;

			var t = index / (_positions.Length - 1f);

			for (int i = 0; i < _sides; i++)
			{
				var x = Mathf.Cos(angle);
				var y = Mathf.Sin(angle);

				circle[i] = _positions[index] + side * x * radius + up * y * radius;

				angle += angleStep;
			}

			return circle;
		}

		//Disconnects the objects from eachother
		private void OnDestroy()
		{
			if (gameObject.scene.isLoaded) //Was Deleted
			{
				if (object1 == null || object2 == null) { return; } //Check if null

				//Disconnects a Power pole from a power pole
				if (object1.GetComponentInParent<PowerPole>() && object2.GetComponentInParent<PowerPole>()) //Check if both selected object are power poles
					PowerSystemNetworkController.Instance.DisconnectPowerPoleAndPowerPole(object1.GetComponentInParent<PowerPole>(), object2.GetComponentInParent<PowerPole>());

				//Disconnects a generator and a power pole
				if (object1.GetComponentInParent<PowerPole>() && object2.GetComponentInParent<Generator>())
					PowerSystemNetworkController.Instance.DisconnectPowerPoleAndGenerator(object1.GetComponentInParent<PowerPole>(), object2.GetComponentInParent<Generator>());
				else if (object1.GetComponentInParent<Generator>() && object2.GetComponentInParent<PowerPole>())
					PowerSystemNetworkController.Instance.DisconnectPowerPoleAndGenerator(object2.GetComponentInParent<PowerPole>(), object1.GetComponentInParent<Generator>());

				//Disconnects a consumer and a power pole
				if (object1.GetComponentInParent<PowerPole>() && object2.GetComponentInParent<Consumer>())
					PowerSystemNetworkController.Instance.DisconnectPowerPoleAndConsumer(object1.GetComponentInParent<PowerPole>(), object2.GetComponentInParent<Consumer>());
				else if (object1.GetComponentInParent<Consumer>() && object2.GetComponentInParent<PowerPole>())
					PowerSystemNetworkController.Instance.DisconnectPowerPoleAndConsumer(object2.GetComponentInParent<PowerPole>(), object1.GetComponentInParent<Consumer>());
			}
			else { }//Was Cleaned Up on Scene Closure
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(object1.transform.parent.position, object1.transform.parent.localScale.x);
			Gizmos.DrawWireSphere(object2.transform.parent.position, object2.transform.parent.localScale.x);

			Gizmos.color = Color.green;

			Gizmos.DrawWireSphere(betweenVector, .25f);
		}
	}

}