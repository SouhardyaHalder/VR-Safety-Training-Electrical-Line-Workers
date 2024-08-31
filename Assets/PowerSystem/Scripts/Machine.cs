using System;
using UnityEngine;
using MrCryptographic.PowerSystem;

namespace MrCryptographic.PowerSystem
{

	public class Machine : MonoBehaviour
	{
		public string ID;
		public PowerSystem powerSystem;

		[DisplayWithoutEdit]
		public bool on;

		public Material onMat;
		public Material offMat;

		public Cable connectedCable;

		[HideInInspector] public MeshRenderer meshRenderer;

		public void OnStart()
		{
			meshRenderer = GetComponent<MeshRenderer>();
			ID = Guid.NewGuid().ToString();
		}

		public void ChangeOnState(bool state)
		{
			on = state;

			powerSystem.CheckSystemDetails();
		}

		private void Update()
		{
			if (on && powerSystem.hasPower)
				meshRenderer.material = onMat;
			else
				meshRenderer.material = offMat;
		}
	}

}