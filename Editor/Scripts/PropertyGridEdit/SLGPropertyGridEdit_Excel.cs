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
    /// 
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
#if ART_SCENE_PROJECT

        /// <summary>
        /// 
        /// </summary>
        static string SLG_PROPERTY_EXCEL_FILENAME = "slg_scene_property.xlsx";

#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        public static void ExportExcel(SLGSceneDB sceneDB, string sceneDir)
        {
#if ART_SCENE_PROJECT
            ExportPropertyGridExcel(sceneDB, sceneDir);
#endif
        }

#if ART_SCENE_PROJECT

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void ExportPropertyGridExcel(SLGSceneDB sceneDB, string sceneDir)
        {
            string filePath = string.Format("{0}/{1}", sceneDir, SLG_PROPERTY_EXCEL_FILENAME);

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

                for (int j = 1; j <= SLGDefine.SLG_GRID_VERTICAL_NUM; j++)
                {
                    for (int i = 1; i <= SLGDefine.SLG_GRID_HORIZONTAL_NUM; i++)
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

