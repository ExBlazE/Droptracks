using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    [System.NonSerialized] public int trackNumber;
    private GameManager gameManager;

    private Rigidbody ballRb;
    private bool ballHit = false;
    private float vanishTime = 0.1f;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ballRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ballHit)
        {
            ballRb.velocity = Vector3.zero;

            float newScale = Time.deltaTime / vanishTime;
            transform.localScale -= Vector3.one * newScale;

            if (transform.localScale.x <= 0)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Target target = other.GetComponent<Target>();
            target.Hit();
            gameManager.ballInTrack[trackNumber] = false;
            ballHit = true;
        }
        else if (other.CompareTag("Ground"))
        {
            gameManager.ballInTrack[trackNumber] = false;
            ballHit = true;
        }
    }
}
