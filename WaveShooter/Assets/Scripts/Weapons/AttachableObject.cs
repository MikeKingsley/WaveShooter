using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableObject : MonoBehaviour
{
    /// <summary>
    /// Weapon attachment points: (1) Magazine  (2) Rail
    /// </summary>
    [Tooltip("Weapon attachment points: (1) Magazine  (2) Rail")]
    public int attachPoint = 1;
}
