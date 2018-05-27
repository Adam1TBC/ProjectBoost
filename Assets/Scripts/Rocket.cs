using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    // Speed multiplyer
    [SerializeField] float mainTrust = 100f;
    [SerializeField] float rcsTrust = 100f; // Rotation
    [SerializeField] float levelLoadDelay = 1f;
    //Audio
    [SerializeField] AudioClip mainEngineClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip successClip;
    //Particle system
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    
    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                state = State.Transcending;

                audioSource.Stop();
                audioSource.PlayOneShot(successClip);

                successParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                state = State.Dying;

                audioSource.Stop();
                audioSource.PlayOneShot(deathClip);

                deathParticles.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;
        }
    }

    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void LoadNextScene()
    {
        // TODO: allow for more than 2 levels
        SceneManager.LoadScene(1);
    }

    void RespondToRotateInput() {

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

    void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) // Can thust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop(); // If the thuster doesn't work we stop audio
            mainEngineParticles.Stop();
        }
    }

    void ApplyThrust() {
        float forceThisFrame = mainTrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * forceThisFrame);

        if (!audioSource.isPlaying) // When it doesn't layer
        {
            audioSource.PlayOneShot(mainEngineClip);
        }

        mainEngineParticles.Play();
    }
}
