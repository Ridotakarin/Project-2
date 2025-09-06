using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedScript : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField]
    private bool _isVertical = false;
    public bool IsVertical
    {
        get { return _isVertical; }
        private set
        {
            _isVertical = value;
        }
    }

    [SerializeField]
    private bool _isBeingUsed = false;
    public bool IsBeingUsed
    {
        get { return _isBeingUsed; }
        private set
        {
            _isBeingUsed = value;
        }
    }

    private bool playerInrange = false;

    public void SetSleep(bool toUse)
    {
        if (!GameFlowManager.Instance.Data.CompletedThirdCutscene) return;

        Debug.Log("is use bed: " + toUse);
        IsBeingUsed = toUse;
        if (toUse)
        {
            GameObject.FindWithTag("PlayerCollision").GetComponent<Collider2D>().isTrigger = true;
            //SceneManager.MoveGameObjectToScene(playerController.gameObject, SceneManager.GetActiveScene());
            //playerController.transform.SetParent(transform);
            if (!IsVertical)
            {
                playerController.transform.position = transform.position + new Vector3(0.4f, 0.5f, 0);
                playerController.transform.rotation = Quaternion.Euler(0, 0, 90);
                playerController.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            else
            {
                playerController.transform.position = transform.position + new Vector3(0, 0.2f, 0);
            }
            playerController.Movement = Vector2.zero;
            playerController.GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            GameObject.FindWithTag("PlayerCollision").GetComponent<Collider2D>().isTrigger = false;
            //playerController.transform.SetParent(null);
            //DontDestroyOnLoad(playerController.gameObject);
            playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerController.GetComponent<SpriteRenderer>().sortingOrder = 0;
            playerController.GetComponent<Collider2D>().isTrigger = false;
        }
        
    }
}
