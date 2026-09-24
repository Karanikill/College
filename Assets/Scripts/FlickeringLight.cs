using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Компоненты")]
    [Tooltip("Источник света (Area Light / Point Light / Spot Light)")]
    public Light myLight;

    [Tooltip("3D-модель трубок/лампы с материалом свечения")]
    public Renderer bulbRenderer;

    [Header("Индекс материала")]
    [Tooltip("Если на меше 1 материал — укажи 0. Если несколько — укажи номер материала трубок.")]
    public int materialIndex = 0;

    [Header("Настройки яркости света (Light)")]
    public float minLightIntensity = 0.05f;
    public float maxLightIntensity = 10f;

    [Header("Настройки Emission (Светящийся стеклянный корпус)")]
    [Tooltip("Цвет свечения колбы")]
    public Color emissionColor = Color.white;

    [Tooltip("Экстремальная яркость для Bloom (по умолчанию 15)")]
    public float maxEmissionIntensity = 15f;

    [Header("Скорость мерцания")]
    public float minWaitTime = 0.02f;
    public float maxWaitTime = 0.15f;

    private Material targetMaterial;
    private float timer;

    void Awake()
    {
        // Инициализация источника света
        if (myLight == null)
            myLight = GetComponent<Light>();

        // Настройка материала
        InitMaterial();
    }

    void InitMaterial()
    {
        if (bulbRenderer != null)
        {
            Material[] mats = bulbRenderer.materials; // Берем копию массива материалов
            if (materialIndex < mats.Length)
            {
                targetMaterial = mats[materialIndex];

                // Включаем ключевые слова для URP Lit шейдера
                targetMaterial.EnableKeyword("_EMISSION");
                targetMaterial.EnableKeyword("_EMISSION_COLOR");
                targetMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Случайный фактор от 0 до 1
            float randomFactor = Random.value;

            // 1. Меняем яркость источника света
            if (myLight != null)
            {
                myLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, randomFactor);
            }

            // 2. Меняем HDR-яркость Emission материала
            if (targetMaterial != null)
            {
                if (randomFactor <= 0.05f)
                {
                    // В момент тушения — полностью убираем цвет
                    targetMaterial.SetColor("_EmissionColor", Color.black);
                }
                else
                {
                    // Вычисляем супер-яркий HDR цвет за счет высокого множителя
                    float currentEmissionIntensity = randomFactor * maxEmissionIntensity;
                    Color finalHdrColor = emissionColor * Mathf.Pow(2f, currentEmissionIntensity);

                    targetMaterial.SetColor("_EmissionColor", finalHdrColor);
                }
            }

            // Перезапуск таймера
            timer = Random.Range(minWaitTime, maxWaitTime);
        }
    }
}