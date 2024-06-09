using System;
using UnityEngine;

public class CustomerPool : ObjectPool
{
    public Customer Spawn(Vector3 pos, Quaternion rot, Action<Customer> onSpawn)
    {
        var customer = pool.Spawn<Customer>(pos, rot, null, true);
        onSpawn?.Invoke(customer);
        return customer;
    }

    public void Despawn(Customer customer)
    {
        pool.Despawn(customer.gameObject);
    }
}
