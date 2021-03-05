using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{

    public GameObject Enemy;

    private float SpawnTime;

    private Vector2 SpawnPoint;

    private int Number = 10;

    // Start is called before the first frame update
    void Start()
    {
        SpawnTime = 2.0f;

        StartCoroutine(Spawn());
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator Spawn()
    {
        while (Number>0)
        {
            float PointX = Random.Range(-5.0f, 5.0f);

            float PointY = Random.Range(0.0f, 5.0f);

            SpawnPoint = new Vector3(PointX, PointY, transform.position.z);

            Instantiate(Enemy, SpawnPoint, transform.rotation);

            yield return new WaitForSeconds(SpawnTime);

            SpawnTime -= 0.01f;
            Number -= 1;
        }
        yield return null;
    }
}
