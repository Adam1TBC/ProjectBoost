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

    bool isTransitioning = false;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (isTransitioning)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                isTransitioning = true;

                audioSource.Stop();
                audioSource.PlayOneShot(successClip);

                successParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                isTransitioning = true;

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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
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
