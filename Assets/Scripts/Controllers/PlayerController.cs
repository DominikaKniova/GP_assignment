using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // movement variables
    private Rigidbody playerRb;
    private CapsuleCollider capsCollider;
    private Vector3 moveDirection;
    private float playerSpeed = 3f;
    private float jumpForce = 5;
    private bool doJump;
    private bool isGrounded = true;

    // audio
    private AudioSource stepsAudio;

    // raycast variables
    private float rayLength = 10.0f;

    // build variables
    public WorldManager worldManager;
    private GameObject lastWireframeBlock;
    private Vector3 centerScreenPoint;

    // block destroy variables
    private float startTime;
    private float destroyTime;
    private bool isDestroying;
    private RaycastHit destroyObjectHit;

    public GameObject progressBar;
    public delegate void ProgressEventHandler(float progress);
    public event ProgressEventHandler ProgressTo;
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        capsCollider = GetComponent<CapsuleCollider>();
        centerScreenPoint = new Vector3(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0);
        stepsAudio = GetComponent<AudioSource>();

        // position the player
        SetInitPlayerPosition();
    }
    void Update()
    {
        Destroy(lastWireframeBlock);

        GetMovementInputs();

        HandleAudio();

        if (isOnWorldBorder())
        {
            // generate new world and set new player position
            worldManager.ReGenerateWorld();
            transform.position = GetPositionInNewWorld();
        }

        if (GameManager.isBuildMode)
        {
            if (isDestroying && Input.GetMouseButton(1))
            {
                // player tries to destroy block
                HandleBlockDestroying();
            }
            else
            {
                RaycastHit hit;
                if (CastRay(out hit))
                {
                    ShowWireframeBlock(hit);

                    if (Input.GetMouseButtonDown(0))
                        CreateBlock(hit);
                    
                    if (Input.GetMouseButtonDown(1))
                    {
                        isDestroying = false;
                        DestroyBlock(hit);
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
                StopDestroying();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetMovementInputs()
    {
        if (IsGrounded())
            isGrounded = true;
        else
            isGrounded = false;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            doJump = true;
        
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void HandleAudio()
    {
        if (isGrounded && !moveDirection.Equals(Vector3.zero))
        {
            if (!stepsAudio.isPlaying)
                stepsAudio.Play();
        }
        else
            stepsAudio.Stop();
    }

    private void MovePlayer()
    {
        Vector3 dir = transform.forward * moveDirection.z + transform.right * moveDirection.x;
        playerRb.MovePosition(transform.position + dir.normalized * Time.fixedDeltaTime * playerSpeed);

        if (doJump)
        {
            doJump = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    /* Check if player is standing on the ground */
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, capsCollider.bounds.extents.y + 0.1f);
    }
    
    
    /* Check if player passed world border */
    private bool isOnWorldBorder()
    {
        if (transform.position.x < 0 || transform.position.x > WorldManager.worldSize 
            || transform.position.z < 0 || transform.position.z > WorldManager.worldSize)
        {
            return true;
        }
        return false;
    }

    private void SetInitPlayerPosition()
    {
        // try to position the player somewhere high 
        Vector3 highPosition = worldManager.GetHighPosition();

        if (highPosition.Equals(Vector3.zero))
        {
            // nothing high was found, so position the player in the center of world
            float worldCenter = WorldManager.worldSize / 2.0f;
            transform.position = worldCenter * (Vector3.right + Vector3.forward);
            int height = worldManager.GetHeightForPosition((int)transform.position.x, (int)transform.position.z);
            transform.position += (height + 2) * Vector3.up;
        }
        else
        {
            transform.position = highPosition + 0.5f * Vector3.one + 2 * Vector3.up;
        }
    }

    /* When player passed world border, position it correctly at the "beginning" of new world */
    private Vector3 GetPositionInNewWorld()
    {
        float x = Mathf.Repeat(transform.position.x, WorldManager.worldSize);
        float z = Mathf.Repeat(transform.position.z, WorldManager.worldSize);
        int y = worldManager.GetHeightForPosition((int) x, (int) z);
        return new Vector3(x, y + 2, z);
    }
    
    private bool CastRay(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        return Physics.Raycast(ray, out hit, rayLength);
    }

    private void ShowWireframeBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            lastWireframeBlock = worldManager.SnapWireframeBlock(hit, transform.position);
        }
    }

    private void CreateBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            worldManager.AddBlock(hit, GameManager.currentBlockType, transform.position);
        }
    }
    private void DestroyBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            startTime = Time.time;
            destroyTime = worldManager.GetDestroyTime(hit);
            progressBar.SetActive(true);
            isDestroying = true;
            destroyObjectHit = hit;
        }
    }

    /* Update progress bar and check when destroy time is reached */
    private void HandleBlockDestroying()
    {
        ProgressTo((Time.time - startTime) / destroyTime);

        if (startTime + destroyTime <= Time.time)
        {
            worldManager.DestroyBlock(destroyObjectHit);
            isDestroying = false;
            progressBar.SetActive(false);
        }
    }

    private void StopDestroying()
    {
        isDestroying = false;
        progressBar.SetActive(false);
    }
}
