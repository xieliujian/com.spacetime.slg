using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 调试用：将材质基色在控制台以 HTML/Hex 形式输出，便于与 SLG 场景物体 Shader 的基色 id 对照。
    /// </summary>
    [ExecuteInEditMode]
    public class SLGTools_HexColor : MonoBehaviour
    {
        /// <summary>
        /// 要导出颜色的材质列表。
        /// </summary>
        [Header("材质")]
        public List<Material> matList = new List<Material>();

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// 遍历 <see cref="matList"/> 中 <see cref="SLGDefine.s_SLG_Shader_SceneObj_BaseColorId"/> 颜色，以 #RRGGBBAA 输出并做 TryParse 回读比对。
        /// </summary>
        public void DumpColorInfo()
        {
            if (matList == null)
                return;

            string dumpInfo = "";
            string revertInfo = "";

            foreach(var mat in matList)
            {
                if (mat == null)
                    continue;

                var color = mat.GetColor(SLGDefine.s_SLG_Shader_SceneObj_BaseColorId);

                var htmlColor = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
                dumpInfo += $"[{mat.name}] [Color] {htmlColor}\n";

                Color outColor;
                ColorUtility.TryParseHtmlString(htmlColor, out outColor);
                revertInfo += $"[{mat.name}] [Color] {outColor}\n";
            }

            ST.Core.Logging.Logger.LogError(dumpInfo);
            ST.Core.Logging.Logger.LogError(revertInfo);
        }
    }
}
