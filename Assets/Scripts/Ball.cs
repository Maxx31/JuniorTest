using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private float moveValue;
    [HideInInspector]
    public float Speed;
    [HideInInspector]
    public float Radius;
    [HideInInspector]
    public float Frequensy;
    [HideInInspector]
    public float _offsetX;
    [HideInInspector]
    public float _offsetY;


    private void Start()
    {
        moveValue = _offsetX;
        moveValue = (moveValue + Time.deltaTime * Speed);

        var y = Mathf.Sin((moveValue / Speed) * Frequensy) * Radius + _offsetY;


        transform.position = new Vector3(moveValue, y, 0);
        StartCoroutine(SelfDestruct());
    }
    void Update()
    {
        moveValue = (moveValue + Time.deltaTime * Speed) ;

        var y = Mathf.Sin ( (moveValue / Speed) * Frequensy) * Radius + _offsetY;


        transform.position = new Vector3(moveValue,y, 0);
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(7f);
        BallSpawn.CurrentBaloons--;
        Destroy(gameObject);
    }
}
