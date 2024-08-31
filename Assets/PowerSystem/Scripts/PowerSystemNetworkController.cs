using UnityEngine;
using MrCryptographic.PowerSystem;

namespace MrCryptographic.PowerSystem
{

	public class PowerSystemNetworkController : MonoBehaviour
	{
		[Header("Prefab")]
		public GameObject PowerSystemPrefab;
		public GameObject CablePrefab;

		[Header("")]
		[Header("Powersystem connection stuff")]
		[Header("(do not reference stuff here, for debug looking only!)")]
		public Transform system1Object;
		public Transform system2Object;
		public PowerSystem system1;
		public PowerSystem system2;

		public bool connectionMode = false;

		public static PowerSystemNetworkController Instance;

		private void Awake()
		{
			Instance = this;
		}

		//Creating power system with first entry as generator
		public PowerSystem CreatePowerSystemAsGenerator(Generator generator)
		{
			GameObject obj = Instantiate(PowerSystemPrefab, this.transform);
			PowerSystem powerSystem = obj.GetComponent<PowerSystem>();
			powerSystem.AddGenerator(generator);
			powerSystem.CheckSystemDetails();
			return powerSystem;
		}

		//Creating power system with first entry as consumer
		public PowerSystem CreatePowerSystemAsConsumer(Consumer consumer)
		{
			GameObject obj = Instantiate(PowerSystemPrefab, this.transform);
			PowerSystem powerSystem = obj.GetComponent<PowerSystem>();
			powerSystem.AddConsumer(consumer);
			powerSystem.CheckSystemDetails();
			return powerSystem;
		}

		//Creating power system with first entry as power pole
		public PowerSystem CreatePowerSystemAsPowerPole(PowerPole powerPole)
		{
			GameObject obj = Instantiate(PowerSystemPrefab, this.transform);
			PowerSystem powerSystem = obj.GetComponent<PowerSystem>();
			powerSystem.AddPowerPole(powerPole);
			powerSystem.CheckSystemDetails();
			return powerSystem;
		}

