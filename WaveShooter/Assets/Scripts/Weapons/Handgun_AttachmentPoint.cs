using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Handgun_AttachmentPoint : MonoBehaviour
{
    public int attachmentPointID = 1;

    Handgun_Controller controller;
    AttachableObject attachableObject;

    bool reactivate = false;
    bool triggerSwitch = true;
    float timeElapsed;

    private void Start()
    {
        controller = transform.root.GetComponent<Handgun_Controller>();

        if (controller == null)
        {
            Debug.LogError("Handgun_Controller not found on root GameObject");
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerSwitch && !controller.pausetriggers && IsAttachmentPointEmpty() && other.tag == "AttachableObject")
        {
            GameObject obj = other.gameObject;
            attachableObject = obj.GetComponent<AttachableObject>();
            if (attachableObject != null && attachmentPointID == attachableObject.attachPoint)
            {
                triggerSwitch = false;
                controller.AttachObject(obj, transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!reactivate && !controller.pausetriggers && IsAttachmentPointEmpty() && other.tag == "AttachableObject")
        {
            StartCoroutine(TriggerActivate());
            reactivate = true;
        }
    }

    IEnumerator TriggerActivate()
    {
        yield return new WaitForSeconds(0.5f);
        triggerSwitch = true;
        reactivate = false;
    }

    bool IsAttachmentPointEmpty()
    {
        return (transform.childCount == 0);
    }

}
