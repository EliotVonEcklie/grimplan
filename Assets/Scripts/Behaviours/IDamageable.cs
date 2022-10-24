using System.Collections;
using UnityEngine;

public interface IDamageable
{
    public float Health { get; }

    public void OnDamage(float damage);
}