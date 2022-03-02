using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;//скорость смены позиции кубика
    public Transform cubeToPlace;
    public GameObject[] cubesToCreate;
    public GameObject allCubes;
    private Rigidbody allCubesRb;
    private bool IsLose, firstCube;
    private List<Vector3> AllCubesPositions = new List<Vector3> {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1)
    };

    private Coroutine showCubePlace;
    private List<GameObject> possibleCubesToCreate = new List<GameObject>();
    private void Start()
    {
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null)
        {
#if !UNITY_EDITOR
            if(Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            GameObject createCube = null;
            if (possibleCubesToCreate.Count == 1)
                createCube = possibleCubesToCreate[0];
            else
                createCube = possibleCubesToCreate[UnityEngine.Random.Range(0, possibleCubesToCreate.Count)];
            GameObject newCube = Instantiate(
                createCube,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            AllCubesPositions.Add(nowCube.getVector());
            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
            SpawnPositions();
        }
        if (!IsLose && allCubesRb.velocity.magnitude > 0.1f)//проверка движения самой башни
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

    }
    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void SpawnPositions()//делаем рандомное положение следующего куба
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
            return false;
        foreach (Vector3 pos in AllCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }
    private void AddPossibleCubes(int till)
    {
        for (int i = 0; i < till; i++)
            possibleCubesToCreate.Add(cubesToCreate[i]);
    }
}



struct CubePos //хранение координат объекта
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }
    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}