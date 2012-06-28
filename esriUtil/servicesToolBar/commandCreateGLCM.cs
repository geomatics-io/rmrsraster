using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using esriUtil;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandCreateGLCM : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCreateGLCM()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Texture.frmCreateGlcmSurface glcmFrm = new esriUtil.Forms.Texture.frmCreateGlcmSurface(map);
            glcmFrm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
