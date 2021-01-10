using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;
    private Rigidbody playerRb;

    private float playerSpeed = 3.0f;
    private float jumpForce = 6.0f;
    private Vector3 direction;

    public bool buildMode = false;
    public bool isGrounded;
    private bool doJump;

    public int currentBlockType = 1;

    private Vector3 centerScreenPoint;

    private GameObject lastWireframeBlock;
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
            lastWireframeBlock = ShowWireframeBlock();

            if (Input.GetMouseButtonDown(0))
            {
                CreateBlock();
            }

            if (Input.GetMouseButtonDown(1))
            {
                DestroyIntersectingBlock();
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
        // fuj
        playerRb.MovePosition(transform.position 
            + (transform.forward * direction.z + transform.right * direction.x).normalized * Time.fixedDeltaTime * playerSpeed);

        if (doJump)
        {
            isGrounded = false;
            doJump = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private GameObject ShowWireframeBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        RaycastHit hit;

        GameObject wireframeBlock = null;

        if (Physics.Raycast(ray, out hit)) 
        {
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
        }

        return wireframeBlock;
    }

    private void CreateBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
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
    }
    private void DestroyIntersectingBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Block")
            {
                hit.collider.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
