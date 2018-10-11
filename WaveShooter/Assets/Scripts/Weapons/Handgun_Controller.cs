using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Handgun_Controller : MonoBehaviour
{
    public WeaponStats stats;

    [Header("Weapon Objects")]
    public Transform firePoint;
    public Transform loadPoint;
    public Transform casingPoint;
    public Transform magEjectPoint;
    public Transform magazineSlot;
    public Transform trigger;
    public GameObject slider;

    [Header("Muzzle Effects")]
    public ParticleEffect muzzleFlash;

    [Header("Magazine Settings")]
    public bool CanMagBeGrabbedFromWeapon;
    public int compatibleMagazineID;

    [Header("Trigger Limits")]
    public float minTriggerRotation = 0;
    public float maxTriggerRotation = 30f;

    VRTK_InteractableObject interactableObject;
    VRTK_ControllerEvents controllerEvents;
    Handgun_Slide slideControl;

    GameObject firstController;
    GameObject muzzle_flash;

    float insertSpeed = 1f;
    float lastFired;

    bool triggerHold;
    bool pauseReChamber;
    [HideInInspector] public bool pausetriggers;

    float touchpadUp = 0.5f;
    float touchpadDown = -0.5f;
    float touchpadLeft = -0.5f;
    float touchpadRight = 0.5f;

    private void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();
        slideControl = slider.GetComponent<Handgun_Slide>();

        if (interactableObject == null)
        {
            Debug.LogError("Handgun_Controller is required to be attached to an Object that has the VRTK_InteractableObject script attached to it");
            return;
        }

        interactableObject.InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
        interactableObject.InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectUngrabbed);
        interactableObject.InteractableObjectUsed += new InteractableObjectEventHandler(ObjectUsed);
        interactableObject.InteractableObjectUnused += new InteractableObjectEventHandler(ObjectUnused);

        PreloadEffects();
    }


    private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (firstController != null)
            return;

        firstController = e.interactingObject;

        controllerEvents = e.interactingObject.GetComponent<VRTK_ControllerEvents>();

        if (VRTK_DeviceFinder.GetControllerHand(e.interactingObject) == SDK_BaseController.ControllerHand.Left)
        {
            interactableObject.allowedUseControllers = VRTK_InteractableObject.AllowedController.LeftOnly;
        }
        else if (VRTK_DeviceFinder.GetControllerHand(e.interactingObject) == SDK_BaseController.ControllerHand.Right)
        {
            interactableObject.allowedUseControllers = VRTK_InteractableObject.AllowedController.RightOnly;
        }

        controllerEvents.TouchpadPressed += TouchpadPressed;
    }

    private void ObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (e.interactingObject != firstController)
            return;

        controllerEvents.TouchpadPressed -= TouchpadPressed;
        controllerEvents = null;
        firstController = null;
        interactableObject.allowedUseControllers = VRTK_InteractableObject.AllowedController.Both;
    }

    private void ObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        triggerHold = true;
    }

    private void ObjectUnused(object sender, InteractableObjectEventArgs e)
    {
        triggerHold = false;
    }

    private void TouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (controllerEvents.GetTouchpadAxis().y <= touchpadDown)
        {
            if (GetInsertedMag() != null)
            {
                DetachObject(GetInsertedMag(), magEjectPoint);
            }
        }

        if (controllerEvents.GetTouchpadAxis().y >= touchpadUp)
        {
            if (slideControl != null && !slideControl.IsDefaultPos() && !triggerHold)
            {
                slideControl.SetPosition(0);
            }
        }
    }

    void Update()
    {
        MoveWeaponTrigger();

        if (slideControl.IsDefaultPos())
        {
            pauseReChamber = false;
        }

        if (!IsChamberEmpty() && slideControl.IsReadyToEject())
        {
            UnChamberRound();
        }

        if (slideControl.IsReadyToChamber() && !pauseReChamber)
        {
            ChamberRound();
        }

        if (CheckAbleToFire())
        {
            lastFired = Time.time;
            Fire();
        }

    }

    void PreloadEffects()
    {
        if (muzzleFlash != null)
        {
            muzzle_flash = Instantiate(muzzleFlash.effect, gameObject.transform) as GameObject;
            muzzle_flash.transform.position = firePoint.position;
            muzzle_flash.transform.rotation = firePoint.rotation;
            muzzle_flash.transform.eulerAngles = muzzleFlash.offset;
        }
    }

    public void DetachObject(GameObject obj, Transform slot)
    {
        float insertTime = Time.time;
        float insertLength = Vector3.Distance(obj.transform.position, slot.position);

        StartCoroutine(MovetoPosition(obj.transform, slot, false, insertLength, insertTime));

        AttachDetachController(obj, false);
    }

    public void DetachObjectGrabbed(GameObject obj)
    {
        obj.transform.parent = null;
        ToggleObjectPhysics(obj);
        AttachDetachController(obj, false);
    }

    public void AttachObject(GameObject obj, Transform slot)
    {
        if (obj.GetComponent<Weapon_Magazine>() != null && !IsMagCompatible(obj.GetComponent<Weapon_Magazine>().magazineID))
            return;

        VRTK_InteractableObject interactableObject = obj.GetComponent<VRTK_InteractableObject>();
        if (interactableObject != null)
        {
            interactableObject.ForceStopInteracting();
        }

        float insertTime = Time.time;
        float insertLength = Vector3.Distance(obj.transform.position, slot.position);

        ToggleObjectPhysics(obj.gameObject);
        StartCoroutine(MovetoPosition(obj.transform, slot, true, insertLength, insertTime));

        AttachDetachController(obj, true);
    }

    void AttachDetachController(GameObject obj, bool attach)
    {
        if (obj.GetComponent<Weapon_Magazine>() != null)
        {
            if (attach)
            {
                obj.GetComponent<Weapon_Magazine>().AttachHandgunController(gameObject);
            } else
            {
                obj.GetComponent<Weapon_Magazine>().DetachHandgunController();
            }
        }
    }

    IEnumerator MovetoPosition(Transform obj, Transform point, bool attach, float totalDist, float startTime)
    {
        pausetriggers = true;

        float distCovered = (Time.time - startTime) * insertSpeed;
        float fracJourney = distCovered / totalDist;

        obj.rotation = point.rotation;
        obj.position = Vector3.Lerp(obj.position, point.position, fracJourney);

        yield return new WaitForEndOfFrame();

        if (distCovered < totalDist)
        {
            StartCoroutine(MovetoPosition(obj, point, attach, totalDist, startTime));
        } else
        {
            ToggleObjectGrabbable(obj.gameObject);
            pausetriggers = false;

            if (attach)
            {
                obj.parent = point;
                obj.position = point.position;
                obj.rotation = point.rotation;
            } else
            {
                obj.parent = null;
                ToggleObjectPhysics(obj.gameObject);
            }
        }
    }

    public GameObject GetInsertedMag()
    {
        if (magazineSlot.childCount != 1)
            return null;

        return magazineSlot.GetChild(0).gameObject;
    }

    GameObject GetChamberedRound()
    {
        if (!IsChamberEmpty())
            return loadPoint.GetChild(0).gameObject;

        return null;
    }

    void ToggleObjectPhysics(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        } else
        {
            Destroy(rb);
        }
    }

    void ToggleObjectGrabbable(GameObject obj)
    {
        bool grab = true;

        if (obj.GetComponent<Weapon_Magazine>() != null && CanMagBeGrabbedFromWeapon)
        {
            grab = false;
        }

        VRTK_InteractableObject controller = obj.GetComponent<VRTK_InteractableObject>();
        if (controller != null && grab)
        {
            controller.isGrabbable = !controller.isGrabbable;
        }
    }

    bool CheckAbleToFire()
    {
        return (slideControl != null && !slideControl.isGrabbed && slideControl.state == 0 && !IsChamberEmpty() && IsRoundUnfired() && triggerHold && Time.time - lastFired > 1 / stats.fireRate);
    }

    void Fire()
    {
        SpawnProjectile();
        RecoilSlider();
    }

    void SpawnProjectile()
    {
        GameObject bullet = GetChamberedRound();
        if (bullet != null)
        {
            Bullet_Controller bullet_Controller = bullet.GetComponent<Bullet_Controller>();
            bullet_Controller.hasFired = true;
            bullet_Controller.ToggleBulletVisibility(false);

            GameObject projectile = Instantiate(bullet_Controller.BulletProjectile) as GameObject;
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = firePoint.rotation;
            
            Projectile_3 projectile_3 = projectile.GetComponent<Projectile_3>();
            projectile_3.speed = bullet_Controller.stats.speed;
            projectile_3.damage = bullet_Controller.stats.damage;
            projectile_3.force = bullet_Controller.stats.force;
            projectile_3.lifetime = bullet_Controller.stats.lifetime;
            projectile_3.defaultImpactEffect = bullet_Controller.stats.defaultImpactEffect;

            PlayFireEffects();
        }
    }

    void RecoilSlider()
    {
        if (slideControl != null)
        {
            slideControl.SetPosition(1);
        }
    }

    void PlayFireEffects()
    {
        if (muzzle_flash != null && muzzle_flash.GetComponent<ParticleSystem>() != null)
        {
            muzzle_flash.GetComponent<ParticleSystem>().Play();
        }
    }

    void MoveWeaponTrigger()
    {
        if (controllerEvents != null)
        {
            var pressure = (maxTriggerRotation * controllerEvents.GetTriggerAxis()) - minTriggerRotation;
            trigger.localEulerAngles = new Vector3(pressure, 0f, 0f);
        }
        else
        {
            trigger.localEulerAngles = new Vector3(minTriggerRotation, 0f, 0f);
        }
    }

    void ChamberRound()
    {
        if (pauseReChamber)
            return;

        pauseReChamber = true;

        GameObject mag = GetInsertedMag();

        if (mag != null)
        {
            Weapon_Magazine weapon_Magazine = mag.GetComponent<Weapon_Magazine>();
            if (weapon_Magazine != null)
            {
                GameObject round = weapon_Magazine.GetTopBullet();
                if (round != null)
                {
                    Bullet_Controller bullet_Controller = round.GetComponent<Bullet_Controller>();
                    if (bullet_Controller != null)
                    {
                        bullet_Controller.TogglePhysics(false);
                        bullet_Controller.RemoveRigidbody();
                        bullet_Controller.isGrabbable = false;
                        bullet_Controller.isLoadedInMag = false;
                    }

                    round.transform.parent = loadPoint.transform;
                    round.transform.position = loadPoint.transform.position;
                    round.transform.rotation = loadPoint.transform.rotation;

                }
            }
        }
    }

    void UnChamberRound()
    {
        GameObject round = GetChamberedRound();

        if (round != null)
        {
            round.transform.parent = null;
            round.transform.position = casingPoint.transform.position;
            round.transform.rotation = casingPoint.transform.rotation;

            Bullet_Controller bullet_Controller = round.GetComponent<Bullet_Controller>();
            if (bullet_Controller != null)
            {
                bullet_Controller.isGrabbable = true;
                bullet_Controller.isLoadedInMag = false;
                bullet_Controller.AddRigidbody();
                bullet_Controller.AddForceToBullet(Vector3.right, 100f);
                bullet_Controller.TogglePhysics(true);
            }

        }
    }

    bool IsChamberEmpty()
    {
        return (loadPoint.transform.childCount == 0);
    }

    bool IsMagCompatible(int id)
    {
        return (compatibleMagazineID == id);
    }

    bool IsRoundUnfired()
    {
        if (!IsChamberEmpty())
        {
            Bullet_Controller bullet_Controller = GetChamberedRound().GetComponent<Bullet_Controller>();
            if (bullet_Controller != null)
            {
                return !bullet_Controller.hasFired;
            }
        }

        return false;
    }

}
