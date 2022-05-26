using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Movement : MonoBehaviour
{
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [SerializeField] private Transform enemy;

    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    private void Awake()
    {
        initScale = enemy.localScale;
    }
    
    private void MoveInDirection(int _direction)
    {
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);

        enemy.position =  new Vector3(enemy.position.x + Time.deltaTime * _direction * speed, enemy.position.y, enemy.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movingLeft)
        {
            if(enemy.position.x >= leftEdge.position.x)
            MoveInDirection(-1);
            else
            {
               DirectionChange(); 
            }
        }
        else
        {
            if(enemy.position.x <= rightEdge.position.x)
            MoveInDirection(1);
            else
            {
                DirectionChange();
            }
        }
        
    }

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }

}
