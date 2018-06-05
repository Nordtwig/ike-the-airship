using UnityEngine;

public class Airship : MonoBehaviour {

    [SerializeField]
    float lateralThrust, mainThrust;

    Rigidbody rigidBody;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		Thrust ();
		Rotate ();
	}

    void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Friendly":
                print("Safe");
                break;
            case "Fuel":
                print("Topped Up");
                break;
            default:
                print("Dead");
                //kill player
                break;
        }

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
