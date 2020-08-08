using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    public Camera mainCamera;
	public float smoothing = 0.7f;
    public Vector3 offset = new Vector3(24f, 30f, 32f);
	public Vector3 velocity = Vector3.one;

    public float defaultCameraSize;

    private bool zoom = false;

    public void Setup ()
    {
        if (mainCamera.orthographic)
        {
            float defaultWidth = mainCamera.orthographicSize * (16f / 9f);
            defaultCameraSize = defaultWidth / mainCamera.aspect;
            transform.position = WorldManager.instance.player.transform.position + offset;
        }
        else
        {
            defaultCameraSize = mainCamera.fieldOfView;
            transform.position = WorldManager.instance.player.transform.position + offset;
        }
    }
    public void FollowPlayer()
    {
        Vector3 targetCamPos = WorldManager.instance.player.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref velocity, smoothing);

        InteractionZoom();
    }

    void InteractionZoom()
    {
        if (mainCamera.orthographic)
        {
            if (zoom)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 5.5f, Time.deltaTime);
            }
            else if (mainCamera.orthographicSize != defaultCameraSize)
            {
                mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, defaultCameraSize, Time.deltaTime * 10);
            }
        }
        else
        {
            if (zoom)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 5.5f, Time.deltaTime);
            }
            else if (mainCamera.fieldOfView != defaultCameraSize)
            {
                mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, defaultCameraSize, Time.deltaTime * 10);
            }
        }
    }

    public void ToggleZoom(bool zoom)
    {
        this.zoom = zoom;
    }
}
