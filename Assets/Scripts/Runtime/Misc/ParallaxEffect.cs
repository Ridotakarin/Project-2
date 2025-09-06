using System.Collections;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float speed = 0.5f;
    private MeshRenderer bgParalax;

    private void Awake()
    {
        bgParalax = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Vector2 offset = new Vector2(Time.time * speed, 0);
        bgParalax.material.mainTextureOffset = offset;
    }

    private void SetSpeed()
    {
        speed *= (float)10f;
        StartCoroutine(enumerator());
        speed /= (float)10f;
    }

    private IEnumerator enumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
