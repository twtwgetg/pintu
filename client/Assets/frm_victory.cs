using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class frm_victory : frmbase
{
    public RawImage img;
    public Button close;
    private void Awake()
    {
        Main.RegistEvent("show_next", (x)=>
        {
            Debug.Log("show_next");
            show();

            DrLevel leevel =  x as DrLevel;
            img.texture = Resources.Load(leevel.LevelFigure) as Texture2D;

            return null;
        });
        close.onClick.AddListener(()=>
        {
            Main.SendEvent("level_next");
            hide();
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
