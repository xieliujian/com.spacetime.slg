using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ST.Core.Debugger;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class SLGTools_HexColor : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("≤ƒ÷ ¡–±Ì")]
        public List<Material> matList = new List<Material>();

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {

        }

        /// <summary>
        /// 
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

                var color = mat.GetColor(SLGDefine.SLG_SHADER_SCENEOBJ_BASECOLOR_ID);

                var htmlColor = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
                dumpInfo += $"[{mat.name}] [Color] {htmlColor}\n";

                Color outColor;
                ColorUtility.TryParseHtmlString(htmlColor, out outColor);
                revertInfo += $"[{mat.name}] [Color] {outColor}\n";
            }

            Debugger.LogError(dumpInfo);
            Debugger.LogError(revertInfo);
        }
    }
}
