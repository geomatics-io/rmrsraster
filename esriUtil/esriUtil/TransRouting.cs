using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;


namespace esriUtil
{
    public class TransRouting
    {
        public enum SpeedUnits {MPH, KPH}
        public TransRouting(IWorkspace outworkspace, IFunctionRasterDataset DEM, SpeedUnits spUnits = SpeedUnits.KPH)
        {
            Dem = DEM;
            sp = DEM.RasterInfo.SpatialReference;
            OutWorkspace = outworkspace;
            Units = spUnits;
            getConversionFactor();
        }
    
        public TransRouting(IWorkspace outworkspace, IFeatureClass facility, IFeatureClass roads, string roadsSpeedField,IFunctionRasterDataset DEM, float onRoadMachRate,float onRoadMachPayLoad,float offRoadSpeed,float offRoadMachRate, float offRoadPayLoad, IFeatureClass Barriers=null, float operationsCost=0,float otherCosts = 0, SpeedUnits spUnits= SpeedUnits.KPH )
        {
            FacilityFeatureClass = facility;
            RoadFeatureClass = roads;
            BarriersFeatureClass = Barriers;
            RoadsSpeedField = roadsSpeedField;
            Dem = DEM;
            sp = DEM.RasterInfo.SpatialReference;
            OnRoadMachineRate = onRoadMachRate;
            OnRoadPayLoad = onRoadMachPayLoad;
            OffRoadSpeed = offRoadSpeed;
            OffRoadMachineRate = offRoadMachRate;
            OffRoadPayLoad = offRoadPayLoad;
            OperationsCost = operationsCost;
            OtherCost = otherCosts;
            OutWorkspace = outworkspace;
            Units = spUnits;
            getConversionFactor();
            
        }

        
        ~TransRouting()
        {
            if (weChecked)
            {
                aoInitialize.CheckInExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }
        }
        rasterUtil rsUtil = new rasterUtil();
        geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        public IFeatureClass FacilityFeatureClass{get;set;}
        IFeatureClass rdsFtrCls = null;
        public IFeatureClass RoadFeatureClass { get { return rdsFtrCls; } set { rdsFtrCls = value; } }
        IFeatureClass barFtr = null;
        public IFeatureClass BarriersFeatureClass { get { return barFtr; } set { barFtr = value; } }
        public IWorkspace OutWorkspace { get; set; }
        public SpeedUnits Units { get; set; }
        string rdSpeedfld = null;
        public string RoadsSpeedField { get { return rdSpeedfld; } set { rdSpeedfld = value; } }//dist/hour
        float onRoadMachineRate;//$/hour
        public float OnRoadMachineRate { get { return onRoadMachineRate; } set { onRoadMachineRate = value; } }
        IFunctionRasterDataset onRoadMachineRateRs = null;
        public IFunctionRasterDataset OnRoadMachineRateRaster { get { return onRoadMachineRateRs; } set { onRoadMachineRateRs = value; } }
        float onRoadPayLoad;//tons
        public float OnRoadPayLoad { get { return onRoadPayLoad; } set { onRoadPayLoad = value; } }
        float offRoadSp;//dist/hour
        public float OffRoadSpeed { get { return offRoadSp; } set { offRoadSp = value; } }
        float offRoadMachineRate;//$/hour
        public float OffRoadMachineRate { get { return offRoadMachineRate; } set { offRoadMachineRate = value; } }
        IFunctionRasterDataset offRoadMachineRateRs = null;
        public IFunctionRasterDataset OffRoadMachineRateRaster { get { return offRoadMachineRateRs; } set { offRoadMachineRateRs = value; } }
        float offRoadPay;//tons
        public float OffRoadPayLoad { get { return offRoadPay; } set { offRoadPay = value; } }
        float opsCost;//$/ton
        public float OperationsCost { get { return opsCost; } set { opsCost = value; } }
        IFunctionRasterDataset opsCostRs = null;
        public IFunctionRasterDataset OperationsCostRaster { get { return opsCostRs; } set { opsCostRs = value; } }
        float oCost;//$/ton
        public float OtherCost { get { return oCost; } set { oCost = value; } }
        IFunctionRasterDataset oCostRs = null;
        public IFunctionRasterDataset OtherCostRaster { get { return oCostRs; } set { oCostRs = value; } }
        public IFunctionRasterDataset Dem { get; set; }
        IFunctionRasterDataset fapd = null;
        public IFunctionRasterDataset FunctionAccumulatedPathDistance { get { return fapd; } set { fapd = value; } }
        IFunctionRasterDataset fapa = null;
        public IFunctionRasterDataset FunctionAccumulatedPathAllocation { get { return fapa; } set { fapa = value; } }
        IFunctionRasterDataset fafpd = null;
        public IFunctionRasterDataset FunctionAccumulatedFromPathDistance { get { return fafpd; } set { fafpd = value; } }
        IFunctionRasterDataset rdRs = null;
        public IFunctionRasterDataset RoadRaster { get { return rdRs; } set { rdRs = value; } }
        ESRI.ArcGIS.esriSystem.IAoInitialize aoInitialize = new ESRI.ArcGIS.esriSystem.AoInitializeClass();
        private ISpatialReference sp = null;
        bool weChecked = false;
        float speedConFactor=1;
        private void getConversionFactor()
        {
            try
            {
                IProjectedCoordinateSystem pj = (IProjectedCoordinateSystem)Dem.RasterInfo.SpatialReference;
                double conFactor = pj.CoordinateUnit.ConversionFactor;
                if (conFactor == 1) 
                {
                    if (Units == SpeedUnits.KPH)
                    {
                        speedConFactor = 1000f;
                    }
                    else
                    {
                        speedConFactor = 5280f * 0.3048f;

                    }
                }
                else
                {
                    if (Units == SpeedUnits.KPH)
                    {
                        speedConFactor = 1000f * 3.28084f;
                    }
                    else
                    {
                        speedConFactor = 5280f;

                    }
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Not projected");
            }
        }
        private void getSpatialAnalystLicense()
        {
            
            bool checkedOut = aoInitialize.IsExtensionCheckedOut(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            if (!checkedOut)
            {
                weChecked = true;
                aoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }
        }
        private void convertFeatureClass()
        {
            if (RoadRaster == null)
            {
                IFeatureClassDescriptor ftrDesc = new FeatureClassDescriptorClass();
                IQueryFilter qf = new QueryFilterClass();
                ftrDesc.Create(RoadFeatureClass, qf, RoadsSpeedField);
                IConversionOp convOp = new RasterConversionOpClass();
                IRasterAnalysisEnvironment rasterAnalysisEnvironment = (IRasterAnalysisEnvironment)convOp;
                IRasterAnalysisGlobalEnvironment rasterAnalysisGlobalEnvironment = (IRasterAnalysisGlobalEnvironment)convOp;
                rasterAnalysisGlobalEnvironment.AvoidDataConversion = true;
                rasterAnalysisEnvironment.OutSpatialReference = sp;
                rasterAnalysisEnvironment.OutWorkspace = OutWorkspace;
                //object cells = Dem.RasterInfo.CellSize;
                object ext = ((IGeoDataset)RoadFeatureClass).Extent;
                //object snap = ((IGeoDataset)Dem);
                rasterAnalysisEnvironment.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, Dem);
                rasterAnalysisEnvironment.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ext, Dem);
                string outRdName = rsUtil.getSafeOutputName(OutWorkspace, "sRoads");
                IRasterDataset geoDset = convOp.ToRasterDataset((IGeoDataset)ftrDesc, "GRID", OutWorkspace, outRdName);
                IGeoDatasetSchemaEdit2 geoSch = (IGeoDatasetSchemaEdit2)geoDset;
                if (geoSch.CanAlterSpatialReference) geoSch.AlterSpatialReference(rasterAnalysisEnvironment.OutSpatialReference);
                RoadRaster = rsUtil.createIdentityRaster(geoDset);
            }

        }
        private void buildPathRasters()
        {
            getSpatialAnalystLicense();
            if (FunctionAccumulatedPathAllocation == null)
            {
                buildPathAllocation();
            }
            //calculate costs
            IFunctionRasterDataset onRdCs = null;
            IFunctionRasterDataset offRdCs = null;
            if (OnRoadMachineRateRaster == null)
            {
                float dollarPerTonConv = (OnRoadMachineRate / onRoadPayLoad) * 0.001f;// * 1000;
                onRdCs = rsUtil.calcArithmaticFunction(FunctionAccumulatedPathAllocation, dollarPerTonConv, esriRasterArithmeticOperation.esriRasterMultiply);
            }
            else
            {
                IFunctionRasterDataset dollarPerTonConv = rsUtil.calcArithmaticFunction(OnRoadMachineRateRaster, onRoadPayLoad * 1000f, esriRasterArithmeticOperation.esriRasterDivide);
                onRdCs = rsUtil.calcArithmaticFunction(FunctionAccumulatedPathAllocation, dollarPerTonConv, esriRasterArithmeticOperation.esriRasterMultiply);
            }
            if (OffRoadMachineRateRaster == null)
            {
                float offDollarPerTonConv = (OffRoadMachineRate / OffRoadPayLoad);
                offRdCs = rsUtil.calcArithmaticFunction(FunctionAccumulatedFromPathDistance, offDollarPerTonConv, esriRasterArithmeticOperation.esriRasterMultiply);
            }
            else
            {
                IFunctionRasterDataset offDollarPerTonConv = rsUtil.calcArithmaticFunction(OffRoadMachineRateRaster, OffRoadPayLoad, esriRasterArithmeticOperation.esriRasterDivide);
                offRdCs = rsUtil.calcArithmaticFunction(FunctionAccumulatedFromPathDistance, offDollarPerTonConv, esriRasterArithmeticOperation.esriRasterMultiply);
            }
            IFunctionRasterDataset t0 = rsUtil.calcArithmaticFunction(onRdCs, offRdCs, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset t1 = rsUtil.calcArithmaticFunction(t0, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset t2 = null;
            IFunctionRasterDataset transCost = null;
            if (OperationsCostRaster == null)
            {
                t2 = rsUtil.calcArithmaticFunction(t1, OperationsCost, esriRasterArithmeticOperation.esriRasterPlus);
            }
            else
            {
                t2 = rsUtil.calcArithmaticFunction(t1, OperationsCostRaster, esriRasterArithmeticOperation.esriRasterPlus);
            }
            if (OtherCostRaster == null)
            {
                transCost = rsUtil.calcArithmaticFunction(t2, OtherCost, esriRasterArithmeticOperation.esriRasterPlus);
            }
            else
            {
                transCost = rsUtil.calcArithmaticFunction(t2, OtherCostRaster, esriRasterArithmeticOperation.esriRasterPlus);
            }
            outDTR = transCost;
        }

        private void buildPathAllocation()
        {
            IDistanceOp2 dOp = new RasterDistanceOpClass();
            IRasterAnalysisEnvironment rasterAnalysisEnvironment = (ESRI.ArcGIS.GeoAnalyst.IRasterAnalysisEnvironment)dOp;
            rasterAnalysisEnvironment.OutSpatialReference = ((IGeoDataset)Dem).SpatialReference;
            rasterAnalysisEnvironment.OutWorkspace = OutWorkspace;
            object ext = ((IGeoDataset)RoadFeatureClass).Extent;
            object snap = ((IGeoDataset)Dem);
            rasterAnalysisEnvironment.SetCellSize(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, Dem);
            rasterAnalysisEnvironment.SetExtent(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, ext, Dem);
            IRasterAnalysisGlobalEnvironment analysisGlobalEnv = (IRasterAnalysisGlobalEnvironment)dOp;
            analysisGlobalEnv.AvoidDataConversion = true;

            IRasterOpBase rasOpBase = (IRasterOpBase)dOp;
            IRasterDatasetName rasDatasetName;
            rasDatasetName = new RasterDatasetNameClass();

            IDatasetName datasetName;
            datasetName = (IDatasetName)rasDatasetName;
            datasetName.WorkspaceName = (IWorkspaceName)((IDataset)OutWorkspace).FullName;
            string onRoadNm = rsUtil.getSafeOutputName(OutWorkspace, "onRoadHr");
            datasetName.Name = onRoadNm;
            rasOpBase.AddOutputDatasetName(0, datasetName);

            object noVl = Type.Missing;
            IFunctionRasterDataset cr1 = rsUtil.calcArithmaticFunction(RoadRaster, speedConFactor, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset costRs = rsUtil.calcArithmaticFunction(1, cr1, esriRasterArithmeticOperation.esriRasterDivide);
            IGeoDataset rpdOut = dOp.PathDistance((IGeoDataset)FacilityFeatureClass, costRs, Dem, noVl, noVl, noVl, noVl, noVl, noVl);//outputs in hours
            FunctionAccumulatedPathDistance = rsUtil.createIdentityRaster((IRaster)rpdOut);
            // Use FunctonAccumulated Path Distance (hours) for allocation
            string frRoadDist = rsUtil.getSafeOutputName(OutWorkspace, "offRoadHr");
            datasetName.Name = frRoadDist;

            string frRoadAllo = rsUtil.getSafeOutputName(OutWorkspace, "onRoadAllo");
            IRasterDatasetName rasDatasetName2 = new RasterDatasetNameClass();
            IDatasetName datasetName2 = (IDatasetName)rasDatasetName2;
            datasetName2.WorkspaceName = (IWorkspaceName)((IDataset)OutWorkspace).FullName;
            datasetName2.Name = frRoadAllo;

            string frRoadBack = rsUtil.getSafeOutputName(OutWorkspace, "offRoadBack");
            IRasterDatasetName rasDatasetName3 = new RasterDatasetNameClass();
            IDatasetName datasetName3 = (IDatasetName)rasDatasetName3;
            datasetName3.WorkspaceName = (IWorkspaceName)((IDataset)OutWorkspace).FullName;
            datasetName3.Name = frRoadBack;
            
            rasOpBase.AddOutputDatasetName(0, datasetName);
            rasOpBase.AddOutputDatasetName(1, datasetName3);
            rasOpBase.AddOutputDatasetName(2, datasetName2);
            IFunctionRasterDataset costRS = null;
            //add in barriers to costRS
            if (OffRoadSpeedRaster != null)
            {
                IFunctionRasterDataset c1 = rsUtil.calcArithmaticFunction(OffRoadSpeedRaster, speedConFactor, esriRasterArithmeticOperation.esriRasterMultiply);
                costRS = rsUtil.calcArithmaticFunction(1, c1, esriRasterArithmeticOperation.esriRasterDivide);
            }
            else
            {
                costRS = rsUtil.constantRasterFunction(FunctionAccumulatedPathDistance, 1 / (OffRoadSpeed * speedConFactor));
            }
            
            //add in barriers to costRs
            IFunctionRasterDataset rdCs = null;//rsUtil.calcArithmaticFunction(FunctionAccumulatedPathDistance, 1000, esriRasterArithmeticOperation.esriRasterMultiply, rstPixelType.PT_LONG);
            if (BarriersFeatureClass != null)
            {
                IFunctionRasterDataset[] rslt = addBarrierRasters(costRS, FunctionAccumulatedPathDistance);
                costRS = rslt[0];
                rdCs = rslt[1];
            }
            else
            {
                rdCs = rsUtil.calcArithmaticFunction(FunctionAccumulatedPathDistance, 1000, esriRasterArithmeticOperation.esriRasterMultiply, rstPixelType.PT_LONG);
            }
            IGeoDataset rpdOut2 = dOp.PathDistanceFull((IGeoDataset)rdCs, true, false, true, costRS, Dem, noVl, noVl, noVl, noVl, noVl, rdCs);
            IRasterBandCollection rsbc = (IRasterBandCollection)rpdOut;
            FunctionAccumulatedFromPathDistance = rsUtil.getBand((IRaster)rpdOut2, 0);
            FunctionAccumulatedPathAllocation = rsUtil.getBand((IRaster)rpdOut2, 1);
        }

        private IFunctionRasterDataset[] addBarrierRasters(object costRS, IFunctionRasterDataset rdCs)
        {
            IFunctionRasterDataset[] output = new IFunctionRasterDataset[2];
            IFunctionRasterDataset b1 = createBarrierRaster();
            IFunctionRasterDataset c1 = rsUtil.calcLessFunction(b1, 0);
            IFunctionRasterDataset c2 = rsUtil.setnullToValueFunction(c1, 1);
            IFunctionRasterDataset c3 = rsUtil.setNullValue(c2, 0);
            output[0] = rsUtil.calcArithmaticFunction(costRS, c3, esriRasterArithmeticOperation.esriRasterMultiply);
            //multiple by 1000
            IFunctionRasterDataset b2 = rsUtil.calcGreaterEqualFunction(b1, 0);
            IFunctionRasterDataset b3 = rsUtil.setnullToValueFunction(b2, 1000);
            IFunctionRasterDataset barRast = rsUtil.setNullValue(b3, 1);
            output[1] = rsUtil.calcArithmaticFunction(rdCs, barRast, esriRasterArithmeticOperation.esriRasterMultiply, rstPixelType.PT_LONG);
            return output;
        }

        private IFunctionRasterDataset createBarrierRaster()
        {
            IFeatureClassDescriptor ftrDesc = new FeatureClassDescriptorClass();
            IQueryFilter qf = new QueryFilterClass();
            ftrDesc.Create(BarriersFeatureClass, qf, BarriersFeatureClass.OIDFieldName);
            IConversionOp convOp = new RasterConversionOpClass();
            IRasterAnalysisEnvironment rasterAnalysisEnvironment = (IRasterAnalysisEnvironment)convOp;
            IRasterAnalysisGlobalEnvironment rasterAnalysisGlobalEnvironment = (IRasterAnalysisGlobalEnvironment)convOp;
            rasterAnalysisGlobalEnvironment.AvoidDataConversion = true;
            rasterAnalysisEnvironment.OutSpatialReference = sp;
            rasterAnalysisEnvironment.OutWorkspace = OutWorkspace;
            //object cells = Dem.RasterInfo.CellSize;
            object ext = ((IGeoDataset)Dem).Extent;
            object snap = ((IGeoDataset)Dem);
            rasterAnalysisEnvironment.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, Dem);
            rasterAnalysisEnvironment.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ext, Dem);
            string outRdName = rsUtil.getSafeOutputName(OutWorkspace, "sBarrier");
            IRasterDataset geoDset = convOp.ToRasterDataset((IGeoDataset)ftrDesc, "GRID", OutWorkspace, outRdName);
            IGeoDatasetSchemaEdit2 geoSch = (IGeoDatasetSchemaEdit2)geoDset;
            if (geoSch.CanAlterSpatialReference) geoSch.AlterSpatialReference(rasterAnalysisEnvironment.OutSpatialReference);
            return rsUtil.createIdentityRaster(geoDset);
        }

        private IFunctionRasterDataset offRdCst = null;
        public IFunctionRasterDataset OffRoadSpeedRaster { get { return offRdCst; } set { offRdCst = value; } }//speed
        private IFunctionRasterDataset outDTR = null;
        public IFunctionRasterDataset OutDollarsTonsRaster
        {
            get
            {
                if (FunctionAccumulatedPathAllocation == null)
                {
                    convertFeatureClass();
                }
                buildPathRasters();
                return outDTR;
            }
        }
    }
}
