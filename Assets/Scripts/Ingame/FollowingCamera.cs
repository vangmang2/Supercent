using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] Transform tfTarget;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = tfTarget.position + new Vector3(0, 10.9300003f, -8.89000034f);
    }
}
