using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleController : MonoBehaviour
{
    public float vehicleSpeed = 1f;
    public Vector2 DefaultFacingDirection;

    [SerializeField]
    private Vector2 _vehicleLastMovement;
    public Vector2 VehicleLastMovement 
    {
        get {  return _vehicleLastMovement; }
        set 
        { 
            _vehicleLastMovement = value;
            animator.SetFloat("Horizontal", Mathf.Abs(_vehicleLastMovement.x));
            animator.SetFloat("Vertical", _vehicleLastMovement.y);
            SetCollision(_vehicleLastMovement);
        }
    }

    //public NetworkVariable<Vector2> VehicleLastMovement = new NetworkVariable<Vector2>(
    //    writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public Animator animator;
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private List<Collider2D> colliders;

    public bool IsFacingRight = true;

    //public NetworkVariable<bool> IsFacingRight = new NetworkVariable<bool>(true,
    //    writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);


    [SerializeField]
    private bool _isBeingRidden = false;
    public bool IsBeingRidden
    {
        get { return _isBeingRidden; }
        private set
        {
            _isBeingRidden = value;
            animator.Play("Running");
            animator.SetBool("IsRiding", value);
        }
    }


    //public override void OnNetworkSpawn()
    //{
    //    if (!IsServer) return;
    //    //VehicleLastMovement.OnValueChanged += SetFacingDirectionByAnimator;
    //}
    //private void OnDisable()
    //{
    //    if (!IsServer) return;
    //    //VehicleLastMovement.OnValueChanged -= SetFacingDirectionByAnimator;
    //}
    void Start()
    {
        animator = GetComponent<Animator>();
        VehicleLastMovement = DefaultFacingDirection;
    }
    private void Update()
    {
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsBeingRidden) return;
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.SetCurrentVehicle(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.CurrentVehicle == this && !player.IsRidingVehicle)
            {
                player.ClearVehicle();
            }
        }
    }
    //public void SetRiding(bool riding, PlayerController player)
    //{
    //    IsBeingRidden = riding;

    //    if (riding)
    //    {
    //        _rider = player;
    //        transform.SetParent(player.transform);
    //        transform.localPosition = Vector3.zero;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        transform.SetParent(null);
    //        _rider = null;
    //        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    //    }
    //}

    public void SetRiding(bool riding, PlayerController rider)
    {
        IsBeingRidden = riding;

        if (riding)
        {
            playerController = rider;

            playerController.IsFacingRight = IsFacingRight;
            playerController.LastMovement = VehicleLastMovement;
            playerController.transform.position = transform.position;
            GameObject.FindWithTag("PlayerCollision").GetComponent<Collider2D>().isTrigger = true;
            //playerController.GetComponent<Collider2D>().isTrigger = true;
            playerController.VehicleSpeed = vehicleSpeed;
            transform.SetParent(playerController.transform);
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            GameObject.FindWithTag("PlayerCollision").GetComponent<Collider2D>().isTrigger = false;

            playerController = null;
            animator.SetFloat("Speed", 0);
            transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
    }

    //[ClientRpc]
    //private void SetPlayerOnRideVehicleClientRpc(NetworkObjectReference playerRef)
    //{
    //    if(playerRef.TryGet(out NetworkObject playerObj))
    //    {
    //        playerController = playerObj.GetComponent<PlayerController>();
    //        playerController.transform.position = transform.position;
    //        playerController.GetComponent<Collider2D>().isTrigger = true;
    //        playerController.VehicleSpeed = vehicleSpeed;
    //    }
        
    //}


    public void SetMovement(Vector2 movement)
    {
        animator.SetFloat("Speed", movement.magnitude);

        if(movement != Vector2.zero) VehicleLastMovement = movement;

    }

    //private void SetFacingDirectionByAnimator(Vector2 oldValue, Vector2 newValue)
    //{
    //    animator.SetFloat("Horizontal", Mathf.Abs(newValue.x));
    //    animator.SetFloat("Vertical", newValue.y);
    //}

    public void SetCollision(Vector2 movement)
    {
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        switch (movement.x, movement.y)
        {
            case (1, 0):
                {
                    colliders[1].enabled = true;
                    break;
                }
            case (0, 1):
                {
                    colliders[2].enabled = true;
                    break;
                }
            case (0, -1):
                {
                    colliders[0].enabled = true;
                    break;
                }
            default:
                {
                    colliders[1].enabled = true;
                    break;
                }
        }
    }

}