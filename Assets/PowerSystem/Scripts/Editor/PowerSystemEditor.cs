using UnityEditor;
using UnityEngine;

namespace MrCryptographic.PowerSystem
{

	[CustomEditor(typeof(PowerSystem))]
	public class PowerSystemEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			PowerSystem powerSystem = (PowerSystem)target;

			GUILayout.Label("Generators: " + powerSystem.generators.Count);
			GUILayout.Label("Consumers: " + powerSystem.consumers.Count);
			GUILayout.Label("Power poles: " + powerSystem.powerPoles.Count);
		}
	}

}
