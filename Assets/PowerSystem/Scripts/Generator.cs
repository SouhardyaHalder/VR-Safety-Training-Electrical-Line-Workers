using UnityEngine;

namespace MrCryptographic.PowerSystem
{

	public class Generator : Machine
	{
		public float PowerOutput = 100f;
		[HideInInspector] public bool connectedToPowerPole;

		private void Start()
		{
			OnStart();
			meshRenderer = GetComponent<MeshRenderer>();
			powerSystem = PowerSystemNetworkController.Instance.CreatePowerSystemAsGenerator(this);
		}

		//Removes itselfe from the power system
		private void OnDestroy()
		{
			if (powerSystem != null && connectedCable != null)
			{
				powerSystem.generators.Remove(ID);
				Destroy(connectedCable.gameObject);
			}
		}
	}

}