using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.XR.Interaction.Toolkit;

public class sceneTransition : MonoBehaviour
{
   public void SceneLoader(int index){
	   SceneManager.LoadScene(index);
   }
}