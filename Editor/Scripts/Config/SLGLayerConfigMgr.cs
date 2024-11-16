
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGLayerConfigMgr
    {
        /// <summary>
        /// SLG层配置路径
        /// </summary>
        static string SLG_LAYER_CONFIG_ABSOLUTE_PATH = Application.dataPath + "/../Packages/com.lingren.slg/Editor/Excel/SLGLayer_WPS.csv";

        /// <summary>
        /// 
        /// </summary>
        static SLGLayerConfigMgr s_Instance;

        /// <summary>
        /// 
        /// </summary>
        public static SLGLayerConfigMgr S
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SLGLayerConfigMgr();
                }

                return s_Instance;
            }
        }

        /// <summary>
        /// 有效信息开始行索引
        /// </summary>
        const int TABLE_INFO_ROW_START_INDEX = 2;

        /// <summary>
        /// 
        /// </summary>
        List<SLGLayerConfig> m_LayerCfgList = new List<SLGLayerConfig>();

        /// <summary>
        /// 
        /// </summary>
        List<SLGLayerConfig> m_RenderLayerCfgList = new List<SLGLayerConfig>();

        /// <summary>
        /// 
        /// </summary>
        List<SLGLayerConfig> m_InfoLayerCfgList = new List<SLGLayerConfig>();

        /// <summary>
        /// 
        /// </summary>
        public List<SLGLayerConfig> renderLayerCfgList
        {
            get { return m_RenderLayerCfgList; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<SLGLayerConfig> infoLayerCfgList
        {
            get { return m_InfoLayerCfgList; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public SLGLayerConfig GetRenderDynamicLayer(string layerName)
        {
            foreach (var layer in m_RenderLayerCfgList)
            {
                if (layer == null)
                    continue;

                if (!layer.renderLayerIsDynamic)
                    continue;

                if (layer.layerName == layerName)
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public SLGLayerConfig GetRenderLayer(string layerName)
        {
            foreach(var layer in m_RenderLayerCfgList)
            {
                if (layer == null)
                    continue;

                if (layer.renderLayerIsDynamic)
                {
                    if (layer.layerName.Contains(layerName))
                    {
                        return layer;
                    }
                }
                else
                {
                    if (layer.layerName == layerName)
                    {
                        return layer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDefaultConfig()
        {
            var configPath = SLG_LAYER_CONFIG_ABSOLUTE_PATH;
            LoadConfig(configPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configPath"></param>
        public void LoadConfig(string configPath)
        {
            m_LayerCfgList.Clear();

            var strList = SLGUtils.ReadCsv(configPath);
            if (strList.Count <= 0)
                return;

            for (int i = TABLE_INFO_ROW_START_INDEX; i < strList.Count; i++)
            {
                var strArray = strList[i];
                if (string.IsNullOrEmpty(strArray[0]))
                    continue;

                SLGLayerConfig cfg = new SLGLayerConfig();
                cfg.LoadConfig(strArray);
                m_LayerCfgList.Add(cfg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            AnalyseLayerList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetRenderLayerNum()
        {
            return m_RenderLayerCfgList.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        void AnalyseLayerList()
        {
            // .
            m_RenderLayerCfgList.Clear();
            m_InfoLayerCfgList.Clear();

            foreach (var cfg in m_LayerCfgList)
            {
                if (cfg == null)
                    continue;

                if (cfg.isInfoLayer)
                {
                    m_InfoLayerCfgList.Add(cfg);
                }
                else
                {
                    m_RenderLayerCfgList.Add(cfg);
                }
            }

            m_RenderLayerCfgList.Sort(SortLayer);
            m_InfoLayerCfgList.Sort(SortLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static int SortLayer(SLGLayerConfig left, SLGLayerConfig right)
        {
            var leftIdx = left.layerID;
            var rightIdx = right.layerID;

            if (leftIdx < rightIdx)
            {
                return -1;
            }

            return 1;
        }
    }
}

