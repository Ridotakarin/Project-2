using UnityEngine;

public class Raycast360TriggerCheck : MonoBehaviour
{
    public float radius = 5f;           
    public int rayCount = 36;          

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            CheckAllDirections();
        }
    }

    void CheckAllDirections()
    {
        Vector2 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, radius);

            if (hit.collider != null)
            {
                Color rayColor = hit.collider.isTrigger ? Color.green : Color.red;
                Debug.DrawRay(origin, direction * hit.distance, rayColor, 1f);

                if (hit.collider.isTrigger)
                {
                    Debug.Log("Hit trigger: " + hit.collider.name + " at direction: " + direction);
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * radius, Color.gray, 1f);
            }
        }
    }
}
