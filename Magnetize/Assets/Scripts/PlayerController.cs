using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isPulled = false;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float pullForce = 100f;
    [SerializeField]
    private float rotateSpeed = 300f;
    [HideInInspector]
    private GameObject closestTower;
    private GameObject hookedTower;
    private Rigidbody2D rb2D;
    private UIController uiControl;
    private Vector3 startPosition;
    private AudioSource myAudio;
    private bool isCrashed = false;

    // Start is called before the first frame update
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        myAudio = GetComponent<AudioSource>();
        uiControl = GameObject.Find("Canvas").GetComponent<UIController>();
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    private void Update()
    {
        rb2D.velocity = -transform.up * moveSpeed;
    }

    public void ConnectTower()
    {
        if (closestTower != null && hookedTower == null)
        {
            hookedTower = closestTower;
        }

        if (hookedTower)
        {
            float distance = Vector2.Distance(transform.position, hookedTower.transform.position);

            //gravitation toward tower
            Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;

            //check 
            float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
            rb2D.AddForce(pullDirection * newPullForce);

            //angular velocity
            rb2D.angularVelocity = -rotateSpeed / distance;

            isPulled = true;
        }
    }

    public void DisconnectTower()
    {
        isPulled = false;
        hookedTower = null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Wall")
        {
            if (!isCrashed)
            {
                myAudio.Play();
                isCrashed = true;
                rb2D.velocity = new Vector3(0f, 0f, 0f);
                rb2D.angularVelocity = 0f;
                StartCoroutine(RestartDelay(1f));
            }
        }
    }

    private IEnumerator RestartDelay(float second)
    {
        yield return new WaitForSeconds(second);
        RestartPosition();
    }

    private void RestartPosition()
    {
        //set to start position
        this.transform.position = startPosition;

        //Restart rotation 
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        isCrashed = false;
        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Goal")
        {
            Debug.Log("Level Clear");
            uiControl.EndGame();
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Tower")
        {
            closestTower = col.gameObject;

            //green color for the tower
            col.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (isPulled) return;

        if (col.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;

            //back to normal color
            col.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
