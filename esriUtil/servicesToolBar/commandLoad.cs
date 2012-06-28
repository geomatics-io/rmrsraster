using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;


namespace servicesToolBar
{
    public class commandLoad : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLoad()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            IActiveView av = (IActiveView)map;
            esriUtil.mapserviceutility mpServ = new esriUtil.mapserviceutility();
            ISpatialFilter spFlt = new SpatialFilter();
            spFlt.Geometry = (IGeometry)av.Extent;
            mpServ.getDbFtrClassesThatNeedUpdating(spFlt);
        }

        protected override void OnUpdate()
        {
        }
    }
}
