using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDonut : MonoBehaviour
{
    [SerializeField] private GameObject goalProof;
    [SerializeField] private GameObject balloon;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Enemy"))
        {
            Instantiate(goalProof, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    } 
}
