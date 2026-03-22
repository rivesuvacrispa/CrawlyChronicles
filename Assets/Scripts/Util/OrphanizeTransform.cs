using System;
using UnityEngine;

namespace Util
{
    public class OrphanizeTransform : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
        }
    }
}