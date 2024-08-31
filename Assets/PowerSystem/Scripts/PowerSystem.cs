using UnityEngine;
using System.Collections.Generic;
using MrCryptographic.PowerSystem;

namespace MrCryptographic.PowerSystem
{

	public class PowerSystem : MonoBehaviour
	{
		[SerializeField]
		public Dictionary<string, Generator> generators = new Dictionary<string, Generator>();
		[SerializeField]
		public Dictionary<string, PowerPole> powerPoles = new Dictionary<string, PowerPole>();
		[SerializeField]
		public Dictionary<string, Consumer> consumers = new Dictionary<string, Consumer>();

		[Header("Current")]
		public float currentPowerGenerated;
		public float currentPowerConsumed;

		[Header("")]
		[Header("Possible")]
		public float possiblePowerGenerated;
		public float possiblePowerConsumed;

		[Header("")]
		[Header("Info")]
		public bool hasPower;

		private GameObject RestartPowerSystemButtonExampleScene;

		private void Start()
		{
			RestartPowerSystemButtonExampleScene = GameObject.Find("RestartPowerSystemButtonExampleScene");
		}

		public void AddGenerator(Generator generator)
		{
			generators.Add(generator.ID, generator);
		}

		public void AddPowerPole(PowerPole powerPole)
		{
			powerPoles.Add(powerPole.ID, powerPole);
		}

		public void AddConsumer(Consumer consumer)
		{
			consumers.Add(consumer.ID, consumer);
		}

		public void CheckSystemDetails()
		{
			if (generators.Count == 0 && consumers.Count == 0 && powerPoles.Count == 0)
				Destroy(this.gameObject);

			currentPowerConsumed = 0;
			currentPowerGenerated = 0;

			possiblePowerConsumed = 0;
			possiblePowerGenerated = 0;

			foreach (var generator in generators)
			{
				possiblePowerGenerated += generator.Value.PowerOutput;

				if (generator.Value.on)
					currentPowerGenerated += generator.Value.PowerOutput;
			}
			foreach (var consumer in consumers)
			{
				possiblePowerConsumed += consumer.Value.PowerInput;

				if (consumer.Value.on)
					currentPowerConsumed += consumer.Value.PowerInput;
			}

			if (currentPowerConsumed > currentPowerGenerated)
				hasPower = false;

			if (RestartPowerSystemButtonExampleScene != null)
			{
				RestartPowerSystemButtonExampleScene.SetActive(!hasPower);
			}
		}

		public bool RestartPowerSystem()
		{
			if (currentPowerConsumed > currentPowerGenerated)
			{
				hasPower = false;
				return false;//Returns true useable to check if the restart is succeeded
			}
			else
			{
				hasPower = true;
				return true;//Returns false useable to check if the restart is succeeded
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;

			foreach (var generator in generators)
				Gizmos.DrawWireSphere(generator.Value.transform.position, generator.Value.transform.localScale.x);
			foreach (var consumer in consumers)
				Gizmos.DrawWireSphere(consumer.Value.transform.position, consumer.Value.transform.localScale.x);
			foreach (var powerPole in powerPoles)
				Gizmos.DrawWireSphere(powerPole.Value.transform.position, powerPole.Value.transform.localScale.x);
		}
	}

}