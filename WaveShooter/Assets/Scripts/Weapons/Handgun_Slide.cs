using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Handgun_Slide : MonoBehaviour {

    public Vector3 defaultPosition;
    public Vector3 fullPullbackPosition;
    public Vector3 casingEjectPosition;
    public Vector3 lastShotPosition;
    public float slideSpeed = 300f;

    [HideInInspector] public int state = 0;
    [HideInInspector] public bool isGrabbed = false;
    [HideInInspector] public bool doChamber = false;
    [HideInInspector] public bool isMove = false;

    float lastDist;
    float lastCasingDist=0;
    float lastCasingDistReturn;

    VRTK_InteractableObject interactableObject;
    VRTK_ControllerEvents controllerEvents;

    void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();

        if (interactableObject == null)
        {
            Debug.LogError("Handgun_Slide is required to be attached to an Object that has the VRTK_InteractableObject script attached to it");
            return;
        }

        interactableObject.InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
        interactableObject.InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectUngrabbed);
    }

    void Update()
    {
        if (IsFullPos())
            doChamber = true;

        if (IsDefaultPos())
            doChamber = false;

    }

    void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbed = true;
    }

    void ObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        SetPosition(0);
        isGrabbed = false;
    }

    public bool IsDefaultPos()
    {
        return (transform.localPosition == defaultPosition);
    }

    public bool IsFullPos()
    {
        return (transform.localPosition == fullPullbackPosition);
    }

    public bool IsReadyToChamber()
    {
        if (!doChamber)
            return false;

        float dist = Vector3.Distance(transform.localPosition, defaultPosition);

        if (dist < lastDist)
        {
            return true;
        }
        else
        {
            lastDist = dist;
            return false;
        }
    }

    public bool IsReadyToEject()
    {
        if (doChamber || IsDefaultPos())
            return false;

        float dist = transform.localPosition.magnitude;

        float ejectzoneStart = (fullPullbackPosition - casingEjectPosition).magnitude;
        float ejectzoneEnd = fullPullbackPosition.magnitude;

        if (dist > ejectzoneStart && dist < ejectzoneEnd)
        {
            return true;
        }
        else
        {
            lastCasingDist = dist;
            return false;
        }
    }

    public void SetPosition(int setState)
    {
        if (isMove)
            return;

        Vector3 setPos;
        state = setState;

        if (setState == 1)
        {
            setPos = fullPullbackPosition;
        }
        else if (setState == 2)
        {
            setPos = lastShotPosition;
        }
        else
        {
            setPos = defaultPosition;
        }

        float moveTime = Time.time;
        float moveDist = Vector3.Distance(transform.localPosition, setPos);

        if (moveDist > 0)
        {
            StartCoroutine(MoveToPosition(setPos, moveDist, moveTime));
            isMove = true;
        }
    }

    IEnumerator MoveToPosition(Vector3 pos, float totalDist, float startTime)
    {
        float distCovered = (Time.time - startTime) * slideSpeed;
        float fracJourney = distCovered / totalDist;

        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, fracJourney);

        yield return new WaitForEndOfFrame();

        if (distCovered <= totalDist)
        {
            StartCoroutine(MoveToPosition(pos, totalDist, startTime));
        } else
        {
            isMove = false;

            if (state == 1)
            {
                SetPosition(0);
            }
        }
    }


}
