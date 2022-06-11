using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public enum Type { HealthKit, TreasureBox, Grenade }
    public Type type;
    public int value;

    private void Update()
    {
        transform.Rotate(Vector3.fwd * 20 * Time.deltaTime);
    }
}
