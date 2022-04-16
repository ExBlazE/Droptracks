using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    private float speed;

    [Space]

    [SerializeField] Material preHitMaterial;
    [SerializeField] Material postHitMaterial;
    private Material targetMaterial;

    private GameManager gameManager;
    [System.NonSerialized] public int trackNumber;

    void Start()
    {
        targetMaterial = GetComponent<Renderer>().material;
        targetMaterial.color = preHitMaterial.color;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.z > 12)
        {
            gameManager.targetInTrack[trackNumber] = false;
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        targetMaterial.color = postHitMaterial.color;
        if (gameManager.GetGameActive())
        {
            gameManager.AddScore(1);
            gameManager.BlinkScore();
        }
    }
}
