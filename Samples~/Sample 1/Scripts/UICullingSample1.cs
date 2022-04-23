using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICullingSample1 : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text m_Text;

    void OnBecameVisible()
    {
        m_Text.text = $"{name}\nInside Viewport";
        Debug.Log($"{name}: OnBecameVisible", this);
    }

    void OnBecameInvisible()
    {
        m_Text.text = $"{name}\nOutside Viewport";
        Debug.Log($"{name}: OnBecameInvisible", this);
    }
}
