using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Weapon_Magazine : MonoBehaviour
{
    public int magazineID;
    public Calibers.CaliberList compatibleRound;
    public Transform ejectPoint;
    public GameObject defaultBulletPrefab;
    public int maxAmmo;
    public int currAmmo;

    public Transform[] slots;

    Stack<GameObject> ammoList = new Stack<GameObject>();

    int startAmmo;

    VRTK_InteractableObject interactableObject;
    VRTK_ControllerEvents controllerEvents;
    Handgun_Controller handgun_Controller;

    void Start ()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();

        if (interactableObject == null)
        {
            Debug.LogError("Weapon_Magazine is required to be attached to an Object that has the VRTK_InteractableObject script attached to it");
            return;
        }

        interactableObject.InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
        interactableObject.InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectUngrabbed);
        interactableObject.InteractableObjectUsed += new InteractableObjectEventHandler(ObjectUsed);
        interactableObject.InteractableObjectUnused += new InteractableObjectEventHandler(ObjectUnused);

        startAmmo = currAmmo;

        InitMagazine();
    }

    void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        transform.parent = null;
        if (handgun_Controller != null)
        {
            handgun_Controller.DetachObjectGrabbed(gameObject);
        }
    }

    void ObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (handgun_Controller == null)
        {
            transform.parent = null;
        }
    }

    void ObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        if (ammoList.Count > 0)
        {
            RemoveRound();
        }
    }

    void ObjectUnused(object sender, InteractableObjectEventArgs e)
    {

    }

    public void AttachHandgunController(GameObject gun)
    {
        handgun_Controller = gun.GetComponent<Handgun_Controller>();

        if (handgun_Controller == null)
        {
            Debug.LogError("Handgun_Controller not found on gun object passed to Weapon_Magazine");
        }
    }

    public void DetachHandgunController()
    {
        handgun_Controller = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            GameObject bullet = other.gameObject;
            if (bullet != null)
            {
                Bullet_Controller bullet_Controller = bullet.GetComponent<Bullet_Controller>();
                if (bullet_Controller != null && !IsMagazineFull() && !bullet_Controller.isLoadedInMag && !bullet_Controller.hasFired && bullet_Controller.RoundType == compatibleRound)
                {
                    ToggleBulletControl(bullet);
                    AddRound(bullet);
                    bullet_Controller.ForceStopInteracting();
                    bullet_Controller.holdingBullet = false;
                }
            }
        }
    }

    void InitMagazine()
    {
        for (int i = 0; i < startAmmo; i++)
        {
            if (!IsMagazineFull())
            {
                AddRound(SpawnRound());
            }
        }
    }

    void AddRound(GameObject obj)
    {
        ammoList.Push(obj);
        DisplayAmmo();
    }

    void RemoveRound()
    {
        GameObject pop = ammoList.Pop();
        EjectRound(pop);
        DisplayAmmo();
    }

    public GameObject GetTopBullet()
    {
        GameObject pop = null;

        if (ammoList.Count > 0)
        {
            pop = ammoList.Pop();
            DisplayAmmo();
        }

        return pop;
    }

    GameObject SpawnRound()
    {
        GameObject obj = null;

        if (defaultBulletPrefab != null)
        {
            obj = Instantiate(defaultBulletPrefab) as GameObject;
            obj.SetActive(false);
            obj.transform.parent = transform;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            ToggleBulletControl(obj);
        }

        return obj;
    }

    void ToggleBulletControl(GameObject obj)
    {
        Bullet_Controller bullet_Controller = obj.GetComponent<Bullet_Controller>();
        if (bullet_Controller != null)
        {
            if (obj.GetComponent<Rigidbody>() != null)
            {
                bullet_Controller.TogglePhysics(false);
                bullet_Controller.RemoveRigidbody();
                bullet_Controller.isGrabbable = false;
                bullet_Controller.isLoadedInMag = true;
            } else
            {
                bullet_Controller.TogglePhysics(true);
                bullet_Controller.AddRigidbody();
                bullet_Controller.isGrabbable = true;
                bullet_Controller.isLoadedInMag = false;
            }
        }
    }

    void EjectRound(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.parent = null;
        obj.transform.position = ejectPoint.position;
        obj.transform.rotation = ejectPoint.rotation;

        ToggleBulletControl(obj);

        if (obj.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.AddRelativeForce(ejectPoint.forward * 1f);
        }
    }

    bool IsMagazineFull()
    {
        return (ammoList.Count == maxAmmo);
    }

    void DisplayAmmo()
    {
        foreach (GameObject item in ammoList)
        {
            item.SetActive(false);
            item.transform.parent = transform;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            GameObject obj = GetItemFromStack(i);
            if (obj != null)
            {
                obj.SetActive(true);
                obj.transform.parent = slots[i];
                obj.transform.position = slots[i].position;
                obj.transform.rotation = slots[i].rotation;
            }
        }
    }

    GameObject GetItemFromStack(int pos)
    {
        int count = 0;
        GameObject obj = null;

        foreach (GameObject item in ammoList)
        {
            if (count == pos)
            {
                return item;
            }
            count++;
        }

        return obj;
    }

}
