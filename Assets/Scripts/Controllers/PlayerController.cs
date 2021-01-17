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
    public ChunkedWorldManager chunkedWorldManager;
    private GameObject lastWireframeBlock;
    private Vector3 centerScreenPoint;
    private bool buildMode = false;
    private int currentBlockType = 1;   

    // block destroying variables
    private float startTime;
    private float destroyTime;
    private bool isDestroying;
    private RaycastHit destroyObjectHit;

    public GameObject progressBar;
    public delegate void ProgressEventHandler(float progress);
    public event ProgressEventHandler ProgressTo;
    
    // wireframe block prefab
    public GameObject wireframeBlockPrefab;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        centerScreenPoint = new Vector3(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0);

        // set init player position
        float worldCenter = ChunkedWorldManager.worldSize / 2.0f;
        transform.position = worldCenter * (Vector3.right + Vector3.forward);
        int height = chunkedWorldManager.GetHeightForPosition((int) transform.position.x, (int) transform.position.z);
        transform.position += (height+2) * Vector3.up;
    }
    void Update()
    {
        if (Time.timeScale == 0)
        {
            Destroy(lastWireframeBlock);
            return;
        }

        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (isOnWorldEdge())
        {
            canMove = false;
            Debug.Log("Generate new world");
            chunkedWorldManager.ReGenerateWorld();
            transform.position = GetPositionInNewWorld();
            canMove = true;
        }

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

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
    void FixedUpdate()
    {
        if (canMove)
            MovePlayer();
    }

    private void MovePlayer()
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

    private bool isOnWorldEdge()
    {
        if (transform.position.x < 0 || transform.position.x > ChunkedWorldManager.worldSize 
            || transform.position.z < 0 || transform.position.z > ChunkedWorldManager.worldSize)
        {
            Debug.Log("On edge");
            return true;
        }
        return false;
    }

    private Vector3 GetPositionInNewWorld()
    {
        float x = Mathf.Repeat(transform.position.x, ChunkedWorldManager.worldSize);
        float z = Mathf.Repeat(transform.position.z, ChunkedWorldManager.worldSize);
        int y = chunkedWorldManager.GetHeightForPosition((int) x, (int) z);
        return new Vector3(x, y + 2, z);
    }
    
    private bool CastRay(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        return Physics.Raycast(ray, out hit);
    }
    private void ShowWireframeBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            Vector3 spawnPosition = chunkedWorldManager.GetWireframePosition(hit) + Vector3.one * 0.5f;
            lastWireframeBlock = Instantiate(wireframeBlockPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void CreateBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "Chunk")
        {
            chunkedWorldManager.AddBlock(hit, currentBlockType);
        }
    }
    private void DestroyBlock(RaycastHit hit)
    {
        Debug.Log(hit.collider.name);
        if (hit.collider.tag == "Chunk")
        {
            startTime = Time.time;
            destroyTime = (float) chunkedWorldManager.GetBlockType(hit);
            isDestroying = true;
            destroyObjectHit = hit;
        }
    }
}
