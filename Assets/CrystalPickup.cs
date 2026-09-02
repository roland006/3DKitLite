using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    [Header("Подбор")]
    [SerializeField] private int amount = 1;

    [Header("VFX")]
    [SerializeField] private GameObject pickupVFX;

    [Header("Движение кристалла")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatHeight = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    private bool collected = false;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вращение
        transform.Rotate(
            Vector3.up * rotationSpeed * Time.deltaTime,
            Space.World
        );

        // Плавное парение вверх-вниз
        float newY =
            startPosition.y +
            Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        CrystalWallet wallet =
            other.GetComponentInParent<CrystalWallet>();

        if (wallet == null)
            return;

        collected = true;

        // Добавляем кристалл в счётчик
        wallet.AddCrystals(amount);

        // Создаём эффект
        if (pickupVFX != null)
        {
            Instantiate(
                pickupVFX,
                transform.position,
                Quaternion.identity
            );
        }

        // Удаляем кристалл
        Destroy(gameObject);
    }
}