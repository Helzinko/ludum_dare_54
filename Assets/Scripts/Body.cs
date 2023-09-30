using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] private Transform shadow;
    [SerializeField] private Vector2 initialOffset = new Vector2(0.08f, -0.08f);

    private void Update()
    {
        shadow.transform.position = (Vector2)transform.position + initialOffset;
    }
}
