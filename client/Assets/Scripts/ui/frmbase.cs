using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frmbase : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isOpen()
    {
        return gb.gameObject.activeSelf;
    }
    protected Transform gb
    {
        get
        {
            return transform.Find("gb");
        }
    }
    public void show()
    {
        gb.gameObject.SetActive(true);
        if (Application.isPlaying)
        {
            OnShow();
        }
    }
    protected virtual void OnShow()
    {

    }
    protected virtual void OnHide()
    {

    }
    public virtual void hide()
    {
        OnHide();
        gb.gameObject.SetActive(false);
    }

    public void loadString()
    {
        var sx = GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        for (int i = 0; i < sx.Length; i++)
        {
            var x = sx[i];
            //var tx = x.GetComponent<stringinfo>();
            //if (tx == null)
            //{
            //    tx = x.gameObject.AddComponent<stringinfo>();
            //}
            //tx.getString();
        }
    }
}
