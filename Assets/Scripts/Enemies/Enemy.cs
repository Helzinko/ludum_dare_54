using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameEntity
{
    public string Name;
    public string Description;

    public bool destroyOnCollision = false;
    public bool offscreenIndication = false;
    public float speed = 5f;

    protected Vector2 direction;

    [SerializeField] private EnemyIndication indicationPrefab;
    private bool indicationIsShowed = false;
    private GameObject indication;

    public virtual Vector2 SpawnPosition()
    {
        return Vector2.zero;
    }

    public virtual void Update()
    {
        if (offscreenIndication && indicationPrefab)
        {
            if (!IsObjectVisible() && !indicationIsShowed)
            {
                var raycast = Physics2D.Raycast(transform.position, Vector3.zero - transform.position);
                if (raycast.collider.tag == "CameraBounds")
                {
                    indication = Instantiate(indicationPrefab, raycast.point, Quaternion.LookRotation(Vector3.forward, Vector2.zero - (Vector2)transform.position), null).gameObject;
                    indicationIsShowed = true;
                }
            }
            else if (IsObjectVisible() && indication)
            {
                Destroy(indication);
            }
        }
    }

    private bool IsObjectVisible()
    {
        var renderer = GetComponentInChildren<Renderer>(false);

        if (renderer)
        {
            Bounds objectBounds = renderer.bounds;
            bool isVisible = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), objectBounds);
            return isVisible;
        }

        return true;
    }
}