using System.Collections.Generic;
using TMPro; // Miktar yazısı için (Opsiyonel)
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    [Header("Deck (Bundle Sistemi)")]
    // ARTIK ITEMDATA DEĞİL, ITEMSTACK TUTUYORUZ
    public List<ItemStack> equippedBundles;

    [Header("Ayarlar")]
    public Transform firePoint;
    public Transform homeTransform;
    public bool isAutoFireEnabled = false;

    [Header("Görsel Referanslar")]
    public SpriteRenderer reticleRenderer;
    public Image nextItemImage;
    public TMP_Text stackCountText;
    public Image autoFireImg;// Reticle üzerindeki "x3" yazısı (Opsiyonel)

    [Header("Debug")]
    public float currentCooldown = 0f;

    // Hangi paketteyiz? (Slot sırası)
    private int currentBundleIndex = 0;

    // Paketin içindeki kaçıncı toptayız? (Yığın sırası)
    private int currentSubIndex = 0;

    private bool isAimingToFire = false;

    void Start()
    {
        
        if (PlayerDataManager.Instance != null)
        {
           
            equippedBundles = new List<ItemStack>();

           
            foreach (ItemStack originalStack in PlayerDataManager.Instance.currentDeck)
            {
                
                ItemStack copiedStack = new ItemStack(originalStack);

                equippedBundles.Add(copiedStack);
            }
        }

        UpdateVisuals();
        updateVisuals();

    }

    void Update()
    {
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;

        HandleShootingInput();
    }

    void HandleShootingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI()) isAimingToFire = true;
            else isAimingToFire = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isAutoFireEnabled && isAimingToFire && currentCooldown <= 0)
            {
                FireNextItem();
            }
            isAimingToFire = false;
        }

        if (isAutoFireEnabled && isAimingToFire && Input.GetMouseButton(0))
        {
            if (currentCooldown <= 0) FireNextItem();
        }
    }


    public void FireNextItem()
    {
        if (equippedBundles.Count == 0) return;

       
        ItemStack currentStack = equippedBundles[currentBundleIndex];
        ItemData itemToFire = currentStack.itemData;

     
        GameObject projectile = Instantiate(itemToFire.projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = firePoint.right * itemToFire.speed;

        ProjectileEffects projScript = projectile.GetComponent<ProjectileEffects>();
        if (projScript != null)
        {
            projScript.projectileData = itemToFire;
        }


        float distToHome = Vector3.Distance(transform.position, homeTransform.position);
        float statsCD = 0;
        if (PlayerDataManager.Instance != null)
            statsCD = PlayerDataManager.Instance.GetModifiedCooldown(itemToFire);
        else
            statsCD = itemToFire.baseCooldown;

        float baseCD = statsCD + (distToHome*0.1f);

        float reductionFactor = (LevelManager.Instance != null) ? LevelManager.Instance.cooldownReduction : 1f;
        currentCooldown = baseCD / reductionFactor;

       
        currentSubIndex++;

        
        if (currentSubIndex >= currentStack.amount)
        {
            currentSubIndex = 0; 
            currentBundleIndex++; 

           
            if (currentBundleIndex >= equippedBundles.Count)
            {
                currentBundleIndex = 0;
            }
        }

        Debug.Log($"Fırlatıldı: {itemToFire.itemName} | Kalan: {currentStack.amount - currentSubIndex}");

        UpdateVisuals();
    }
    public void AddTemporaryItem(ItemData item, int amount)
    {
        // Varsa üzerine ekle
        foreach (var stack in equippedBundles)
        {
            if (stack.itemData == item)
            {
                stack.amount += amount;
                UpdateVisuals();
                return;
            }
        }

        // Yoksa yeni paket aç
        ItemStack newStack = new ItemStack();
        newStack.itemData = item;
        newStack.amount = amount;

        equippedBundles.Add(newStack);
        UpdateVisuals();

        Debug.Log($"GEÇİCİ DESTEK: {item.itemName} x{amount} eklendi!");
    }
    void UpdateVisuals()
    {
        if (equippedBundles.Count == 0) return;

        ItemStack currentStack = equippedBundles[currentBundleIndex];

      
        if (reticleRenderer != null)
        {
            reticleRenderer.sprite = currentStack.itemData.itemIcon;
        }

       
        if (stackCountText != null)
        {
            int remainingInStack = currentStack.amount - currentSubIndex;
           
            stackCountText.text = remainingInStack > 1 ? $"x{remainingInStack}" : "";
        }

       
        int nextIndex = currentBundleIndex + 1;
        if (nextIndex >= equippedBundles.Count) nextIndex = 0;

        if (nextItemImage != null)
        {
            nextItemImage.sprite = equippedBundles[nextIndex].itemData.itemIcon;
        }
    }

    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void ToggleAutoFire()
    {
        isAutoFireEnabled = !isAutoFireEnabled;
        updateVisuals();
    }
    public void updateVisuals()
    {
        if (isAutoFireEnabled)
        {
            autoFireImg.color = Color.green;
        }
        else
        {
            autoFireImg.color = Color.red;
        }
    }
}