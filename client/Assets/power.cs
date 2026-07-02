using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class power : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float value;

    public float Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = Mathf.Clamp01(value);
            Refresh();
        }
    }

    private void Awake()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        value = Mathf.Clamp01(value);
        Refresh();
    }
#endif

    public void Refresh()
    {
        int imageCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Image>() != null)
            {
                imageCount++;
            }
        }

        int showCount = Mathf.Clamp(Mathf.CeilToInt((1f - value) * imageCount), 0, imageCount);
        int imageIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Image>() == null)
            {
                continue;
            }

            child.gameObject.SetActive(imageIndex < showCount);
            imageIndex++;
        }
    }
}
