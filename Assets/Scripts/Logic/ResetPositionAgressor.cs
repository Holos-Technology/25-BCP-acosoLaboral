using System;
using UnityEngine;

public class ResetPositionAgressor : MonoBehaviour
{
   private void OnEnable()
   {
      transform.localPosition = Vector3.zero;
   }
}
