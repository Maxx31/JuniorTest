using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] ball;

    [SerializeField]
    private Transform _spawnPlace;

    [SerializeField]
    private float _minSpawnTime;
    [SerializeField]
    private float _maxSpawnTime;
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _minSpawnPos;
    [SerializeField]
    private float _maxSpawnPos;
    [SerializeField]
    private float _minRadius;
    [SerializeField]
    private float _maxRadius; 
    [SerializeField]
    private float _minFreq;
    [SerializeField]
    private float _maxFreq;

    float nextTime = 0f;
    public static int CurrentBaloons = 0;
    private void Update()
    {
        if (nextTime <= Time.time && CurrentBaloons < 3)
        {
            CurrentBaloons++;

            int Left = Random.Range((int)0, (int)2); //0 = left

            GameObject Balloon = Instantiate(ball[Random.Range((int)0, (int)2)], _spawnPlace);
            Ball balloonSkript = Balloon.GetComponent<Ball>();
            if (Left == 0)
            {
                balloonSkript._offsetX = -100;
                balloonSkript._offsetY = Random.Range(_minSpawnPos, _maxSpawnPos);
            }
            else
            {
                balloonSkript._offsetX = 500;
                balloonSkript._offsetY = Random.Range(_minSpawnPos, _maxSpawnPos);
            }
            balloonSkript.Speed = Random.Range(_minSpeed, _maxSpeed);
            balloonSkript.Radius = Random.Range(_minRadius, _maxRadius);
            balloonSkript.Frequensy = Random.Range(_minFreq, _maxFreq);
            if (Left == 1) balloonSkript.Speed *= -1;
            balloonSkript.Radius = 112f;

            nextTime = Time.time + Random.Range(_minSpawnTime, _maxSpawnTime); 
        }
    }
}
