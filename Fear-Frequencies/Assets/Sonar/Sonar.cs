using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Sonar : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float waveSpeed = 10f;
    [SerializeField] private float waveMaxDistance = 100f;
    private WavePoolManager _wavePool;

    [SerializeField] private Text text;
    private float _countDown;

    // Start is called before the first frame update
    void Start()
    {
        _wavePool = new WavePoolManager(100, waveSpeed, waveMaxDistance, material);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            _wavePool.SpawnWave(transform.position);

        _wavePool.Update(Time.deltaTime);

        if (_countDown <= 0)
        {
            text.text = $"{(int)(1.0f / Time.deltaTime)}";
            _countDown = 0.3f;
        }
        else
        {
            _countDown -= Time.deltaTime;
        }
    }

    class Wave
    {
        public Vector3 Origin;
        public float Range;
        public float Strength;
        public bool Active;

        public Wave(Vector3 origin, float range, float strength)
        {
            Origin = origin;
            Range = range;
            Strength = strength;
            Active = false;
        }
    }

    class WavePoolManager
    {
        private readonly int _poolCapacity;
        private readonly List<Wave> _wavePool;

        private int _poolCount = 0;

        private readonly float _waveSpeed;
        private readonly float _waveMaxDistance;

        private readonly Material _material;
        private Texture2D _texture2D;

        public WavePoolManager(int poolCapacity, float waveSpeed, float waveMaxDistance, Material material)
        {
            _waveSpeed = waveSpeed;
            _waveMaxDistance = waveMaxDistance;

            _poolCapacity = poolCapacity;
            _wavePool = new List<Wave>(poolCapacity);
            for (int i = 0; i < poolCapacity; i++)
                _wavePool.Add(new Wave(Vector3.zero, 0, 0));

            _material = material;
            _texture2D = new Texture2D(5, poolCapacity, TextureFormat.RGBAFloat, false);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _poolCapacity; i++)
            {
                if (!_wavePool[i].Active) 
                    continue;
                

                _wavePool[i].Range += _waveSpeed * deltaTime;

                _wavePool[i].Strength = 1 - _wavePool[i].Range / _waveMaxDistance / 5;

                if (_wavePool[i].Strength <= 0)
                    _wavePool[i].Active = false;
            }

            Color[] data = new Color[5 * _poolCapacity];

            for (int i = 0; i < _poolCapacity; i++)
            {
                Vector3 pos = _wavePool[i].Origin;
                float range = _wavePool[i].Range;
                if (range > _waveMaxDistance)
                    range = _waveMaxDistance;
                float strength = _wavePool[i].Strength;

                data[i * 5 + 0] = new Color(pos.x, pos.x, pos.x, 0);
                data[i * 5 + 1] = new Color(pos.y, pos.y, pos.y, 0);
                data[i * 5 + 2] = new Color(pos.z, pos.z, pos.z, 0);
                data[i * 5 + 3] = new Color(range, range, range, 0);
                data[i * 5 + 4] = new Color(strength, strength, strength, 0);
            }
            
            _texture2D.SetPixels(data);
            _texture2D.Apply();

            _material.SetTexture("_Wave_Data", _texture2D);
        }

        public void SpawnWave(Vector3 origin)
        {
            _wavePool[_poolCount].Origin = origin;
            _wavePool[_poolCount].Range = 0;
            _wavePool[_poolCount].Strength = 1;
            _wavePool[_poolCount].Active = true;

            _poolCount++;
            if (_poolCount == _poolCapacity)
                _poolCount = 0;
        }
    }
}