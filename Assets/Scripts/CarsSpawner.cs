using System.Collections.Generic;
using UnityEngine;

public class CarsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _carTemplate;
    [SerializeField] private Vector3[] _spawningPoints;
    [SerializeField] private float _spawningIntervalTime = 1.0f;
    private float _cooldownTimer = 0;
    [SerializeField] private Vector3 _direction;

    private IEnumerator<Vector3> _loopedSpawningPoints;

    private void Awake()
    {
        _loopedSpawningPoints = CreateLoopedSpawningPoints();
    }

    private void Update()
    {
        _cooldownTimer += Time.deltaTime;

        if(_cooldownTimer >= _spawningIntervalTime)
        {
            _cooldownTimer = 0;
            Instantiate(_carTemplate, transform.position + GetNextSpawningPoint(), Quaternion.LookRotation(_direction));
            //RandomizeCooldownDuration();
        }
    }

    private void RandomizeCooldownDuration()
    {
        _spawningIntervalTime = Random.Range(0.2f, 1.0f);
    }

    private Vector3 GetNextSpawningPoint()
    {
        _loopedSpawningPoints.MoveNext();
        return _loopedSpawningPoints.Current;
    }

    private IEnumerator<Vector3> CreateLoopedSpawningPoints()
    {
        while(true)
        {
            //int randomPointIndex = Random.Range(0, _spawningPoints.Length);
            //yield return _spawningPoints[randomPointIndex];
            for(int i = 0; i < _spawningPoints.Length; i++)
            {
                yield return _spawningPoints[i];
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, _direction.normalized * 2.0f);

        foreach(Vector3 position in _spawningPoints)
        {
            Gizmos.DrawSphere(transform.position + position, 2.0f);
        }
    }
}
