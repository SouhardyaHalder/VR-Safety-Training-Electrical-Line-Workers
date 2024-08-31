using UnityEngine;

namespace MrCryptographic.PowerSystem
{

	public class Consumer : Machine
	{
		public float PowerInput = 50f;
		[HideInInspector] public bool connectedToPowerPole;

		private void Start()
		{
			OnStart();
			powerSystem = PowerSystemNetworkController.Instance.CreatePowerSystemAsConsumer(this);
		}

		//Removes itselfe from the power system
		private void OnDestroy()
		{
			if (powerSystem != null && connectedCable != null)
			{
				powerSystem.consumers.Remove(ID);
				Destroy(connectedCable.gameObject);
			}
		}
	}

}