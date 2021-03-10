using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputTabNavigation : MonoBehaviour
{
    EventSystem system;
    void Start ()
    {
        system = EventSystem.current;
    }
    
    public void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
 
            if (next!= null) {
                       
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield !=null) inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
                       
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            //else Debug.Log("next nagivation element not found");
   
        }
    }

}
