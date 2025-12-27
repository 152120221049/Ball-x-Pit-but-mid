using DG.Tweening;
using NUnit.Framework.Interfaces;
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
    public GameObject aimGuide;
    public Image nextItemImage;
    public TMP_Text stackCountText;
    public Image autoFireImg;// Reticle üzerindeki "x3" yazısı (Opsiyonel)

    [Header("Debug")]
    public float currentCooldown = 0f;

    // Hangi paketteyiz? (Slot sırası)
    private int currentBundleIndex = 0;

    // Paketin içindeki kaçıncı toptayız? (Yığın sırası)
    private int currentSubIndex = 0;

    private int movementFingerId = -1; // Joystick tutan parmak
    private int firingFingerId = -1;   // Ateş eden parmak
    public float aimLockTimer = 0.02f;
    void Start()
    {
        
        if (PlayerDataManager.Instance != null)
        {
           
            equippedBundles = new List<ItemStack>();
            foreach (var perk in PlayerDataManager.Instance.equippedPerks)
            {
                perk.OnGameStart();
            }

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
        if (aimLockTimer > 0)
            aimLockTimer -= Time.deltaTime;
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.currentControlMode == ControlMode.DualInput)
        {
            HandleTouchInput();
#if UNITY_EDITOR
            HandleMouseInput();
#endif
        }
    }
    public void TryFire()
    {
        if (currentCooldown <= 0)
        {
            FireNextItem();
        }
    }
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // -- DOKUNMA BAŞLADI --
                if (touch.phase == TouchPhase.Began)
                {
                    // UI / Joystick Kontrolü
                    if (IsPointerOverUIObject(touch.fingerId))
                    {
                        if (movementFingerId == -1)
                            movementFingerId = touch.fingerId;
                        if (reticleRenderer != null) reticleRenderer.enabled = false;
                    }
                    else
                    {
                        // YENİ KONTROL: Eğer Aim Kilidi aktifse, nişan almayı BAŞLATMA!
                        if (aimLockTimer <= 0f)
                        {
                            if (firingFingerId == -1)
                            {
                                firingFingerId = touch.fingerId;
                            }
                        }
                    }
                }

                // -- SÜRÜKLEME --
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (touch.fingerId == firingFingerId)
                    {
                        if (isAutoFireEnabled && currentCooldown <= 0)
                        {
                            FireNextItem();
                        }
                    }
                }

                // -- DOKUNMA BİTTİ --
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    // HAREKET PARMAĞI KALKTIYSA
                    if (touch.fingerId == movementFingerId)
                    {
                        movementFingerId = -1;

                        // YENİ: Joystick bırakıldığı an kısa bir süre Aim almayı yasakla!
                        // Bu, parmağı kaldırırken oluşan titremelerin ateş etmesini engeller.
                        aimLockTimer = 0.2f; // 0.2 saniyelik kilit (İsteğe göre artır/azalt)
                        if (reticleRenderer != null) reticleRenderer.enabled = false;
                    }

                    // ATEŞ PARMAĞI KALKTIYSA
                    else if (touch.fingerId == firingFingerId)
                    {
                        if (!isAutoFireEnabled && currentCooldown <= 0)
                        {
                            FireNextItem();
                        }
                        firingFingerId = -1;
                    }
                }
            }
        }
    }

    // --- 2. EDITOR / MOUSE MANTIĞI (Simulator İçin) ---
    void HandleMouseInput()
    {
        // 1. TIKLAMA BAŞLADI (DOWN)
        if (Input.GetMouseButtonDown(0))
        {
            // Eğer UI (Joystick) üzerindeysek -> HAREKET
            if (EventSystem.current.IsPointerOverGameObject())
            {
                movementFingerId = 99; // Mouse hareket kimliği
                if (reticleRenderer != null) reticleRenderer.enabled = false;
            }
            // Değilsek -> ATEŞ (Ama Kilit Süresi dolmuşsa!)
            else
            {
                // YENİ: Aim Kilidi aktifse ateşlemeye başlama
                if (aimLockTimer <= 0f)
                {
                    firingFingerId = 99; // Mouse ateş kimliği
                }
            }
        }

        // 2. TIKLAMA SÜRÜYOR (HELD)
        if (Input.GetMouseButton(0))
        {
            // Sadece "Ateş Eden Kimlik" biz isek ateş et
            if (firingFingerId == 99 && isAutoFireEnabled && currentCooldown <= 0)
            {
                FireNextItem();
            }
        }

        // 3. TIKLAMA BİTTİ (UP)
        if (Input.GetMouseButtonUp(0))
        {
            // Eğer HAREKET (Joystick) bırakıldıysa -> KİLİDİ DEVREYE SOK
            if (movementFingerId == 99)
            {
                aimLockTimer = 0.2f; // 0.2 saniye boyunca ateş edilemez
                if (reticleRenderer != null) reticleRenderer.enabled = false;
                movementFingerId = -1;
            }

            // Eğer ATEŞ bırakıldıysa -> ATEŞ ET (AutoFire kapalıysa)
            else if (firingFingerId == 99)
            {
                if (!isAutoFireEnabled && currentCooldown <= 0)
                {
                    FireNextItem();
                }
                firingFingerId = -1;
            }

            // Her ihtimale karşı ID'leri temizle
            if (movementFingerId == 99) movementFingerId = -1;
            if (firingFingerId == 99) firingFingerId = -1;
        }
    }


    public void FireNextItem()
    {
        if (equippedBundles.Count == 0) return;

       
        ItemStack currentStack = equippedBundles[currentBundleIndex];
        ItemData itemToFire = currentStack.itemData;

     
        
        GameObject projectile = Instantiate(itemToFire.projectilePrefab, firePoint.position, firePoint.rotation);
        if (itemToFire.shootSound != null)
        {
            AudioManager.Instance.PlayShootSound(itemToFire.shootSound);
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = firePoint.right * itemToFire.speed;

        ProjectileEffects projScript = projectile.GetComponent<ProjectileEffects>();
        if (projScript != null)
        {
            projScript.projectileData = itemToFire;
        }
        if (PlayerDataManager.Instance != null)
        {
            // Takılı olan tüm perkleri gez
            foreach (PerkBase perk in PlayerDataManager.Instance.equippedPerks)
            {
                // "Ben ateş ettim, yapacağın bir şey var mı?" diye sor
                perk.OnFire(this);
            }
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
        if (item == null) return;

        // 1. Zaten elimizde var mı? (Listeyi index ile geziyoruz)
        for (int i = 0; i < equippedBundles.Count; i++)
        {
            // ÖNEMLİ: Referans eşitliği yerine itemData eşitliğine bakıyoruz
            if (equippedBundles[i].itemData == item)
            {
                equippedBundles[i].amount += amount;

                Debug.Log($"MEVCUT EŞYA ARTTI: {item.itemName} | Yeni Miktar: {equippedBundles[i].amount}");

                // Eğer şu an elimizde tuttuğumuz eşya arttıysa, UI'ı hemen güncelle
                if (i == currentBundleIndex)
                {
                    UpdateVisuals();
                    // Efekt: Miktar yazısını büyütüp küçült (Juice)
                    if (stackCountText) stackCountText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
                }
                return;
            }
        }

        // 2. Yoksa listenin sonuna yeni paket ekle
        ItemStack newStack = new ItemStack();
        newStack.itemData = item;
        newStack.amount = amount;

        equippedBundles.Add(newStack);

        Debug.Log($"YENİ PAKET EKLENDİ: {item.itemName} (Sıraya Girdi)");

        // Eğer deste daha önce boştuysa, hemen bu yeni silahı elimize alalım
        if (equippedBundles.Count == 1)
        {
            currentBundleIndex = 0;
            currentSubIndex = 0;
        }

        UpdateVisuals();
    }
    void UpdateVisuals()
    {
        if (equippedBundles == null || equippedBundles.Count == 0) return;

        // 1. MODA GÖRE NİŞANGAH (RETICLE) GÖRÜNÜRLÜĞÜ
        if (reticleRenderer != null)
        {
            // Hibrit modda nişangah hep açık, Dual modda sadece ateş ederken/dokunurken
            if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.currentControlMode == ControlMode.HybridJoystick)
            {
                if (reticleRenderer != null && !reticleRenderer.enabled)
                {
                    aimGuide.SetActive(true);
                }
            }
            // Dual modda görünürlük HandleTouchInput içinden veya ateş anında yönetilir.
        }

        // 2. GÜNCEL EŞYA VERİSİ
        ItemStack currentStack = equippedBundles[currentBundleIndex];

        // İkonu Güncelle
        if (reticleRenderer != null)
        {
            reticleRenderer.sprite = currentStack.itemData.itemIcon;
        }

        // 3. MİKTAR YAZISI (x5, x10 vb.)
        if (stackCountText != null)
        {
            int remainingInStack = currentStack.amount - currentSubIndex;
            // 1'den büyükse miktar yaz, değilse temizle
            stackCountText.text = remainingInStack > 1 ? $"x{remainingInStack}" : "";
        }

        // 4. SIRADAKİ EŞYA GÖRSELİ (UI)
        int nextIndex = (currentBundleIndex + 1) % equippedBundles.Count;
        if (nextItemImage != null)
        {
            nextItemImage.sprite = equippedBundles[nextIndex].itemData.itemIcon;
        }
    }

    private bool IsPointerOverUIObject(int fingerId)
    {
        // EventSystem, belirli bir parmak ID'sinin UI üzerinde olup olmadığını kontrol eder
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        // Parmağın o anki pozisyonunu bul
        foreach (Touch t in Input.touches)
        {
            if (t.fingerId == fingerId)
            {
                eventDataCurrentPosition.position = t.position;
                break;
            }
        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
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