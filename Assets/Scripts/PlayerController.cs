using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float[] hitForce;
    public float timeBetweenHits = 2.5f;
    public float moveSpeed = 50.0f;
    public Rigidbody head;
    public LayerMask layerMask;
    public Camera cam;
    public Animator bodyAnimator;
    public Rigidbody marineBody;

    DeathParticles deathParticles;
    Vector3 currentLookTarget = Vector3.zero;
    CharacterController characterController;
    bool isHits = false;
    bool isDead = false;
    float timeSinceHit = 0;
    int hitNumber = -1;

    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();
        deathParticles = gameObject.GetComponentInChildren<DeathParticles>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.SimpleMove(moveDirection * moveSpeed);
        if (isHits)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > timeBetweenHits)
            {
                isHits = false;
                timeSinceHit = 0;
            }
        }
    }
    private void FixedUpdate()
    {
        //variables
        Vector3 MoveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        
        //adding hinge joint to the marine head
        if (MoveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
        }
        else
        {
            head.AddForce(transform.right * 150, ForceMode.Acceleration);
            bodyAnimator.SetBool("IsMoving", true);
        }
           

        //showing the raycast 
        //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);

        //raycast make the player can turn with mouse

        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
            }
        }
        //1
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //2
        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
        //3
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);

    }
    private void OnTriggerEnter(Collider other)
    {
        Alien alien = other.gameObject.GetComponent<Alien>();
        if (alien != null)
        {
            if (!isHits)
            {
                hitNumber += 1;
                CameraShake cameraShake = cam.GetComponent<CameraShake>();
                if (hitNumber < hitForce.Length)
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                }
                else
                {
                    Die();
                }
                isHits = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
            }
            alien.Die();
        }
    }
    public void Die()
    {
        bodyAnimator.SetBool("IsMoving", false);
        marineBody.transform.parent = null;
        marineBody.isKinematic = false;
        marineBody.useGravity = true;
        marineBody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        marineBody.gameObject.GetComponent<Gun>().enabled = false;
        Destroy(head.gameObject.GetComponent<HingeJoint>());
        head.transform.parent = null;
        head.useGravity = true;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.marineDeath);
        deathParticles.Activate();
        Destroy(gameObject);

    }
}
