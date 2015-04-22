using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.IO;
using SmartSchool.Customization.Data;
using System.Threading;
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool;

namespace SH_YearScoreReport_hwsh
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            FISCA.Permission.Catalog cat = FISCA.Permission.RoleAclSource.Instance["學生"]["功能按鈕"];
            cat.Add(new FISCA.Permission.RibbonFeature("SHSchool.SH_hwsh_YearScoreReport", "學年成績通知單ePost(惠文)"));

            var btn = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["學年成績通知單ePost(惠文)"];
            btn.Enable = false;
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate { btn.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current["SHSchool.SH_hwsh_YearScoreReport"].Executable; };
            btn.Click += new EventHandler(Program_Click);


        }

        private static string GetNumber(decimal? p)
        {
            if (p == null) return "";
            string levelNumber;
            switch (((int)p.Value))
            {
                // region 對應levelNumber
                case 0:
                    levelNumber = "";
                    break;
                case 1:
                    levelNumber = "Ⅰ";
                    break;
                case 2:
                    levelNumber = "Ⅱ";
                    break;
                case 3:
                    levelNumber = "Ⅲ";
                    break;
                case 4:
                    levelNumber = "Ⅳ";
                    break;
                case 5:
                    levelNumber = "Ⅴ";
                    break;
                case 6:
                    levelNumber = "Ⅵ";
                    break;
                case 7:
                    levelNumber = "Ⅶ";
                    break;
                case 8:
                    levelNumber = "Ⅷ";
                    break;
                case 9:
                    levelNumber = "Ⅸ";
                    break;
                case 10:
                    levelNumber = "Ⅹ";
                    break;
                default:
                    levelNumber = "" + (p);
                    break;

            }
            return levelNumber;
        }


        // 第1學期取得
        static Dictionary<string, decimal> _studPassSumCreditDict1 = new Dictionary<string, decimal>();
        // 第2學期取得
        static Dictionary<string, decimal> _studPassSumCreditDict2 = new Dictionary<string, decimal>();

        // 第1學期累計取得學分
        static Dictionary<string, decimal> _studSumPassCreditDict1 = new Dictionary<string, decimal>();
        // 第2學期累計取得學分
        static Dictionary<string, decimal> _studSumPassCreditDict2 = new Dictionary<string, decimal>();

        // 第1學期加權總分
        static Dictionary<string, decimal> _studSumScoreDict1 = new Dictionary<string, decimal>();

        // 第2學期加權總分
        static Dictionary<string, decimal> _studSumScoreDict2 = new Dictionary<string, decimal>();

        // 累計取得
        static Dictionary<string, decimal> _studPassSumCreditDictAll = new Dictionary<string, decimal>();
        static DataTable _dtEpost = new DataTable();

        static void Program_Click(object sender_, EventArgs e_)
        {
            AccessHelper helper = new AccessHelper();
            List<StudentRecord> lista = helper.StudentHelper.GetSelectedStudent();

            // 取得學生及格與補考標準
            Dictionary<string, Dictionary<string, decimal>> StudentApplyLimitDict = Utility.GetStudentApplyLimitDict(lista);

            ConfigForm form = new ConfigForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                AccessHelper accessHelper = new AccessHelper();
                //return;
                List<StudentRecord> overflowRecords = new List<StudentRecord>();
                //取得列印設定
                Configure conf = form.Configure;
                //建立測試的選取學生(先期不管怎麼選就是印這些人)
                List<string> selectedStudents = K12.Presentation.NLDPanels.Student.SelectedSource;

                //建立合併欄位總表
                DataTable table = new DataTable();
                // region 所有的合併欄位
                table.Columns.Add("學生系統編號");
                table.Columns.Add("學生班級年級");
                table.Columns.Add("學校名稱");
                table.Columns.Add("學校地址");
                table.Columns.Add("學校電話");
                table.Columns.Add("收件人地址");
                //«通訊地址»«通訊地址郵遞區號»«通訊地址內容»
                //«戶籍地址»«戶籍地址郵遞區號»«戶籍地址內容»
                //«監護人»«父親»«母親»«科別名稱»
                table.Columns.Add("通訊地址");
                table.Columns.Add("通訊地址郵遞區號");
                table.Columns.Add("通訊地址內容");
                table.Columns.Add("戶籍地址");
                table.Columns.Add("戶籍地址郵遞區號");
                table.Columns.Add("戶籍地址內容");
                table.Columns.Add("監護人");
                table.Columns.Add("父親");
                table.Columns.Add("母親");
                table.Columns.Add("科別名稱");


                table.Columns.Add("收件人");
                table.Columns.Add("學年度");
                table.Columns.Add("學期");
                table.Columns.Add("班級科別名稱");
                table.Columns.Add("班級");
                table.Columns.Add("班導師");
                table.Columns.Add("座號");
                table.Columns.Add("學號");
                table.Columns.Add("姓名");
                table.Columns.Add("第1學期取得學分數");
                table.Columns.Add("第2學期取得學分數");
                table.Columns.Add("第1學期累計取得學分數");
                table.Columns.Add("第2學期累計取得學分數");
                table.Columns.Add("累計取得學分數");
                table.Columns.Add("第1學期加權總分");
                table.Columns.Add("第2學期加權總分");

                if (conf.SubjectLimit == 0)
                    conf.SubjectLimit = 30;

                for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                {
                    table.Columns.Add("科目名稱" + subjectIndex);
                    table.Columns.Add("第2學期學分數" + subjectIndex);
                    table.Columns.Add("科目成績" + subjectIndex);
                    // 新增學期科目相關成績--
                    table.Columns.Add("科目必選修" + subjectIndex);
                    table.Columns.Add("科目校部定" + subjectIndex);
                    table.Columns.Add("科目註記" + subjectIndex);
                    table.Columns.Add("科目取得學分" + subjectIndex);
                    table.Columns.Add("科目未取得學分註記" + subjectIndex);
                    table.Columns.Add("第2學期科目原始成績" + subjectIndex);
                    table.Columns.Add("第2學期科目補考成績" + subjectIndex);
                    table.Columns.Add("第2學期科目重修成績" + subjectIndex);
                    table.Columns.Add("第2學期科目手動調整成績" + subjectIndex);
                    table.Columns.Add("第2學期科目學年調整成績" + subjectIndex);
                    table.Columns.Add("第2學期科目成績" + subjectIndex);
                    table.Columns.Add("第2學期科目原始成績註記" + subjectIndex);
                    table.Columns.Add("第2學期科目補考成績註記" + subjectIndex);
                    table.Columns.Add("第2學期科目重修成績註記" + subjectIndex);
                    table.Columns.Add("第2學期科目手動成績註記" + subjectIndex);
                    table.Columns.Add("第2學期科目學年成績註記" + subjectIndex);
                    table.Columns.Add("第2學期科目可補考註記" + subjectIndex);
                    table.Columns.Add("第2學期科目不可補考註記" + subjectIndex);
                    // 新增第2學期科目排名
                    table.Columns.Add("第2學期科目排名成績" + subjectIndex);
                    table.Columns.Add("第2學期科目班排名" + subjectIndex);
                    table.Columns.Add("第2學期科目班排名母數" + subjectIndex);
                    table.Columns.Add("第2學期科目科排名" + subjectIndex);
                    table.Columns.Add("第2學期科目科排名母數" + subjectIndex);
                    table.Columns.Add("第2學期科目類別1排名" + subjectIndex);
                    table.Columns.Add("第2學期科目類別1排名母數" + subjectIndex);
                    table.Columns.Add("第2學期科目類別2排名" + subjectIndex);
                    table.Columns.Add("第2學期科目類別2排名母數" + subjectIndex);
                    table.Columns.Add("第2學期科目全校排名" + subjectIndex);
                    table.Columns.Add("第2學期科目全校排名母數" + subjectIndex);
                    // 新增第1學期科目相關成績--
                    table.Columns.Add("第1學期學分數" + subjectIndex);
                    table.Columns.Add("第1學期科目原始成績" + subjectIndex);
                    table.Columns.Add("第1學期科目補考成績" + subjectIndex);
                    table.Columns.Add("第1學期科目重修成績" + subjectIndex);
                    table.Columns.Add("第1學期科目手動調整成績" + subjectIndex);
                    table.Columns.Add("第1學期科目學年調整成績" + subjectIndex);
                    table.Columns.Add("第1學期科目成績" + subjectIndex);
                    table.Columns.Add("第1學期科目原始成績註記" + subjectIndex);
                    table.Columns.Add("第1學期科目補考成績註記" + subjectIndex);
                    table.Columns.Add("第1學期科目重修成績註記" + subjectIndex);
                    table.Columns.Add("第1學期科目手動成績註記" + subjectIndex);
                    table.Columns.Add("第1學期科目學年成績註記" + subjectIndex);
                    table.Columns.Add("第1學期科目可補考註記" + subjectIndex);
                    table.Columns.Add("第1學期科目不可補考註記" + subjectIndex);
                    table.Columns.Add("第1學期科目取得學分" + subjectIndex);
                    table.Columns.Add("第1學期科目未取得學分註記" + subjectIndex);

                    // 新增學年科目成績--
                    table.Columns.Add("學年科目成績" + subjectIndex);


                }

                // 第2學期分項成績 --
                table.Columns.Add("第2學期學業成績");
                table.Columns.Add("第2學期體育成績");
                table.Columns.Add("第2學期國防通識成績");
                table.Columns.Add("第2學期健康與護理成績");
                table.Columns.Add("第2學期實習科目成績");
                table.Columns.Add("第2學期專業科目成績");
                table.Columns.Add("第2學期學業(原始)成績");
                table.Columns.Add("第2學期體育(原始)成績");
                table.Columns.Add("第2學期國防通識(原始)成績");
                table.Columns.Add("第2學期健康與護理(原始)成績");
                table.Columns.Add("第2學期實習科目(原始)成績");
                table.Columns.Add("第2學期專業科目(原始)成績");
                table.Columns.Add("第2學期德行成績");
                // 第2學期學業成績排名
                table.Columns.Add("第2學期學業成績班排名");
                table.Columns.Add("第2學期學業成績科排名");
                table.Columns.Add("第2學期學業成績類別1排名");
                table.Columns.Add("第2學期學業成績類別2排名");
                table.Columns.Add("第2學期學業成績校排名");
                table.Columns.Add("第2學期學業成績班排名母數");
                table.Columns.Add("第2學期學業成績科排名母數");
                table.Columns.Add("第2學期學業成績類別1排名母數");
                table.Columns.Add("第2學期學業成績類別2排名母數");
                table.Columns.Add("第2學期學業成績校排名母數");
                // 導師評語 --
                table.Columns.Add("第1學期導師評語");
                table.Columns.Add("第2學期導師評語");
                // 獎懲統計 --
                table.Columns.Add("第1學期大功統計");
                table.Columns.Add("第1學期小功統計");
                table.Columns.Add("第1學期嘉獎統計");
                table.Columns.Add("第1學期大過統計");
                table.Columns.Add("第1學期小過統計");
                table.Columns.Add("第1學期警告統計");
                table.Columns.Add("第1學期留校察看");
                table.Columns.Add("第2學期大功統計");
                table.Columns.Add("第2學期小功統計");
                table.Columns.Add("第2學期嘉獎統計");
                table.Columns.Add("第2學期大過統計");
                table.Columns.Add("第2學期小過統計");
                table.Columns.Add("第2學期警告統計");
                table.Columns.Add("第2學期留校察看");
                table.Columns.Add("學年大功統計");
                table.Columns.Add("學年小功統計");
                table.Columns.Add("學年嘉獎統計");
                table.Columns.Add("學年大過統計");
                table.Columns.Add("學年小過統計");
                table.Columns.Add("學年警告統計");
                table.Columns.Add("學年留校察看");

                // 第1學期分項成績 --
                table.Columns.Add("第1學期學業成績");
                table.Columns.Add("第1學期體育成績");
                table.Columns.Add("第1學期國防通識成績");
                table.Columns.Add("第1學期健康與護理成績");
                table.Columns.Add("第1學期實習科目成績");
                table.Columns.Add("第1學期德行成績");
                table.Columns.Add("第1學期專業科目成績");
                table.Columns.Add("第1學期學業(原始)成績");
                table.Columns.Add("第1學期體育(原始)成績");
                table.Columns.Add("第1學期國防通識(原始)成績");
                table.Columns.Add("第1學期健康與護理(原始)成績");
                table.Columns.Add("第1學期實習科目(原始)成績");
                table.Columns.Add("第1學期專業科目(原始)成績");

                // 第1學期學業成績排名
                table.Columns.Add("第1學期學業成績班排名");
                table.Columns.Add("第1學期學業成績科排名");
                table.Columns.Add("第1學期學業成績類別1排名");
                table.Columns.Add("第1學期學業成績類別2排名");
                table.Columns.Add("第1學期學業成績校排名");
                table.Columns.Add("第1學期學業成績班排名母數");
                table.Columns.Add("第1學期學業成績科排名母數");
                table.Columns.Add("第1學期學業成績類別1排名母數");
                table.Columns.Add("第1學期學業成績類別2排名母數");
                table.Columns.Add("第1學期學業成績校排名母數");


                // 學年分項成績 --
                table.Columns.Add("學年學業成績");
                table.Columns.Add("學年體育成績");
                table.Columns.Add("學年國防通識成績");
                table.Columns.Add("學年健康與護理成績");
                table.Columns.Add("學年實習科目成績");
                table.Columns.Add("學年德行成績");
                table.Columns.Add("學年學業成績班排名");

                // 服務學習時數
                table.Columns.Add("第1學期服務學習時數");
                table.Columns.Add("第2學期服務學習時數");
                table.Columns.Add("學年服務學習時數");

                // 惠文自訂 缺曠規則
                List<string> ATTypeList = Utility.GetATTypeList();

                // 缺曠統計
                // 動態新增缺曠統計，使用模式一般_曠課、一般_事假..
                foreach (string name in ATTypeList)
                {
                    table.Columns.Add("第1學期" + name);
                    table.Columns.Add("第2學期" + name);
                    table.Columns.Add("學年" + name);
                }

                table.Columns.Add("學期科目成績及格標準");
                table.Columns.Add("學期科目成績補考標準");
                table.Columns.Add("學年平均");


                //宣告產生的報表
                Aspose.Words.Document document = new Aspose.Words.Document();

                //用一個BackgroundWorker包起來
                System.ComponentModel.BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker();
                bkw.WorkerReportsProgress = true;
                System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 學年成績單產生 S");
                bkw.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("學年成績單產生中", e.ProgressPercentage);
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 學年成績單產生 " + e.ProgressPercentage);
                };
                Exception exc = null;
                bkw.RunWorkerCompleted += delegate
                {



                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 學年成績單產生 E");
                    string err = "下列學生因成績項目超過樣板支援上限，\n超出部分科目成績無法印出，建議調整樣板內容。";
                    if (overflowRecords.Count > 0)
                    {
                        foreach (var stuRec in overflowRecords)
                        {
                            err += "\n" + (stuRec.RefClass == null ? "" : (stuRec.RefClass.ClassName + "班" + stuRec.SeatNo + "號")) + "[" + stuRec.StudentNumber + "]" + stuRec.StudentName;
                        }
                    }
                    // region 儲存檔案
                    string inputReportName = "個人學年成績單";
                    System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
                    folder.Description = "請選擇目的資料夾";
                    if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string folderPath = folder.SelectedPath;
                        Dictionary<string, List<int>> _ClassDic = new Dictionary<string, List<int>>();

                        int index = 0;
                        foreach (DataRow row in table.Rows)
                        {
                            string className = row["班級"].ToString();
                            if (!_ClassDic.ContainsKey(className))
                            {
                                _ClassDic.Add(className, new List<int>());
                            }
                            _ClassDic[className].Add(index);
                            index++;
                        }

                        try
                        {

                            List<DataRow> list = new List<DataRow>();
                            foreach (string className in _ClassDic.Keys)
                            {
                                foreach (int idx in _ClassDic[className])
                                {
                                    list.Add(table.Rows[idx]);
                                }

                                list.Sort(DataSort);

                                DataTable dt = new DataTable();
                                foreach (DataColumn dc in table.Columns)
                                    dt.Columns.Add(dc.ColumnName);

                                foreach (DataRow row in list)
                                {
                                    dt.ImportRow(row);
                                }

                                document = conf.Template.Clone();
                                document.MailMerge.Execute(dt);
                                document.MailMerge.RemoveEmptyParagraphs = true;
                                document.MailMerge.DeleteFields();
                                document.Save(folderPath + "\\" + inputReportName + "_" + className + ".docx", Aspose.Words.SaveFormat.Docx);
                                //document = null;
                                //dt = null;                                
                                list.Clear();
                                //GC.Collect();
                            }
                            System.Diagnostics.Process.Start(folderPath);
                        }
                        catch (Exception ex)
                        {
                            SmartSchool.ErrorReporting.ErrorMessgae errormsg = new SmartSchool.ErrorReporting.ErrorMessgae(ex);
                            FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。" + ex.Message, "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                    }


                    FISCA.Presentation.MotherForm.SetStatusBarMessage("學年成績單產生完成。", 100);
                    if (overflowRecords.Count > 0)
                        MessageBox.Show(err);
                    if (exc != null)
                    {
                        //throw new Exception("產生學年成績單發生錯誤", exc);
                    }

                    // 處理 epost
                    if (conf.ExportEpost)
                    {
                        // 檢查是否產生 Excel
                        Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                        Utility.CompletedXlsCsv("個人學年成績單ePost", _dtEpost);
                    }

                };
                bkw.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    var studentRecords = accessHelper.StudentHelper.GetStudents(selectedStudents);

                    ManualResetEvent scoreReady = new ManualResetEvent(false);
                    ManualResetEvent elseReady = new ManualResetEvent(false);
                    // region 偷跑取得考試成績
                    // 有成績科目名稱對照
                    new Thread(new ThreadStart(delegate
                    {
                        // 取得學生學期科目成績
                        int sSchoolYear;
                        int.TryParse(conf.SchoolYear, out sSchoolYear);


                        #region 整理學生學期、學年成績
                        try
                        {

                            accessHelper.StudentHelper.FillSchoolYearEntryScore(true, studentRecords);
                            accessHelper.StudentHelper.FillSchoolYearSubjectScore(true, studentRecords);

                            accessHelper.StudentHelper.FillSemesterEntryScore(true, studentRecords);
                            accessHelper.StudentHelper.FillSemesterSubjectScore(true, studentRecords);
                            accessHelper.StudentHelper.FillSemesterMoralScore(true, studentRecords);
                            accessHelper.StudentHelper.FillField("SchoolYearEntryClassRating", studentRecords);

                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            string sidList = "";
                            Dictionary<string, StudentRecord> stuDictionary = new Dictionary<string, StudentRecord>();
                            foreach (var stuRec in studentRecords)
                            {
                                sidList += (sidList == "" ? "" : ",") + stuRec.StudentID;
                                stuDictionary.Add(stuRec.StudentID, stuRec);
                            }

                            FISCA.Data.QueryHelper qh = new FISCA.Data.QueryHelper();

                            #region 第1學期學業成績排名
                            DataTable dt1 = new DataTable();
                            string strSQL1 = "select * from sems_entry_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=1";
                            dt1 = qh.Select(strSQL1);
                            foreach (System.Data.DataRow dr in dt1.Rows)
                            {
                                if ("" + dr["entry_group"] != "1") continue;
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                if ("" + dr["class_rating"] != "")
                                {
                                    //第1學期學業成績班排名
                                    doc.LoadXml("" + dr["class_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第1學期學業成績班排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期學業成績班排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第1學期學業成績科排名
                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第1學期學業成績科排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期學業成績科排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第1學期學業成績類別1排名
                                //第1學期學業成績類別2排名

                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        System.Xml.XmlElement ele = (System.Xml.XmlElement)element.SelectSingleNode("Item[@分項='學業']");
                                        if (ele != null)
                                        {
                                            if (!rec.Fields.ContainsKey("第1學期學業成績類別1"))
                                            {
                                                rec.Fields.Add("第1學期學業成績類別1", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("第1學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("第1學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第1學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                            else
                                            {
                                                rec.Fields.Add("第1學期學業成績類別2", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("第1學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("第1學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第1學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //第1學期學業成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第1學期學業成績校排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期學業成績校排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                            }
                            #endregion

                            #region 第2學期學業成績排名
                            DataTable dt2 = new DataTable();
                            string strSQL2 = "select * from sems_entry_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=2";
                            dt2 = qh.Select(strSQL2);
                            foreach (System.Data.DataRow dr in dt2.Rows)
                            {
                                if ("" + dr["entry_group"] != "1") continue;
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                if ("" + dr["class_rating"] != "")
                                {
                                    //第2學期學業成績班排名
                                    doc.LoadXml("" + dr["class_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第2學期學業成績班排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期學業成績班排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第2學期學業成績科排名
                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第2學期學業成績科排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期學業成績科排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第2學期學業成績類別1排名
                                //第2學期學業成績類別2排名

                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        System.Xml.XmlElement ele = (System.Xml.XmlElement)element.SelectSingleNode("Item[@分項='學業']");
                                        if (ele != null)
                                        {
                                            if (!rec.Fields.ContainsKey("第2學期學業成績類別1"))
                                            {
                                                rec.Fields.Add("第2學期學業成績類別1", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("第2學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("第2學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第2學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                            else
                                            {
                                                rec.Fields.Add("第2學期學業成績類別2", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("第2學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("第2學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第2學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //第2學期學業成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第2學期學業成績校排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期學業成績校排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                            }
                            #endregion


                            #region 第1學期科目成績排名
                            dt1 = new DataTable();
                            strSQL1 = "select * from sems_subj_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=1";
                            dt1 = qh.Select(strSQL1);
                            foreach (System.Data.DataRow dr in dt1.Rows)
                            {
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                //第1學期學業科目成績班排名
                                if ("" + dr["class_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["class_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 成績="83" 成績人數="50" 排名="33" 科目="公民與社會" 科目級別="1"/>
                                        rec.Fields.Add("第1學期科目排名成績" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績"));
                                        rec.Fields.Add("第1學期科目班排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期科目班排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第1學期學業科目成績科排名
                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第1學期科目科排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期科目科排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第1學期學業成績類別1排名
                                //第1學期學業成績類別2排名
                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        string cat = element.GetAttribute("類別");
                                        if (!rec.Fields.ContainsKey("第1學期科目成績類別1"))
                                        {
                                            rec.Fields.Add("第1學期科目成績類別1", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("第1學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("第1學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第1學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            rec.Fields.Add("第1學期科目成績類別2", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("第1學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("第1學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第1學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //第1學期學業科目成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第1學期科目校排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第1學期科目校排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                            }
                            #endregion

                            #region 第2學期科目成績排名
                            dt2 = new DataTable();
                            strSQL2 = "select * from sems_subj_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=2";
                            dt2 = qh.Select(strSQL2);
                            foreach (System.Data.DataRow dr in dt2.Rows)
                            {
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                //第2學期學業科目成績班排名
                                if ("" + dr["class_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["class_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 成績="83" 成績人數="50" 排名="33" 科目="公民與社會" 科目級別="1"/>
                                        rec.Fields.Add("第2學期科目排名成績" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績"));
                                        rec.Fields.Add("第2學期科目班排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期科目班排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第2學期學業科目成績科排名
                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第2學期科目科排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期科目科排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //第2學期學業成績類別1排名
                                //第2學期學業成績類別2排名
                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        string cat = element.GetAttribute("類別");
                                        if (!rec.Fields.ContainsKey("第2學期科目成績類別1"))
                                        {
                                            rec.Fields.Add("第2學期科目成績類別1", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("第2學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("第2學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第2學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            rec.Fields.Add("第2學期科目成績類別2", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("第2學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("第2學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("第2學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //第2學期學業科目成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("第2學期科目校排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("第2學期科目校排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                            }
                            #endregion

                            accessHelper.StudentHelper.FillAttendance(studentRecords);
                            accessHelper.StudentHelper.FillReward(studentRecords);
                        }
                        catch (Exception exception)
                        {
                            exc = exception;
                        }
                        finally
                        {
                            elseReady.Set();
                        }

                    })).Start();
                        #endregion
                    try
                    {
                        // region 大範圍

                        string key = "";
                        bkw.ReportProgress(0);
                        // 清空epost 使用欄位
                        _dtEpost.Columns.Clear();
                        _dtEpost.Clear();
                        // 處理 epost 欄位
                        _dtEpost.Columns.Add("CN");
                        _dtEpost.Columns.Add("POSTALCODE");
                        _dtEpost.Columns.Add("POSTALADDRESS");
                        _dtEpost.Columns.Add("學年度");
                        _dtEpost.Columns.Add("班級");
                        _dtEpost.Columns.Add("座號");
                        _dtEpost.Columns.Add("學號");
                        _dtEpost.Columns.Add("姓名");

                        for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                        {
                            _dtEpost.Columns.Add("科目名稱" + subjectIndex);
                            _dtEpost.Columns.Add("第1學期學分數" + subjectIndex);
                            _dtEpost.Columns.Add("第1學期科目成績" + subjectIndex);
                            _dtEpost.Columns.Add("第1學期備註" + subjectIndex);
                            _dtEpost.Columns.Add("第2學期學分數" + subjectIndex);
                            _dtEpost.Columns.Add("第2學期科目成績" + subjectIndex);
                            _dtEpost.Columns.Add("第2學期備註" + subjectIndex);
                        }
                        _dtEpost.Columns.Add("第1學期大功統計");
                        _dtEpost.Columns.Add("第2學期大功統計");
                        _dtEpost.Columns.Add("學年大功統計");
                        _dtEpost.Columns.Add("第1學期大過統計");
                        _dtEpost.Columns.Add("第2學期大過統計");
                        _dtEpost.Columns.Add("學年大過統計");
                        _dtEpost.Columns.Add("第1學期學業成績");
                        _dtEpost.Columns.Add("第2學期學業成績");
                        _dtEpost.Columns.Add("第1學期小功統計");
                        _dtEpost.Columns.Add("第2學期小功統計");
                        _dtEpost.Columns.Add("學年小功統計");
                        _dtEpost.Columns.Add("第1學期小過統計");
                        _dtEpost.Columns.Add("第2學期小過統計");
                        _dtEpost.Columns.Add("學年小過統計");
                        _dtEpost.Columns.Add("第1學期學業成績班排名");
                        _dtEpost.Columns.Add("第2學期體育成績");
                        _dtEpost.Columns.Add("第1學期嘉獎統計");
                        _dtEpost.Columns.Add("第2學期嘉獎統計");
                        _dtEpost.Columns.Add("學年嘉獎統計");
                        _dtEpost.Columns.Add("第1學期警告統計");
                        _dtEpost.Columns.Add("第2學期警告統計");
                        _dtEpost.Columns.Add("學年警告統計");
                        _dtEpost.Columns.Add("第1學期取得學分數");
                        _dtEpost.Columns.Add("第2學期取得學分數");
                        _dtEpost.Columns.Add("第1學期留校察看");
                        _dtEpost.Columns.Add("第2學期留校察看");
                        _dtEpost.Columns.Add("學年留校察看");
                        _dtEpost.Columns.Add("第1學期累計取得學分數");
                        _dtEpost.Columns.Add("第2學期累計取得學分數");
                        _dtEpost.Columns.Add("第1學期加權總分");
                        _dtEpost.Columns.Add("第2學期加權總分");

                        foreach (string str in ATTypeList)
                            _dtEpost.Columns.Add(str);

                        //_dtEpost.Columns.Add("學業成績");
                        //_dtEpost.Columns.Add("實習成績");
                        //_dtEpost.Columns.Add("總成績名次");
                        //_dtEpost.Columns.Add("取得學分");
                        //_dtEpost.Columns.Add("累計學分");
                        //_dtEpost.Columns.Add("大功");
                        //_dtEpost.Columns.Add("小功");
                        //_dtEpost.Columns.Add("嘉獎");
                        //_dtEpost.Columns.Add("大過");
                        //_dtEpost.Columns.Add("小過");
                        //_dtEpost.Columns.Add("警告");
                        //_dtEpost.Columns.Add("留校察看");


                        //// 固定會對照
                        //Dictionary<string, string> eKeyValDict = new Dictionary<string, string>();
                        //eKeyValDict.Add("收件人", "CN");
                        //eKeyValDict.Add("學年度", "學年度");
                        //eKeyValDict.Add("學期", "學期");
                        //eKeyValDict.Add("班級", "班級");
                        //eKeyValDict.Add("座號", "座號");
                        //eKeyValDict.Add("學號", "學號");
                        //eKeyValDict.Add("姓名", "姓名");
                        //eKeyValDict.Add("學期學業成績", "學業成績");
                        //eKeyValDict.Add("學期實習科目成績", "實習成績");
                        //eKeyValDict.Add("第2學期取得學分數", "取得學分");
                        //eKeyValDict.Add("累計取得學分數", "累計學分");
                        //eKeyValDict.Add("大功統計", "大功");
                        //eKeyValDict.Add("小功統計", "小功");
                        //eKeyValDict.Add("嘉獎統計", "嘉獎");
                        //eKeyValDict.Add("大過統計", "大過");
                        //eKeyValDict.Add("小過統計", "小過");
                        //eKeyValDict.Add("警告統計", "警告");
                        //eKeyValDict.Add("留校察看", "留校察看");
                        //eKeyValDict.Add("班導師", "導師姓名");
                        //eKeyValDict.Add("導師評語", "導師評語");


                        // 綜合評語
                        List<string> CommList = new List<string>();

                        #region 日常行為表現資料表
                        SmartSchool.Customization.Data.SystemInformation.getField("文字評量對照表");
                        foreach (System.Xml.XmlElement ele in (SmartSchool.Customization.Data.SystemInformation.Fields["文字評量對照表"] as System.Xml.XmlElement).SelectNodes("Content/Morality"))
                        {
                            string face = ele.GetAttribute("Face");
                            string f1 = "第1學期綜合表現：" + face;
                            string f2 = "第2學期綜合表現：" + face;
                            if (!table.Columns.Contains(f1))
                                table.Columns.Add(f1);

                            if (!table.Columns.Contains(f2))
                                table.Columns.Add(f2);

                            if (!CommList.Contains(face))
                                CommList.Add(face);

                            if (!_dtEpost.Columns.Contains(f1))
                                _dtEpost.Columns.Add(f1);
                            if (!_dtEpost.Columns.Contains(f2))
                                _dtEpost.Columns.Add(f2);

                            //if (!eKeyValDict.ContainsKey(f1))
                            //    eKeyValDict.Add(f1, face);
                        }
                        #endregion
                        #region 缺曠對照表
                        List<K12.Data.PeriodMappingInfo> periodMappingInfos = K12.Data.PeriodMapping.SelectAll();
                        Dictionary<string, string> dicPeriodMappingType = new Dictionary<string, string>();
                        List<string> periodTypes = new List<string>();
                        foreach (K12.Data.PeriodMappingInfo periodMappingInfo in periodMappingInfos)
                        {
                            if (!dicPeriodMappingType.ContainsKey(periodMappingInfo.Name))
                                dicPeriodMappingType.Add(periodMappingInfo.Name, periodMappingInfo.Type);

                            if (!periodTypes.Contains(periodMappingInfo.Type))
                                periodTypes.Add(periodMappingInfo.Type);
                        }

                        List<string> atN = new List<string>();
                        atN.Add("第1學期");
                        atN.Add("第2學期");
                        atN.Add("學年");
                        int aidx = 1;
                        foreach (var absence in K12.Data.AbsenceMapping.SelectAll())
                        {
                            foreach (var pt in periodTypes)
                            {
                                string attendanceKey = pt + "_" + absence.Name;
                                foreach (string str in atN)
                                {
                                    string attendanceKeyName = str + attendanceKey;
                                    if (!table.Columns.Contains(attendanceKeyName))
                                    {
                                        table.Columns.Add(attendanceKeyName);
                                    }
                                    if (!_dtEpost.Columns.Contains(attendanceKeyName))
                                        _dtEpost.Columns.Add(attendanceKeyName);
                                }

                                //if (pt == "一般")
                                //    aidx = 1;
                                //else
                                //    aidx = 2;

                                //string attendanceKey1 = absence.Name + aidx;


                                //if (!eKeyValDict.ContainsKey(attendanceKey))
                                //    eKeyValDict.Add(attendanceKey, attendanceKey1);
                            }
                        }
                        #endregion

                        if (!_dtEpost.Columns.Contains("第1學期導師姓名"))
                            _dtEpost.Columns.Add("第1學期導師姓名");
                        if (!_dtEpost.Columns.Contains("第1學期導師評語"))
                            _dtEpost.Columns.Add("第1學期導師評語");
                        if (!_dtEpost.Columns.Contains("第2學期導師姓名"))
                            _dtEpost.Columns.Add("第2學期導師姓名");
                        if (!_dtEpost.Columns.Contains("第2學期導師評語"))
                            _dtEpost.Columns.Add("第2學期導師評語");



                        bkw.ReportProgress(3);
                        // region 整理學生住址
                        accessHelper.StudentHelper.FillContactInfo(studentRecords);

                        // region 整理學生父母及監護人
                        accessHelper.StudentHelper.FillParentInfo(studentRecords);

                        bkw.ReportProgress(10);
                        // region 整理同年級學生
                        //整理選取學生的年級
                        Dictionary<string, List<StudentRecord>> gradeyearStudents = new Dictionary<string, List<StudentRecord>>();
                        foreach (var studentRec in studentRecords)
                        {
                            string grade = "";
                            if (studentRec.RefClass != null)
                                grade = "" + studentRec.RefClass.GradeYear;
                            if (!gradeyearStudents.ContainsKey(grade))
                                gradeyearStudents.Add(grade, new List<StudentRecord>());
                            gradeyearStudents[grade].Add(studentRec);
                        }
                        foreach (var classRec in accessHelper.ClassHelper.GetAllClass())
                        {
                            if (gradeyearStudents.ContainsKey("" + classRec.GradeYear))
                            {
                                //用班級去取出可能有相關的學生
                                foreach (var studentRec in classRec.Students)
                                {
                                    string grade = "";
                                    if (studentRec.RefClass != null)
                                        grade = "" + studentRec.RefClass.GradeYear;
                                    if (!gradeyearStudents[grade].Contains(studentRec))
                                        gradeyearStudents[grade].Add(studentRec);
                                }
                            }
                        }


                        bkw.ReportProgress(15);
                        #region 取得學生類別
                        Dictionary<string, List<K12.Data.StudentTagRecord>> studentTags = new Dictionary<string, List<K12.Data.StudentTagRecord>>();
                        List<string> list = new List<string>();
                        foreach (var sRecs in gradeyearStudents.Values)
                        {
                            foreach (var stuRec in sRecs)
                            {
                                list.Add(stuRec.StudentID);
                            }
                        }
                        foreach (var tag in K12.Data.StudentTag.SelectByStudentIDs(list))
                        {
                            if (!studentTags.ContainsKey(tag.RefStudentID))
                                studentTags.Add(tag.RefStudentID, new List<K12.Data.StudentTagRecord>());
                            studentTags[tag.RefStudentID].Add(tag);
                        }
                        #endregion

                        bkw.ReportProgress(20);
                        //等到成績載完
                        //   scoreReady.WaitOne();
                        bkw.ReportProgress(35);
                        int progressCount = 0;
                        // region 計算總分及各項目排名
                        Dictionary<string, string> studentTag1Group = new Dictionary<string, string>();
                        Dictionary<string, string> studentTag2Group = new Dictionary<string, string>();

                        int total = 0;
                        foreach (var gss in gradeyearStudents.Values)
                        {
                            total += gss.Count;
                        }
                        bkw.ReportProgress(40);
                        foreach (string gradeyear in gradeyearStudents.Keys)
                        {
                            //找出全年級學生
                            foreach (var studentRec in gradeyearStudents[gradeyear])
                            {
                                string studentID = studentRec.StudentID;
                                bool rank = true;
                                string tag1ID = "";
                                string tag2ID = "";
                                // region 分析學生所屬類別
                                if (studentTags.ContainsKey(studentID))
                                {
                                    foreach (var tag in studentTags[studentID])
                                    {
                                        // region 判斷學生是否屬於不排名類別
                                        if (conf.RankFilterTagList.Contains(tag.RefTagID))
                                        {
                                            rank = false;
                                        }

                                        // region 判斷學生在類別排名1中所屬的類別
                                        if (tag1ID == "" && conf.TagRank1TagList.Contains(tag.RefTagID))
                                        {
                                            tag1ID = tag.RefTagID;
                                            studentTag1Group.Add(studentID, tag1ID);
                                        }

                                        // region 判斷學生在類別排名2中所屬的類別
                                        if (tag2ID == "" && conf.TagRank2TagList.Contains(tag.RefTagID))
                                        {
                                            tag2ID = tag.RefTagID;
                                            studentTag2Group.Add(studentID, tag2ID);
                                        }

                                    }
                                }


                                progressCount++;
                                bkw.ReportProgress(40 + progressCount * 30 / total);
                            }
                        }


                        // 先取得 K12 StudentRec,因為後面透過 k12.data 取資料有的傳入ID,有的傳入 Record 有點亂
                        List<K12.Data.StudentRecord> StudRecList = new List<K12.Data.StudentRecord>();
                        List<string> StudIDList = (from data in studentRecords select data.StudentID).ToList();
                        StudRecList = K12.Data.Student.SelectByIDs(StudIDList);

                        int SchoolYear;
                        int.TryParse(conf.SchoolYear, out SchoolYear);


                        Dictionary<string, decimal> ServiceLearningByDateDict2 = new Dictionary<string, decimal>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict2 = new Dictionary<string, Dictionary<string, int>>();
                        Dictionary<string, decimal> ServiceLearningByDateDict1 = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> ServiceLearningByDateDict = new Dictionary<string, decimal>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict = new Dictionary<string, Dictionary<string, int>>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict1 = new Dictionary<string, Dictionary<string, int>>();

                        // 取得暫存資料 學習服務區間時數                       
                        // 第2學期
                        ServiceLearningByDateDict2 = Utility.GetServiceLearningBySchoolYearSemester(StudIDList, SchoolYear, 2);

                        // 第1學期
                        ServiceLearningByDateDict1 = Utility.GetServiceLearningBySchoolYearSemester(StudIDList, SchoolYear, 1);
                        // 學年
                        ServiceLearningByDateDict = Utility.GetServiceLearningBySchoolYear(StudIDList, SchoolYear);

                        // 取得學年缺曠
                        AttendanceCountDict = Utility.GetAttendanceCountBySchoolYear(StudRecList, SchoolYear);

                        // 取得缺曠第1學期
                        AttendanceCountDict1 = Utility.GetAttendanceCountBySchoolYearSemester(StudRecList, SchoolYear, 1);
                        // 取得缺曠第2學期
                        AttendanceCountDict2 = Utility.GetAttendanceCountBySchoolYearSemester(StudRecList, SchoolYear, 2);

                        List<K12.Data.PeriodMappingInfo> PeriodMappingList = K12.Data.PeriodMapping.SelectAll();
                        // 節次>類別
                        Dictionary<string, string> PeriodMappingDict = new Dictionary<string, string>();
                        foreach (K12.Data.PeriodMappingInfo rec in PeriodMappingList)
                        {
                            if (!PeriodMappingDict.ContainsKey(rec.Name))
                                PeriodMappingDict.Add(rec.Name, rec.Type);
                        }

                        bkw.ReportProgress(70);
                        elseReady.WaitOne();

                        _studPassSumCreditDict1.Clear();
                        _studPassSumCreditDict2.Clear();
                        _studSumPassCreditDict1.Clear();
                        _studSumPassCreditDict2.Clear();
                        _studSumScoreDict1.Clear();
                        _studSumScoreDict2.Clear();
                        _studPassSumCreditDictAll.Clear();

                        progressCount = 0;
                        // region 填入資料表
                        foreach (var stuRec in studentRecords)
                        {
                            // 第1學期取得學分數
                            if (!_studPassSumCreditDict1.ContainsKey(stuRec.StudentID))
                                _studPassSumCreditDict1.Add(stuRec.StudentID, 0);

                            // 第2學期取得學分數
                            if (!_studPassSumCreditDict2.ContainsKey(stuRec.StudentID))
                                _studPassSumCreditDict2.Add(stuRec.StudentID, 0);

                            // 第1學期累計取得學分數
                            if (!_studSumPassCreditDict1.ContainsKey(stuRec.StudentID))
                                _studSumPassCreditDict1.Add(stuRec.StudentID, 0);

                            // 第2學期累計取得學分數
                            if (!_studSumPassCreditDict2.ContainsKey(stuRec.StudentID))
                                _studSumPassCreditDict2.Add(stuRec.StudentID, 0);

                            // 第1學期加權總分
                            if (!_studSumScoreDict1.ContainsKey(stuRec.StudentID))
                                _studSumScoreDict1.Add(stuRec.StudentID, 0);

                            // 第2學期加權總分
                            if (!_studSumScoreDict2.ContainsKey(stuRec.StudentID))
                                _studSumScoreDict2.Add(stuRec.StudentID, 0);

                            // 學年取得學分數
                            if (!_studPassSumCreditDictAll.ContainsKey(stuRec.StudentID))
                                _studPassSumCreditDictAll.Add(stuRec.StudentID, 0);

                            string studentID = stuRec.StudentID;
                            string gradeYear = (stuRec.RefClass == null ? "" : "" + stuRec.RefClass.GradeYear);
                            DataRow row = table.NewRow();
                            // region 基本資料

                            // 服務學習時數
                            if (ServiceLearningByDateDict1.ContainsKey(studentID))
                            {
                                // 處理學生上學習服務時數	
                                row["第1學期服務學習時數"] = ServiceLearningByDateDict1[studentID];
                            }

                            if (ServiceLearningByDateDict2.ContainsKey(studentID))
                            {
                                // 處理學生下學習服務時數	
                                row["第2學期服務學習時數"] = ServiceLearningByDateDict2[studentID];
                            }

                            if (ServiceLearningByDateDict.ContainsKey(studentID))
                            {
                                // 處理學生學年學習服務時數	
                                row["學年服務學習時數"] = ServiceLearningByDateDict[studentID];
                            }

                            // 處理缺曠
                            if (AttendanceCountDict.ContainsKey(studentID))
                            {
                                foreach (KeyValuePair<string, int> data in AttendanceCountDict[studentID])
                                {
                                    string keyS = "學年" + data.Key;

                                    if (table.Columns.Contains(keyS))

                                        row[keyS] = data.Value;
                                }
                            }

                            if (AttendanceCountDict1.ContainsKey(studentID))
                            {
                                foreach (KeyValuePair<string, int> data in AttendanceCountDict1[studentID])
                                {
                                    string keyS = "第1學期" + data.Key;

                                    if (table.Columns.Contains(keyS))

                                        row[keyS] = data.Value;
                                }
                            }

                            if (AttendanceCountDict2.ContainsKey(studentID))
                            {
                                foreach (KeyValuePair<string, int> data in AttendanceCountDict2[studentID])
                                {
                                    string keyS = "第2學期" + data.Key;

                                    if (table.Columns.Contains(keyS))

                                        row[keyS] = data.Value;
                                }
                            }

                            row["學生系統編號"] = stuRec.StudentID;
                            row["學校名稱"] = SmartSchool.Customization.Data.SystemInformation.SchoolChineseName;
                            row["學校地址"] = SmartSchool.Customization.Data.SystemInformation.Address;
                            row["學校電話"] = SmartSchool.Customization.Data.SystemInformation.Telephone;
                            row["收件人地址"] = stuRec.ContactInfo.MailingAddress.FullAddress != "" ?
                                                stuRec.ContactInfo.MailingAddress.FullAddress : stuRec.ContactInfo.PermanentAddress.FullAddress;
                            row["收件人"] = stuRec.ParentInfo.CustodianName != "" ? stuRec.ParentInfo.CustodianName :
                                                (stuRec.ParentInfo.FatherName != "" ? stuRec.ParentInfo.FatherName :
                                                    (stuRec.ParentInfo.FatherName != "" ? stuRec.ParentInfo.MotherName : stuRec.StudentName));
                            //«通訊地址»«通訊地址郵遞區號»«通訊地址內容»
                            //«戶籍地址»«戶籍地址郵遞區號»«戶籍地址內容»
                            //«監護人»«父親»«母親»«科別名稱»
                            row["通訊地址"] = stuRec.ContactInfo.MailingAddress.FullAddress;
                            row["通訊地址郵遞區號"] = stuRec.ContactInfo.MailingAddress.ZipCode;
                            row["通訊地址內容"] = stuRec.ContactInfo.MailingAddress.County + stuRec.ContactInfo.MailingAddress.Town + stuRec.ContactInfo.MailingAddress.DetailAddress;
                            row["戶籍地址"] = stuRec.ContactInfo.PermanentAddress.FullAddress;
                            row["戶籍地址郵遞區號"] = stuRec.ContactInfo.PermanentAddress.ZipCode;
                            row["戶籍地址內容"] = stuRec.ContactInfo.PermanentAddress.County + stuRec.ContactInfo.PermanentAddress.Town + stuRec.ContactInfo.PermanentAddress.DetailAddress;
                            row["監護人"] = stuRec.ParentInfo.CustodianName;
                            row["父親"] = stuRec.ParentInfo.FatherName;
                            row["母親"] = stuRec.ParentInfo.MotherName;
                            row["科別名稱"] = stuRec.Department;

                            row["學年度"] = conf.SchoolYear;
                            //row["學期"] = conf.Semester;
                            row["班級科別名稱"] = stuRec.RefClass == null ? "" : stuRec.RefClass.Department;
                            row["班級"] = stuRec.RefClass == null ? "" : stuRec.RefClass.ClassName;
                            row["學生班級年級"] = stuRec.RefClass == null ? "" : stuRec.RefClass.GradeYear;
                            row["班導師"] = (stuRec.RefClass == null || stuRec.RefClass.RefTeacher == null) ? "" : stuRec.RefClass.RefTeacher.TeacherName;
                            row["座號"] = stuRec.SeatNo;
                            row["學號"] = stuRec.StudentNumber;
                            row["姓名"] = stuRec.StudentName;

                            int currentGradeYear = -1;
                            foreach (var semesterEntryScore in stuRec.SemesterEntryScoreList)
                            {
                                if (("" + semesterEntryScore.SchoolYear) == conf.SchoolYear)
                                {
                                    if (("" + semesterEntryScore.Semester) == "1")
                                    {
                                        row["第1學期" + semesterEntryScore.Entry + "成績"] = semesterEntryScore.Score;
                                        currentGradeYear = semesterEntryScore.GradeYear;
                                    }
                                    if (("" + semesterEntryScore.Semester) == "2")
                                    {
                                        row["第2學期" + semesterEntryScore.Entry + "成績"] = semesterEntryScore.Score;
                                        currentGradeYear = semesterEntryScore.GradeYear;
                                    }

                                }
                            }


                            foreach (var k in new string[] { "班", "科", "校" })
                            {
                                if (stuRec.Fields.ContainsKey("第1學期學業成績" + k + "排名")) row["第1學期學業成績" + k + "排名"] = "" + stuRec.Fields["第1學期學業成績" + k + "排名"];
                                if (stuRec.Fields.ContainsKey("第1學期學業成績" + k + "排名母數")) row["第1學期學業成績" + k + "排名母數"] = "" + stuRec.Fields["第1學期學業成績" + k + "排名母數"];

                                if (stuRec.Fields.ContainsKey("第2學期學業成績" + k + "排名")) row["第2學期學業成績" + k + "排名"] = "" + stuRec.Fields["第2學期學業成績" + k + "排名"];
                                if (stuRec.Fields.ContainsKey("第2學期學業成績" + k + "排名母數")) row["第2學期學業成績" + k + "排名母數"] = "" + stuRec.Fields["第2學期學業成績" + k + "排名母數"];
                            }

                            //類別1
                            if (studentTag1Group.ContainsKey(studentID))
                            {
                                foreach (var tag in studentTags[studentID])
                                {
                                    if (tag.RefTagID == studentTag1Group[studentID])
                                    {
                                        key = "第1學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第1學期學業成績類別1排名"] = "" + stuRec.Fields[key];
                                        key = "第1學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第1學期學業成績類別1排名母數"] = "" + stuRec.Fields[key];

                                        key = "第2學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第2學期學業成績類別1排名"] = "" + stuRec.Fields[key];
                                        key = "第2學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第2學期學業成績類別1排名母數"] = "" + stuRec.Fields[key];

                                    }
                                }
                            }
                            //類別2
                            if (studentTag2Group.ContainsKey(studentID))
                            {
                                foreach (var tag in studentTags[studentID])
                                {
                                    if (tag.RefTagID == studentTag2Group[studentID])
                                    {
                                        key = "第1學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第1學期學業成績類別2排名"] = "" + stuRec.Fields[key];
                                        key = "第1學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第1學期學業成績類別2排名母數"] = "" + stuRec.Fields[key];
                                        key = "第1學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第2學期學業成績類別2排名"] = "" + stuRec.Fields[key];
                                        key = "第2學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["第2學期學業成績類別2排名母數"] = "" + stuRec.Fields[key];

                                    }
                                }
                            }

                            #region 學年學業成績及排名
                            foreach (var schoolYearEntryScore in stuRec.SchoolYearEntryScoreList)
                            {
                                if (("" + schoolYearEntryScore.SchoolYear) == conf.SchoolYear)
                                {
                                    row["學年" + schoolYearEntryScore.Entry + "成績"] = schoolYearEntryScore.Score;
                                }
                            }
                            if (stuRec.Fields.ContainsKey("SchoolYearEntryClassRating"))
                            {
                                System.Xml.XmlElement _sems_ratings = stuRec.Fields["SchoolYearEntryClassRating"] as System.Xml.XmlElement;
                                string path = string.Format("SchoolYearEntryScore[SchoolYear='{0}']/ClassRating/Rating/Item[@分項='學業']/@排名", conf.SchoolYear);
                                System.Xml.XmlNode result = _sems_ratings.SelectSingleNode(path);
                                if (result != null)
                                {
                                    row["學年學業成績班排名"] = result.InnerText;
                                }
                            }
                            #endregion

                            #region 整理科目順序

                            List<string> subjectNames = new List<string>();

                            // 學期科目
                            foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                            {
                                if (("" + semesterSubjectScore.SchoolYear) == conf.SchoolYear)
                                {
                                    if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是")
                                    {
                                        if (!subjectNames.Contains(semesterSubjectScore.Subject))
                                            subjectNames.Add(semesterSubjectScore.Subject);
                                    }
                                }
                            }

                            // 學年科目
                            foreach (var schoolYearSubjectScore in stuRec.SchoolYearSubjectScoreList)
                            {
                                if (("" + schoolYearSubjectScore.SchoolYear) == conf.SchoolYear)
                                {
                                    if (!subjectNames.Contains(schoolYearSubjectScore.Subject))
                                        subjectNames.Add(schoolYearSubjectScore.Subject);
                                }
                            }

                            var subjectNameList = new List<string>();
                            // 有勾選科目
                            foreach (string name in subjectNames)
                            {
                                if (conf.PrintSubjectList.Contains(name))
                                    subjectNameList.Add(name);
                            }

                            subjectNameList.Sort(new StringComparer("國文"
                                            , "英文"
                                            , "數學"
                                            , "理化"
                                            , "生物"
                                            , "社會"
                                            , "物理"
                                            , "化學"
                                            , "歷史"
                                            , "地理"
                                            , "公民"));

                            #endregion

                            // 處理學期取得學分與累計取得學分
                            foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                            {
                                if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是")
                                {

                                    // 第1學期取得
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == "1" && semesterSubjectScore.Pass)
                                        _studPassSumCreditDict1[stuRec.StudentID] += semesterSubjectScore.CreditDec();                                        

                                    // 第2學期取得
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == "2" && semesterSubjectScore.Pass)
                                        _studPassSumCreditDict2[stuRec.StudentID] += semesterSubjectScore.CreditDec();

                                    // 第1學期累計取得學分
                                    if (semesterSubjectScore.Semester.ToString() == "1" && semesterSubjectScore.Pass)
                                        _studSumPassCreditDict1[stuRec.StudentID] += semesterSubjectScore.CreditDec();

                                    // 第2學期累計取得學分
                                    if (semesterSubjectScore.Semester.ToString() == "2" && semesterSubjectScore.Pass)
                                        _studSumPassCreditDict2[stuRec.StudentID] += semesterSubjectScore.CreditDec();

                                    // 累計取得
                                    if (semesterSubjectScore.Pass)
                                        _studPassSumCreditDictAll[stuRec.StudentID] += semesterSubjectScore.CreditDec();

                                    // 第1學期加權總分
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == "1")
                                        _studSumScoreDict1[stuRec.StudentID] += semesterSubjectScore.Score * semesterSubjectScore.CreditDec();

                                    // 第2學期加權總分
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == "2")
                                        _studSumScoreDict2[stuRec.StudentID] += semesterSubjectScore.Score * semesterSubjectScore.CreditDec();

                                }
                            }

                            row["第1學期取得學分數"] = _studPassSumCreditDict1[stuRec.StudentID];
                            row["第2學期取得學分數"] = _studPassSumCreditDict2[stuRec.StudentID];
                            row["第1學期累計取得學分數"] = _studSumPassCreditDict1[stuRec.StudentID];
                            row["第2學期累計取得學分數"] = _studSumPassCreditDict2[stuRec.StudentID];
                            row["累計取得學分數"] = _studPassSumCreditDictAll[stuRec.StudentID];
                            row["第1學期加權總分"] = _studSumScoreDict1[stuRec.StudentID];
                            row["第2學期加權總分"] = _studSumScoreDict2[stuRec.StudentID];

                            int subjectIndex = 1;
                            // 學期科目
                            foreach (string subjectName in subjectNameList)
                            {
                                if (subjectIndex <= conf.SubjectLimit)
                                {
                                    decimal? subjectNumber = null;

                                    foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                                    {
                                        if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是"
                                            && semesterSubjectScore.Subject == subjectName
                                            && ("" + semesterSubjectScore.SchoolYear) == conf.SchoolYear)
                                        {
                                            decimal level;
                                            #region 第1學期學期成績
                                            if (("" + semesterSubjectScore.Semester) == "1")
                                            {
                                                subjectNumber = decimal.TryParse(semesterSubjectScore.Level, out level) ? (decimal?)level : null;
                                                row["科目名稱" + subjectIndex] = semesterSubjectScore.Subject + GetNumber(subjectNumber);
                                                row["第1學期學分數" + subjectIndex] = semesterSubjectScore.CreditDec();
                                                row["科目必選修" + subjectIndex] = semesterSubjectScore.Require ? "必修" : "選修";
                                                row["科目校部定" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("修課校部訂");
                                                row["科目註記" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("註記");

                                                row["第1學期科目取得學分" + subjectIndex] = semesterSubjectScore.Pass ? "是" : "否";
                                                row["第1學期科目未取得學分註記" + subjectIndex] = semesterSubjectScore.Pass ? "" : "\f";

                                                //"原始成績", "學年調整成績", "擇優採計成績", "補考成績", "重修成績"
                                                if (semesterSubjectScore.Detail.GetAttribute("不需評分") != "是")
                                                {
                                                    row["第1學期科目原始成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("原始成績");
                                                    row["第1學期科目補考成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("補考成績");
                                                    row["第1學期科目重修成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("重修成績");
                                                    row["第1學期科目手動調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("擇優採計成績");
                                                    row["第1學期科目學年調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("學年調整成績");
                                                    row["第1學期科目成績" + subjectIndex] = semesterSubjectScore.Score;

                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("原始成績"))
                                                        row["第1學期科目原始成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("補考成績"))
                                                        row["第1學期科目補考成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("重修成績"))
                                                        row["第1學期科目重修成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("擇優採計成績"))
                                                        row["第1學期科目手動成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("學年調整成績"))
                                                        row["第1學期科目學年成績註記" + subjectIndex] = "\f";
                                                }


                                            }
                                            #endregion


                                            #region 第2學期學期成績

                                            if (("" + semesterSubjectScore.Semester) == "2")
                                            {
                                                subjectNumber = decimal.TryParse(semesterSubjectScore.Level, out level) ? (decimal?)level : null;
                                                row["科目名稱" + subjectIndex] = semesterSubjectScore.Subject + GetNumber(subjectNumber);
                                                row["第2學期學分數" + subjectIndex] = semesterSubjectScore.CreditDec();
                                                row["科目必選修" + subjectIndex] = semesterSubjectScore.Require ? "必修" : "選修";
                                                row["科目校部定" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("修課校部訂");
                                                row["科目註記" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("註記");
                                                row["科目取得學分" + subjectIndex] = semesterSubjectScore.Pass ? "是" : "否";
                                                row["科目未取得學分註記" + subjectIndex] = semesterSubjectScore.Pass ? "" : "\f";

                                                //"原始成績", "學年調整成績", "擇優採計成績", "補考成績", "重修成績"
                                                if (semesterSubjectScore.Detail.GetAttribute("不需評分") != "是")
                                                {
                                                    row["第2學期科目原始成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("原始成績");
                                                    row["第2學期科目補考成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("補考成績");
                                                    row["第2學期科目重修成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("重修成績");
                                                    row["第2學期科目手動調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("擇優採計成績");
                                                    row["第2學期科目學年調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("學年調整成績");
                                                    row["第2學期科目成績" + subjectIndex] = semesterSubjectScore.Score;

                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("原始成績"))
                                                        row["第2學期科目原始成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("補考成績"))
                                                        row["第2學期科目補考成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("重修成績"))
                                                        row["第2學期科目重修成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("擇優採計成績"))
                                                        row["第2學期科目手動成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("學年調整成績"))
                                                        row["第2學期科目學年成績註記" + subjectIndex] = "\f";
                                                }
                                                // region 第2學期科目班、科、校、類別1、類別2排名
                                                key = "第2學期科目排名成績" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目排名成績" + subjectIndex] = "" + stuRec.Fields[key];
                                                //班
                                                key = "第2學期科目班排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目班排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                key = "第2學期科目班排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目班排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                                //科
                                                key = "第2學期科目科排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目科排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                key = "第2學期科目班科名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目科排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                                //校
                                                key = "第2學期科目校排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目全校排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                key = "第2學期科目科校名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                if (stuRec.Fields.ContainsKey(key))
                                                    row["第2學期科目全校排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                                //類別1
                                                if (studentTag1Group.ContainsKey(studentID))
                                                {
                                                    foreach (var tag in studentTags[studentID])
                                                    {
                                                        if (tag.RefTagID == studentTag1Group[studentID])
                                                        {
                                                            key = "第2學期科目成績" + tag.Name + "排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                            if (stuRec.Fields.ContainsKey(key))
                                                                row["第2學期科目類別1排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                            key = "第2學期科目成績" + tag.Name + "排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                            if (stuRec.Fields.ContainsKey(key))
                                                                row["第2學期科目類別1排名母數" + subjectIndex] = "" + stuRec.Fields[key];

                                                        }
                                                    }
                                                }
                                                //類別2
                                                if (studentTag2Group.ContainsKey(studentID))
                                                {
                                                    foreach (var tag in studentTags[studentID])
                                                    {
                                                        if (tag.RefTagID == studentTag2Group[studentID])
                                                        {
                                                            key = "第2學期科目成績" + tag.Name + "排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                            if (stuRec.Fields.ContainsKey(key))
                                                                row["第2學期科目類別2排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                            key = "第2學期科目成績" + tag.Name + "排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                            if (stuRec.Fields.ContainsKey(key))
                                                                row["第2學期科目類別2排名母數" + subjectIndex] = "" + stuRec.Fields[key];

                                                        }
                                                    }
                                                }
                                            }

                                            #endregion
                                        }
                                    }

                                    #region 學年成績
                                    foreach (var schoolYearSubjectScore in stuRec.SchoolYearSubjectScoreList)
                                    {
                                        if (("" + schoolYearSubjectScore.SchoolYear) == conf.SchoolYear
                                        && schoolYearSubjectScore.Subject == subjectName)
                                        {
                                            row["科目名稱" + subjectIndex] = schoolYearSubjectScore.Subject;
                                            row["學年科目成績" + subjectIndex] = schoolYearSubjectScore.Score;
                                        }
                                    }
                                    #endregion

                                    subjectIndex++;
                                }
                                else
                                {
                                    //重要!!發現資料在樣板中印不下時一定要記錄起來，否則使用者自己不會去發現的
                                    if (!overflowRecords.Contains(stuRec))
                                        overflowRecords.Add(stuRec);
                                }
                            }


                            #region 學務資料

                            #region 綜合表現
                            foreach (SemesterMoralScoreInfo info in stuRec.SemesterMoralScoreList)
                            {
                                if (("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    if (("" + info.Semester) == "1")
                                    {
                                        row["第1學期導師評語"] = info.SupervisedByComment;
                                        System.Xml.XmlElement xml = info.Detail;
                                        foreach (System.Xml.XmlElement each in xml.SelectNodes("TextScore/Morality"))
                                        {
                                            string face = each.GetAttribute("Face");
                                            if ((SmartSchool.Customization.Data.SystemInformation.Fields["文字評量對照表"] as System.Xml.XmlElement).SelectSingleNode("Content/Morality[@Face='" + face + "']") != null)
                                            {
                                                string comment = each.InnerText;
                                                row["第1學期綜合表現：" + face] = each.InnerText;
                                            }
                                        }
                                    }
                                    if (("" + info.Semester) == "2")
                                    {
                                        row["第2學期導師評語"] = info.SupervisedByComment;
                                        System.Xml.XmlElement xml = info.Detail;
                                        foreach (System.Xml.XmlElement each in xml.SelectNodes("TextScore/Morality"))
                                        {
                                            string face = each.GetAttribute("Face");
                                            if ((SmartSchool.Customization.Data.SystemInformation.Fields["文字評量對照表"] as System.Xml.XmlElement).SelectSingleNode("Content/Morality[@Face='" + face + "']") != null)
                                            {
                                                string comment = each.InnerText;
                                                row["第2學期綜合表現：" + face] = each.InnerText;
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region 獎懲統計

                            int 第1學期大功 = 0;
                            int 第1學期小功 = 0;
                            int 第1學期嘉獎 = 0;
                            int 第1學期大過 = 0;
                            int 第1學期小過 = 0;
                            int 第1學期警告 = 0;
                            bool 第1學期留校察看 = false;
                            int 第2學期大功 = 0;
                            int 第2學期小功 = 0;
                            int 第2學期嘉獎 = 0;
                            int 第2學期大過 = 0;
                            int 第2學期小過 = 0;
                            int 第2學期警告 = 0;
                            bool 第2學期留校察看 = false;
                            int 學年大功 = 0;
                            int 學年小功 = 0;
                            int 學年嘉獎 = 0;
                            int 學年大過 = 0;
                            int 學年小過 = 0;
                            int 學年警告 = 0;
                            bool 學年留校察看 = false;

                            foreach (RewardInfo info in stuRec.RewardList)
                            {
                                if (("" + info.Semester) == "1" && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    第1學期大功 += info.AwardA;
                                    第1學期小功 += info.AwardB;
                                    第1學期嘉獎 += info.AwardC;
                                    if (!info.Cleared)
                                    {
                                        第1學期大過 += info.FaultA;
                                        第1學期小過 += info.FaultB;
                                        第1學期警告 += info.FaultC;
                                    }
                                    if (info.UltimateAdmonition)
                                        第1學期留校察看 = true;
                                }

                                if (("" + info.Semester) == "2" && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    第2學期大功 += info.AwardA;
                                    第2學期小功 += info.AwardB;
                                    第2學期嘉獎 += info.AwardC;
                                    if (!info.Cleared)
                                    {
                                        第2學期大過 += info.FaultA;
                                        第2學期小過 += info.FaultB;
                                        第2學期警告 += info.FaultC;
                                    }
                                    if (info.UltimateAdmonition)
                                        第2學期留校察看 = true;
                                }
                                if (("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    學年大功 += info.AwardA;
                                    學年小功 += info.AwardB;
                                    學年嘉獎 += info.AwardC;
                                    if (!info.Cleared)
                                    {
                                        學年大過 += info.FaultA;
                                        學年小過 += info.FaultB;
                                        學年警告 += info.FaultC;
                                    }
                                    if (info.UltimateAdmonition)
                                        學年留校察看 = true;
                                }
                            }
                            row["第1學期大功統計"] = 第1學期大功 == 0 ? "" : ("" + 第1學期大功);
                            row["第1學期小功統計"] = 第1學期小功 == 0 ? "" : ("" + 第1學期小功);
                            row["第1學期嘉獎統計"] = 第1學期嘉獎 == 0 ? "" : ("" + 第1學期嘉獎);
                            row["第1學期大過統計"] = 第1學期大過 == 0 ? "" : ("" + 第1學期大過);
                            row["第1學期小過統計"] = 第1學期小過 == 0 ? "" : ("" + 第1學期小過);
                            row["第1學期警告統計"] = 第1學期警告 == 0 ? "" : ("" + 第1學期警告);
                            row["第1學期留校察看"] = 第1學期留校察看 ? "是" : "";
                            row["第2學期大功統計"] = 第2學期大功 == 0 ? "" : ("" + 第2學期大功);
                            row["第2學期小功統計"] = 第2學期小功 == 0 ? "" : ("" + 第2學期小功);
                            row["第2學期嘉獎統計"] = 第2學期嘉獎 == 0 ? "" : ("" + 第2學期嘉獎);
                            row["第2學期大過統計"] = 第2學期大過 == 0 ? "" : ("" + 第2學期大過);
                            row["第2學期小過統計"] = 第2學期小過 == 0 ? "" : ("" + 第2學期小過);
                            row["第2學期警告統計"] = 第2學期警告 == 0 ? "" : ("" + 第2學期警告);
                            row["第2學期留校察看"] = 第2學期留校察看 ? "是" : "";
                            row["學年大功統計"] = 學年大功 == 0 ? "" : ("" + 學年大功);
                            row["學年小功統計"] = 學年小功 == 0 ? "" : ("" + 學年小功);
                            row["學年嘉獎統計"] = 學年嘉獎 == 0 ? "" : ("" + 學年嘉獎);
                            row["學年大過統計"] = 學年大過 == 0 ? "" : ("" + 學年大過);
                            row["學年小過統計"] = 學年小過 == 0 ? "" : ("" + 學年小過);
                            row["學年警告統計"] = 學年警告 == 0 ? "" : ("" + 學年警告);
                            row["學年留校察看"] = 學年留校察看 ? "是" : "";
                            #endregion

                            #region 缺曠統計
                            Dictionary<string, int> 缺曠項目統計 = new Dictionary<string, int>();
                            foreach (AttendanceInfo info in stuRec.AttendanceList)
                            {
                                // 第1學期
                                if (("" + info.Semester) == "1" && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    string infoType = "";
                                    if (dicPeriodMappingType.ContainsKey(info.Period))
                                        infoType = dicPeriodMappingType[info.Period];
                                    else
                                        infoType = "";
                                    string attendanceKey = "第1學期" + infoType + "_" + info.Absence;
                                    if (!缺曠項目統計.ContainsKey(attendanceKey))
                                        缺曠項目統計.Add(attendanceKey, 0);
                                    缺曠項目統計[attendanceKey]++;
                                }

                                // 第2學期
                                if (("" + info.Semester) == "2" && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    string infoType = "";
                                    if (dicPeriodMappingType.ContainsKey(info.Period))
                                        infoType = dicPeriodMappingType[info.Period];
                                    else
                                        infoType = "";
                                    string attendanceKey = "第2學期" + infoType + "_" + info.Absence;
                                    if (!缺曠項目統計.ContainsKey(attendanceKey))
                                        缺曠項目統計.Add(attendanceKey, 0);
                                    缺曠項目統計[attendanceKey]++;
                                }

                                // 學年
                                if (("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    string infoType = "";
                                    if (dicPeriodMappingType.ContainsKey(info.Period))
                                        infoType = dicPeriodMappingType[info.Period];
                                    else
                                        infoType = "";
                                    string attendanceKey = "學年" + infoType + "_" + info.Absence;
                                    if (!缺曠項目統計.ContainsKey(attendanceKey))
                                        缺曠項目統計.Add(attendanceKey, 0);
                                    缺曠項目統計[attendanceKey]++;
                                }

                            }

                            foreach (string attendanceKey in 缺曠項目統計.Keys)
                            {
                                row[attendanceKey] = 缺曠項目統計[attendanceKey] == 0 ? "" : ("" + 缺曠項目統計[attendanceKey]);
                            }

                            #endregion

                            #endregion


                            table.Rows.Add(row);
                            // debug
                            table.TableName = "test";
                            table.WriteXml(Application.StartupPath + @"\學年成績test.xml");
                            progressCount++;
                            bkw.ReportProgress(70 + progressCount * 20 / selectedStudents.Count);
                        }

                        bkw.ReportProgress(90);

                        //// 取得樣版合併欄位名稱
                        //StreamWriter sw = new StreamWriter(Application.StartupPath + "\\fieldnames.txt");
                        //foreach (string str in conf.Template.MailMerge.GetFieldNames())
                        //{
                        //    sw.WriteLine(str);
                        //}
                        //sw.Flush();
                        //sw.Close();
                        //document = conf.Template;
                        //document.MailMerge.Execute(table);
                        //document.MailMerge.RemoveEmptyParagraphs = true;
                        //document.MailMerge.DeleteFields();

                        #region 處理 epost
                        foreach (DataRow dr in table.Rows)
                        {
                            DataRow data = _dtEpost.NewRow();

                            // 取得學生及格與補考標準
                            string studID = dr["學生系統編號"].ToString();
                            int grYear;
                            int.TryParse(dr["學生班級年級"].ToString(), out grYear);

                            // 及格
                            decimal scA = 0;
                            // 補考
                            decimal scB = 0;
                            if (StudentApplyLimitDict.ContainsKey(studID))
                            {
                                string sA = grYear + "_及";
                                string sB = grYear + "_補";

                                if (StudentApplyLimitDict[studID].ContainsKey(sA))
                                    scA = StudentApplyLimitDict[studID][sA];

                                if (StudentApplyLimitDict[studID].ContainsKey(sB))
                                    scB = StudentApplyLimitDict[studID][sB];
                            }

                            dr["學期科目成績及格標準"] = scA;
                            dr["學期科目成績補考標準"] = scB;

                            // POSTALADDRESS
                            string address = dr["收件人地址"].ToString();
                            string zip1 = dr["通訊地址郵遞區號"].ToString() + " ";
                            string zip2 = dr["戶籍地址郵遞區號"].ToString() + " ";
                            if (address.Contains(zip1))
                            {
                                address = address.Replace(zip1, "");
                                data["POSTALCODE"] = dr["通訊地址郵遞區號"].ToString();
                            }

                            if (address.Contains(zip2))
                            {
                                address = address.Replace(zip2, "");
                                data["POSTALCODE"] = dr["戶籍地址郵遞區號"].ToString();
                            }

                            data["POSTALADDRESS"] = address;

                            // 處理科目成績
                            for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                            {
                                if (dr["科目名稱" + subjectIndex].ToString() != "")
                                {
                                    // 第1學期科目成績
                                    decimal sc1;
                                    if (decimal.TryParse(dr["第1學期科目成績" + subjectIndex].ToString(), out sc1))
                                    {
                                        // 小於及格標準
                                        if (sc1 < scA)
                                        {
                                            // 可以補考,不可補考
                                            if (sc1 >= scB)
                                            {
                                                data["第1學期備註" + subjectIndex] = "*";
                                                // *
                                                dr["第1學期科目可補考註記" + subjectIndex] = "\f";
                                            }
                                            else
                                            {
                                                data["第1學期備註" + subjectIndex] = "#";
                                                // #
                                                dr["第1學期科目不可補考註記" + subjectIndex] = "\f";
                                            }
                                        }
                                    }

                                    // 第2學期科目成績
                                    decimal sc2;
                                    if (decimal.TryParse(dr["第2學期科目成績" + subjectIndex].ToString(), out sc2))
                                    {
                                        // 小於及格標準
                                        if (sc2 < scA)
                                        {
                                            // 可以補考,不可補考
                                            if (sc2 >= scB)
                                            {
                                                data["第2學期備註" + subjectIndex] = "*";
                                                // *
                                                dr["第2學期科目可補考註記" + subjectIndex] = "\f";
                                            }
                                            else
                                            {
                                                data["第2學期備註" + subjectIndex] = "#";
                                                // #
                                                dr["第2學期科目不可補考註記" + subjectIndex] = "\f";
                                            }
                                        }
                                    }

                                }
                            }

                            // 處理資料填入ePost欄位
                            foreach (DataColumn dc in _dtEpost.Columns)
                            {
                                if (table.Columns.Contains(dc.ColumnName))
                                    data[dc.ColumnName] = dr[dc.ColumnName];
                            }

                            // 處理綜合評語字串前加"
                            foreach (string str in CommList)
                            {
                                if (_dtEpost.Columns.Contains(str))
                                    data[str] = @"""" + data[str].ToString() + @"""";
                            }

                            data["第1學期導師評語"] = @"""" + data["第1學期導師評語"].ToString() + @"""";
                            data["第2學期導師評語"] = @"""" + data["第2學期導師評語"].ToString() + @"""";
                            _dtEpost.Rows.Add(data);
                        }
                        #endregion

                        // debug
                        _dtEpost.TableName = "epost";
                        _dtEpost.WriteXml(Application.StartupPath + "\\學年成績testEpost.xml");

                    }
                    catch (Exception exception)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("發生錯誤:\n" + exception.Message);
                        exc = exception;
                    }
                };
                bkw.RunWorkerAsync();
            }

        }

        private static int DataSort(DataRow x, DataRow y)
        {
            string xx = x["座號"].ToString().PadLeft(3, '0');
            string yy = y["座號"].ToString().PadLeft(3, '0');

            return xx.CompareTo(yy);
        }
    }
}
