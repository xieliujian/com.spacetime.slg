using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


#if ART_SCENE_PROJECT
using OfficeOpenXml;
#endif

namespace ST.SLG
{
    /// <summary>
    /// SLG 属性网格编辑器 Excel 导出（分部类），在开启 <c>ART_SCENE_PROJECT</c> 时将属性格数据写出为 xlsx。
    /// </summary>
    public partial class SLGPropertyGridEdit
    {

        /// <summary>
        /// 将全图属性格导出为 Excel 文件到指定场景目录下（需定义 <c>ART_SCENE_PROJECT</c> 并引用 EPPlus，否则无操作）。
        /// </summary>
        /// <param name="sceneDB">场景数据库。</param>
        /// <param name="sceneDir">场景所在目录（用于拼接输出路径）。</param>
        public static void ExportExcel(SLGSceneDB sceneDB, string sceneDir)
        {
#if ART_SCENE_PROJECT
            ExportPropertyGridExcel(sceneDB, sceneDir);
#endif
        }

#if ART_SCENE_PROJECT

        static void ExportPropertyGridExcel(SLGSceneDB sceneDB, string sceneDir)
        {
            string filePath = string.Format("{0}/{1}", sceneDir, SLGEditDefine.SLG_PROPERTY_EXCEL_FILENAME);

            FileInfo file = new FileInfo(filePath);
            if (file == null)
                return;

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(filePath);
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                //Add the headers  
                worksheet.Cells[1, 1].Value = "序列";
                worksheet.Cells[1, 2].Value = "坐标X";
                worksheet.Cells[1, 3].Value = "坐标Y";
                worksheet.Cells[1, 4].Value = "选择类型";
                worksheet.Cells[1, 5].Value = "资源等级";

                worksheet.Cells[2, 1].Value = "CS";
                worksheet.Cells[2, 2].Value = "CS";
                worksheet.Cells[2, 3].Value = "CS";
                worksheet.Cells[2, 4].Value = "CS";
                worksheet.Cells[2, 5].Value = "CS";

                worksheet.Cells[3, 1].Value = "int";
                worksheet.Cells[3, 2].Value = "int";
                worksheet.Cells[3, 3].Value = "int";
                worksheet.Cells[3, 4].Value = "int";
                worksheet.Cells[3, 5].Value = "int";

                worksheet.Cells[4, 1].Value = "Id";
                worksheet.Cells[4, 2].Value = "CoordX";
                worksheet.Cells[4, 3].Value = "CoordY";
                worksheet.Cells[4, 4].Value = "SelType";
                worksheet.Cells[4, 5].Value = "ResLv";

                int column = 0;

                for (int j = 1; j <= SLGDefine.s_SLG_Grid_VerticalNum; j++)
                {
                    for (int i = 1; i <= SLGDefine.s_SLG_Grid_HorizontalNum; i++)
                    {
                        Vector2Int propPos = new Vector2Int(i, j);

                        var propertyDB = sceneDB.FindPropertyGridDB(propPos);
                        if (propertyDB == null)
                            continue;

                        column++;
                        int newColumn = 4 + column;
                        worksheet.Cells[newColumn, 1].Value = propertyDB.index;
                        worksheet.Cells[newColumn, 2].Value = propertyDB.pos.x;
                        worksheet.Cells[newColumn, 3].Value = propertyDB.pos.y;
                        worksheet.Cells[newColumn, 4].Value = propertyDB.selType;
                        worksheet.Cells[newColumn, 5].Value = propertyDB.resLvType;
                    }
                }

                package.Save();
            }
        }

#endif



    }
}
