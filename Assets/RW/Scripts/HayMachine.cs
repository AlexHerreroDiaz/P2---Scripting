using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HayMachine : MonoBehaviour
{
    
    
    public float movementSpeed;
    public float horizontalBoundary = 22;

    public GameObject hayBalePrefab; // 1
    public GameObject specialHayBalePrefab;
    public Transform haySpawnpoint; // 2
    public float shootInterval; // 3
    private float shootTimer; // 4

    public Transform modelParent; // 1

    // 2
    public GameObject blueModelPrefab;
    public GameObject yellowModelPrefab;
    public GameObject redModelPrefab;

    public Text specialShootText; // Reference to the UI Text element

    public bool isSpecialShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadModel();
    }

    private void LoadModel()
    {
        Destroy(modelParent.GetChild(0).gameObject); // 1

        switch (GameSettings.hayMachineColor) // 2
        {
            case HayMachineColor.Blue:
                Instantiate(blueModelPrefab, modelParent);
            break;

            case HayMachineColor.Yellow:
                Instantiate(yellowModelPrefab, modelParent);
            break;

            case HayMachineColor.Red:
                Instantiate(redModelPrefab, modelParent);
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement ();
        UpdateShooting();

        if (specialShootText != null)
        {
            specialShootText.text = isSpecialShoot ? "Active" : "Not Active";
        }
    }

    private void UpdateMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // 1

        if (horizontalInput < 0 && transform.position.x > -horizontalBoundary) // 1
        {
            transform.Translate(transform.right * -movementSpeed * Time.deltaTime);
        }
        else if (horizontalInput > 0 && transform.position.x < horizontalBoundary) // 2
        {
            transform.Translate(transform.right * movementSpeed * Time.deltaTime);
        }
    }

    private void ShootHay()
    {

        if (isSpecialShoot) // Check if the next shoot is special
        {
            // Instantiate special hay bale covering the entire width of the field
            Instantiate(specialHayBalePrefab, haySpawnpoint.position, Quaternion.identity);
            isSpecialShoot = false; // Reset to normal shoot
        }
        else
        {
            Instantiate(hayBalePrefab, haySpawnpoint.position, Quaternion.identity);
        }
        SoundManager.Instance.PlayShootClip();
    }

    private void UpdateShooting()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0 && Input.GetKey(KeyCode.Space))
        {
            shootTimer = shootInterval;
            ShootHay(); // Normal shoot
        }
    }

}
