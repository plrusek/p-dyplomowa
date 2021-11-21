using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject PlayerToFollow;
    public float vertOffset;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var playerRigidBody = PlayerToFollow.GetComponent<Rigidbody>();
        var playerLookAhead = playerRigidBody.velocity.x / 1.5f;
        var playerJumpAhead = playerRigidBody.velocity.y / 1.5f;

        var followVector = new Vector3(PlayerToFollow.transform.position.x + playerLookAhead, PlayerToFollow.transform.position.y + vertOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, followVector, Time.fixedDeltaTime * 2f);

        var lookVector = (PlayerToFollow.transform.position + new Vector3(playerLookAhead, -Mathf.Max(playerJumpAhead, 0) - vertOffset, 0) - transform.position);
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, lookVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.fixedDeltaTime * 0.5f);
        //transform.LookAt(lookVector);
    }
}
