using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrCryptographic.PowerSystem;

namespace MrCryptographic.PowerSystem
{

	public class PowerPole : MonoBehaviour
	{
		public string ID;
		public PowerSystem powerSystem;

		[SerializeField]
		public Dictionary<string, PowerPole> connectedPowerPoles = new Dictionary<string, PowerPole>();
		public Dictionary<string, Consumer> connectedConsumers = new Dictionary<string, Consumer>();
		public Dictionary<string, Generator> connectedGenerators = new Dictionary<string, Generator>();

		public List<Cable> connectedCables = new List<Cable>();

		public void Start()
		{
			ID = Guid.NewGuid().ToString();
			powerSystem = PowerSystemNetworkController.Instance.CreatePowerSystemAsPowerPole(this);
		}

		public void ReloadIntoRightPowerSystem(PowerSystem newPowerSystem)
		{
			if (newPowerSystem == null)
				powerSystem = newPowerSystem = PowerSystemNetworkController.Instance.CreatePowerSystemAsPowerPole(this);

			if (!newPowerSystem.powerPoles.ContainsKey(ID))
			{
				newPowerSystem.powerPoles.Add(ID, this);
				powerSystem = newPowerSystem;
			}

			foreach (var powerPole in connectedPowerPoles)
				if (!newPowerSystem.powerPoles.ContainsKey(powerPole.Value.ID))
					powerPole.Value.ReloadIntoRightPowerSystem(newPowerSystem);

			foreach (var consumer in connectedConsumers)
				if (!newPowerSystem.consumers.ContainsKey(consumer.Value.ID))
				{
					newPowerSystem.consumers.Add(consumer.Value.ID, consumer.Value);
					consumer.Value.powerSystem = newPowerSystem;

					foreach (Transform child in PowerSystemNetworkController.Instance.transform)
					{
						if (child.GetComponent<PowerSystem>())
							if (child.GetComponent<PowerSystem>() != newPowerSystem && child.GetComponent<PowerSystem>().consumers.ContainsKey(consumer.Value.ID))
							{
								child.GetComponent<PowerSystem>().consumers.Remove(consumer.Value.ID);
								child.GetComponent<PowerSystem>().CheckSystemDetails();
							}
					}
				}

			foreach (var generator in connectedGenerators)
				if (!newPowerSystem.generators.ContainsKey(generator.Value.ID))
				{
					newPowerSystem.generators.Add(generator.Value.ID, generator.Value);
					generator.Value.powerSystem = newPowerSystem;

					foreach (Transform child in PowerSystemNetworkController.Instance.transform)
					{
						if (child.GetComponent<PowerSystem>())
							if (child.GetComponent<PowerSystem>() != newPowerSystem && child.GetComponent<PowerSystem>().generators.ContainsKey(generator.Value.ID))
							{
								child.GetComponent<PowerSystem>().generators.Remove(generator.Value.ID);
								child.GetComponent<PowerSystem>().CheckSystemDetails();
							}
					}
				}

			foreach (Transform child in PowerSystemNetworkController.Instance.transform)
			{
				if (child.GetComponent<PowerSystem>())
					if (child.GetComponent<PowerSystem>() != newPowerSystem && child.GetComponent<PowerSystem>().powerPoles.ContainsKey(ID))
					{
						child.GetComponent<PowerSystem>().powerPoles.Remove(ID);
						child.GetComponent<PowerSystem>().CheckSystemDetails();
					}
			}

			newPowerSystem.CheckSystemDetails();
		}

		public IEnumerator Destroy()
		{
			foreach (Cable cable in connectedCables)
				if (cable != null)
					Destroy(cable.gameObject);

			yield return new WaitForSeconds(1);

			Destroy(powerSystem.gameObject);
			Destroy(this.gameObject);
		}
	}

}