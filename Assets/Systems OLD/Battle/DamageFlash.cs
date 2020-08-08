using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageFlash
{
    public MeshRenderer[] noSkinDamagedOverlays;
    public SkinnedMeshRenderer damagedOverlay;    
    public bool showDamageSkinned = false;
    public bool showDamageMesh = false;

    Color overlayColor = new Color(1f, 0f, 0f, 0.6f);
    float damagedSpeed = 5f;

    public void Fade()
    {
        if (damagedOverlay != null)
        {
            Flash(damagedOverlay);
        }

        if (noSkinDamagedOverlays.Length > 0)
        {
            Flash(noSkinDamagedOverlays);
        }
    }

    void Flash(SkinnedMeshRenderer overlay)
    {
        if (showDamageSkinned)
        {
            overlay.materials[1].color = overlayColor;
        }
        else
        {
            overlay.materials[1].color = Color.Lerp(overlay.materials[1].color, Color.clear, damagedSpeed * Time.deltaTime);
        }

        showDamageSkinned = false;
    }

    void Flash(MeshRenderer[] overlay)
    {
        if (showDamageMesh)
        {
            foreach (MeshRenderer meshRenderer in overlay)
            {
                meshRenderer.material.color = overlayColor;
            }
        }
        else
        {
            foreach (MeshRenderer meshRenderer in overlay)
            {
                meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.clear, damagedSpeed * Time.deltaTime);
            }
        }

        showDamageMesh = false;
    }
}
