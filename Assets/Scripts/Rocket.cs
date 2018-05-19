using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    // Speed multiplyer
    [SerializeField] float mainTrust = 100f;
    [SerializeField] float rcsTrust = 100f; // Rotation

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
        ProcessInput();
    }

    void ProcessInput() {
        if (state == State.Alive)
        {
            Trust();
            Rotate();
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
                Invoke("LoadNextScene", 1f);
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
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
