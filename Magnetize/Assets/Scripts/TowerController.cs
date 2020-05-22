using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnMouseDown()
    {
        if (!player.isPulled)
        {
            player.ConnectTower();
        }
    }

    private void OnMouseUp()
    {
        player.DisconnectTower();
    }
}
