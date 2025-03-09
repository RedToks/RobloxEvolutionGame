using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetect : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            PointerEventData eventData = new PointerEventData(EventSystem.current) { position = mousePos };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            Debug.Log("Objects under cursor:");
            foreach (var result in results)
            {
                Debug.Log(result.gameObject.name);
            }
        }
    }

}
