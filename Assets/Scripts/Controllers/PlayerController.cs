using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // movement variables
    private Rigidbody playerRb;
    private Vector3 direction;

    private float playerSpeed = 3.0f;
    private float jumpForce = 4.5f;

    public bool isGrounded;
    private bool doJump;
    private bool canMove = true;

    // build variables
    public WorldManager worldManager;
    public ChunkedWorldManager chunkedWorldManager;
    private GameObject lastWireframeBlock;
    private Vector3 centerScreenPoint;
    private bool buildMode = false;
    private int currentBlockType = 0;   

    // block destroying variables
    private float startTime;
    private float destroyTime;
    private bool isDestroying;
    private RaycastHit destroyObjectHit;

    public GameObject progressBar;
    public delegate void ProgressEventHandler(float progress);
    public event ProgressEventHandler ProgressTo;

    public GameObject wireframeBlockPrefab;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        centerScreenPoint = new Vector3(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0);

        int y = chunkedWorldManager.GetHeightForPosition(transform.position);
        transform.position += transform.position.y * Vector3.down + y * Vector3.up + Vector3.up * 2;
    }
    void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            doJump = true;
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
                    lastWireframeBlock = ShowWireframeBlock(hit);

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
            currentBlockType = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlockType = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBlockType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentBlockType = 3;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        //if (collision.gameObject.CompareTag("WorldBorder"))
        //{
        //    Debug.Log("Generate new world");
        //    canMove = false;
        //    worldManager.ResetWorld();
        //    int y = worldManager.GetHeightForPosition(transform.position);
        //    transform.position += transform.position.y * Vector3.down + y * Vector3.up + Vector3.up * 2;
        //    canMove = true;
        //}
    }
    void FixedUpdate()
    {
        if (canMove)
            MovePlayer();
    }

    void MovePlayer()
    {
        playerRb.MovePosition(transform.position 
            + (transform.forward * direction.z + transform.right * direction.x).normalized * Time.fixedDeltaTime * playerSpeed);

        if (doJump)
        {
            isGrounded = false;
            doJump = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private bool CastRay(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        return Physics.Raycast(ray, out hit);
    }
    private GameObject ShowWireframeBlock(RaycastHit hit)
    {
        GameObject wireframeBlock = null;

        switch (hit.collider.tag)
        {
            case "Block":
                wireframeBlock = worldManager.SnapWireframe(hit.transform.position + hit.normal);
                break;

            case "Ground":
                wireframeBlock = worldManager.SnapWireframe(hit.point);
                break;
            case "Chunk":
                Vector3 spawnPosition = chunkedWorldManager.GetWireframePosition(hit) + Vector3.one * 0.5f;
                wireframeBlock = Instantiate(wireframeBlockPrefab, spawnPosition, Quaternion.identity);
                break;
            default:
                break;
        }

        return wireframeBlock;
    }

    private void CreateBlock(RaycastHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Block":
                worldManager.AddBlock(hit.transform.position + hit.normal, currentBlockType);
                break;

            case "Ground":
                worldManager.AddBlock(hit.point, currentBlockType);
                break;

            case "Chunk":
                chunkedWorldManager.AddBlock(hit, currentBlockType + 1);
                break;
            default:
                break;
        }
}
    private void DestroyBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Block")
        {
            startTime = Time.time;
            destroyTime = hit.collider.gameObject.GetComponent<BlockController>().destroyTime;
            isDestroying = true;
            //destroyObject = hit.collider.gameObject;
        }

        if (hit.collider.tag == "Chunk")
        {
            startTime = Time.time;
            destroyTime = (float) chunkedWorldManager.GetBlockType(hit);
            isDestroying = true;
            destroyObjectHit = hit;
        }
    }
}
