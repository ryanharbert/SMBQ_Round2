using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMBQ.Tools
{
    [RequireComponent(typeof(Camera))]
    public class KeepCameraWidth : MonoBehaviour
    {
        [SerializeField] private Vector2 aspectRatio = new Vector2(2560f, 1440f);
    
        private void Awake()
        {
            Camera c = GetComponent<Camera>();
            if (c != null)
            {
                float defaultWidth = c.orthographicSize * (aspectRatio.x / aspectRatio.y);
                c.orthographicSize = defaultWidth / c.aspect;
            }
        }
    }
}