		private void Update()//checking if the player is connecting/disconnecting power poles / machine's
		{
			if (!connectionMode) { return; }
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hit))
				{
					if (hit.transform.GetComponent<PowerPole>() != null) //set system1/2 + object1/2 when power pole
					{
						PowerPole powerPole = hit.transform.GetComponent<PowerPole>();
						if (system1 == null) //when system1 is free
						{
							system1 = powerPole.powerSystem;
							system1Object = powerPole.transform;
						}
						else if (system2 == null) //when system 1 is not free use system 2
						{
							system2 = powerPole.powerSystem;
							system2Object = powerPole.transform;
						}
					}
					else if (hit.transform.GetComponent<Generator>() != null) //set system1/2 + object1/2 when generator
					{
						if (!hit.transform.GetComponent<Generator>().connectedToPowerPole)
						{
							Generator generator = hit.transform.GetComponent<Generator>();
							if (system1 == null)//when system1 is free
							{
								system1 = generator.powerSystem;
								system1Object = generator.transform;
							}
							else if (system2 == null)//when system 1 is not free use system 2
							{
								system2 = generator.powerSystem;
								system2Object = generator.transform;
							}
						}
					}
					else if (hit.transform.GetComponent<Consumer>() != null) //set system1/2 + object1/2 when consumer
						if (!hit.transform.GetComponent<Consumer>().connectedToPowerPole)
						{
							Consumer consumer = hit.transform.GetComponent<Consumer>();
							if (system1 == null)//when system1 is free
							{
								system1 = consumer.powerSystem;
								system1Object = consumer.transform;
							}
							else if (system2 == null)//when system 1 is not free use system 2
							{
								system2 = consumer.powerSystem;
								system2Object = consumer.transform;
							}
						}
				}
			}

			if (system1 != null && system2 != null)
				if (system1 != system2) // check if both systens are the same
				{
					if (system1Object.GetComponent<Generator>() && system2Object.GetComponent<Generator>())
					{
						ResetSystem1and2();
						return;//When both objects are generators return
					}
					if (system1Object.GetComponent<Consumer>() && system2Object.GetComponent<Consumer>())
					{
						ResetSystem1and2();
						return;//When both objects are consumers return
					}

					ConnectPowerPoles();//When connecting different typs z.b generator + consumer, consumer + gernerator + power pole, consumer + power pole <- power poles are an exception
				}
				else
					ResetSystem1and2();
		}

		public void ConnectPowerPoles()
		{
			// Combine the lists of generators, power poles, and consumers from both power systems
			if (system2.generators.Count > 0)
				foreach (var generator in system2.generators)
					system1.generators.Add(generator.Value.ID, generator.Value);
			if (system2.powerPoles.Count > 0)
				foreach (var powerPole in system2.powerPoles)
					system1.powerPoles.Add(powerPole.Value.ID, powerPole.Value);
			if (system2.consumers.Count > 0)
				foreach (var consumer in system2.consumers)
					system1.consumers.Add(consumer.Value.ID, consumer.Value);

			// Remove the second power system script from all the objects it was controlling and checking if there is anything to move
			if (system2.generators.Count > 0)
				foreach (var generator in system2.generators)
				{
					generator.Value.powerSystem = system1;
				}
			if (system2.powerPoles.Count > 0)
				foreach (var powerPole in system2.powerPoles)
				{
					powerPole.Value.powerSystem = system1;
				}
			if (system2.consumers.Count > 0)
				foreach (var consumer in system2.consumers)
				{
					consumer.Value.powerSystem = system1;
				}

			//Dont let this see yandere dev
			//Linking PowerPole with Generator if requirements are right
			if (system1Object.GetComponent<PowerPole>() && system2Object.GetComponent<Generator>())
			{
				system1Object.GetComponent<PowerPole>().connectedGenerators.Add(system2Object.GetComponent<Generator>().ID, system2Object.GetComponent<Generator>());
			}
			else if (system2Object.GetComponent<PowerPole>() && system1Object.GetComponent<Generator>())
			{
				system2Object.GetComponent<PowerPole>().connectedGenerators.Add(system1Object.GetComponent<Generator>().ID, system1Object.GetComponent<Generator>());
			}
			//Linking PowerPole with Consumer if requirements are right
			if (system1Object.GetComponent<PowerPole>() && system2Object.GetComponent<Consumer>())
			{
				system1Object.GetComponent<PowerPole>().connectedConsumers.Add(system2Object.GetComponent<Consumer>().ID, system2Object.GetComponent<Consumer>());
			}
			else if (system2Object.GetComponent<PowerPole>() != null && system1Object.GetComponent<Consumer>() != null)
			{
				system2Object.GetComponent<PowerPole>().connectedConsumers.Add(system1Object.GetComponent<Consumer>().ID, system1Object.GetComponent<Consumer>());
			}
			//Linking PowerPole with PowerPole if requirements are right
			if (system1Object.GetComponent<PowerPole>() && system2Object.GetComponent<PowerPole>())
			{
				system1Object.GetComponent<PowerPole>().connectedPowerPoles.Add(system2Object.GetComponent<PowerPole>().ID, system2Object.GetComponent<PowerPole>());
				system2Object.GetComponent<PowerPole>().connectedPowerPoles.Add(system1Object.GetComponent<PowerPole>().ID, system1Object.GetComponent<PowerPole>());
			}

			Destroy(system2.gameObject); //Destroying the old system with no use anymore 

			system1.CheckSystemDetails(); //Check System Details (update currentGeneratedPower / currentCunsumedPower)

			Cable cable = Instantiate(CablePrefab.GetComponent<Cable>(), this.transform);//Creates the cable object
			cable.SetConnection(system1Object.Find("Connection Point").gameObject, system2Object.Find("Connection Point").gameObject);//Sets the cable to the corect positions;

			ResetSystem1and2();//Resets the reference slots
		}

		//Disconnects a power pole and a power pole
		public void DisconnectPowerPoleAndPowerPole(PowerPole pole1, PowerPole pole2)
		{
			//Disconnect the two power poles
			pole1.connectedPowerPoles.Remove(pole2.ID);
			pole2.connectedPowerPoles.Remove(pole1.ID);

			//Reload the system
			pole1.ReloadIntoRightPowerSystem(null);
			pole2.ReloadIntoRightPowerSystem(null);
		}

		//Disconnects a power pole and a generator
		public void DisconnectPowerPoleAndGenerator(PowerPole powerPole, Generator generator)
		{
			generator.powerSystem.generators.Remove(generator.ID); //Removes the generator from the current power system he is in
			generator.powerSystem.CheckSystemDetails(); //Reloads the details from the system because the generator is not there anymore
			generator.powerSystem = CreatePowerSystemAsGenerator(generator); //Creates a new Power system for itself. (Wen he is connected again this two power system are merched)

			powerPole.connectedGenerators.Remove(generator.ID); //Removes the connection from the power pole to the generator
		}

		//Disconnects a power pole ans a consumer
		public void DisconnectPowerPoleAndConsumer(PowerPole powerPole, Consumer consumer)
		{
			consumer.powerSystem.consumers.Remove(consumer.ID); //Removes the consumer from the current power system he is in
			consumer.powerSystem.CheckSystemDetails(); //Reloads the details from the system because the consumer is not there anymore
			consumer.powerSystem = CreatePowerSystemAsConsumer(consumer); //Creates a new Power system for itself. (Wen he is connected again this two power system are merched)

			powerPole.connectedConsumers.Remove(consumer.ID); //Removes the connection from the power pole to the consumer
		}

		//Resets the reference slots
		private void ResetSystem1and2()
		{
			system1 = null;
			system1Object = null;
			system2 = null;
			system2Object = null;
		}

		public void SetConnectionMode(bool on)
		{
			if (on)
				connectionMode = true;
			else
				connectionMode = false;
		}
	}

}