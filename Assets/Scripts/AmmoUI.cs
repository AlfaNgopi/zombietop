using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections;


public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public WeaponScript weapon;

    private float reloadTime;

    private Coroutine reloadDisplayRoutine;

    void Start()
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon reference not set in AmmoUI!");
        }
        ammoText = GetComponent<TextMeshProUGUI>();
        if (ammoText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on AmmoUI object!");
        }

        reloadTime = weapon.getReloadTime();

        UpdateText(); // Initial update to show starting ammo


    }

    void OnEnable() { WeaponScript.OnAmmoChanged += UpdateText; }
    void OnDisable() { WeaponScript.OnAmmoChanged -= UpdateText; }

    void UpdateText()
    {

        if (weapon == null || ammoText == null)
        {
            ammoText.text = "0";
            return;
        }



        // Stop any existing dot animation
        if (reloadDisplayRoutine != null) StopCoroutine(reloadDisplayRoutine);

        if (weapon.isCurrentlyReloading())
        {
            // Start the dot animation only when reloading
            reloadDisplayRoutine = StartCoroutine(AnimateReloadDots());
            return; // Don't update ammo count while showing dots
        }
        


        ammoText.text = weapon.getAmmo().ToString(); // Update the text with current ammo
    }

    IEnumerator AnimateReloadDots()
    {
        float timer = 0;
        while (weapon.isCurrentlyReloading())
        {
            timer += Time.deltaTime;
            int dotCount = (int)((timer / reloadTime) * 10);
            ammoText.text = new string('.', dotCount);
            yield return null; // Wait for next frame
        }
    }

}