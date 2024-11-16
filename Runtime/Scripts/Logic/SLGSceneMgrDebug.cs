using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGSceneMgr
    {
        /// <summary>
        /// 
        /// </summary>
        public const string GM_CMD_STR = "slgdebug";

        /// <summary>
        /// 
        /// </summary>
        const string CMD_STR_DYNAMIC_MAP_INDEX = "DynamicMapIndex";
        const string CMD_STR_ADD_AREA_GRID_INFO = "AddAreaGridInfo";
        const string CMD_STR_ADD_AREA_GRID_INFO_LIST = "AddAreaGridInfoList";
        const string CMD_STR_REMOVE_AREA_GRID_INFO = "RemoveAreaGridInfo";
        const string CMD_STR_REMOVE_AREA_GRID_INFO_LIST = "RemoveAreaGridInfoList";
        const string CMD_STR_ADD_SCENE_LINE_INFO = "AddSceneLineInfo";
        const string CMD_STR_REMOVE_SCENE_LINE_INFO = "RemoveSceneLineInfo";
        const string CMD_STR_ADD_SCENE_LINE_INFO_LIST = "AddSceneLineInfoList";
        const string CMD_STR_REMOVE_SCENE_LINE_INFO_LIST = "RemoveSceneLineInfoList";
        const string CMD_STR_SET_AREA_PROPERTY_LAYER_VISIBLE = "SetAreaPropertyLayerVisible";
        const string CMD_STR_SUBMIT_AREA_INFO = "SubmitAreaInfo";
        const string CMD_STR_INST_GPU_SKIN = "InstGpuSkin";
        const string CMD_STR_INST_GPU_SKIN_LIST = "InstGpuSkinList";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        public void DebugCmd(string _cmd, string _val)
        {
            // slgdebug DynamicMapIndex 1
            // slgdebug AddAreaGridInfo 5,1,1,1,1,1,1
            // slgdebug AddAreaGridInfoList 5,1,1,100,100,1,1,1,1
            // slgdebug RemoveAreaGridInfo 5,1,1
            // slgdebug RemoveAreaGridInfoList 5,1,1,100,100
            // slgdebug AddSceneLineInfo 1,1,1,10,10,0
            // slgdebug RemoveSceneLineInfo 1
            // slgdebug AddSceneLineInfoList 500
            // slgdebug RemoveSceneLineInfoList 500
            // slgdebug SetAreaPropertyLayerVisible 10,1
            // slgdebug SubmitAreaInfo 2
            // slgdebug InstGpuSkin n601_gpuskin1.prefab,-80,2,-80
            // slgdebug InstGpuSkinList n601_gpuskin1.prefab,200,-100,-100,-50,-50

            if (_cmd == CMD_STR_DYNAMIC_MAP_INDEX)
            {
                DynamicMapIndexDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_ADD_AREA_GRID_INFO)
            {
                AddAreaGridInfoDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_ADD_AREA_GRID_INFO_LIST)
            {
                AddAreaGridInfoListDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_REMOVE_AREA_GRID_INFO)
            {
                RemoveAreaGridInfoDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_REMOVE_AREA_GRID_INFO_LIST)
            {
                RemoveAreaGridInfoListDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_ADD_SCENE_LINE_INFO)
            {
                AddSceneLineInfoDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_REMOVE_SCENE_LINE_INFO)
            {
                RemoveSceneLineInfoDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_ADD_SCENE_LINE_INFO_LIST)
            {
                AddSceneLineInfoListDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_REMOVE_SCENE_LINE_INFO_LIST)
            {
                RemoveSceneLineInfoListDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_SET_AREA_PROPERTY_LAYER_VISIBLE)
            {
                SetAreaPropertyLayerVisibleDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_SUBMIT_AREA_INFO)
            {
                SubmitAreaInfoDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_INST_GPU_SKIN)
            {
                InstGpuSkinDebugCmd(_cmd, _val);
            }
            else if (_cmd == CMD_STR_INST_GPU_SKIN_LIST)
            {
                InstGpuSkinListDebugCmd(_cmd, _val);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void SubmitAreaInfoDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 1)
                return;

            var layerID = int.Parse(infoArray[0]);
            m_Scene.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void InstGpuSkinListDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null)
                return;

            var prefabName = infoArray[0];
            var bundlePath = SLGWarp.S.warp.GetPrefabFullName(prefabName);

            var instNum = int.Parse(infoArray[1]);

            Vector3 startPos = new Vector3();
            startPos.x = float.Parse(infoArray[2]);
            startPos.z = float.Parse(infoArray[3]);

            Vector3 endPos = new Vector3();
            endPos.x = float.Parse(infoArray[4]);
            endPos.z = float.Parse(infoArray[5]);

            SLGWarp.S.warp.StartLoadRes(() =>
            {
                GameObject template = SLGWarp.S.warp.GetResource(prefabName) as GameObject;
                if (template == null)
                    return;

                for (int i = 0; i < instNum; i++)
                {
                    var instGo = GameObject.Instantiate(template);
                    if (instGo == null)
                        return;

                    var posX = Random.Range(startPos.x, endPos.x);
                    var posZ = Random.Range(startPos.z, endPos.z);
                    instGo.transform.position = new Vector3(posX, 0f, posZ);
                    instGo.transform.localEulerAngles = Vector3.zero;
                    instGo.transform.localScale = Vector3.one;
                }

            }, bundlePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void InstGpuSkinDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 4)
                return;

            var prefabName = infoArray[0];
            var bundlePath = SLGWarp.S.warp.GetPrefabFullName(prefabName);

            Vector3 pos = new Vector3();
            pos.x = float.Parse(infoArray[1]);
            pos.y = float.Parse(infoArray[2]);
            pos.z = float.Parse(infoArray[3]);

            SLGWarp.S.warp.StartLoadRes(() =>
            {
                GameObject template = SLGWarp.S.warp.GetResource(prefabName) as GameObject;
                if (template == null)
                    return;

                var instGo = GameObject.Instantiate(template);
                if (instGo == null)
                    return;

                instGo.transform.position = pos;
                instGo.transform.localEulerAngles = Vector3.zero;
                instGo.transform.localScale = Vector3.one;

            }, bundlePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void RemoveSceneLineInfoDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 1)
                return;

            uint uniqueID = uint.Parse(infoArray[0]);

            RemoveSceneLineInfo(uniqueID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void SetAreaPropertyLayerVisibleDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 2)
                return;

            int layerID = int.Parse(infoArray[0]);
            bool visible = int.Parse(infoArray[1]) > 0 ? true : false;

            m_Scene.SetAreaPropertyLayerVisible(layerID, visible);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void RemoveSceneLineInfoListDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 1)
                return;

            uint totalNum = uint.Parse(infoArray[0]);

            for (int i = 0; i < totalNum; i++)
            {
                RemoveSceneLineInfo((uint)i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void AddSceneLineInfoDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 6)
                return;

            uint uniqueID = uint.Parse(infoArray[0]);

            Vector2Int startPos = new Vector2Int();
            startPos.x = int.Parse(infoArray[1]);
            startPos.y = int.Parse(infoArray[2]);

            Vector2Int endPos = new Vector2Int();
            endPos.x = int.Parse(infoArray[3]);
            endPos.y = int.Parse(infoArray[4]);

            bool isEnemy = int.Parse(infoArray[5]) > 0 ? true : false;

            AddSceneLineInfo(uniqueID, startPos, endPos, isEnemy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void AddSceneLineInfoListDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 1)
                return;

            uint totalNum = uint.Parse(infoArray[0]);

            for (int i = 0; i < totalNum; i++)
            {
                Vector2Int startPos = new Vector2Int();
                startPos.x = Random.Range(1, 51);
                startPos.y = Random.Range(1, 51);

                Vector2Int endPos = new Vector2Int();
                endPos.x = Random.Range(1, 51);
                endPos.y = Random.Range(1, 51);

                bool isEnemy = Random.Range(0, 2) > 0 ? true : false;

                AddSceneLineInfo((uint)i, startPos, endPos, isEnemy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void RemoveAreaGridInfoListDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 5)
                return;

            int layerID = int.Parse(infoArray[0]);

            Vector2Int startPos = new Vector2Int();
            startPos.x = int.Parse(infoArray[1]);
            startPos.y = int.Parse(infoArray[2]);

            Vector2Int endPos = new Vector2Int();
            endPos.x = int.Parse(infoArray[3]);
            endPos.y = int.Parse(infoArray[4]);

            for (int i = startPos.x; i < endPos.x; i++)
            {
                for (int j = startPos.y; j < endPos.y; j++)
                {
                    var logicPos = new Vector2Int(i, j);
                    m_Scene.RemoveAreaGridInfo(layerID, logicPos);
                }
            }

            m_Scene.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void RemoveAreaGridInfoDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 3)
                return;

            int layerID = int.Parse(infoArray[0]);

            Vector2Int logicPos = new Vector2Int();
            logicPos.x = int.Parse(infoArray[1]);
            logicPos.y = int.Parse(infoArray[2]);

            m_Scene.RemoveAreaGridInfo(layerID, logicPos);
            m_Scene.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void AddAreaGridInfoListDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 9)
                return;

            int layerID = int.Parse(infoArray[0]);

            Vector2Int startPos = new Vector2Int();
            startPos.x = int.Parse(infoArray[1]);
            startPos.y = int.Parse(infoArray[2]);

            Vector2Int endPos = new Vector2Int();
            endPos.x = int.Parse(infoArray[3]);
            endPos.y = int.Parse(infoArray[4]);

            UnityEngine.Color color = new UnityEngine.Color();
            color.r = float.Parse(infoArray[5]);
            color.g = float.Parse(infoArray[6]);
            color.b = float.Parse(infoArray[7]);
            color.a = float.Parse(infoArray[8]);

            for (int i = startPos.x; i < endPos.x; i++)
            {
                for (int j = startPos.y; j < endPos.y; j++)
                {
                    var logicPos = new Vector2Int(i, j);
                    m_Scene.AddAreaGridInfo(layerID, logicPos, color);
                }
            }

            m_Scene.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void AddAreaGridInfoDebugCmd(string _cmd, string _val)
        {
            var infoArray = _val.Split(',');
            if (infoArray == null || infoArray.Length != 7)
                return;

            int layerID = int.Parse(infoArray[0]);

            Vector2Int logicPos = new Vector2Int();
            logicPos.x = int.Parse(infoArray[1]);
            logicPos.y = int.Parse(infoArray[2]);

            UnityEngine.Color color = new UnityEngine.Color();
            color.r = float.Parse(infoArray[3]);
            color.g = float.Parse(infoArray[4]);
            color.b = float.Parse(infoArray[5]);
            color.a = float.Parse(infoArray[6]);

            m_Scene.AddAreaGridInfo(layerID, logicPos, color);
            m_Scene.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <param name="_val"></param>
        void DynamicMapIndexDebugCmd(string _cmd, string _val)
        {
            var dynamicMapIndex = int.Parse(_val);
            SetDynamicMapIndex(dynamicMapIndex);
        }
    }
}

