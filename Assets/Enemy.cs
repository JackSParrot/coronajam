using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> Enemies = new List<Enemy>();
    public bool IsAlive
    {
        get { return true; }
    }

    public void Hit(int damage)
    {

    }

    void OnEnable()
    {
        Enemies.Add(this);    
    }

    void OnDisable()
    {
        Enemies.Remove(this);
    }
}
