using UnityEngine;
using UnityEngine.SceneManagement;

public class Airship : MonoBehaviour {

    //TODO Fix lightning bug

    [SerializeField]
    float lateralThrust, mainThrust;

    Rigidbody rigidBody;
	AudioSource audioSource;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    // Use this for initialization
    void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive) {
            Thrust();
            Rotate();
        }
	}

    void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Friendly":
                state = State.Alive;
                //Do nothing
                break;
            case "Finish":
                /*if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    SceneManager.LoadScene(1);
                }
                else if (SceneManager.GetActiveScene().buildIndex == 1) {
                    SceneManager.LoadScene(1);
                }*/
                state = State.Transcending;
                Invoke("LoadNextLevel", 1f); //parameterize this time
                break;
            default:
                print("Dead");
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }

    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); //TODO allow for more levels
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);  
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space))
        {
            //Can thrust while rotating
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
            {
                //So it doesn't layer sound
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate () {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = lateralThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate (Vector3.forward * rotationThisFrame);
		} else if (Input.GetKey(KeyCode.D)) {
			transform.Rotate (-Vector3.forward * rotationThisFrame);
		}

        rigidBody.freezeRotation = false;
    }
}
