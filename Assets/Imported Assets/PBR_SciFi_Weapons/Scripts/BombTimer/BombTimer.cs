using UnityEngine;
using TMPro;

public class BombTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeRemaining = 60;
    public TMP_Text timerText;
    
    [Header("Warning Settings")]
    public float warningThreshold = 5f;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float blinkRate = 0.5f;
    
    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab;
    public GameObject detonatorObject;
    public bool destroyDetonator = false;
    public Vector3 explosionOffset = Vector3.zero;
    
    [Header("Audio Prefabs Settings")]
    public GameObject tickSoundPrefab;
    public GameObject finalTickSoundPrefab;
    public GameObject explosionSoundPrefab;
    public float tickInterval = 1f; 
    
    private bool isWarning = false;
    private bool hasExploded = false;
    private float blinkTimer = 0f;
    private bool blinkState = false;
    private float lastTickTime = 0f;
    
    private GameObject currentTickInstance;
    private GameObject explosionSoundInstance;

    void Start()
    {
        if (explosionSoundPrefab != null)
        {
            explosionSoundInstance = Instantiate(explosionSoundPrefab, transform.position, Quaternion.identity);
            explosionSoundInstance.transform.SetParent(transform);
            explosionSoundInstance.SetActive(false);
        }
        
        if (timerText != null)
        {
            timerText.color = normalColor;
        }
        else
        {
            Debug.LogWarning("TextMesh не назначен для компонента BombTimer!");
        }
    }

    void Update()
    {
        if (timeRemaining > 0 && !hasExploded)
        {
            timeRemaining -= Time.deltaTime;
            
            if (Time.time - lastTickTime >= tickInterval)
            {
                PlayTickSound();
                lastTickTime = Time.time;
            }
            
            if (timeRemaining <= warningThreshold && !isWarning)
            {
                isWarning = true;
            }
            
            if (isWarning && timerText != null)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= blinkRate)
                {
                    blinkState = !blinkState;
                    timerText.color = blinkState ? warningColor : normalColor;
                    blinkTimer = 0f;
                }
            }
            
            DisplayTime(timeRemaining);
        }
        else if (!hasExploded)
        {
            timeRemaining = 0;
            DisplayTime(0);
            
            Explode();
            hasExploded = true;
        }
    }

    void PlayTickSound()
    {
        if (currentTickInstance != null)
        {
            Destroy(currentTickInstance);
            currentTickInstance = null;
        }
        
        GameObject soundPrefabToUse = (timeRemaining <= warningThreshold && finalTickSoundPrefab != null) ? 
                                     finalTickSoundPrefab : tickSoundPrefab;
        
        if (soundPrefabToUse != null)
        {
            currentTickInstance = Instantiate(soundPrefabToUse, transform.position, Quaternion.identity);
            currentTickInstance.transform.SetParent(transform); 
            
            Destroy(currentTickInstance, tickInterval);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timerText == null) return;
        
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    
    void Explode()
    {
        if (explosionSoundInstance != null)
        {
            explosionSoundInstance.SetActive(true);
            
            AudioSource explosionAudio = explosionSoundInstance.GetComponent<AudioSource>();
            if (explosionAudio != null && !explosionAudio.playOnAwake)
            {
                explosionAudio.Play();
            }
            
            if (explosionAudio != null && explosionAudio.clip != null)
            {
                float clipLength = explosionAudio.clip.length;
                Destroy(explosionSoundInstance, clipLength + 0.5f);
            }
            else
            {
                  Destroy(explosionSoundInstance, 5f);
            }
        }
        
        if (explosionEffectPrefab != null)
        {
            Vector3 explosionPosition = transform.position + explosionOffset;
            GameObject explosionInstance = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);
            
            ParticleSystem ps = explosionInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if(!explosionInstance.GetComponent<ParticleSystemDestroyer>())
                {
                    explosionInstance.AddComponent<ParticleSystemDestroyer>();
                }
            }
        }
        
        if (detonatorObject != null)
        {
            if (destroyDetonator)
            {
                Destroy(detonatorObject);
            }
            else
            {
                detonatorObject.SetActive(false);
            }
        }
        
        if (timerText != null && !destroyDetonator)
        {
            timerText.gameObject.SetActive(false);
        }
    }
    
    public void ResetTimer(float newTime = -1)
    {
        hasExploded = false;
        isWarning = false;
        
        if (newTime > 0)
            timeRemaining = newTime;
        else
            timeRemaining = 60;
            
        if (timerText != null)
        {
            timerText.color = normalColor;
            timerText.gameObject.SetActive(true);
        }
        
        if (explosionSoundPrefab != null && (explosionSoundInstance == null || !explosionSoundInstance.activeInHierarchy))
        {
            if (explosionSoundInstance != null)
            {
                Destroy(explosionSoundInstance);
            }
            
            explosionSoundInstance = Instantiate(explosionSoundPrefab, transform.position, Quaternion.identity);
            explosionSoundInstance.transform.SetParent(transform);
            explosionSoundInstance.SetActive(false);
        }
        
        DisplayTime(timeRemaining);
    }
}

public class ParticleSystemDestroyer : MonoBehaviour
{
    private ParticleSystem ps;
    
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        if (ps != null && !ps.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}