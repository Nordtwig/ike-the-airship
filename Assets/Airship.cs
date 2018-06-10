using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Airship : MonoBehaviour {

    [Header("Variables")]
    [SerializeField] float lateralThrust = 200f;
    [SerializeField] float mainThrust = 20f;
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] float cameraDistance = 30f;
    [SerializeField] float cameraScrollSpeed = 5;

    [Header("Audio")]
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loadSound;

    [Header("Particles")]
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    [Header("Components")]
    [SerializeField] Camera airshipCamera;

    bool collisionsDisabled = false;

    
    Rigidbody rigidBody;
	AudioSource audioSource;
   

    enum State {Alive, Dying, Transcending, Debug};
    State state;

    // Use this for initialization
    void Start () {
        state = State.Alive;
        rigidBody = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
        //TODO Somewhere stop sound on death
        airshipCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -cameraDistance);
        if (state == State.Alive || state == State.Debug) {
            RespondToThrustInput();
            RespondToRotateInput();
            RespondToZoomInput();
        }
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
        
        
	}

    private void RespondToZoomInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            if (Input.GetAxis("Mouse ScrollWheel") >= 0)
            {
                cameraDistance = cameraDistance - cameraScrollSpeed;
            }
            else {
                cameraDistance = cameraDistance + cameraScrollSpeed;
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive || collisionsDisabled) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                StartWinSequence();
                break;
            default:
                print(collision.gameObject.tag);
                StartDeathSequence();
                break;
        }

    }

    private void StartWinSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //parameterize this time
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0; //loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);  
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space))
        { //Can thrust while rotating
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void RespondToRotateInput () {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = lateralThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate (Vector3.forward * rotationThisFrame);
		} else if (Input.GetKey(KeyCode.D)) {
			transform.Rotate (-Vector3.forward * rotationThisFrame);
		}

        rigidBody.freezeRotation = false;
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            //So it doesn't layer sound
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            print("Next Level Override!");
            LoadNextLevel();
        }
    }
}
