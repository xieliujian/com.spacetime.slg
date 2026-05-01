
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// SLG 层配置管理器：自 Excel 加载渲染层与信息层行配置，并拆分为渲染层列表与信息层列表供编辑工具与运行时使用。
    /// </summary>
    public class SLGLayerConfigMgr
    {
        static SLGLayerConfigMgr s_Instance;

        /// <summary>
        /// 单例访问器，首次访问时创建实例。
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

        List<SLGLayerConfig> m_LayerCfgList = new List<SLGLayerConfig>();

        List<SLGLayerConfig> m_RenderLayerCfgList = new List<SLGLayerConfig>();

        List<SLGLayerConfig> m_InfoLayerCfgList = new List<SLGLayerConfig>();

        /// <summary>
        /// 仅含非信息层的渲染层，按 <c>layerID</c> 升序；含静态与动态子层项。
        /// </summary>
        public List<SLGLayerConfig> renderLayerCfgList
        {
            get { return m_RenderLayerCfgList; }
        }

        /// <summary>
        /// 仅含信息层项，按 <c>layerID</c> 升序。
        /// </summary>
        public List<SLGLayerConfig> infoLayerCfgList
        {
            get { return m_InfoLayerCfgList; }
        }

        /// <summary>
        /// 在渲染层配置中按「动态子根」名查找：用于动态子地图挂点与流式增删子节点时匹配配置行。
        /// </summary>
        /// <param name="layerName">子根或层名，通常与表内 layerName 一致</param>
        /// <returns>匹配项，未找到则返回 null</returns>
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
        /// 在渲染层列表中按名称查找；动态层时支持按包含关系匹配，静态层为精确匹配。
        /// </summary>
        /// <param name="layerName">层或子节点名</param>
        /// <returns>匹配的配置，未找到则 null</returns>
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
        /// 自默认路径 <see cref="SLG_LAYER_CONFIG_ABSOLUTE_PATH"/> 加载层配置表。
        /// </summary>
        public void LoadDefaultConfig()
        {
            LoadConfig(SLGEditDefine.s_SLGLayerConfig_AbsolutePath);
        }

        /// <summary>
        /// 从指定 Excel 文件路径重新加载并覆盖当前内存中的全量行列表（尚未拆分渲染/信息层时须再调用 <see cref="Init"/>）。
        /// </summary>
        /// <param name="configPath">Excel 文件绝对路径（.xlsx 或 .xls）</param>
        public void LoadConfig(string configPath)
        {
            m_LayerCfgList.Clear();

            DataTable table = ST.Core.ExcelUtils.ReadExcel(configPath, 0, SLGEditDefine.s_TableHeader_RowIndex, SLGEditDefine.s_TableData_RowStartIndex);
            if (table == null || table.Rows.Count <= 0)
                return;

            int colCount = table.Columns.Count;
            foreach (DataRow row in table.Rows)
            {
                var strArray = new string[colCount];
                for (int col = 0; col < colCount; col++)
                    strArray[col] = row[col]?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(strArray[0]))
                    continue;

                SLGLayerConfig cfg = new SLGLayerConfig();
                cfg.LoadConfig(strArray);
                m_LayerCfgList.Add(cfg);
            }
        }

        /// <summary>
        /// 在 <see cref="LoadConfig(string)"/> 或 <see cref="LoadDefaultConfig"/> 之后调用，将 <c>m_LayerCfgList</c> 按是否信息层拆成 <see cref="renderLayerCfgList"/> 与 <see cref="infoLayerCfgList"/> 并排序。
        /// </summary>
        public void Init()
        {
            AnalyseLayerList();
        }

        /// <summary>
        /// 返回当前已加载的渲染层（非信息层）行数，可用于编辑器中按行分配 Y 向基准偏移等。
        /// </summary>
        public int GetRenderLayerNum()
        {
            return m_RenderLayerCfgList.Count;
        }

        void AnalyseLayerList()
        {
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
