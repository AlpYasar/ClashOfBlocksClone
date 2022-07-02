using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;


public class PlayerInput : MonoBehaviour
{
    public Vector3Event pointerEvent;
    
    [SerializeField] private Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectMouseClick();
    }
    
    void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Debug.Log("Hit: " + hit.collider.name);
                pointerEvent.Raise(hit.point);
            }
        }
    }
}
