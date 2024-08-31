using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteract : MonoBehaviour
{
    // Start is called before the first frame update
   public Button button;

    void Start()
    {
        button = GetComponent<Button>();

        // Set the button as interactable
        button.interactable = true;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
