using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FISCA.Data;
using System.Data;
using SmartSchool.Evaluation.WearyDogComputerHelper;
using System.Xml;
using SmartSchool.Customization.Data;
using SmartSchool.Customization.Data.StudentExtension;
using FISCA.DSAUtil;
using SmartSchool.Evaluation.ScoreCalcRule;
using K12.Data;
using Aspose.Cells;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace SH_YearScoreReport_hwsh
{
    class Utility
    {
        /// <summary>
        /// 取得學生分項成績(學業、體育、國防通識、健康與護理、實習科目、德行)
        /// </summary>
        /// <param name="studentIDLis"></param>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, decimal?>> GetStudentSemsEntryScore(List<string> studentIDLis, int SchoolYear, int Semester)
        {
            // DB XML
            //<Entry 分項="學業" 成績="81.5"/><Entry 分項="體育" 成績="90"/><Entry 分項="健康與護理" 成績="95"/><Entry 分項="國防通識" 成績="89"/><Entry 分項="實習科目" 成績="96"/></SemesterEntryScore>"
            //<SemesterEntryScore>
            //<Entry 分項="德行" 成績="83.4" 鎖定="False"/>
            //</SemesterEntryScore>
            Dictionary<string, Dictionary<string, decimal?>> retVal = new Dictionary<string, Dictionary<string, decimal?>>();

            if (studentIDLis.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = "select ref_student_id,score_info from sems_entry_score where ref_student_id in(" + string.Join(",", studentIDLis.ToArray()) + ") and school_year=" + SchoolYear + " and semester=" + Semester + " order by ref_student_id";
                DataTable dt = qh.Select(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    string sid = dr[0].ToString();

                    if (!retVal.ContainsKey(sid))
                    {
                        Dictionary<string, decimal?> val = new Dictionary<string, decimal?>();
                        retVal.Add(sid, val);
                    }

                    string strScore = dr[1].ToString();
                    if (!string.IsNullOrWhiteSpace(strScore))
                    {
                        XElement elmScore = XElement.Parse(strScore);
                        foreach (XElement elm in elmScore.Elements("Entry"))
                        {
                            if (elm.Attribute("分項") != null)
                            {
                                string name = elm.Attribute("分項").Value;
                                decimal dd;
                                retVal[sid].Add(name, null);
                                if (elm.Attribute("成績") != null)
                                    if (decimal.TryParse(elm.Attribute("成績").Value, out dd))
                                        retVal[sid][name] = dd;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 透過學年度學期取得學生缺曠統計(傳入學生系統編號、開始日期、結束日期；回傳：學生系統編號、獎懲名稱,統計值
        /// </summary>
        /// <param name="StudIDList"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, int>> GetAttendanceCountBySchoolYearSemester(List<K12.Data.StudentRecord> StudRecordList, int SchoolYear, int Semester)
        {
            Dictionary<string, Dictionary<string, int>> retVal = new Dictionary<string, Dictionary<string, int>>();

            List<PeriodMappingInfo> PeriodMappingList = PeriodMapping.SelectAll();
            // 節次>類別
            Dictionary<string, string> PeriodMappingDict = new Dictionary<string, string>();
            foreach (PeriodMappingInfo rec in PeriodMappingList)
            {
                if (!PeriodMappingDict.ContainsKey(rec.Name))
                    PeriodMappingDict.Add(rec.Name, rec.Type);
            }


            // 取得惠文高中特有缺曠
            List<string> ATTypeList = GetATTypeList();

            List<AttendanceRecord> attendList = K12.Data.Attendance.SelectBySchoolYearAndSemester(StudRecordList, SchoolYear, Semester);

            // 計算統計資料
            foreach (AttendanceRecord rec in attendList)
            {
                if (!retVal.ContainsKey(rec.RefStudentID))
                {
                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    foreach (string str in ATTypeList)
                        dict.Add(str, 0);
                    retVal.Add(rec.RefStudentID, dict);
                }                    

                foreach (AttendancePeriod per in rec.PeriodDetail)
                {
                    if (!PeriodMappingDict.ContainsKey(per.Period))
                        continue;

                    // ex.一般:曠課
                    string key = PeriodMappingDict[per.Period] + "_" + per.AbsenceType;

                    // 缺曠轉換
                    key = ATTypeParse(key);

                    if (!retVal[rec.RefStudentID].ContainsKey(key))
                        retVal[rec.RefStudentID].Add(key, 0);

                    retVal[rec.RefStudentID][key]++;
                }
            }

            return retVal;
        }

        /// <summary>
        /// 透過學年度取得學生缺曠統計(傳入學生系統編號、開始日期、結束日期；回傳：學生系統編號、獎懲名稱,統計值
        /// </summary>
        /// <param name="StudIDList"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, int>> GetAttendanceCountBySchoolYear(List<K12.Data.StudentRecord> StudRecordList, int SchoolYear)
        {
            Dictionary<string, Dictionary<string, int>> retVal = new Dictionary<string, Dictionary<string, int>>();

            List<PeriodMappingInfo> PeriodMappingList = PeriodMapping.SelectAll();
            // 節次>類別
            Dictionary<string, string> PeriodMappingDict = new Dictionary<string, string>();
            foreach (PeriodMappingInfo rec in PeriodMappingList)
            {
                if (!PeriodMappingDict.ContainsKey(rec.Name))
                    PeriodMappingDict.Add(rec.Name, rec.Type);
            }

            // 取得惠文高中特有缺曠
            List<string> ATTypeList = GetATTypeList();

            List<AttendanceRecord> attendList = K12.Data.Attendance.SelectBySchoolYearAndSemester(StudRecordList, SchoolYear, null);

            // 計算統計資料
            foreach (AttendanceRecord rec in attendList)
            {
                if (!retVal.ContainsKey(rec.RefStudentID))
                {
                    Dictionary<string, int> dict = new Dictionary<string, int>();                    
                    foreach (string str in ATTypeList)
                        dict.Add(str, 0);
                    retVal.Add(rec.RefStudentID, dict);
                }
                foreach (AttendancePeriod per in rec.PeriodDetail)
                {
                    if (!PeriodMappingDict.ContainsKey(per.Period))
                        continue;

                    // ex.一般:曠課
                    string key = PeriodMappingDict[per.Period] + "_" + per.AbsenceType;

                    // 缺曠轉換
                    key = ATTypeParse(key);

                    if (retVal[rec.RefStudentID].ContainsKey(key))
                        retVal[rec.RefStudentID][key]++;
                }
            }

            return retVal;
        }

        /// <summary>
        /// 缺曠轉換
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ATTypeParse(string key)
        {
            string retValue = "其他假";

            if (key.Contains("事假"))
                retValue = "事假";

            if (key.Contains("病假"))
                retValue = "病假";

            if (key.Contains("公假"))
                retValue = "公假";

            if (key.Contains("喪假"))
                retValue = "喪假";            

            if (key.Contains("曠課") || key.Contains("遲到"))
            {
                if (key == "一般_曠課")
                    retValue = "曠課";
                else  if (key == "一般_遲到")
                    retValue = "遲到";
                else
                    retValue = "升降旗";
            }

            return retValue;
        }
       


        /// <summary>
        /// 透過學生編號取得學生服務學習時數 傳入學生編號、學年度,回傳：學生編號、內容、值
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> GetServiceLearningBySchoolYear(List<string> StudentIDList, int SchoolYear)
        {
            Dictionary<string, decimal> retVal = new Dictionary<string, decimal>();

            if (StudentIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string query = "select ref_student_id,school_year,semester,hours from $k12.service.learning.record where ref_student_id in('" + string.Join("','", StudentIDList.ToArray()) + "') and school_year=" + SchoolYear + " order by ref_student_id,school_year,semester;";
                DataTable dt = qh.Select(query);
                foreach (DataRow dr in dt.Rows)
                {
                    string sid = dr[0].ToString();
                    decimal hr;
                    decimal.TryParse(dr[3].ToString(), out hr);

                    if (!retVal.ContainsKey(sid))
                        retVal.Add(sid, 0);

                    retVal[sid] += hr;

                }
            }
            return retVal;
        }

        /// <summary>
        /// 透過學生編號取得學生服務學習時數 傳入學生編號、學年度,回傳：學生編號、內容、值
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> GetServiceLearningBySchoolYearSemester(List<string> StudentIDList, int SchoolYear, int Semester)
        {
            Dictionary<string, decimal> retVal = new Dictionary<string, decimal>();

            if (StudentIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string query = "select ref_student_id,school_year,semester,hours from $k12.service.learning.record where ref_student_id in('" + string.Join("','", StudentIDList.ToArray()) + "') and school_year=" + SchoolYear + " and semester=" + Semester + " order by ref_student_id,school_year,semester;";
                DataTable dt = qh.Select(query);
                foreach (DataRow dr in dt.Rows)
                {
                    string sid = dr[0].ToString();
                    decimal hr;
                    decimal.TryParse(dr[3].ToString(), out hr);

                    if (!retVal.ContainsKey(sid))
                        retVal.Add(sid, 0);

                    retVal[sid] += hr;

                }
            }
            return retVal;
        }


        /// <summary>
        /// 取得缺曠對照 List,一般_曠課..
        /// </summary>
        /// <returns></returns>
        public static List<string> GetATMappingKey()
        {
            List<string> retVal = new List<string>();
            List<string> key1List = new List<string>();
            List<string> Key2List = new List<string>();
            foreach (PeriodMappingInfo data in PeriodMapping.SelectAll())
                if (!key1List.Contains(data.Type))
                    key1List.Add(data.Type);

            foreach (AbsenceMappingInfo data in AbsenceMapping.SelectAll())
                if (!Key2List.Contains(data.Name))
                    Key2List.Add(data.Name);

            // 一般_曠課
            foreach (string key1 in key1List)
                foreach (string key2 in Key2List)
                    retVal.Add(key1 + "_" + key2);

            return retVal;
        }

        /// <summary>
        /// 取得學生及格與補考標準，參數用學生IDList,回傳:key:StudentID,1_及,數字
        /// </summary>
        /// <param name="StudRecList"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, decimal>> GetStudentApplyLimitDict(List<SmartSchool.Customization.Data.StudentRecord> StudRecList)
        {

            Dictionary<string, Dictionary<string, decimal>> retVal = new Dictionary<string, Dictionary<string, decimal>>();


            foreach (SmartSchool.Customization.Data.StudentRecord studRec in StudRecList)
            {
                //及格標準<年級,及格與補考標準>
                if (!retVal.ContainsKey(studRec.StudentID))
                    retVal.Add(studRec.StudentID, new Dictionary<string, decimal>());

                XmlElement scoreCalcRule = SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studRec.StudentID) == null ? null : SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studRec.StudentID).ScoreCalcRuleElement;
                if (scoreCalcRule == null)
                {

                }
                else
                {

                    DSXmlHelper helper = new DSXmlHelper(scoreCalcRule);
                    decimal tryParseDecimal;
                    decimal tryParseDecimala;

                    foreach (XmlElement element in helper.GetElements("及格標準/學生類別"))
                    {
                        string cat = element.GetAttribute("類別");
                        bool useful = false;
                        //掃描學生的類別作比對
                        foreach (CategoryInfo catinfo in studRec.StudentCategorys)
                        {
                            if (catinfo.Name == cat || catinfo.FullName == cat)
                                useful = true;
                        }
                        //學生是指定的類別或類別為"預設"
                        if (cat == "預設" || useful)
                        {
                            for (int gyear = 1; gyear <= 4; gyear++)
                            {
                                switch (gyear)
                                {
                                    case 1:
                                        if (decimal.TryParse(element.GetAttribute("一年級及格標準"), out tryParseDecimal))
                                        {
                                            string k1s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k1s))
                                                retVal[studRec.StudentID].Add(k1s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k1s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k1s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("一年級補考標準"), out tryParseDecimala))
                                        {
                                            string k1a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k1a))
                                                retVal[studRec.StudentID].Add(k1a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k1a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k1a] = tryParseDecimala;
                                        }

                                        break;
                                    case 2:
                                        if (decimal.TryParse(element.GetAttribute("二年級及格標準"), out tryParseDecimal))
                                        {
                                            string k2s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k2s))
                                                retVal[studRec.StudentID].Add(k2s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k2s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k2s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("二年級補考標準"), out tryParseDecimala))
                                        {
                                            string k2a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k2a))
                                                retVal[studRec.StudentID].Add(k2a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k2a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k2a] = tryParseDecimala;

                                        }

                                        break;
                                    case 3:
                                        if (decimal.TryParse(element.GetAttribute("三年級及格標準"), out tryParseDecimal))
                                        {
                                            string k3s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k3s))
                                                retVal[studRec.StudentID].Add(k3s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k3s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k3s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("三年級補考標準"), out tryParseDecimala))
                                        {
                                            string k3a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k3a))
                                                retVal[studRec.StudentID].Add(k3a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k3a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k3a] = tryParseDecimala;
                                        }

                                        break;
                                    case 4:
                                        if (decimal.TryParse(element.GetAttribute("四年級及格標準"), out tryParseDecimal))
                                        {
                                            string k4s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k4s))
                                                retVal[studRec.StudentID].Add(k4s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k4s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k4s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("四年級補考標準"), out tryParseDecimala))
                                        {
                                            string k4a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k4a))
                                                retVal[studRec.StudentID].Add(k4a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k4a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k4a] = tryParseDecimala;
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }


        public static void CompletedXlsCsv(string inputReportName, DataTable dt)
        {

            #region 儲存檔案
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".txt");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            //Workbook workbook = inputXls;
            StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Unicode);

            // StringBuilder sb = new StringBuilder();

            //sb.Append(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()));
            //    sb.Append(Encoding.Unicode.GetString(Encoding.Unicode.GetPreamble()));
            //if (workbook == null || workbook.Worksheets == null || workbook.Worksheets.Count == 0)
            //{
            //    MessageBox.Show("沒有資料！");
            //    return;
            //}
            //Cells cells = workbook.Worksheets[0].Cells;
            DataTable dataTable = dt;//workbook.Worksheets[0].Cells.ExportDataTable(cells.MinRow, cells.MinColumn, cells.Rows.Count, cells.Columns.Count);
            //  int row_count = 0;


            //dataTable.Rows.Cast<DataRow>().ToList().ForEach((x) =>
            //{
            //    row_count++;
            //    sb.Append(string.Join(",", x.ItemArray));
            //    if (!(row_count == dataTable.Rows.Count))
            //        sb.AppendLine();
            //});

            List<string> strList = new List<string>();
            foreach (DataColumn dc in dt.Columns)
                strList.Add(dc.ColumnName);

            sw.WriteLine(string.Join(",", strList.ToArray()));
            // sb.AppendLine(string.Join(",",strList.ToArray()));

            foreach (DataRow dr in dt.Rows)
            {
                List<string> subList = new List<string>();
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    subList.Add(dr[col].ToString());
                }
                sw.WriteLine(string.Join(",", subList.ToArray()));
                //sb.AppendLine(string.Join(",",subList.ToArray()));
            }

            //string out_put_string = sb.ToString();

            sw.Close();
            try
            {
                //FileStream fs = new FileStream(path, FileMode.Create);
                //fs.Write(Encoding.Unicode.GetBytes(out_put_string), 0, Encoding.Unicode.GetByteCount(out_put_string));
                //fs.Close();
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                try
                {
                    System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = reportName + ".txt";
                    sd.Filter = "txt檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //FileStream fs = new FileStream(sd.FileName, FileMode.Create);
                        //fs.Write(Encoding.Unicode.GetBytes(out_put_string), 0, Encoding.Unicode.GetByteCount(out_put_string));
                        //fs.Close();
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion


            //string reportName = inputReportName;

            //string path = Path.Combine(Application.StartupPath, "Reports");
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);
            //path = Path.Combine(path, reportName + ".csv");

            //Workbook wb = inputXls;

            //if (File.Exists(path))
            //{
            //    int i = 1;
            //    while (true)
            //    {
            //        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
            //        if (!File.Exists(newPath))
            //        {
            //            path = newPath;
            //            break;
            //        }
            //    }
            //}

            //try
            //{
            //    wb.Save(path, Aspose.Cells.FileFormatType.CSV);
            //    System.Diagnostics.Process.Start(path);
            //}
            //catch
            //{
            //    SaveFileDialog sd = new SaveFileDialog();
            //    sd.Title = "另存新檔";
            //    sd.FileName = reportName + ".csv";
            //    sd.Filter = "CSV檔案 (*.csv)|*.xls|所有檔案 (*.*)|*.*";
            //    if (sd.ShowDialog() == DialogResult.OK)
            //    {
            //        try
            //        {
            //            wb.Save(sd.FileName, Aspose.Cells.FileFormatType.CSV);

            //        }
            //        catch
            //        {
            //            MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }
            //    }
            //}
        }


        public static void CompletedXlsCsvAnsi(string inputReportName, Workbook inputXls)
        {

            string reportName = inputReportName;

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".csv");

            Workbook wb = inputXls;

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                wb.Save(path, Aspose.Cells.FileFormatType.CSV);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".csv";
                sd.Filter = "CSV檔案 (*.csv)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, Aspose.Cells.FileFormatType.CSV);

                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 傳入學年度，取得學生狀態一般的科目名稱
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <returns></returns>
        public static List<string> GetSubjectNameListBySchoolYear(string SchoolYear)
        {
            List<string> retVal = new List<string>();
            QueryHelper qh = new QueryHelper();

            string strSQL = @"SELECT DISTINCT tmp.subject
FROM xpath_table( 'id',
				'''<root>''||score_info||''</root>''',
				'sems_subj_score',
				'/root/SemesterSubjectScoreInfo/Subject/@科目',
				'ref_student_id IN ( select student.id from student 
									INNER JOIN class ON student.ref_class_id = class.id 
									WHERE student.status=1 and class.grade_year in(1,2,3)) AND sems_subj_score.school_year=" + SchoolYear + @"')				 
AS tmp(id int, subject varchar(200))  order by subject asc";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr[0].ToString();
                if (!retVal.Contains(name))
                    retVal.Add(name);
            }
            retVal.Sort(new StringComparer("國文"
                                , "英文"
                                , "數學"
                                , "理化"
                                , "生物"
                                , "社會"
                                , "物理"
                                , "化學"
                                , "歷史"
                                , "地理"
                                , "公民")
                );
            return retVal;
        }

        /// <summary>
        /// 傳入學年度，回傳畫面上所選學生科目
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <returns></returns>
        public static List<string> GetSubjectNameListBySchoolYearSelectStudentID(List<string> StudentIDList, string SchoolYear)
        {
            List<string> retVal = new List<string>();
            QueryHelper qh = new QueryHelper();

            string strSQL = @"SELECT DISTINCT tmp.subject
FROM xpath_table( 'id',
				'''<root>''||score_info||''</root>''',
				'sems_subj_score',
				'/root/SemesterSubjectScoreInfo/Subject/@科目',
				'ref_student_id IN ( select student.id from student 
									INNER JOIN class ON student.ref_class_id = class.id 
									WHERE student.id in(" + string.Join(",", StudentIDList.ToArray()) + @")) AND sems_subj_score.school_year=" + SchoolYear + @"')				 
AS tmp(id int, subject varchar(200))  order by subject asc";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr[0].ToString();
                if (!retVal.Contains(name))
                    retVal.Add(name);
            }


            retVal.Sort(new StringComparer("國文"
                                , "英文"
                                , "數學"
                                , "理化"
                                , "生物"
                                , "社會"
                                , "物理"
                                , "化學"
                                , "歷史"
                                , "地理"
                                , "公民")
                );
            return retVal;
        }

        /// <summary>
        /// 取得惠文高中缺曠
        /// </summary>
        /// <returns></returns>
        public static List<string> GetATTypeList()
        {
            List<string> ATTypeList = new List<string>();
            ATTypeList.Add("升降旗");
            ATTypeList.Add("遲到");
            ATTypeList.Add("曠課");
            ATTypeList.Add("事假");
            ATTypeList.Add("病假");
            ATTypeList.Add("公假");
            ATTypeList.Add("喪假");
            ATTypeList.Add("其他假");

            return ATTypeList;
        }
    }
}
