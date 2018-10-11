using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Bullet_Controller : VRTK_InteractableObject
{
    AudioManager audioManager;
    Calibers cals;

    [Header("Bullet Options")]
    public Calibers.CaliberList RoundType;
    public GameObject BulletPrefab;
    public GameObject BulletProjectile;
    public float ShellLifetime = 60f;
    public BulletStats stats;

    public MeshRenderer BulletMesh;
    public MeshRenderer ShellMesh;

    [Header("Sounds")]
    public Sound Casing;

    [HideInInspector]
    public bool isLoadedInMag;

    [HideInInspector]
    public bool holdingBullet = false;

    [HideInInspector]
    public bool hasFired = false;

    Rigidbody storedRigidbodyData;
    bool soundPlaying = false;


    protected override void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        isLoadedInMag = false;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            storedRigidbodyData = rigidbody;
        }
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject)
    {
        base.Grabbed(currentGrabbingObject);

        //Limit hands grabbing when picked up
        if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject.controllerEvents.gameObject) == SDK_BaseController.ControllerHand.Left)
        {
            allowedTouchControllers = AllowedController.LeftOnly;
            allowedUseControllers = AllowedController.LeftOnly;
        }
        else if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject.controllerEvents.gameObject) == SDK_BaseController.ControllerHand.Right)
        {
            allowedTouchControllers = AllowedController.RightOnly;
            allowedUseControllers = AllowedController.RightOnly;
        }

        //holding a bullet with controller
        holdingBullet = true;
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        //Unlimit hands
        allowedTouchControllers = AllowedController.Both;
        allowedUseControllers = AllowedController.Both;

        holdingBullet = false;
    }

    public void ToggleBulletVisibility(bool state)
    {
        BulletMesh.enabled = state;
    }

    public void ToggleShellVisibility(bool state)
    {
        ShellMesh.enabled = state;
    }

    public void TogglePhysics(bool state)
    {
        Collider col = GetComponent<Collider>();
        col.enabled = state;
    }

    public void ToggleRigidbody(bool state)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = !state;
            rigidbody.useGravity = state;
        }
        else
        {
            Debug.LogWarning("Rigidbody not found on " + transform.name);
        }
    }

    public void RemoveRigidbody()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
            Destroy(rigidbody);
    }

    public void AddRigidbody()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Rigidbody newRB = gameObject.AddComponent<Rigidbody>();
            newRB = storedRigidbodyData;
        }
    }

    public void AddForceToBullet(Vector3 pos, float force)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.AddRelativeForce(pos * force);
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (GetComponent<Rigidbody>().useGravity)
        {
            soundPlaying = true;
            PlayCollisionSound();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        soundPlaying = false;
        PlayCollisionSound();
    }

    void PlayCollisionSound()
    {
        if (!soundPlaying)
            return;

        audioManager.PlayNew(Casing);
    }*/
}
