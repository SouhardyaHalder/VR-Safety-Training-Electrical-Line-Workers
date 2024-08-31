using UnityEngine;
using UnityEngine.UI;

namespace MrCryptographic.PowerSystem
{
	[RequireComponent(typeof(CharacterController))]
	public class FPSController : MonoBehaviour
	{
		[Header("Referenced")]
		public GameObject consumer;
		public GameObject generator;
		public GameObject powerPole;

		public Camera playerCamera;

		CharacterController characterController;
		Vector3 moveDirection = Vector3.zero;
		float rotationX = 0;

		[HideInInspector]
		public bool canMove = true;

		void Start()
		{
			characterController = GetComponent<CharacterController>();

			// Lock cursor
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update()
		{
			// We are grounded, so recalculate move direction based on axes
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 right = transform.TransformDirection(Vector3.right);
			// Press Left Shift to run
			bool isRunning = Input.GetKey(KeyCode.LeftShift);
			float curSpeedX = canMove ? (isRunning ? 11.5f : 7.5f) * Input.GetAxis("Vertical") : 0;
			float curSpeedY = canMove ? (isRunning ? 11.5f : 7.5f) * Input.GetAxis("Horizontal") : 0;
			float movementDirectionY = moveDirection.y;
			moveDirection = (forward * curSpeedX) + (right * curSpeedY);

			if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
			{
				moveDirection.y = 8.0f;
			}
			else
			{
				moveDirection.y = movementDirectionY;
			}

			// Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
			// when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
			// as an acceleration (ms^-2)
			if (!characterController.isGrounded)
			{
				moveDirection.y -= 20.0f * Time.deltaTime;
			}

			// Move the controller
			characterController.Move(moveDirection * Time.deltaTime);

			// Player and Camera rotation
			if (canMove)
			{
				rotationX += -Input.GetAxis("Mouse Y") * 2.0f;
				rotationX = Mathf.Clamp(rotationX, -45.0f, 45.0f);
				playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
				transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 2.0f, 0);
			}

			UpdateInput();
		}

		private void UpdateInput()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
				SpawnObjectOnPoint(consumer);
			if (Input.GetKeyDown(KeyCode.Alpha2))
				SpawnObjectOnPoint(generator);
			if (Input.GetKeyDown(KeyCode.Alpha3))
				SpawnObjectOnPoint(powerPole);
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				PowerSystemNetworkController.Instance.connectionMode = !PowerSystemNetworkController.Instance.connectionMode;
				string mode = "Mode: ";
				mode += PowerSystemNetworkController.Instance.connectionMode ? "on" : "off";
				GameObject.Find("ConnectionMode Text").GetComponent<Text>().text = mode;
			}
		}

		private void SpawnObjectOnPoint(GameObject obj)
		{
			Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				Instantiate(obj, hit.point + new Vector3(0, obj.transform.localScale.y / 2, 0), Quaternion.identity);
			}
		}

	}
}