using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] public float currentHealth; //{get; private set;}
    //[SerializeField] private int damage;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float _damage)
    {
        //currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

       // if (currentHealth > 0)
       // {
            currentHealth -= _damage;
        //}

        //else
       // {
           // currentHealth -= damage;
       // }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.E))
        // {
        //     TakeDamage(damage);
        // }
    }
}
