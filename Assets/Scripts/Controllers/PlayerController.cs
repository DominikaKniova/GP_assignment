using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // movement variables
    private Rigidbody playerRb;
    private CapsuleCollider capsCollider;
    private Vector3 direction;

    private float playerSpeed = 3f;
    private float playerSpeedSqr;
    private float jumpForce = 5;

    private bool doJump;
    private bool canMove = true;

    // raycast variables
    private float rayLength = 10.0f;

    // build variables
    public WorldManager chunkedWorldManager;
    private GameObject lastWireframeBlock;
    private Vector3 centerScreenPoint;
    private bool buildMode = false;
    private byte currentBlockType = 1;   

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
        playerSpeedSqr = playerSpeed * playerSpeed;

        // position the player
        SetInitPlayerPosition();
    }
    void Update()
    {
        if (Time.timeScale == 0)
        {
            Destroy(lastWireframeBlock);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            doJump = true;
        }

        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (isOnWorldEdge())
        {
            canMove = false;
            Debug.Log("Generate new world");
            chunkedWorldManager.ReGenerateWorld();
            transform.position = GetPositionInNewWorld();
            canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            Destroy(lastWireframeBlock);
        }

        UpdateBlockType();

        if (buildMode)
        {
            Destroy(lastWireframeBlock);

            if (isDestroying && Input.GetMouseButton(1))
            {
                progressBar.SetActive(true);
                ProgressTo((Time.time - startTime) / destroyTime);
                if (startTime + destroyTime <= Time.time)
                {
                    chunkedWorldManager.DestroyBlock(destroyObjectHit);
                    isDestroying = false;
                    progressBar.SetActive(false);
                }
            }
            else
            {
                RaycastHit hit;
                if (CastRay(out hit))
                {
                    ShowWireframeBlock(hit);

                    if (Input.GetMouseButtonDown(0))
                    {
                        Destroy(lastWireframeBlock);
                        CreateBlock(hit);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        Destroy(lastWireframeBlock);
                        isDestroying = false;
                        DestroyBlock(hit);
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                isDestroying = false;
                progressBar.SetActive(false);
            }
        }
    }
    private void UpdateBlockType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBlockType = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlockType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBlockType = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentBlockType = 4;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, capsCollider.bounds.extents.y + 0.1f);
    }
    
    void FixedUpdate()
    {
        if (canMove)
            MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 dir = transform.forward * direction.z + transform.right * direction.x;

        //if (Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2) < playerSpeedSqr)
        //    playerRb.AddForce(dir * playerSpeed, ForceMode.Impulse);

        playerRb.MovePosition(transform.position + dir * Time.fixedDeltaTime * playerSpeed);
        //transform.Translate(direction * playerSpeed * Time.fixedDeltaTime);

        if (doJump)
        {
            doJump = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private bool isOnWorldEdge()
    {
        if (transform.position.x < 0 || transform.position.x > WorldManager.worldSize 
            || transform.position.z < 0 || transform.position.z > WorldManager.worldSize)
        {
            Debug.Log("On edge");
            return true;
        }
        return false;
    }

    private void SetInitPlayerPosition()
    {
        // try to position the player somewhere high 
        Vector3 highPosition = chunkedWorldManager.GetHighPosition();

        if (highPosition.Equals(Vector3.zero))
        {
            // nothing high was found, so position the player in the center of world
            float worldCenter = WorldManager.worldSize / 2.0f;
            transform.position = worldCenter * (Vector3.right + Vector3.forward);
            int height = chunkedWorldManager.GetHeightForPosition((int)transform.position.x, (int)transform.position.z);
            transform.position += (height + 2) * Vector3.up;
        }
        else
        {
            transform.position = highPosition + 0.5f * Vector3.one + 2 * Vector3.up;
        }
    }
    private Vector3 GetPositionInNewWorld()
    {
        float x = Mathf.Repeat(transform.position.x, WorldManager.worldSize);
        float z = Mathf.Repeat(transform.position.z, WorldManager.worldSize);
        int y = chunkedWorldManager.GetHeightForPosition((int) x, (int) z);
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
            lastWireframeBlock = chunkedWorldManager.SnapWireframeBlock(hit, transform.position);
        }
    }

    private void CreateBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            chunkedWorldManager.AddBlock(hit, currentBlockType, transform.position);
        }
    }
    private void DestroyBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            startTime = Time.time;
            destroyTime = chunkedWorldManager.GetDestroyTime(hit);
            isDestroying = true;
            destroyObjectHit = hit;
        }
    }
}
