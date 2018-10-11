using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour {

    Camera m_MainCamera;

    private void Start()
    {
        CheckCamera();
    }

    void Update()
    {
        CheckCamera();
    }

    void CheckCamera()
    {

        if (m_MainCamera != null)
        {
            transform.LookAt(m_MainCamera.gameObject.transform);
        } else
        {
            m_MainCamera = Camera.main;
        }
    }

}
