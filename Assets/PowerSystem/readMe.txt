Use MrCryptographic.PowerSystem to use systems from the generators, consumers,...

Setup:
-Have a "PowersystemNetworkController" in your scene, because its there to connect and disconnect systems

How it works:
-To connect two power system use the "PowersystemNetworkController", when 2 system objects are given it connects them -> always have a "PowersystemNetworkController" in your scene
-When a generator, consumer, ... is created it creates a power system for it. Connect two system with the "PowersystemNetworkController" to make one bigger system
-Set power input/output in the prefab of the generator/consumer
-When power consumption > power generation the power system sets all mashines to have no power
-To restart the system in the example scene there is a button to restart in other in other cases call the "RestartPowerSystem" function in the power system script -> returns a bool if the power system restart has succeeded or not(true/false)

Look at the example scenes to see in action how it works
In the example scene folders are also scripts these scripts are only used in the example scene -> I have made this because when somebody dosent want the example scenes and he unselect the example scene folders that he dosen't get the scripts that are only used for that