using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MrCryptographic.PowerSystem
{

	//Don't look at this. This is pure "bullshit". Its olny made to show stats in the info panel at the example scene so I don't have given any effort at this!
	public class Panel : MonoBehaviour
	{
		public bool infoMode = true;
		public bool destroyMode = false;

		public Toggle onToggle;

		public Text OnOffText;
		public Text OutputInputText;
		public Text hasPowerText;
		public Text OutputInputValueText;

		public Button RestartPowerSystemButton;

		private Generator generator;
		private Consumer consumer;

		private void Update()
		{
			if (infoMode)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out RaycastHit hit))
					{
						if (hit.transform.GetComponent<PowerPole>())
						{
							consumer = null;
							generator = null;
							OnOffText.text = "";
							OutputInputText.text = "";
							OutputInputValueText.text = "";
							onToggle.gameObject.SetActive(false);
							hasPowerText.text = hit.transform.GetComponent<PowerPole>().powerSystem.hasPower ? "true" : "false";
							if (!hit.transform.GetComponent<PowerPole>().powerSystem.hasPower)
								RestartPowerSystemButton.gameObject.SetActive(true);
							else
								RestartPowerSystemButton.gameObject.SetActive(false);
						}
						else if (hit.transform.GetComponent<Consumer>())
						{
							consumer = hit.transform.GetComponent<Consumer>();
							generator = null;
							OnOffText.text = "On/Off";
							OutputInputText.text = "Input: ";
							OutputInputValueText.text = consumer.PowerInput.ToString();
							onToggle.gameObject.SetActive(true);
							onToggle.isOn = consumer.on;
							hasPowerText.text = consumer.powerSystem.hasPower ? "true" : "false";
							if (!consumer.powerSystem.hasPower)
								RestartPowerSystemButton.gameObject.SetActive(true);
							else
								RestartPowerSystemButton.gameObject.SetActive(false);
						}
						else if (hit.transform.GetComponent<Generator>())
						{
							generator = hit.transform.GetComponent<Generator>();
							consumer = null;
							OnOffText.text = "On/Off";
							OutputInputText.text = "Output: ";
							OutputInputValueText.text = generator.PowerOutput.ToString();
							onToggle.gameObject.SetActive(true);
							onToggle.isOn = hit.transform.GetComponent<Generator>().on;
							hasPowerText.text = hit.transform.GetComponent<Generator>().powerSystem.hasPower ? "true" : "false";
							if (!generator.powerSystem.hasPower)
								RestartPowerSystemButton.gameObject.SetActive(true);
							else
								RestartPowerSystemButton.gameObject.SetActive(false);
						}
					}
				}
			}
			else if (destroyMode)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out RaycastHit hit))
					{
						if (hit.transform.GetComponent<PowerPole>() == null)
							Destroy(hit.transform.gameObject);
						else
							hit.transform.GetComponent<PowerPole>().StartCoroutine(hit.transform.GetComponent<PowerPole>().Destroy());
					}
				}
			}
		}

		public void Toogle(bool on)
		{
			if (generator != null)
				generator.ChangeOnState(on);
			if (consumer != null)
				consumer.ChangeOnState(on);

			CheckPower();
		}

		public void RestartPowerSystem()
		{
			if (generator != null)
				if (!generator.powerSystem.RestartPowerSystem())
				{
					RestartPowerSystemButton.gameObject.SetActive(true);
					hasPowerText.text = "false";
				}
				else
				{
					RestartPowerSystemButton.gameObject.SetActive(false);
					hasPowerText.text = "true";
				}

			if (consumer != null)
				if (!consumer.powerSystem.RestartPowerSystem())
				{
					RestartPowerSystemButton.gameObject.SetActive(true);
					hasPowerText.text = "false";
				}
				else
				{
					RestartPowerSystemButton.gameObject.SetActive(false);
					hasPowerText.text = "true";
				}
		}

		private void CheckPower()
		{
			if (generator != null)
				if (!generator.powerSystem.hasPower)
				{
					RestartPowerSystemButton.gameObject.SetActive(true);
					hasPowerText.text = "false";
				}
				else
				{
					RestartPowerSystemButton.gameObject.SetActive(false);
					hasPowerText.text = "true";
				}

			if (consumer != null)
				if (!consumer.powerSystem.hasPower)
				{
					RestartPowerSystemButton.gameObject.SetActive(true);
					hasPowerText.text = "false";
				}
				else
				{
					RestartPowerSystemButton.gameObject.SetActive(false);
					hasPowerText.text = "true";
				}
		}

		public void SetInfoMode(bool on)
		{
			if (on)
				infoMode = true;
			else
				infoMode = false;
		}

		public void SetDestroyMode(bool on)
		{
			if (on)
				destroyMode = true;
			else
				destroyMode = false;
		}

		public void ResetScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

}
