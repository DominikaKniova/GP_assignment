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

    private bool isGrounded;
    private bool doJump;

    // build variables
    private GridManager gridManager;
    private GameObject lastWireframeBlock;
    private Vector3 centerScreenPoint;
    private bool buildMode = false;
    private int currentBlockType = 0;   

    // block destroying variables
    private float startTime;
    private float destroyTime;
    private bool isDestroying;
    private GameObject destroyObject;

    public GameObject progressBar;
    public delegate void ProgressEventHandler(float progress);
    public event ProgressEventHandler ProgressTo;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gridManager = GameObject.Find("Grid Manager").GetComponent<GridManager>();
        centerScreenPoint = new Vector3(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0);
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
                    Destroy(destroyObject);
                    isDestroying = false;
                    progressBar.SetActive(false);
                }
            }

            RaycastHit hit;
            if (CastRay(out hit))
            {
                lastWireframeBlock = ShowWireframeBlock(hit);

                if (Input.GetMouseButtonDown(0))
                {
                    CreateBlock(hit);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    isDestroying = false;
                    DestroyBlock(hit);
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                isDestroying = false;
                destroyObject = null;
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
    }

    void FixedUpdate()
    {
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
                wireframeBlock = gridManager.SnapWireframe(hit.transform.position + hit.normal);
                break;

            case "Ground":
                wireframeBlock = gridManager.SnapWireframe(hit.point);
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
                gridManager.AddBlock(hit.transform.position + hit.normal, currentBlockType);
                break;

            case "Ground":
                gridManager.AddBlock(hit.point, currentBlockType);
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
            destroyObject = hit.collider.gameObject;
        }
    }
}
