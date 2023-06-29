using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class angleControl : MonoBehaviour
{
    public GameObject slopeObject; // Objekts, kas attēlo nogāzi (trijstūri)
    public GameObject rectangleObject; // Objekts, kas attēlo taisnstūri
    public Button button30Degrees;
    public Button button60Degrees;
    public Button button75Degrees;
    public InputField massInputField;
    public InputField frictionInputField;
    public Button confirmButton;
    public Button startButton; // Poga "Start"
    public Text slidingFrictionField; // Lauks, kurā tiek attēlota slīdēšanas berzes spēka vērtība
    public Button restartButton;

    private float slopeAngle = 0f; // Pašreizējais nogāzes leņķis
    private Vector3 initialScale; // Sākotnējais trijstūra mērogs
    private float mass = 0f; // Taisnstūra masa
    private float frictionCoefficient = 0f; // Slīdēšanas berzes koeficients

    private Rigidbody2D rectangleRigidbody; // Rigidbody2D komponents taisnstūram
    private bool isFalling = false; // Zīme, kas norāda, vai taisnstūris kritīs

    private Dictionary<Button, float> rotationValues = new Dictionary<Button, float>(); // Vārdnīca rotācijas Z vērtībām
    private Dictionary<Button, Vector3> positionValues = new Dictionary<Button, Vector3>(); // Vārdnīca pozīcijas vērtībām

    private void Start()
    {
        // Saglabā sākotnējo trijstūra mērogu
        initialScale = slopeObject.transform.localScale;

        // Piešķir funkcijas pogu nospiestes notikuma apstrādei
        button30Degrees.onClick.AddListener(SetSlopeAngleToInitial);
        button60Degrees.onClick.AddListener(DecreaseScaleXByHalf);
        button75Degrees.onClick.AddListener(SetSlopeAngle75Degrees);

        // Piešķir funkcijas pogu nospiestes notikuma apstrādei, lai mainītu rotāciju Z un pozīciju
        button30Degrees.onClick.AddListener(() => SetRotationAndPositionValues(210.531f, new Vector3(-1.12820f, -0.61735f, 0f)));
        button60Degrees.onClick.AddListener(() => SetRotationAndPositionValues(409.74f, new Vector3(-0.91321f, -0.82840f, 0f)));
        button75Degrees.onClick.AddListener(() => SetRotationAndPositionValues(55.834f, new Vector3(-0.88560f, -0.94280f, 0f)));

        // Piešķir funkciju pogai "Apstiprināt" nospiestes notikuma apstrādei
        confirmButton.onClick.AddListener(ConfirmValues);

        // Piešķir funkciju pogai "Start" nospiestes notikuma apstrādei
        startButton.onClick.AddListener(StartFalling);

        restartButton.onClick.AddListener(RestartScene);

        // Iegūst taisnstūra Rigidbody2D komponentu
        rectangleRigidbody = rectangleObject.GetComponent<Rigidbody2D>();

        // Ierobežo taisnstūrim fizikālo kustību
        if (rectangleRigidbody != null)
        {
            rectangleRigidbody.isKinematic = true;
        }
    }

    private void UpdateSlopeVisual()
    {
        // Maina nogāzes vizuālo leņķi, mainot objekta mērogu pa X asi
        // Skalē trijstūri no sākotnējā mēroga
        slopeObject.transform.localScale = new Vector3(initialScale.x * Mathf.Tan(slopeAngle * Mathf.Deg2Rad), initialScale.y, initialScale.z);
    }

    private void SetSlopeAngleToInitial()
    {
        slopeAngle = 0f;
        slopeObject.transform.localScale = initialScale;
    }

    private void DecreaseScaleXByHalf()
    {
        slopeAngle = 60f;
        slopeObject.transform.localScale = new Vector3(initialScale.x * 0.5f, initialScale.y, initialScale.z);
    }

    private void SetSlopeAngle75Degrees()
    {
        slopeAngle = 75f;
        slopeObject.transform.localScale = new Vector3(initialScale.x * 0.4f, initialScale.y, initialScale.z);
    }

    private void SetRotationAndPositionValues(float rotationValue, Vector3 positionValue)
    {
        SetRotationValue(rotationValue);
        SetPositionValue(positionValue);
    }

    private void SetRotationValue(float rotationValue)
    {
        if (rectangleObject != null)
        {
            rectangleObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
        }
    }

    private void SetPositionValue(Vector3 positionValue)
    {
        if (rectangleObject != null)
        {
            rectangleObject.transform.position = positionValue;
        }
    }

    private void ConfirmValues()
    {
        // Iegūst masas un slīdēšanas berzes koeficienta vērtības no ievades laukiem
        float.TryParse(massInputField.text, out mass);
        float.TryParse(frictionInputField.text, out frictionCoefficient);

        // Pārbauda, vai taisnstūram ir Rigidbody2D komponents
        if (rectangleRigidbody != null)
        {
            // Piešķir masas vērtību taisnstūrim
            rectangleRigidbody.mass = mass;

            // Iegūst taisnstūra Collider2D komponentu
            Collider2D rectangleCollider = rectangleObject.GetComponent<Collider2D>();
            if (rectangleCollider != null)
            {
                // Izveido jaunu PhysicsMaterial2D un piešķir to taisnstūrim
                PhysicsMaterial2D rectangleMaterial = new PhysicsMaterial2D();
                rectangleMaterial.friction = frictionCoefficient;
                rectangleCollider.sharedMaterial = rectangleMaterial;
            }
            else
            {
                Debug.LogError("Collider2D nav atrasts.");
            }
        }
        else
        {
            Debug.LogError("Rigidbody2D nav atrasts.");
        }

        // Papildu darbības, kas saistītas ar vērtību apstiprināšanu
        // ...

        // Izvada vērtības konsolē pārbaudei
        Debug.Log("Vērtības - Masa: " + mass + ", Berzes koeficients: " + frictionCoefficient);
    }

    private void StartFalling()
    {
        if (rectangleRigidbody != null && !isFalling)
        {
            rectangleRigidbody.isKinematic = false;
            isFalling = true;
        }
    }

    private void FixedUpdate()
    {
        if (rectangleRigidbody != null && isFalling)
        {
            // Iegūst pašreizējo taisnstūra ātrumu
            Vector2 velocity = rectangleRigidbody.velocity;

            // Aprēķina slīdēšanas berzes spēku
            float slidingFriction = frictionCoefficient * mass * 9.8f * Mathf.Cos(slopeAngle * Mathf.Deg2Rad);

            // Ievada slīdēšanas berzes spēka vērtību ievades laukā
            slidingFrictionField.text = slidingFriction.ToString();

            // Piemēro slīdēšanas berzes spēku taisnstūrim
            Vector2 frictionForce = -velocity.normalized * slidingFriction;
            rectangleRigidbody.AddForce(frictionForce);
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
