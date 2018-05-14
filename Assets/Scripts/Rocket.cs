using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    // Speed multiplyer
    [SerializeField] float mainTrust = 100f;
    [SerializeField] float rcsTrust = 100f; // Rotation

    Rigidbody rigidBody;
    AudioSource audioSource;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    void ProcessInput()
    {
        Trust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            default:
                print("Dead");
                break;
        }
    }

    void Rotate() {

        rigidBody.freezeRotation = true; // take manual control of rotation
        
        float rotationThisFrame = rcsTrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physcis control of the rotation
    }

    void Trust() {
        if (Input.GetKey(KeyCode.Space)) // Can thust while rotating
        {
            float forceThisFrame = mainTrust * Time.deltaTime;

            rigidBody.AddRelativeForce(Vector3.up * forceThisFrame);

            if (!audioSource.isPlaying) // When it doesn't layer
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop(); // If the thuster doesn't work we stop audio
        }
    }
}
