using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    [SerializeField] List<WayPath> path = new List<WayPath>();
    [SerializeField][Range(0f, 5f)] float Speed;
    Enemy enemy;


    void OnEnable()
    {
        FindPath();
        ReturnPosition();
        StartCoroutine(EnemyIntendedPath());
    }

    private void Start()
    {
        enemy = GetComponentInChildren<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on the same GameObject as EnemyMovement.");
        }
    }

    void FindPath()
    {
        path.Clear();
        GameObject parent = GameObject.FindGameObjectWithTag("Path");

        foreach (Transform child in parent.transform)
        {
            WayPath waypath = child.GetComponent<WayPath>();

            if (waypath != null)
            {
                path.Add(waypath);
            }
        }
    }

    void ReturnPosition()
    {
        transform.position = path[0].transform.position;
    }


    void FinishPath()
    {
        gameObject.SetActive(false);
        enemy.GoldWithdraw();
    }
    IEnumerator EnemyIntendedPath()
    {

        if (path.Count == 0)
        {
            Debug.LogWarning("No paths found. Exiting EnemyIntendedPath.");
            yield break;
        }
        foreach (WayPath wayPath in path)
        {

            Vector3 startPosition = transform.position;
            Vector3 endPosition = wayPath.transform.position;
            float percent = 0f;

            transform.LookAt(endPosition);

            while (percent < 1f)
            {
                percent += Time.deltaTime * Speed;
                transform.position = Vector3.Lerp(startPosition, endPosition, percent);
                yield return new WaitForEndOfFrame();
            }
        }
        FinishPath();
    }
}
