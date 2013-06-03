using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil
{
    public class segmentation
    {
        public segmentation(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        public segmentation()
        {
            rsUtil = new rasterUtil();
        }
        private rasterUtil rsUtil = null;
        public rasterUtil RasterUtility { get { return rsUtil; } } 
    }
}
