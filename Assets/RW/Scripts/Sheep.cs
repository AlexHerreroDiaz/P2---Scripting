using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float runSpeed; // 1
    public float gotHayDestroyDelay; // 2
    private bool hitByHay; // 3
    public float dropDestroyDelay; // 1
    private Collider myCollider; // 2
    private Rigidbody myRigidbody; // 3

    private SheepSpawner sheepSpawner;

    public float heartOffset; // 1
    public GameObject heartPrefab; // 2

    public GameObject hayMachine;

    public Renderer myRenderer;
    public bool isSpecialSheep = false; 
    public Material goldenMaterial; // Reference to the golden material

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();

        isSpecialSheep = Random.Range(0, 100) < 10; // we have a 20% chance of being a special sheep
        hayMachine = GameObject.Find("Hay Machine");

        if (isSpecialSheep)
        {
            runSpeed = 15;
            if (myRenderer != null && goldenMaterial != null)
            {
                myRenderer.material = goldenMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }

    private void HitByHay()
    {
        sheepSpawner.RemoveSheepFromList(gameObject);

        hitByHay = true; // 1
        runSpeed = 0; // 2

        Destroy(gameObject, gotHayDestroyDelay); // 3

        if (isSpecialSheep && hayMachine != null)
        {
            // Access the HayMachine script and set isSpecialShoot to true
            hayMachine.GetComponent<HayMachine>().isSpecialShoot = true;
        }

        Instantiate(heartPrefab, transform.position + new Vector3(0, heartOffset, 0), Quaternion.identity);
        
        TweenScale tweenScale = gameObject.AddComponent<TweenScale>();; // 1
        tweenScale.targetScale = 0; // 2
        tweenScale.timeToReachTarget = gotHayDestroyDelay; // 3
        
        SoundManager.Instance.PlaySheepHitClip();

        GameStateManager.Instance.SavedSheep();
    }

    private void OnTriggerEnter(Collider other) // 1
    {
        if (other.CompareTag("Special Hay") && !hitByHay) // 2
        {
            //Destroy(other.gameObject); // 3
            HitByHay(); // 4
        }
        if (other.CompareTag("Hay") && !hitByHay) // 2
        {
            Destroy(other.gameObject); // 3
            HitByHay(); // 4
        }
        else if (other.CompareTag("DropSheep"))
        {
            Drop();
        }
    }
    private void Drop()
    {
        GameStateManager.Instance.DroppedSheep();

        sheepSpawner.RemoveSheepFromList(gameObject);

        myRigidbody.isKinematic = false; // 1
        myCollider.isTrigger = false; // 2
        Destroy(gameObject, dropDestroyDelay); // 3

        SoundManager.Instance.PlaySheepDroppedClip();
    }

    public void SetSpawner(SheepSpawner spawner)
    {
        sheepSpawner = spawner;
    }
}
