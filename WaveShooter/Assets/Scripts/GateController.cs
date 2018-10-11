using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GateController : MonoBehaviour {

    [HideInInspector]
    public bool GateDestoryed = false;

    public bool rebuildGate;
    public int TeamID=1;
    public float GateHealth = 1000f;
    public GameObject GatePrefab;
    public GameObject GateDestoryedEffect;
    public Canvas healthBar;
    public float healthBarOffset;

    [HideInInspector]
    public float currentHealth = 0f;

    Collider col;
    Slider bar;

    private void Awake()
    {
        currentHealth = GateHealth;
        col = GetComponent<Collider>();

        if (healthBar != null)
        {
            Vector3 hbPos = new Vector3(transform.position.x, transform.position.y + healthBarOffset, transform.position.z);
            Canvas hb = Instantiate(healthBar, hbPos, transform.rotation, transform);
            healthBar = hb;
            bar = healthBar.transform.Find("Slider").GetComponent<Slider>();
            bar.maxValue = GateHealth;
            bar.value = currentHealth;
        }


    }

    public void UpdateHealthBar()
    {
        if (bar != null)
        {
            bar.value = currentHealth;
        }
    }

    public void DamageGate(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        DestoryGate();
    }

    public void RebuildGate()
    {
        GateDestoryed = false;
        currentHealth = GateHealth;

        UpdateHealthBar();

        if (GatePrefab != null)
        {
            GatePrefab.SetActive(true);
        }

        if (col != null)
        {
            col.enabled = true;
        }
    }

    public void DestoryGate()
    {
        if (currentHealth > 0 || GateDestoryed)
            return;

        GateDestoryed = true;

        if (GatePrefab != null)
        {
            GatePrefab.SetActive(false);
        }

        if (col != null)
        {
            col.enabled = false;
        }

        if (rebuildGate)
        {
            StartCoroutine(RebuildTimer(30f));
        }
    }

    IEnumerator RebuildTimer(float time)
    {
        yield return new WaitForSeconds(time);
        RebuildGate();
    }

}
