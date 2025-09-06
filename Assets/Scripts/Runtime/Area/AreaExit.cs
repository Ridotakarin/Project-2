using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class AreaExit : MonoBehaviour
{
    [SerializeField] private bool _isMultiSceneLoad = true;

    [SerializeField] private bool _useClickToExit;
    public bool UseClickToExit
    {
        get { return _useClickToExit; }
        set { _useClickToExit = value; }
    }


    private bool _canClick;
    public bool CanClick
    {
        get { return _canClick; }
        set { _canClick = value; }
    }
    [SerializeField] private Loader.Scene sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    private float waitToLoadTime = 0f;

    private void Update()
    {
        OnClick();
    }


    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player")) 
        {
            if (!UseClickToExit)
            {
                PlayerController.LocalInstance.CanMove = false;
                //UI_Fade.Instance.gameObject.SetActive(true);
                SceneManagement.SetTransitionName(sceneTransitionName);
                //UI_Fade.Instance.FadeToBlack();
                StartCoroutine(LoadSceneRoutine());
            }
            
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        while (waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }
        string currentSceneName = SceneManagement.GetCurrentSceneName();
        if (sceneToLoad == Loader.Scene.MineCaveScene && currentSceneName == Loader.Scene.MineScene.ToString())
        {
            CaveManager.Instance.CurrentLocalCaveLevel = 0; // Reset cave level when exiting to world scene
        }
        
        Loader.Load(sceneToLoad, _isMultiSceneLoad);
    }


    public void OnClick()
    {
        if (UseClickToExit && CanClick && Input.GetMouseButtonDown(1))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float radius = 0.5f; // Bigger than exact pixel click

            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPoint, radius);
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    CanClick = false;
                    PlayerController.LocalInstance.CanMove = false;
                    //UI_Fade.Instance.gameObject.SetActive(true);
                    SceneManagement.SetTransitionName(sceneTransitionName.ToString());
                    //UI_Fade.Instance.FadeToBlack();
                    StartCoroutine(LoadSceneRoutine());
                }
            }
        }
    }
}
