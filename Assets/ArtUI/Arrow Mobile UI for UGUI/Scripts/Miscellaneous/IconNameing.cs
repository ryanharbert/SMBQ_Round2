using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [ExecuteInEditMode]
    public class IconNameing : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Image m_Image;
        #pragma warning restore 0649

        // Use this for initialization
        void Start()
        {
            SetName();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            SetName();
        }
#endif

        private void SetName()
        {
            if (m_Image != null)
            {
                gameObject.name = "Button (" + m_Image.sprite.name + ")";
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Remove All Scripts")]
        public void RemoveAll()
        {
            IconNameing[] scripts = Object.FindObjectsOfType<IconNameing>();

            foreach (IconNameing script in scripts)
            {
                DestroyImmediate(script);
            }
        }
#endif
    }
}