using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.IO;
//using K12.Data;
//using SHSchool.Data;
using SmartSchool.Customization.Data;
using System.Threading;
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool;

namespace SH_SemesterScoreReport_hwsh
{
    // 惠文客製期末成績通知單epost
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            FISCA.Permission.Catalog cat = FISCA.Permission.RoleAclSource.Instance["學生"]["功能按鈕"];
            cat.Add(new FISCA.Permission.RibbonFeature("SHSchool.SH_hwSH_SemesterScoreReport_hwsh", "期末成績通知單(惠文)epost"));

            var btn = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["期末成績通知單(惠文)epost"];
            btn.Enable = true;

            //K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate { btn.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current["SHSchool.SH_hwSH_SemesterScoreReport_hwsh"].Executable; };
            btn.Click += new EventHandler(Program_Click);


        }

        private static string GetNumber(decimal? p)
        {
            if (p == null) return "";
            string levelNumber;
            switch (((int)p.Value))
            {
                #region 對應levelNumber
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
                #endregion
            }
            return levelNumber;
        }

        // 本學期修習
        static Dictionary<string, decimal> _studSumCreditDict1 = new Dictionary<string, decimal>();

        // 本學期取得
        static Dictionary<string, decimal> _studPassSumCreditDict1 = new Dictionary<string, decimal>();
        
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
                #region 所有的合併欄位
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
                table.Columns.Add("試別");

                table.Columns.Add("收件人");
                table.Columns.Add("學年度");
                table.Columns.Add("學期");
                table.Columns.Add("班級科別名稱");
                table.Columns.Add("班級");
                table.Columns.Add("班導師");
                table.Columns.Add("座號");
                table.Columns.Add("學號");
                table.Columns.Add("姓名");
                table.Columns.Add("定期評量");
                table.Columns.Add("本學期修習學分數");
                table.Columns.Add("本學期取得學分數");
                table.Columns.Add("累計取得學分數");

                for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                {
                    table.Columns.Add("科目名稱" + subjectIndex);
                    table.Columns.Add("學分數" + subjectIndex);
                    table.Columns.Add("前次成績" + subjectIndex);
                    table.Columns.Add("科目成績" + subjectIndex);
                    // 新增學期科目相關成績--
                    table.Columns.Add("科目必選修" + subjectIndex);
                    table.Columns.Add("科目校部定" + subjectIndex);
                    table.Columns.Add("科目註記" + subjectIndex);
                    table.Columns.Add("科目取得學分" + subjectIndex);
                    table.Columns.Add("科目未取得學分註記" + subjectIndex);
                    table.Columns.Add("學期科目原始成績" + subjectIndex);
                    table.Columns.Add("學期科目補考成績" + subjectIndex);
                    table.Columns.Add("學期科目重修成績" + subjectIndex);
                    table.Columns.Add("學期科目手動調整成績" + subjectIndex);
                    table.Columns.Add("學期科目學年調整成績" + subjectIndex);
                    table.Columns.Add("學期科目成績" + subjectIndex);
                    table.Columns.Add("學期科目原始成績註記" + subjectIndex);
                    table.Columns.Add("學期科目補考成績註記" + subjectIndex);
                    table.Columns.Add("學期科目重修成績註記" + subjectIndex);
                    table.Columns.Add("學期科目手動成績註記" + subjectIndex);
                    table.Columns.Add("學期科目學年成績註記" + subjectIndex);
                    table.Columns.Add("學期科目可補考註記" + subjectIndex);
                    table.Columns.Add("學期科目不可補考註記" + subjectIndex);
                    // 新增學期科目排名
                    table.Columns.Add("學期科目排名成績" + subjectIndex);
                    table.Columns.Add("學期科目班排名" + subjectIndex);
                    table.Columns.Add("學期科目班排名母數" + subjectIndex);
                    table.Columns.Add("學期科目科排名" + subjectIndex);
                    table.Columns.Add("學期科目科排名母數" + subjectIndex);
                    table.Columns.Add("學期科目類別1排名" + subjectIndex);
                    table.Columns.Add("學期科目類別1排名母數" + subjectIndex);
                    table.Columns.Add("學期科目類別2排名" + subjectIndex);
                    table.Columns.Add("學期科目類別2排名母數" + subjectIndex);
                    table.Columns.Add("學期科目全校排名" + subjectIndex);
                    table.Columns.Add("學期科目全校排名母數" + subjectIndex);
                    // 新增上學期科目相關成績--
                    table.Columns.Add("上學期科目原始成績" + subjectIndex);
                    table.Columns.Add("上學期科目補考成績" + subjectIndex);
                    table.Columns.Add("上學期科目重修成績" + subjectIndex);
                    table.Columns.Add("上學期科目手動調整成績" + subjectIndex);
                    table.Columns.Add("上學期科目學年調整成績" + subjectIndex);
                    table.Columns.Add("上學期科目成績" + subjectIndex);
                    table.Columns.Add("上學期科目原始成績註記" + subjectIndex);
                    table.Columns.Add("上學期科目補考成績註記" + subjectIndex);
                    table.Columns.Add("上學期科目重修成績註記" + subjectIndex);
                    table.Columns.Add("上學期科目手動成績註記" + subjectIndex);
                    table.Columns.Add("上學期科目學年成績註記" + subjectIndex);
                    table.Columns.Add("上學期科目取得學分" + subjectIndex);
                    table.Columns.Add("上學期科目未取得學分註記" + subjectIndex);

                    // 新增學年科目成績--
                    table.Columns.Add("學年科目成績" + subjectIndex);
                    //定期評量成績個項欄位--
                    table.Columns.Add("班排名" + subjectIndex);
                    table.Columns.Add("班排名母數" + subjectIndex);
                    table.Columns.Add("科排名" + subjectIndex);
                    table.Columns.Add("科排名母數" + subjectIndex);
                    table.Columns.Add("類別1排名" + subjectIndex);
                    table.Columns.Add("類別1排名母數" + subjectIndex);
                    table.Columns.Add("類別2排名" + subjectIndex);
                    table.Columns.Add("類別2排名母數" + subjectIndex);
                    table.Columns.Add("全校排名" + subjectIndex);
                    table.Columns.Add("全校排名母數" + subjectIndex);
                    #region 瘋狂的組距及分析
                    table.Columns.Add("班高標" + subjectIndex); table.Columns.Add("科高標" + subjectIndex); table.Columns.Add("校高標" + subjectIndex); table.Columns.Add("類1高標" + subjectIndex); table.Columns.Add("類2高標" + subjectIndex);
                    table.Columns.Add("班均標" + subjectIndex); table.Columns.Add("科均標" + subjectIndex); table.Columns.Add("校均標" + subjectIndex); table.Columns.Add("類1均標" + subjectIndex); table.Columns.Add("類2均標" + subjectIndex);
                    table.Columns.Add("班低標" + subjectIndex); table.Columns.Add("科低標" + subjectIndex); table.Columns.Add("校低標" + subjectIndex); table.Columns.Add("類1低標" + subjectIndex); table.Columns.Add("類2低標" + subjectIndex);
                    table.Columns.Add("班標準差" + subjectIndex); table.Columns.Add("科標準差" + subjectIndex); table.Columns.Add("校標準差" + subjectIndex); table.Columns.Add("類1標準差" + subjectIndex); table.Columns.Add("類2標準差" + subjectIndex);
                    table.Columns.Add("班組距" + subjectIndex + "count90"); table.Columns.Add("科組距" + subjectIndex + "count90"); table.Columns.Add("校組距" + subjectIndex + "count90"); table.Columns.Add("類1組距" + subjectIndex + "count90"); table.Columns.Add("類2組距" + subjectIndex + "count90");
                    table.Columns.Add("班組距" + subjectIndex + "count80"); table.Columns.Add("科組距" + subjectIndex + "count80"); table.Columns.Add("校組距" + subjectIndex + "count80"); table.Columns.Add("類1組距" + subjectIndex + "count80"); table.Columns.Add("類2組距" + subjectIndex + "count80");
                    table.Columns.Add("班組距" + subjectIndex + "count70"); table.Columns.Add("科組距" + subjectIndex + "count70"); table.Columns.Add("校組距" + subjectIndex + "count70"); table.Columns.Add("類1組距" + subjectIndex + "count70"); table.Columns.Add("類2組距" + subjectIndex + "count70");
                    table.Columns.Add("班組距" + subjectIndex + "count60"); table.Columns.Add("科組距" + subjectIndex + "count60"); table.Columns.Add("校組距" + subjectIndex + "count60"); table.Columns.Add("類1組距" + subjectIndex + "count60"); table.Columns.Add("類2組距" + subjectIndex + "count60");
                    table.Columns.Add("班組距" + subjectIndex + "count50"); table.Columns.Add("科組距" + subjectIndex + "count50"); table.Columns.Add("校組距" + subjectIndex + "count50"); table.Columns.Add("類1組距" + subjectIndex + "count50"); table.Columns.Add("類2組距" + subjectIndex + "count50");
                    table.Columns.Add("班組距" + subjectIndex + "count40"); table.Columns.Add("科組距" + subjectIndex + "count40"); table.Columns.Add("校組距" + subjectIndex + "count40"); table.Columns.Add("類1組距" + subjectIndex + "count40"); table.Columns.Add("類2組距" + subjectIndex + "count40");
                    table.Columns.Add("班組距" + subjectIndex + "count30"); table.Columns.Add("科組距" + subjectIndex + "count30"); table.Columns.Add("校組距" + subjectIndex + "count30"); table.Columns.Add("類1組距" + subjectIndex + "count30"); table.Columns.Add("類2組距" + subjectIndex + "count30");
                    table.Columns.Add("班組距" + subjectIndex + "count20"); table.Columns.Add("科組距" + subjectIndex + "count20"); table.Columns.Add("校組距" + subjectIndex + "count20"); table.Columns.Add("類1組距" + subjectIndex + "count20"); table.Columns.Add("類2組距" + subjectIndex + "count20");
                    table.Columns.Add("班組距" + subjectIndex + "count10"); table.Columns.Add("科組距" + subjectIndex + "count10"); table.Columns.Add("校組距" + subjectIndex + "count10"); table.Columns.Add("類1組距" + subjectIndex + "count10"); table.Columns.Add("類2組距" + subjectIndex + "count10");
                    table.Columns.Add("班組距" + subjectIndex + "count100Up"); table.Columns.Add("科組距" + subjectIndex + "count100Up"); table.Columns.Add("校組距" + subjectIndex + "count100Up"); table.Columns.Add("類1組距" + subjectIndex + "count100Up"); table.Columns.Add("類2組距" + subjectIndex + "count100Up");
                    table.Columns.Add("班組距" + subjectIndex + "count90Up"); table.Columns.Add("科組距" + subjectIndex + "count90Up"); table.Columns.Add("校組距" + subjectIndex + "count90Up"); table.Columns.Add("類1組距" + subjectIndex + "count90Up"); table.Columns.Add("類2組距" + subjectIndex + "count90Up");
                    table.Columns.Add("班組距" + subjectIndex + "count80Up"); table.Columns.Add("科組距" + subjectIndex + "count80Up"); table.Columns.Add("校組距" + subjectIndex + "count80Up"); table.Columns.Add("類1組距" + subjectIndex + "count80Up"); table.Columns.Add("類2組距" + subjectIndex + "count80Up");
                    table.Columns.Add("班組距" + subjectIndex + "count70Up"); table.Columns.Add("科組距" + subjectIndex + "count70Up"); table.Columns.Add("校組距" + subjectIndex + "count70Up"); table.Columns.Add("類1組距" + subjectIndex + "count70Up"); table.Columns.Add("類2組距" + subjectIndex + "count70Up");
                    table.Columns.Add("班組距" + subjectIndex + "count60Up"); table.Columns.Add("科組距" + subjectIndex + "count60Up"); table.Columns.Add("校組距" + subjectIndex + "count60Up"); table.Columns.Add("類1組距" + subjectIndex + "count60Up"); table.Columns.Add("類2組距" + subjectIndex + "count60Up");
                    table.Columns.Add("班組距" + subjectIndex + "count50Up"); table.Columns.Add("科組距" + subjectIndex + "count50Up"); table.Columns.Add("校組距" + subjectIndex + "count50Up"); table.Columns.Add("類1組距" + subjectIndex + "count50Up"); table.Columns.Add("類2組距" + subjectIndex + "count50Up");
                    table.Columns.Add("班組距" + subjectIndex + "count40Up"); table.Columns.Add("科組距" + subjectIndex + "count40Up"); table.Columns.Add("校組距" + subjectIndex + "count40Up"); table.Columns.Add("類1組距" + subjectIndex + "count40Up"); table.Columns.Add("類2組距" + subjectIndex + "count40Up");
                    table.Columns.Add("班組距" + subjectIndex + "count30Up"); table.Columns.Add("科組距" + subjectIndex + "count30Up"); table.Columns.Add("校組距" + subjectIndex + "count30Up"); table.Columns.Add("類1組距" + subjectIndex + "count30Up"); table.Columns.Add("類2組距" + subjectIndex + "count30Up");
                    table.Columns.Add("班組距" + subjectIndex + "count20Up"); table.Columns.Add("科組距" + subjectIndex + "count20Up"); table.Columns.Add("校組距" + subjectIndex + "count20Up"); table.Columns.Add("類1組距" + subjectIndex + "count20Up"); table.Columns.Add("類2組距" + subjectIndex + "count20Up");
                    table.Columns.Add("班組距" + subjectIndex + "count10Up"); table.Columns.Add("科組距" + subjectIndex + "count10Up"); table.Columns.Add("校組距" + subjectIndex + "count10Up"); table.Columns.Add("類1組距" + subjectIndex + "count10Up"); table.Columns.Add("類2組距" + subjectIndex + "count10Up");
                    table.Columns.Add("班組距" + subjectIndex + "count90Down"); table.Columns.Add("科組距" + subjectIndex + "count90Down"); table.Columns.Add("校組距" + subjectIndex + "count90Down"); table.Columns.Add("類1組距" + subjectIndex + "count90Down"); table.Columns.Add("類2組距" + subjectIndex + "count90Down");
                    table.Columns.Add("班組距" + subjectIndex + "count80Down"); table.Columns.Add("科組距" + subjectIndex + "count80Down"); table.Columns.Add("校組距" + subjectIndex + "count80Down"); table.Columns.Add("類1組距" + subjectIndex + "count80Down"); table.Columns.Add("類2組距" + subjectIndex + "count80Down");
                    table.Columns.Add("班組距" + subjectIndex + "count70Down"); table.Columns.Add("科組距" + subjectIndex + "count70Down"); table.Columns.Add("校組距" + subjectIndex + "count70Down"); table.Columns.Add("類1組距" + subjectIndex + "count70Down"); table.Columns.Add("類2組距" + subjectIndex + "count70Down");
                    table.Columns.Add("班組距" + subjectIndex + "count60Down"); table.Columns.Add("科組距" + subjectIndex + "count60Down"); table.Columns.Add("校組距" + subjectIndex + "count60Down"); table.Columns.Add("類1組距" + subjectIndex + "count60Down"); table.Columns.Add("類2組距" + subjectIndex + "count60Down");
                    table.Columns.Add("班組距" + subjectIndex + "count50Down"); table.Columns.Add("科組距" + subjectIndex + "count50Down"); table.Columns.Add("校組距" + subjectIndex + "count50Down"); table.Columns.Add("類1組距" + subjectIndex + "count50Down"); table.Columns.Add("類2組距" + subjectIndex + "count50Down");
                    table.Columns.Add("班組距" + subjectIndex + "count40Down"); table.Columns.Add("科組距" + subjectIndex + "count40Down"); table.Columns.Add("校組距" + subjectIndex + "count40Down"); table.Columns.Add("類1組距" + subjectIndex + "count40Down"); table.Columns.Add("類2組距" + subjectIndex + "count40Down");
                    table.Columns.Add("班組距" + subjectIndex + "count30Down"); table.Columns.Add("科組距" + subjectIndex + "count30Down"); table.Columns.Add("校組距" + subjectIndex + "count30Down"); table.Columns.Add("類1組距" + subjectIndex + "count30Down"); table.Columns.Add("類2組距" + subjectIndex + "count30Down");
                    table.Columns.Add("班組距" + subjectIndex + "count20Down"); table.Columns.Add("科組距" + subjectIndex + "count20Down"); table.Columns.Add("校組距" + subjectIndex + "count20Down"); table.Columns.Add("類1組距" + subjectIndex + "count20Down"); table.Columns.Add("類2組距" + subjectIndex + "count20Down");
                    table.Columns.Add("班組距" + subjectIndex + "count10Down"); table.Columns.Add("科組距" + subjectIndex + "count10Down"); table.Columns.Add("校組距" + subjectIndex + "count10Down"); table.Columns.Add("類1組距" + subjectIndex + "count10Down"); table.Columns.Add("類2組距" + subjectIndex + "count10Down");
                    #endregion
                }
                table.Columns.Add("總分");
                table.Columns.Add("總分班排名");
                table.Columns.Add("總分班排名母數");
                table.Columns.Add("總分科排名");
                table.Columns.Add("總分科排名母數");
                table.Columns.Add("總分全校排名");
                table.Columns.Add("總分全校排名母數");
                table.Columns.Add("平均");
                table.Columns.Add("平均班排名");
                table.Columns.Add("平均班排名母數");
                table.Columns.Add("平均科排名");
                table.Columns.Add("平均科排名母數");
                table.Columns.Add("平均全校排名");
                table.Columns.Add("平均全校排名母數");
                // 學期分項成績 --
                table.Columns.Add("學期學業成績");
                table.Columns.Add("學期體育成績");
                table.Columns.Add("學期國防通識成績");
                table.Columns.Add("學期健康與護理成績");
                table.Columns.Add("學期實習科目成績");
                table.Columns.Add("學期學業(原始)成績");
                table.Columns.Add("學期體育(原始)成績");
                table.Columns.Add("學期國防通識(原始)成績");
                table.Columns.Add("學期健康與護理(原始)成績");
                table.Columns.Add("學期實習科目(原始)成績");
                table.Columns.Add("學期專業科目成績");
                table.Columns.Add("學期專業科目(原始)成績");
                table.Columns.Add("學期德行成績");
                // 學期學業成績排名
                table.Columns.Add("學期學業成績班排名");
                table.Columns.Add("學期學業成績科排名");
                table.Columns.Add("學期學業成績類別1排名");
                table.Columns.Add("學期學業成績類別2排名");
                table.Columns.Add("學期學業成績校排名");
                table.Columns.Add("學期學業成績班排名母數");
                table.Columns.Add("學期學業成績科排名母數");
                table.Columns.Add("學期學業成績類別1排名母數");
                table.Columns.Add("學期學業成績類別2排名母數");
                table.Columns.Add("學期學業成績校排名母數");
                // 導師評語 --
                table.Columns.Add("導師評語");
                // 獎懲統計 --
                table.Columns.Add("大功統計");
                table.Columns.Add("小功統計");
                table.Columns.Add("嘉獎統計");
                table.Columns.Add("大過統計");
                table.Columns.Add("小過統計");
                table.Columns.Add("警告統計");
                table.Columns.Add("留校察看");
                // 上學期分項成績 --
                table.Columns.Add("上學期學業成績");
                table.Columns.Add("上學期體育成績");
                table.Columns.Add("上學期國防通識成績");
                table.Columns.Add("上學期健康與護理成績");
                table.Columns.Add("上學期實習科目成績");
                table.Columns.Add("上學期德行成績");
                // 學年分項成績 --
                table.Columns.Add("學年學業成績");
                table.Columns.Add("學年體育成績");
                table.Columns.Add("學年國防通識成績");
                table.Columns.Add("學年健康與護理成績");
                table.Columns.Add("學年實習科目成績");
                table.Columns.Add("學年德行成績");
                table.Columns.Add("學年學業成績班排名");

                // 服務學習時數
                table.Columns.Add("前學期服務學習時數");
                table.Columns.Add("本學期服務學習時數");
                table.Columns.Add("學年服務學習時數");

                // 缺曠統計
                // 動態新增缺曠統計，使用模式一般_曠課、一般_事假..
                foreach (string name in Utility.GetATMappingKey())
                {
                    table.Columns.Add("前學期" + name);
                    table.Columns.Add("本學期" + name);
                    table.Columns.Add("學年" + name);
                }
                // --
                table.Columns.Add("加權總分");
                table.Columns.Add("加權總分班排名");
                table.Columns.Add("加權總分班排名母數");
                table.Columns.Add("加權總分科排名");
                table.Columns.Add("加權總分科排名母數");
                table.Columns.Add("加權總分全校排名");
                table.Columns.Add("加權總分全校排名母數");
                table.Columns.Add("加權平均");
                table.Columns.Add("加權平均班排名");
                table.Columns.Add("加權平均班排名母數");
                table.Columns.Add("加權平均科排名");
                table.Columns.Add("加權平均科排名母數");
                table.Columns.Add("加權平均全校排名");
                table.Columns.Add("加權平均全校排名母數");

                table.Columns.Add("類別排名1");
                table.Columns.Add("類別1總分");
                table.Columns.Add("類別1總分排名");
                table.Columns.Add("類別1總分排名母數");
                table.Columns.Add("類別1平均");
                table.Columns.Add("類別1平均排名");
                table.Columns.Add("類別1平均排名母數");
                table.Columns.Add("類別1加權總分");
                table.Columns.Add("類別1加權總分排名");
                table.Columns.Add("類別1加權總分排名母數");
                table.Columns.Add("類別1加權平均");
                table.Columns.Add("類別1加權平均排名");
                table.Columns.Add("類別1加權平均排名母數");

                table.Columns.Add("類別排名2");
                table.Columns.Add("類別2總分");
                table.Columns.Add("類別2總分排名");
                table.Columns.Add("類別2總分排名母數");
                table.Columns.Add("類別2平均");
                table.Columns.Add("類別2平均排名");
                table.Columns.Add("類別2平均排名母數");
                table.Columns.Add("類別2加權總分");
                table.Columns.Add("類別2加權總分排名");
                table.Columns.Add("類別2加權總分排名母數");
                table.Columns.Add("類別2加權平均");
                table.Columns.Add("類別2加權平均排名");
                table.Columns.Add("類別2加權平均排名母數");
                table.Columns.Add("學期科目成績及格標準");
                table.Columns.Add("學期科目成績補考標準");

                #region 瘋狂的組距及分析
                table.Columns.Add("總分班高標"); table.Columns.Add("總分科高標"); table.Columns.Add("總分校高標"); table.Columns.Add("平均班高標"); table.Columns.Add("平均科高標"); table.Columns.Add("平均校高標"); table.Columns.Add("加權總分班高標"); table.Columns.Add("加權總分科高標"); table.Columns.Add("加權總分校高標"); table.Columns.Add("加權平均班高標"); table.Columns.Add("加權平均科高標"); table.Columns.Add("加權平均校高標"); table.Columns.Add("類1總分高標"); table.Columns.Add("類1平均高標"); table.Columns.Add("類1加權總分高標"); table.Columns.Add("類1加權平均高標"); table.Columns.Add("類2總分高標"); table.Columns.Add("類2平均高標"); table.Columns.Add("類2加權總分高標"); table.Columns.Add("類2加權平均高標");
                table.Columns.Add("總分班均標"); table.Columns.Add("總分科均標"); table.Columns.Add("總分校均標"); table.Columns.Add("平均班均標"); table.Columns.Add("平均科均標"); table.Columns.Add("平均校均標"); table.Columns.Add("加權總分班均標"); table.Columns.Add("加權總分科均標"); table.Columns.Add("加權總分校均標"); table.Columns.Add("加權平均班均標"); table.Columns.Add("加權平均科均標"); table.Columns.Add("加權平均校均標"); table.Columns.Add("類1總分均標"); table.Columns.Add("類1平均均標"); table.Columns.Add("類1加權總分均標"); table.Columns.Add("類1加權平均均標"); table.Columns.Add("類2總分均標"); table.Columns.Add("類2平均均標"); table.Columns.Add("類2加權總分均標"); table.Columns.Add("類2加權平均均標");
                table.Columns.Add("總分班低標"); table.Columns.Add("總分科低標"); table.Columns.Add("總分校低標"); table.Columns.Add("平均班低標"); table.Columns.Add("平均科低標"); table.Columns.Add("平均校低標"); table.Columns.Add("加權總分班低標"); table.Columns.Add("加權總分科低標"); table.Columns.Add("加權總分校低標"); table.Columns.Add("加權平均班低標"); table.Columns.Add("加權平均科低標"); table.Columns.Add("加權平均校低標"); table.Columns.Add("類1總分低標"); table.Columns.Add("類1平均低標"); table.Columns.Add("類1加權總分低標"); table.Columns.Add("類1加權平均低標"); table.Columns.Add("類2總分低標"); table.Columns.Add("類2平均低標"); table.Columns.Add("類2加權總分低標"); table.Columns.Add("類2加權平均低標");
                table.Columns.Add("總分班標準差"); table.Columns.Add("總分科標準差"); table.Columns.Add("總分校標準差"); table.Columns.Add("平均班標準差"); table.Columns.Add("平均科標準差"); table.Columns.Add("平均校標準差"); table.Columns.Add("加權總分班標準差"); table.Columns.Add("加權總分科標準差"); table.Columns.Add("加權總分校標準差"); table.Columns.Add("加權平均班標準差"); table.Columns.Add("加權平均科標準差"); table.Columns.Add("加權平均校標準差"); table.Columns.Add("類1總分標準差"); table.Columns.Add("類1平均標準差"); table.Columns.Add("類1加權總分標準差"); table.Columns.Add("類1加權平均標準差"); table.Columns.Add("類2總分標準差"); table.Columns.Add("類2平均標準差"); table.Columns.Add("類2加權總分標準差"); table.Columns.Add("類2加權平均標準差");
                table.Columns.Add("總分班組距count90"); table.Columns.Add("總分科組距count90"); table.Columns.Add("總分校組距count90"); table.Columns.Add("平均班組距count90"); table.Columns.Add("平均科組距count90"); table.Columns.Add("平均校組距count90"); table.Columns.Add("加權總分班組距count90"); table.Columns.Add("加權總分科組距count90"); table.Columns.Add("加權總分校組距count90"); table.Columns.Add("加權平均班組距count90"); table.Columns.Add("加權平均科組距count90"); table.Columns.Add("加權平均校組距count90"); table.Columns.Add("類1總分組距count90"); table.Columns.Add("類1平均組距count90"); table.Columns.Add("類1加權總分組距count90"); table.Columns.Add("類1加權平均組距count90"); table.Columns.Add("類2總分組距count90"); table.Columns.Add("類2平均組距count90"); table.Columns.Add("類2加權總分組距count90"); table.Columns.Add("類2加權平均組距count90");
                table.Columns.Add("總分班組距count80"); table.Columns.Add("總分科組距count80"); table.Columns.Add("總分校組距count80"); table.Columns.Add("平均班組距count80"); table.Columns.Add("平均科組距count80"); table.Columns.Add("平均校組距count80"); table.Columns.Add("加權總分班組距count80"); table.Columns.Add("加權總分科組距count80"); table.Columns.Add("加權總分校組距count80"); table.Columns.Add("加權平均班組距count80"); table.Columns.Add("加權平均科組距count80"); table.Columns.Add("加權平均校組距count80"); table.Columns.Add("類1總分組距count80"); table.Columns.Add("類1平均組距count80"); table.Columns.Add("類1加權總分組距count80"); table.Columns.Add("類1加權平均組距count80"); table.Columns.Add("類2總分組距count80"); table.Columns.Add("類2平均組距count80"); table.Columns.Add("類2加權總分組距count80"); table.Columns.Add("類2加權平均組距count80");
                table.Columns.Add("總分班組距count70"); table.Columns.Add("總分科組距count70"); table.Columns.Add("總分校組距count70"); table.Columns.Add("平均班組距count70"); table.Columns.Add("平均科組距count70"); table.Columns.Add("平均校組距count70"); table.Columns.Add("加權總分班組距count70"); table.Columns.Add("加權總分科組距count70"); table.Columns.Add("加權總分校組距count70"); table.Columns.Add("加權平均班組距count70"); table.Columns.Add("加權平均科組距count70"); table.Columns.Add("加權平均校組距count70"); table.Columns.Add("類1總分組距count70"); table.Columns.Add("類1平均組距count70"); table.Columns.Add("類1加權總分組距count70"); table.Columns.Add("類1加權平均組距count70"); table.Columns.Add("類2總分組距count70"); table.Columns.Add("類2平均組距count70"); table.Columns.Add("類2加權總分組距count70"); table.Columns.Add("類2加權平均組距count70");
                table.Columns.Add("總分班組距count60"); table.Columns.Add("總分科組距count60"); table.Columns.Add("總分校組距count60"); table.Columns.Add("平均班組距count60"); table.Columns.Add("平均科組距count60"); table.Columns.Add("平均校組距count60"); table.Columns.Add("加權總分班組距count60"); table.Columns.Add("加權總分科組距count60"); table.Columns.Add("加權總分校組距count60"); table.Columns.Add("加權平均班組距count60"); table.Columns.Add("加權平均科組距count60"); table.Columns.Add("加權平均校組距count60"); table.Columns.Add("類1總分組距count60"); table.Columns.Add("類1平均組距count60"); table.Columns.Add("類1加權總分組距count60"); table.Columns.Add("類1加權平均組距count60"); table.Columns.Add("類2總分組距count60"); table.Columns.Add("類2平均組距count60"); table.Columns.Add("類2加權總分組距count60"); table.Columns.Add("類2加權平均組距count60");
                table.Columns.Add("總分班組距count50"); table.Columns.Add("總分科組距count50"); table.Columns.Add("總分校組距count50"); table.Columns.Add("平均班組距count50"); table.Columns.Add("平均科組距count50"); table.Columns.Add("平均校組距count50"); table.Columns.Add("加權總分班組距count50"); table.Columns.Add("加權總分科組距count50"); table.Columns.Add("加權總分校組距count50"); table.Columns.Add("加權平均班組距count50"); table.Columns.Add("加權平均科組距count50"); table.Columns.Add("加權平均校組距count50"); table.Columns.Add("類1總分組距count50"); table.Columns.Add("類1平均組距count50"); table.Columns.Add("類1加權總分組距count50"); table.Columns.Add("類1加權平均組距count50"); table.Columns.Add("類2總分組距count50"); table.Columns.Add("類2平均組距count50"); table.Columns.Add("類2加權總分組距count50"); table.Columns.Add("類2加權平均組距count50");
                table.Columns.Add("總分班組距count40"); table.Columns.Add("總分科組距count40"); table.Columns.Add("總分校組距count40"); table.Columns.Add("平均班組距count40"); table.Columns.Add("平均科組距count40"); table.Columns.Add("平均校組距count40"); table.Columns.Add("加權總分班組距count40"); table.Columns.Add("加權總分科組距count40"); table.Columns.Add("加權總分校組距count40"); table.Columns.Add("加權平均班組距count40"); table.Columns.Add("加權平均科組距count40"); table.Columns.Add("加權平均校組距count40"); table.Columns.Add("類1總分組距count40"); table.Columns.Add("類1平均組距count40"); table.Columns.Add("類1加權總分組距count40"); table.Columns.Add("類1加權平均組距count40"); table.Columns.Add("類2總分組距count40"); table.Columns.Add("類2平均組距count40"); table.Columns.Add("類2加權總分組距count40"); table.Columns.Add("類2加權平均組距count40");
                table.Columns.Add("總分班組距count30"); table.Columns.Add("總分科組距count30"); table.Columns.Add("總分校組距count30"); table.Columns.Add("平均班組距count30"); table.Columns.Add("平均科組距count30"); table.Columns.Add("平均校組距count30"); table.Columns.Add("加權總分班組距count30"); table.Columns.Add("加權總分科組距count30"); table.Columns.Add("加權總分校組距count30"); table.Columns.Add("加權平均班組距count30"); table.Columns.Add("加權平均科組距count30"); table.Columns.Add("加權平均校組距count30"); table.Columns.Add("類1總分組距count30"); table.Columns.Add("類1平均組距count30"); table.Columns.Add("類1加權總分組距count30"); table.Columns.Add("類1加權平均組距count30"); table.Columns.Add("類2總分組距count30"); table.Columns.Add("類2平均組距count30"); table.Columns.Add("類2加權總分組距count30"); table.Columns.Add("類2加權平均組距count30");
                table.Columns.Add("總分班組距count20"); table.Columns.Add("總分科組距count20"); table.Columns.Add("總分校組距count20"); table.Columns.Add("平均班組距count20"); table.Columns.Add("平均科組距count20"); table.Columns.Add("平均校組距count20"); table.Columns.Add("加權總分班組距count20"); table.Columns.Add("加權總分科組距count20"); table.Columns.Add("加權總分校組距count20"); table.Columns.Add("加權平均班組距count20"); table.Columns.Add("加權平均科組距count20"); table.Columns.Add("加權平均校組距count20"); table.Columns.Add("類1總分組距count20"); table.Columns.Add("類1平均組距count20"); table.Columns.Add("類1加權總分組距count20"); table.Columns.Add("類1加權平均組距count20"); table.Columns.Add("類2總分組距count20"); table.Columns.Add("類2平均組距count20"); table.Columns.Add("類2加權總分組距count20"); table.Columns.Add("類2加權平均組距count20");
                table.Columns.Add("總分班組距count10"); table.Columns.Add("總分科組距count10"); table.Columns.Add("總分校組距count10"); table.Columns.Add("平均班組距count10"); table.Columns.Add("平均科組距count10"); table.Columns.Add("平均校組距count10"); table.Columns.Add("加權總分班組距count10"); table.Columns.Add("加權總分科組距count10"); table.Columns.Add("加權總分校組距count10"); table.Columns.Add("加權平均班組距count10"); table.Columns.Add("加權平均科組距count10"); table.Columns.Add("加權平均校組距count10"); table.Columns.Add("類1總分組距count10"); table.Columns.Add("類1平均組距count10"); table.Columns.Add("類1加權總分組距count10"); table.Columns.Add("類1加權平均組距count10"); table.Columns.Add("類2總分組距count10"); table.Columns.Add("類2平均組距count10"); table.Columns.Add("類2加權總分組距count10"); table.Columns.Add("類2加權平均組距count10");
                table.Columns.Add("總分班組距count100Up"); table.Columns.Add("總分科組距count100Up"); table.Columns.Add("總分校組距count100Up"); table.Columns.Add("平均班組距count100Up"); table.Columns.Add("平均科組距count100Up"); table.Columns.Add("平均校組距count100Up"); table.Columns.Add("加權總分班組距count100Up"); table.Columns.Add("加權總分科組距count100Up"); table.Columns.Add("加權總分校組距count100Up"); table.Columns.Add("加權平均班組距count100Up"); table.Columns.Add("加權平均科組距count100Up"); table.Columns.Add("加權平均校組距count100Up"); table.Columns.Add("類1總分組距count100Up"); table.Columns.Add("類1平均組距count100Up"); table.Columns.Add("類1加權總分組距count100Up"); table.Columns.Add("類1加權平均組距count100Up"); table.Columns.Add("類2總分組距count100Up"); table.Columns.Add("類2平均組距count100Up"); table.Columns.Add("類2加權總分組距count100Up"); table.Columns.Add("類2加權平均組距count100Up");
                table.Columns.Add("總分班組距count90Up"); table.Columns.Add("總分科組距count90Up"); table.Columns.Add("總分校組距count90Up"); table.Columns.Add("平均班組距count90Up"); table.Columns.Add("平均科組距count90Up"); table.Columns.Add("平均校組距count90Up"); table.Columns.Add("加權總分班組距count90Up"); table.Columns.Add("加權總分科組距count90Up"); table.Columns.Add("加權總分校組距count90Up"); table.Columns.Add("加權平均班組距count90Up"); table.Columns.Add("加權平均科組距count90Up"); table.Columns.Add("加權平均校組距count90Up"); table.Columns.Add("類1總分組距count90Up"); table.Columns.Add("類1平均組距count90Up"); table.Columns.Add("類1加權總分組距count90Up"); table.Columns.Add("類1加權平均組距count90Up"); table.Columns.Add("類2總分組距count90Up"); table.Columns.Add("類2平均組距count90Up"); table.Columns.Add("類2加權總分組距count90Up"); table.Columns.Add("類2加權平均組距count90Up");
                table.Columns.Add("總分班組距count80Up"); table.Columns.Add("總分科組距count80Up"); table.Columns.Add("總分校組距count80Up"); table.Columns.Add("平均班組距count80Up"); table.Columns.Add("平均科組距count80Up"); table.Columns.Add("平均校組距count80Up"); table.Columns.Add("加權總分班組距count80Up"); table.Columns.Add("加權總分科組距count80Up"); table.Columns.Add("加權總分校組距count80Up"); table.Columns.Add("加權平均班組距count80Up"); table.Columns.Add("加權平均科組距count80Up"); table.Columns.Add("加權平均校組距count80Up"); table.Columns.Add("類1總分組距count80Up"); table.Columns.Add("類1平均組距count80Up"); table.Columns.Add("類1加權總分組距count80Up"); table.Columns.Add("類1加權平均組距count80Up"); table.Columns.Add("類2總分組距count80Up"); table.Columns.Add("類2平均組距count80Up"); table.Columns.Add("類2加權總分組距count80Up"); table.Columns.Add("類2加權平均組距count80Up");
                table.Columns.Add("總分班組距count70Up"); table.Columns.Add("總分科組距count70Up"); table.Columns.Add("總分校組距count70Up"); table.Columns.Add("平均班組距count70Up"); table.Columns.Add("平均科組距count70Up"); table.Columns.Add("平均校組距count70Up"); table.Columns.Add("加權總分班組距count70Up"); table.Columns.Add("加權總分科組距count70Up"); table.Columns.Add("加權總分校組距count70Up"); table.Columns.Add("加權平均班組距count70Up"); table.Columns.Add("加權平均科組距count70Up"); table.Columns.Add("加權平均校組距count70Up"); table.Columns.Add("類1總分組距count70Up"); table.Columns.Add("類1平均組距count70Up"); table.Columns.Add("類1加權總分組距count70Up"); table.Columns.Add("類1加權平均組距count70Up"); table.Columns.Add("類2總分組距count70Up"); table.Columns.Add("類2平均組距count70Up"); table.Columns.Add("類2加權總分組距count70Up"); table.Columns.Add("類2加權平均組距count70Up");
                table.Columns.Add("總分班組距count60Up"); table.Columns.Add("總分科組距count60Up"); table.Columns.Add("總分校組距count60Up"); table.Columns.Add("平均班組距count60Up"); table.Columns.Add("平均科組距count60Up"); table.Columns.Add("平均校組距count60Up"); table.Columns.Add("加權總分班組距count60Up"); table.Columns.Add("加權總分科組距count60Up"); table.Columns.Add("加權總分校組距count60Up"); table.Columns.Add("加權平均班組距count60Up"); table.Columns.Add("加權平均科組距count60Up"); table.Columns.Add("加權平均校組距count60Up"); table.Columns.Add("類1總分組距count60Up"); table.Columns.Add("類1平均組距count60Up"); table.Columns.Add("類1加權總分組距count60Up"); table.Columns.Add("類1加權平均組距count60Up"); table.Columns.Add("類2總分組距count60Up"); table.Columns.Add("類2平均組距count60Up"); table.Columns.Add("類2加權總分組距count60Up"); table.Columns.Add("類2加權平均組距count60Up");
                table.Columns.Add("總分班組距count50Up"); table.Columns.Add("總分科組距count50Up"); table.Columns.Add("總分校組距count50Up"); table.Columns.Add("平均班組距count50Up"); table.Columns.Add("平均科組距count50Up"); table.Columns.Add("平均校組距count50Up"); table.Columns.Add("加權總分班組距count50Up"); table.Columns.Add("加權總分科組距count50Up"); table.Columns.Add("加權總分校組距count50Up"); table.Columns.Add("加權平均班組距count50Up"); table.Columns.Add("加權平均科組距count50Up"); table.Columns.Add("加權平均校組距count50Up"); table.Columns.Add("類1總分組距count50Up"); table.Columns.Add("類1平均組距count50Up"); table.Columns.Add("類1加權總分組距count50Up"); table.Columns.Add("類1加權平均組距count50Up"); table.Columns.Add("類2總分組距count50Up"); table.Columns.Add("類2平均組距count50Up"); table.Columns.Add("類2加權總分組距count50Up"); table.Columns.Add("類2加權平均組距count50Up");
                table.Columns.Add("總分班組距count40Up"); table.Columns.Add("總分科組距count40Up"); table.Columns.Add("總分校組距count40Up"); table.Columns.Add("平均班組距count40Up"); table.Columns.Add("平均科組距count40Up"); table.Columns.Add("平均校組距count40Up"); table.Columns.Add("加權總分班組距count40Up"); table.Columns.Add("加權總分科組距count40Up"); table.Columns.Add("加權總分校組距count40Up"); table.Columns.Add("加權平均班組距count40Up"); table.Columns.Add("加權平均科組距count40Up"); table.Columns.Add("加權平均校組距count40Up"); table.Columns.Add("類1總分組距count40Up"); table.Columns.Add("類1平均組距count40Up"); table.Columns.Add("類1加權總分組距count40Up"); table.Columns.Add("類1加權平均組距count40Up"); table.Columns.Add("類2總分組距count40Up"); table.Columns.Add("類2平均組距count40Up"); table.Columns.Add("類2加權總分組距count40Up"); table.Columns.Add("類2加權平均組距count40Up");
                table.Columns.Add("總分班組距count30Up"); table.Columns.Add("總分科組距count30Up"); table.Columns.Add("總分校組距count30Up"); table.Columns.Add("平均班組距count30Up"); table.Columns.Add("平均科組距count30Up"); table.Columns.Add("平均校組距count30Up"); table.Columns.Add("加權總分班組距count30Up"); table.Columns.Add("加權總分科組距count30Up"); table.Columns.Add("加權總分校組距count30Up"); table.Columns.Add("加權平均班組距count30Up"); table.Columns.Add("加權平均科組距count30Up"); table.Columns.Add("加權平均校組距count30Up"); table.Columns.Add("類1總分組距count30Up"); table.Columns.Add("類1平均組距count30Up"); table.Columns.Add("類1加權總分組距count30Up"); table.Columns.Add("類1加權平均組距count30Up"); table.Columns.Add("類2總分組距count30Up"); table.Columns.Add("類2平均組距count30Up"); table.Columns.Add("類2加權總分組距count30Up"); table.Columns.Add("類2加權平均組距count30Up");
                table.Columns.Add("總分班組距count20Up"); table.Columns.Add("總分科組距count20Up"); table.Columns.Add("總分校組距count20Up"); table.Columns.Add("平均班組距count20Up"); table.Columns.Add("平均科組距count20Up"); table.Columns.Add("平均校組距count20Up"); table.Columns.Add("加權總分班組距count20Up"); table.Columns.Add("加權總分科組距count20Up"); table.Columns.Add("加權總分校組距count20Up"); table.Columns.Add("加權平均班組距count20Up"); table.Columns.Add("加權平均科組距count20Up"); table.Columns.Add("加權平均校組距count20Up"); table.Columns.Add("類1總分組距count20Up"); table.Columns.Add("類1平均組距count20Up"); table.Columns.Add("類1加權總分組距count20Up"); table.Columns.Add("類1加權平均組距count20Up"); table.Columns.Add("類2總分組距count20Up"); table.Columns.Add("類2平均組距count20Up"); table.Columns.Add("類2加權總分組距count20Up"); table.Columns.Add("類2加權平均組距count20Up");
                table.Columns.Add("總分班組距count10Up"); table.Columns.Add("總分科組距count10Up"); table.Columns.Add("總分校組距count10Up"); table.Columns.Add("平均班組距count10Up"); table.Columns.Add("平均科組距count10Up"); table.Columns.Add("平均校組距count10Up"); table.Columns.Add("加權總分班組距count10Up"); table.Columns.Add("加權總分科組距count10Up"); table.Columns.Add("加權總分校組距count10Up"); table.Columns.Add("加權平均班組距count10Up"); table.Columns.Add("加權平均科組距count10Up"); table.Columns.Add("加權平均校組距count10Up"); table.Columns.Add("類1總分組距count10Up"); table.Columns.Add("類1平均組距count10Up"); table.Columns.Add("類1加權總分組距count10Up"); table.Columns.Add("類1加權平均組距count10Up"); table.Columns.Add("類2總分組距count10Up"); table.Columns.Add("類2平均組距count10Up"); table.Columns.Add("類2加權總分組距count10Up"); table.Columns.Add("類2加權平均組距count10Up");
                table.Columns.Add("總分班組距count90Down"); table.Columns.Add("總分科組距count90Down"); table.Columns.Add("總分校組距count90Down"); table.Columns.Add("平均班組距count90Down"); table.Columns.Add("平均科組距count90Down"); table.Columns.Add("平均校組距count90Down"); table.Columns.Add("加權總分班組距count90Down"); table.Columns.Add("加權總分科組距count90Down"); table.Columns.Add("加權總分校組距count90Down"); table.Columns.Add("加權平均班組距count90Down"); table.Columns.Add("加權平均科組距count90Down"); table.Columns.Add("加權平均校組距count90Down"); table.Columns.Add("類1總分組距count90Down"); table.Columns.Add("類1平均組距count90Down"); table.Columns.Add("類1加權總分組距count90Down"); table.Columns.Add("類1加權平均組距count90Down"); table.Columns.Add("類2總分組距count90Down"); table.Columns.Add("類2平均組距count90Down"); table.Columns.Add("類2加權總分組距count90Down"); table.Columns.Add("類2加權平均組距count90Down");
                table.Columns.Add("總分班組距count80Down"); table.Columns.Add("總分科組距count80Down"); table.Columns.Add("總分校組距count80Down"); table.Columns.Add("平均班組距count80Down"); table.Columns.Add("平均科組距count80Down"); table.Columns.Add("平均校組距count80Down"); table.Columns.Add("加權總分班組距count80Down"); table.Columns.Add("加權總分科組距count80Down"); table.Columns.Add("加權總分校組距count80Down"); table.Columns.Add("加權平均班組距count80Down"); table.Columns.Add("加權平均科組距count80Down"); table.Columns.Add("加權平均校組距count80Down"); table.Columns.Add("類1總分組距count80Down"); table.Columns.Add("類1平均組距count80Down"); table.Columns.Add("類1加權總分組距count80Down"); table.Columns.Add("類1加權平均組距count80Down"); table.Columns.Add("類2總分組距count80Down"); table.Columns.Add("類2平均組距count80Down"); table.Columns.Add("類2加權總分組距count80Down"); table.Columns.Add("類2加權平均組距count80Down");
                table.Columns.Add("總分班組距count70Down"); table.Columns.Add("總分科組距count70Down"); table.Columns.Add("總分校組距count70Down"); table.Columns.Add("平均班組距count70Down"); table.Columns.Add("平均科組距count70Down"); table.Columns.Add("平均校組距count70Down"); table.Columns.Add("加權總分班組距count70Down"); table.Columns.Add("加權總分科組距count70Down"); table.Columns.Add("加權總分校組距count70Down"); table.Columns.Add("加權平均班組距count70Down"); table.Columns.Add("加權平均科組距count70Down"); table.Columns.Add("加權平均校組距count70Down"); table.Columns.Add("類1總分組距count70Down"); table.Columns.Add("類1平均組距count70Down"); table.Columns.Add("類1加權總分組距count70Down"); table.Columns.Add("類1加權平均組距count70Down"); table.Columns.Add("類2總分組距count70Down"); table.Columns.Add("類2平均組距count70Down"); table.Columns.Add("類2加權總分組距count70Down"); table.Columns.Add("類2加權平均組距count70Down");
                table.Columns.Add("總分班組距count60Down"); table.Columns.Add("總分科組距count60Down"); table.Columns.Add("總分校組距count60Down"); table.Columns.Add("平均班組距count60Down"); table.Columns.Add("平均科組距count60Down"); table.Columns.Add("平均校組距count60Down"); table.Columns.Add("加權總分班組距count60Down"); table.Columns.Add("加權總分科組距count60Down"); table.Columns.Add("加權總分校組距count60Down"); table.Columns.Add("加權平均班組距count60Down"); table.Columns.Add("加權平均科組距count60Down"); table.Columns.Add("加權平均校組距count60Down"); table.Columns.Add("類1總分組距count60Down"); table.Columns.Add("類1平均組距count60Down"); table.Columns.Add("類1加權總分組距count60Down"); table.Columns.Add("類1加權平均組距count60Down"); table.Columns.Add("類2總分組距count60Down"); table.Columns.Add("類2平均組距count60Down"); table.Columns.Add("類2加權總分組距count60Down"); table.Columns.Add("類2加權平均組距count60Down");
                table.Columns.Add("總分班組距count50Down"); table.Columns.Add("總分科組距count50Down"); table.Columns.Add("總分校組距count50Down"); table.Columns.Add("平均班組距count50Down"); table.Columns.Add("平均科組距count50Down"); table.Columns.Add("平均校組距count50Down"); table.Columns.Add("加權總分班組距count50Down"); table.Columns.Add("加權總分科組距count50Down"); table.Columns.Add("加權總分校組距count50Down"); table.Columns.Add("加權平均班組距count50Down"); table.Columns.Add("加權平均科組距count50Down"); table.Columns.Add("加權平均校組距count50Down"); table.Columns.Add("類1總分組距count50Down"); table.Columns.Add("類1平均組距count50Down"); table.Columns.Add("類1加權總分組距count50Down"); table.Columns.Add("類1加權平均組距count50Down"); table.Columns.Add("類2總分組距count50Down"); table.Columns.Add("類2平均組距count50Down"); table.Columns.Add("類2加權總分組距count50Down"); table.Columns.Add("類2加權平均組距count50Down");
                table.Columns.Add("總分班組距count40Down"); table.Columns.Add("總分科組距count40Down"); table.Columns.Add("總分校組距count40Down"); table.Columns.Add("平均班組距count40Down"); table.Columns.Add("平均科組距count40Down"); table.Columns.Add("平均校組距count40Down"); table.Columns.Add("加權總分班組距count40Down"); table.Columns.Add("加權總分科組距count40Down"); table.Columns.Add("加權總分校組距count40Down"); table.Columns.Add("加權平均班組距count40Down"); table.Columns.Add("加權平均科組距count40Down"); table.Columns.Add("加權平均校組距count40Down"); table.Columns.Add("類1總分組距count40Down"); table.Columns.Add("類1平均組距count40Down"); table.Columns.Add("類1加權總分組距count40Down"); table.Columns.Add("類1加權平均組距count40Down"); table.Columns.Add("類2總分組距count40Down"); table.Columns.Add("類2平均組距count40Down"); table.Columns.Add("類2加權總分組距count40Down"); table.Columns.Add("類2加權平均組距count40Down");
                table.Columns.Add("總分班組距count30Down"); table.Columns.Add("總分科組距count30Down"); table.Columns.Add("總分校組距count30Down"); table.Columns.Add("平均班組距count30Down"); table.Columns.Add("平均科組距count30Down"); table.Columns.Add("平均校組距count30Down"); table.Columns.Add("加權總分班組距count30Down"); table.Columns.Add("加權總分科組距count30Down"); table.Columns.Add("加權總分校組距count30Down"); table.Columns.Add("加權平均班組距count30Down"); table.Columns.Add("加權平均科組距count30Down"); table.Columns.Add("加權平均校組距count30Down"); table.Columns.Add("類1總分組距count30Down"); table.Columns.Add("類1平均組距count30Down"); table.Columns.Add("類1加權總分組距count30Down"); table.Columns.Add("類1加權平均組距count30Down"); table.Columns.Add("類2總分組距count30Down"); table.Columns.Add("類2平均組距count30Down"); table.Columns.Add("類2加權總分組距count30Down"); table.Columns.Add("類2加權平均組距count30Down");
                table.Columns.Add("總分班組距count20Down"); table.Columns.Add("總分科組距count20Down"); table.Columns.Add("總分校組距count20Down"); table.Columns.Add("平均班組距count20Down"); table.Columns.Add("平均科組距count20Down"); table.Columns.Add("平均校組距count20Down"); table.Columns.Add("加權總分班組距count20Down"); table.Columns.Add("加權總分科組距count20Down"); table.Columns.Add("加權總分校組距count20Down"); table.Columns.Add("加權平均班組距count20Down"); table.Columns.Add("加權平均科組距count20Down"); table.Columns.Add("加權平均校組距count20Down"); table.Columns.Add("類1總分組距count20Down"); table.Columns.Add("類1平均組距count20Down"); table.Columns.Add("類1加權總分組距count20Down"); table.Columns.Add("類1加權平均組距count20Down"); table.Columns.Add("類2總分組距count20Down"); table.Columns.Add("類2平均組距count20Down"); table.Columns.Add("類2加權總分組距count20Down"); table.Columns.Add("類2加權平均組距count20Down");
                table.Columns.Add("總分班組距count10Down"); table.Columns.Add("總分科組距count10Down"); table.Columns.Add("總分校組距count10Down"); table.Columns.Add("平均班組距count10Down"); table.Columns.Add("平均科組距count10Down"); table.Columns.Add("平均校組距count10Down"); table.Columns.Add("加權總分班組距count10Down"); table.Columns.Add("加權總分科組距count10Down"); table.Columns.Add("加權總分校組距count10Down"); table.Columns.Add("加權平均班組距count10Down"); table.Columns.Add("加權平均科組距count10Down"); table.Columns.Add("加權平均校組距count10Down"); table.Columns.Add("類1總分組距count10Down"); table.Columns.Add("類1平均組距count10Down"); table.Columns.Add("類1加權總分組距count10Down"); table.Columns.Add("類1加權平均組距count10Down"); table.Columns.Add("類2總分組距count10Down"); table.Columns.Add("類2平均組距count10Down"); table.Columns.Add("類2加權總分組距count10Down"); table.Columns.Add("類2加權平均組距count10Down");
                #endregion
                #endregion




                //宣告產生的報表
                Aspose.Words.Document document = new Aspose.Words.Document();

                //用一個BackgroundWorker包起來
                System.ComponentModel.BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker();
                bkw.WorkerReportsProgress = true;
                System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 期末成績單產生 S");
                bkw.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("期末成績單產生中", e.ProgressPercentage);
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 期末成績單產生 " + e.ProgressPercentage);
                };
                Exception exc = null;
                bkw.RunWorkerCompleted += delegate
                {



                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 期末成績單產生 E");
                    string err = "下列學生因成績項目超過樣板支援上限，\n超出部分科目成績無法印出，建議調整樣板內容。";
                    if (overflowRecords.Count > 0)
                    {
                        foreach (var stuRec in overflowRecords)
                        {
                            err += "\n" + (stuRec.RefClass == null ? "" : (stuRec.RefClass.ClassName + "班" + stuRec.SeatNo + "號")) + "[" + stuRec.StudentNumber + "]" + stuRec.StudentName;
                        }
                    }
                    #region 儲存檔案
                    string inputReportName = "個人學期成績單";
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
                            Aspose.Words.Document temp = new Aspose.Words.Document();
                            temp = conf.Template;
                            DataTable dt = table.Clone();
                            List<DataRow> list = new List<DataRow>();
                            foreach (string className in _ClassDic.Keys)
                            {
                                foreach (int idx in _ClassDic[className])
                                {
                                    list.Add(table.Rows[idx]);
                                    //dt.ImportRow(table.Rows[idx]);
                                }

                                list.Sort(DataSort);
                                foreach (DataRow row in list)
                                {
                                    dt.ImportRow(row);
                                }

                                document = temp.Clone();
                                document.MailMerge.Execute(dt);
                                document.Save(folderPath + "\\" + inputReportName + "_" + className + ".doc", Aspose.Words.SaveFormat.Doc);
                                dt.Clear();
                                list.Clear();
                            }
                            System.Diagnostics.Process.Start(folderPath);
                        }
                        catch (Exception ex)
                        {
                            SmartSchool.ErrorReporting.ErrorMessgae errormsg = new SmartSchool.ErrorReporting.ErrorMessgae(ex);
                            FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                    }
                    #endregion
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("期末成績單產生完成。", 100);
                    if (overflowRecords.Count > 0)
                        MessageBox.Show(err);
                    if (exc != null)
                    {
                        //throw new Exception("產生期末成績單發生錯誤", exc);
                    }

                    // 處理 epost
                    if (conf.ExportEpost)
                    {
                        // 檢查是否產生 Excel
                        Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                        Utility.CompletedXlsCsv("個人學期成績單epost", _dtEpost);
                    }

                };
                bkw.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    var studentRecords = accessHelper.StudentHelper.GetStudents(selectedStudents);
                    Dictionary<string, Dictionary<string, Dictionary<string, ExamScoreInfo>>> studentExamSores = new Dictionary<string, Dictionary<string, Dictionary<string, ExamScoreInfo>>>();
                    Dictionary<string, Dictionary<string, ExamScoreInfo>> studentRefExamSores = new Dictionary<string, Dictionary<string, ExamScoreInfo>>();
                    ManualResetEvent scoreReady = new ManualResetEvent(false);
                    ManualResetEvent elseReady = new ManualResetEvent(false);
                    #region 偷跑取得考試成績
                    // 有成績科目名稱對照
                    new Thread(new ThreadStart(delegate
                    {
                        // 取得學生學期科目成績
                        int sSchoolYear, sSemester;
                        int.TryParse(conf.SchoolYear, out sSchoolYear);
                        int.TryParse(conf.Semester, out sSemester);
                        #region 整理學生定期評量成績
                        #region 篩選課程學年度、學期、科目取得有可能有需要的資料
                        List<CourseRecord> targetCourseList = new List<CourseRecord>();
                        try
                        {
                            foreach (var courseRecord in accessHelper.CourseHelper.GetAllCourse(sSchoolYear, sSemester))
                            {
                                //用科目濾出可能有用到的課程
                                if (conf.PrintSubjectList.Contains(courseRecord.Subject)
                                    || conf.TagRank1SubjectList.Contains(courseRecord.Subject)
                                    || conf.TagRank2SubjectList.Contains(courseRecord.Subject))
                                    targetCourseList.Add(courseRecord);
                            }
                        }
                        catch (Exception exception)
                        {
                            exc = exception;
                        }
                        #endregion
                        try
                        {
                            if (conf.ExamRecord != null || conf.RefenceExamRecord != null)
                            {
                                accessHelper.CourseHelper.FillExam(targetCourseList);
                                var tcList = new List<CourseRecord>();
                                var totalList = new List<CourseRecord>();
                                foreach (var courseRec in targetCourseList)
                                {
                                    if (conf.ExamRecord != null && courseRec.ExamList.Contains(conf.ExamRecord.Name))
                                    {
                                        tcList.Add(courseRec);
                                        totalList.Add(courseRec);
                                    }
                                    if (tcList.Count == 180)
                                    {
                                        accessHelper.CourseHelper.FillStudentAttend(tcList);
                                        accessHelper.CourseHelper.FillExamScore(tcList);
                                        tcList.Clear();
                                    }
                                }
                                accessHelper.CourseHelper.FillStudentAttend(tcList);
                                accessHelper.CourseHelper.FillExamScore(tcList);
                                foreach (var courseRecord in totalList)
                                {
                                    #region 整理本次定期評量成績
                                    if (conf.ExamRecord != null && courseRecord.ExamList.Contains(conf.ExamRecord.Name))
                                    {
                                        foreach (var attendStudent in courseRecord.StudentAttendList)
                                        {
                                            if (!studentExamSores.ContainsKey(attendStudent.StudentID)) studentExamSores.Add(attendStudent.StudentID, new Dictionary<string, Dictionary<string, ExamScoreInfo>>());
                                            if (!studentExamSores[attendStudent.StudentID].ContainsKey(courseRecord.Subject)) studentExamSores[attendStudent.StudentID].Add(courseRecord.Subject, new Dictionary<string, ExamScoreInfo>());
                                            studentExamSores[attendStudent.StudentID][courseRecord.Subject].Add("" + attendStudent.CourseID, null);
                                        }
                                        foreach (var examScoreRec in courseRecord.ExamScoreList)
                                        {
                                            if (examScoreRec.ExamName == conf.ExamRecord.Name)
                                            {
                                                studentExamSores[examScoreRec.StudentID][courseRecord.Subject]["" + examScoreRec.CourseID] = examScoreRec;
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 整理前次定期評量成績
                                    if (conf.RefenceExamRecord != null && courseRecord.ExamList.Contains(conf.RefenceExamRecord.Name))
                                    {
                                        foreach (var examScoreRec in courseRecord.ExamScoreList)
                                        {
                                            if (examScoreRec.ExamName == conf.RefenceExamRecord.Name)
                                            {
                                                if (!studentRefExamSores.ContainsKey(examScoreRec.StudentID))
                                                    studentRefExamSores.Add(examScoreRec.StudentID, new Dictionary<string, ExamScoreInfo>());
                                                studentRefExamSores[examScoreRec.StudentID].Add("" + examScoreRec.CourseID, examScoreRec);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            exc = exception;
                        }
                        finally
                        {
                            scoreReady.Set();
                        }
                        #endregion
                        #region 整理學生學期、學年成績
                        try
                        {
                            if (sSemester == 2 && conf.WithSchoolYearScore)
                            {
                                accessHelper.StudentHelper.FillSchoolYearEntryScore(true, studentRecords);
                                accessHelper.StudentHelper.FillSchoolYearSubjectScore(true, studentRecords);
                            }
                            accessHelper.StudentHelper.FillSemesterEntryScore(true, studentRecords);
                            accessHelper.StudentHelper.FillSemesterSubjectScore(true, studentRecords);
                            accessHelper.StudentHelper.FillSemesterMoralScore(true, studentRecords);
                            //accessHelper.StudentHelper.FillField("SemesterEntryClassRating", studentRecords);
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
                            #region 學期學業成績排名
                            string strSQL = "select * from sems_entry_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=" + sSemester + "";
                            System.Data.DataTable dt = qh.Select(strSQL);
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                if ("" + dr["entry_group"] != "1") continue;
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                if ("" + dr["class_rating"] != "")
                                {
                                    //學期學業成績班排名
                                    doc.LoadXml("" + dr["class_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("學期學業成績班排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期學業成績班排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //學期學業成績科排名

                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("學期學業成績科排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期學業成績科排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                                //學期學業成績類別1排名
                                //學期學業成績類別2排名

                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        System.Xml.XmlElement ele = (System.Xml.XmlElement)element.SelectSingleNode("Item[@分項='學業']");
                                        if (ele != null)
                                        {
                                            if (!rec.Fields.ContainsKey("學期學業成績類別1"))
                                            {
                                                rec.Fields.Add("學期學業成績類別1", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                            else
                                            {
                                                rec.Fields.Add("學期學業成績類別2", element.GetAttribute("類別"));
                                                if (!rec.Fields.ContainsKey("學期學業成績" + element.GetAttribute("類別") + "排名"))
                                                {
                                                    rec.Fields.Add("學期學業成績" + element.GetAttribute("類別") + "排名", ele.GetAttribute("排名"));
                                                    rec.Fields.Add("學期學業成績" + element.GetAttribute("類別") + "排名母數", ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //學期學業成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    System.Xml.XmlElement ele = (System.Xml.XmlElement)doc.SelectSingleNode("Rating/Item[@分項='學業']");
                                    if (ele != null)
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("學期學業成績校排名", ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期學業成績校排名母數", ele.GetAttribute("成績人數"));
                                    }
                                }
                            }
                            #endregion
                            #region 學期科目成績排名
                            strSQL = "select * from sems_subj_score where ref_student_id in (" + sidList + ") and school_year=" + sSchoolYear + " and semester=" + sSemester + "";
                            dt = qh.Select(strSQL);
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                StudentRecord rec = stuDictionary["" + dr["ref_student_id"]];
                                //學期學業成績班排名
                                if ("" + dr["class_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["class_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 成績="83" 成績人數="50" 排名="33" 科目="公民與社會" 科目級別="1"/>
                                        rec.Fields.Add("學期科目排名成績" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績"));
                                        rec.Fields.Add("學期科目班排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期科目班排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //學期學業成績科排名
                                if ("" + dr["dept_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["dept_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("學期科目科排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期科目科排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                    }
                                }
                                //學期學業成績類別1排名
                                //學期學業成績類別2排名
                                if ("" + dr["group_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["group_rating"]);
                                    foreach (System.Xml.XmlElement element in doc.SelectNodes("Ratings/Rating"))
                                    {
                                        string cat = element.GetAttribute("類別");
                                        if (!rec.Fields.ContainsKey("學期科目成績類別1"))
                                        {
                                            rec.Fields.Add("學期科目成績類別1", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            rec.Fields.Add("學期科目成績類別2", cat);
                                            foreach (System.Xml.XmlElement ele in element.SelectNodes("Item"))
                                            {
                                                if (!rec.Fields.ContainsKey("學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別")))
                                                {
                                                    rec.Fields.Add("學期科目成績" + cat + "排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                                    rec.Fields.Add("學期科目成績" + cat + "排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
                                                }
                                            }
                                        }
                                    }
                                }
                                //學期學業成績校排名
                                if ("" + dr["year_rating"] != "")
                                {
                                    doc.LoadXml("" + dr["year_rating"]);
                                    foreach (System.Xml.XmlElement ele in doc.SelectNodes("Rating/Item"))
                                    {
                                        //<Item 分項="學業" 成績="90.6" 成績人數="35" 排名="2"/>
                                        rec.Fields.Add("學期科目校排名" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("排名"));
                                        rec.Fields.Add("學期科目校排名母數" + ele.GetAttribute("科目") + "^^^" + ele.GetAttribute("科目級別"), ele.GetAttribute("成績人數"));
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
                        #endregion
                    })).Start();
                    #endregion
                    try
                    {
                        #region 大範圍

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
                        _dtEpost.Columns.Add("學期");
                        _dtEpost.Columns.Add("班級");
                        _dtEpost.Columns.Add("座號");
                        _dtEpost.Columns.Add("學號");
                        _dtEpost.Columns.Add("姓名");

                        for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                        {
                            _dtEpost.Columns.Add("科目" + subjectIndex);
                            _dtEpost.Columns.Add("學分" + subjectIndex);
                            _dtEpost.Columns.Add("成績" + subjectIndex);
                            _dtEpost.Columns.Add("名次" + subjectIndex);
                            _dtEpost.Columns.Add("備註" + subjectIndex);
                        }

                        _dtEpost.Columns.Add("學業成績");
                        _dtEpost.Columns.Add("實習成績");
                        _dtEpost.Columns.Add("總成績名次");
                        _dtEpost.Columns.Add("取得學分");
                        _dtEpost.Columns.Add("累計學分");
                        _dtEpost.Columns.Add("大功");
                        _dtEpost.Columns.Add("小功");
                        _dtEpost.Columns.Add("嘉獎");
                        _dtEpost.Columns.Add("大過");
                        _dtEpost.Columns.Add("小過");
                        _dtEpost.Columns.Add("警告");
                        _dtEpost.Columns.Add("留校察看");


                        // 固定會對照
                        Dictionary<string, string> eKeyValDict = new Dictionary<string, string>();
                        eKeyValDict.Add("收件人", "CN");
                        eKeyValDict.Add("學年度", "學年度");
                        eKeyValDict.Add("學期", "學期");
                        eKeyValDict.Add("班級", "班級");
                        eKeyValDict.Add("座號", "座號");
                        eKeyValDict.Add("學號", "學號");
                        eKeyValDict.Add("姓名", "姓名");
                        eKeyValDict.Add("學期學業成績", "學業成績");
                        eKeyValDict.Add("學期實習科目成績", "實習成績");
                        eKeyValDict.Add("本學期修習學分數", "修習學分");
                        eKeyValDict.Add("本學期取得學分數", "取得學分");
                        eKeyValDict.Add("累計取得學分數", "累計學分");
                        eKeyValDict.Add("大功統計", "大功");
                        eKeyValDict.Add("小功統計", "小功");
                        eKeyValDict.Add("嘉獎統計", "嘉獎");
                        eKeyValDict.Add("大過統計", "大過");
                        eKeyValDict.Add("小過統計", "小過");
                        eKeyValDict.Add("警告統計", "警告");
                        eKeyValDict.Add("留校察看", "留校察看");
                        eKeyValDict.Add("班導師", "導師姓名");
                        eKeyValDict.Add("導師評語", "導師評語");

                        // 綜合評語
                        List<string> CommList = new List<string>();

                        #region 日常行為表現資料表
                        SmartSchool.Customization.Data.SystemInformation.getField("文字評量對照表");
                        foreach (System.Xml.XmlElement ele in (SmartSchool.Customization.Data.SystemInformation.Fields["文字評量對照表"] as System.Xml.XmlElement).SelectNodes("Content/Morality"))
                        {
                            string face = ele.GetAttribute("Face");
                            string f1 = "綜合表現：" + face;
                            if (!table.Columns.Contains(f1))
                            {
                                table.Columns.Add(f1);
                            }

                            if (!CommList.Contains(face))
                                CommList.Add(face);

                            if (!_dtEpost.Columns.Contains(face))
                                _dtEpost.Columns.Add(face);

                            if (!eKeyValDict.ContainsKey(f1))
                                eKeyValDict.Add(f1, face);
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

                        int aidx = 1;
                        foreach (var absence in K12.Data.AbsenceMapping.SelectAll())
                        {
                            foreach (var pt in periodTypes)
                            {
                                string attendanceKey = pt + "_" + absence.Name;
                                if (!table.Columns.Contains(attendanceKey))
                                {
                                    table.Columns.Add(attendanceKey);
                                }

                                if (pt == "一般")
                                    aidx = 1;
                                else
                                    aidx = 2;

                                string attendanceKey1 = absence.Name + aidx;
                                if (!_dtEpost.Columns.Contains(attendanceKey1))
                                    _dtEpost.Columns.Add(attendanceKey1);

                                if (!eKeyValDict.ContainsKey(attendanceKey))
                                    eKeyValDict.Add(attendanceKey, attendanceKey1);
                            }
                        }

                        if (!_dtEpost.Columns.Contains("導師姓名"))
                            _dtEpost.Columns.Add("導師姓名");
                        if (!_dtEpost.Columns.Contains("導師評語"))
                            _dtEpost.Columns.Add("導師評語");

                        #endregion
                        bkw.ReportProgress(3);
                        #region 整理學生住址
                        accessHelper.StudentHelper.FillContactInfo(studentRecords);
                        #endregion
                        #region 整理學生父母及監護人
                        accessHelper.StudentHelper.FillParentInfo(studentRecords);
                        #endregion
                        bkw.ReportProgress(10);
                        #region 整理同年級學生
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
                       
                        #endregion
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
                        scoreReady.WaitOne();
                        bkw.ReportProgress(35);
                        int progressCount = 0;
                        #region 計算總分及各項目排名
                        Dictionary<string, string> studentTag1Group = new Dictionary<string, string>();
                        Dictionary<string, string> studentTag2Group = new Dictionary<string, string>();
                        Dictionary<string, List<decimal>> ranks = new Dictionary<string, List<decimal>>();
                        Dictionary<string, List<string>> rankStudents = new Dictionary<string, List<string>>();
                        Dictionary<string, decimal> studentPrintSubjectSum = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag1SubjectSum = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag2SubjectSum = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentPrintSubjectAvg = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag1SubjectAvg = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag2SubjectAvg = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentPrintSubjectSumW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag1SubjectSumW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag2SubjectSumW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentPrintSubjectAvgW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag1SubjectAvgW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> studentTag2SubjectAvgW = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> analytics = new Dictionary<string, decimal>();
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
                                #region 分析學生所屬類別
                                if (studentTags.ContainsKey(studentID))
                                {
                                    foreach (var tag in studentTags[studentID])
                                    {
                                        #region 判斷學生是否屬於不排名類別
                                        if (conf.RankFilterTagList.Contains(tag.RefTagID))
                                        {
                                            rank = false;
                                        }
                                        #endregion
                                        #region 判斷學生在類別排名1中所屬的類別
                                        if (tag1ID == "" && conf.TagRank1TagList.Contains(tag.RefTagID))
                                        {
                                            tag1ID = tag.RefTagID;
                                            studentTag1Group.Add(studentID, tag1ID);
                                        }
                                        #endregion
                                        #region 判斷學生在類別排名2中所屬的類別
                                        if (tag2ID == "" && conf.TagRank2TagList.Contains(tag.RefTagID))
                                        {
                                            tag2ID = tag.RefTagID;
                                            studentTag2Group.Add(studentID, tag2ID);
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                                bool summaryRank = true;
                                bool tag1SummaryRank = true;
                                bool tag2SummaryRank = true;
                                if (studentExamSores.ContainsKey(studentID))
                                {
                                    decimal printSubjectSum = 0;
                                    int printSubjectCount = 0;
                                    decimal tag1SubjectSum = 0;
                                    int tag1SubjectCount = 0;
                                    decimal tag2SubjectSum = 0;
                                    int tag2SubjectCount = 0;
                                    decimal printSubjectSumW = 0;
                                    decimal printSubjectCreditSum = 0;
                                    decimal tag1SubjectSumW = 0;
                                    decimal tag1SubjectCreditSum = 0;
                                    decimal tag2SubjectSumW = 0;
                                    decimal tag2SubjectCreditSum = 0;
                                    foreach (var subjectName in studentExamSores[studentID].Keys)
                                    {
                                        if (conf.PrintSubjectList.Contains(subjectName))
                                        {
                                            #region 是列印科目
                                            foreach (var sceTakeRecord in studentExamSores[studentID][subjectName].Values)
                                            {
                                                if (sceTakeRecord != null && sceTakeRecord.SpecialCase == "")
                                                {
                                                    printSubjectSum += sceTakeRecord.ExamScore;//計算總分
                                                    printSubjectCount++;
                                                    //計算加權總分
                                                    printSubjectSumW += sceTakeRecord.ExamScore * sceTakeRecord.CreditDec();
                                                    printSubjectCreditSum += sceTakeRecord.CreditDec();
                                                    if (rank && sceTakeRecord.Status == "一般")//不在過濾名單且為一般生才做排名
                                                    {
                                                        if (sceTakeRecord.RefClass != null)
                                                        {
                                                            //各科目班排名
                                                            key = "班排名" + sceTakeRecord.RefClass.ClassID + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                            ranks[key].Add(sceTakeRecord.ExamScore);
                                                            rankStudents[key].Add(studentID);
                                                        }
                                                        if (sceTakeRecord.Department != "")
                                                        {
                                                            //各科目科排名
                                                            key = "科排名" + sceTakeRecord.Department + "^^^" + gradeyear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                            ranks[key].Add(sceTakeRecord.ExamScore);
                                                            rankStudents[key].Add(studentID);
                                                        }
                                                        //各科目全校排名
                                                        key = "全校排名" + gradeyear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                        if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                        if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                        ranks[key].Add(sceTakeRecord.ExamScore);
                                                        rankStudents[key].Add(studentID);
                                                    }
                                                }
                                                else
                                                {
                                                    summaryRank = false;
                                                }
                                            }
                                            #endregion
                                        }
                                        if (tag1ID != "" && conf.TagRank1SubjectList.Contains(subjectName))
                                        {
                                            #region 有Tag1且是排名科目
                                            foreach (var sceTakeRecord in studentExamSores[studentID][subjectName].Values)
                                            {
                                                if (sceTakeRecord != null && sceTakeRecord.SpecialCase == "")
                                                {
                                                    tag1SubjectSum += sceTakeRecord.ExamScore;//計算總分
                                                    tag1SubjectCount++;
                                                    //計算加權總分
                                                    tag1SubjectSumW += sceTakeRecord.ExamScore * sceTakeRecord.CreditDec();
                                                    tag1SubjectCreditSum += sceTakeRecord.CreditDec();
                                                    //各科目類別1排名
                                                    if (rank && sceTakeRecord.Status == "一般")//不在過濾名單且為一般生才做排名
                                                    {
                                                        if (conf.PrintSubjectList.Contains(subjectName))//是列印科目才算科目排名                                                
                                                        {
                                                            key = "類別1排名" + tag1ID + "^^^" + gradeyear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                            ranks[key].Add(sceTakeRecord.ExamScore);
                                                            rankStudents[key].Add(studentID);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    tag1SummaryRank = false;
                                                }
                                            }
                                            #endregion
                                        }
                                        if (tag2ID != "" && conf.TagRank2SubjectList.Contains(subjectName))
                                        {
                                            #region 有Tag2且是排名科目
                                            foreach (var sceTakeRecord in studentExamSores[studentID][subjectName].Values)
                                            {
                                                if (sceTakeRecord != null && sceTakeRecord.SpecialCase == "")
                                                {
                                                    tag2SubjectSum += sceTakeRecord.ExamScore;//計算總分
                                                    tag2SubjectCount++;
                                                    //計算加權總分
                                                    tag2SubjectSumW += sceTakeRecord.ExamScore * sceTakeRecord.CreditDec();
                                                    tag2SubjectCreditSum += sceTakeRecord.CreditDec();
                                                    //各科目類別2排名
                                                    if (rank && sceTakeRecord.Status == "一般")//不在過濾名單且為一般生才做排名
                                                    {
                                                        if (conf.PrintSubjectList.Contains(subjectName))//是列印科目才算科目排名                                                
                                                        {
                                                            key = "類別2排名" + tag2ID + "^^^" + gradeyear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                            ranks[key].Add(sceTakeRecord.ExamScore);
                                                            rankStudents[key].Add(studentID);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    tag2SummaryRank = false;
                                                }
                                            }
                                            #endregion
                                        }

                                    }
                                    if (printSubjectCount > 0)
                                    {
                                        #region 有列印科目處理加總成績
                                        //總分
                                        studentPrintSubjectSum.Add(studentID, printSubjectSum);
                                        //平均四捨五入至小數點第二位
                                        studentPrintSubjectAvg.Add(studentID, Math.Round(printSubjectSum / printSubjectCount, 2, MidpointRounding.AwayFromZero));
                                        if (rank && studentRec.Status == "一般" && summaryRank == true)//不在過濾名單且沒有特殊成績狀況且為一般生才做排名
                                        {
                                            //總分班排名
                                            key = "總分班排名" + studentRec.RefClass.ClassID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(printSubjectSum);
                                            rankStudents[key].Add(studentID);
                                            //總分科排名
                                            key = "總分科排名" + studentRec.Department + "^^^" + gradeyear;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(printSubjectSum);
                                            rankStudents[key].Add(studentID);
                                            //總分全校排名
                                            key = "總分全校排名" + gradeyear;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(printSubjectSum);
                                            rankStudents[key].Add(studentID);
                                            //平均班排名
                                            key = "平均班排名" + studentRec.RefClass.ClassID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(Math.Round(printSubjectSum / printSubjectCount, 2, MidpointRounding.AwayFromZero));
                                            rankStudents[key].Add(studentID);
                                            //平均科排名
                                            key = "平均科排名" + studentRec.Department + "^^^" + gradeyear;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(Math.Round(printSubjectSum / printSubjectCount, 2, MidpointRounding.AwayFromZero));
                                            rankStudents[key].Add(studentID);
                                            //平均全校排名
                                            key = "平均全校排名" + gradeyear;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(Math.Round(printSubjectSum / printSubjectCount, 2, MidpointRounding.AwayFromZero));
                                            rankStudents[key].Add(studentID);
                                        }
                                        #endregion
                                        if (printSubjectCreditSum > 0)
                                        {
                                            #region 有總學分數處理加總
                                            //加權總分
                                            studentPrintSubjectSumW.Add(studentID, printSubjectSumW);
                                            //加權平均四捨五入至小數點第二位
                                            studentPrintSubjectAvgW.Add(studentID, Math.Round(printSubjectSumW / printSubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                            if (rank && studentRec.Status == "一般" && summaryRank == true)//不在過濾名單且為一般生才做排名
                                            {
                                                //加權總分班排名
                                                key = "加權總分班排名" + studentRec.RefClass.ClassID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(printSubjectSumW);
                                                rankStudents[key].Add(studentID);
                                                //加權總分科排名
                                                key = "加權總分科排名" + studentRec.Department + "^^^" + gradeyear;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(printSubjectSumW);
                                                rankStudents[key].Add(studentID);
                                                //加權總分全校排名
                                                key = "加權總分全校排名" + gradeyear;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(printSubjectSumW);
                                                rankStudents[key].Add(studentID);
                                                //加權平均班排名
                                                key = "加權平均班排名" + studentRec.RefClass.ClassID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(Math.Round(printSubjectSumW / printSubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                                rankStudents[key].Add(studentID);
                                                //加權平均科排名
                                                key = "加權平均科排名" + studentRec.Department + "^^^" + gradeyear;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(Math.Round(printSubjectSumW / printSubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                                rankStudents[key].Add(studentID);
                                                //加權平均全校排名
                                                key = "加權平均全校排名" + gradeyear;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(Math.Round(printSubjectSumW / printSubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                                rankStudents[key].Add(studentID);
                                            }
                                            #endregion
                                        }
                                    }
                                    //類別1總分平均排名
                                    if (tag1SubjectCount > 0)
                                    {
                                        //總分
                                        studentTag1SubjectSum.Add(studentID, tag1SubjectSum);
                                        //平均四捨五入至小數點第二位
                                        studentTag1SubjectAvg.Add(studentID, Math.Round(tag1SubjectSum / tag1SubjectCount, 2, MidpointRounding.AwayFromZero));
                                        if (rank && studentRec.Status == "一般" && tag1SummaryRank == true)//不在過濾名單且為一般生才做排名
                                        {
                                            key = "類別1總分排名" + "^^^" + gradeyear + "^^^" + tag1ID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(tag1SubjectSum);
                                            rankStudents[key].Add(studentID);

                                            key = "類別1平均排名" + "^^^" + gradeyear + "^^^" + tag1ID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(Math.Round(tag1SubjectSum / tag1SubjectCount, 2, MidpointRounding.AwayFromZero));
                                            rankStudents[key].Add(studentID);
                                        }
                                        //類別1加權總分平均排名
                                        if (tag1SubjectCreditSum > 0)
                                        {
                                            studentTag1SubjectSumW.Add(studentID, tag1SubjectSumW);
                                            studentTag1SubjectAvgW.Add(studentID, Math.Round(tag1SubjectSumW / tag1SubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                            if (rank && studentRec.Status == "一般" && tag1SummaryRank == true)//不在過濾名單且為一般生才做排名
                                            {
                                                key = "類別1加權總分排名" + "^^^" + gradeyear + "^^^" + tag1ID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(tag1SubjectSumW);
                                                rankStudents[key].Add(studentID);

                                                key = "類別1加權平均排名" + "^^^" + gradeyear + "^^^" + tag1ID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(Math.Round(tag1SubjectSumW / tag1SubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                                rankStudents[key].Add(studentID);
                                            }
                                        }
                                    }
                                    //類別2總分平均排名
                                    if (tag2SubjectCount > 0)
                                    {
                                        //總分
                                        studentTag2SubjectSum.Add(studentID, tag2SubjectSum);
                                        //平均四捨五入至小數點第二位
                                        studentTag2SubjectAvg.Add(studentID, Math.Round(tag2SubjectSum / tag2SubjectCount, 2, MidpointRounding.AwayFromZero));
                                        if (rank && studentRec.Status == "一般" && tag2SummaryRank == true)//不在過濾名單且為一般生才做排名
                                        {
                                            key = "類別2總分排名" + "^^^" + gradeyear + "^^^" + tag2ID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(tag2SubjectSum);
                                            rankStudents[key].Add(studentID);
                                            key = "類別2平均排名" + "^^^" + gradeyear + "^^^" + tag2ID;
                                            if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                            if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                            ranks[key].Add(Math.Round(tag2SubjectSum / tag2SubjectCount, 2, MidpointRounding.AwayFromZero));
                                            rankStudents[key].Add(studentID);
                                        }
                                        //類別2加權總分平均排名
                                        if (tag2SubjectCreditSum > 0)
                                        {
                                            studentTag2SubjectSumW.Add(studentID, tag2SubjectSumW);
                                            studentTag2SubjectAvgW.Add(studentID, Math.Round(tag2SubjectSumW / tag2SubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                            if (rank && studentRec.Status == "一般" && tag2SummaryRank == true)//不在過濾名單且為一般生才做排名
                                            {
                                                key = "類別2加權總分排名" + "^^^" + gradeyear + "^^^" + tag2ID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(tag2SubjectSumW);
                                                rankStudents[key].Add(studentID);

                                                key = "類別2加權平均排名" + "^^^" + gradeyear + "^^^" + tag2ID;
                                                if (!ranks.ContainsKey(key)) ranks.Add(key, new List<decimal>());
                                                if (!rankStudents.ContainsKey(key)) rankStudents.Add(key, new List<string>());
                                                ranks[key].Add(Math.Round(tag2SubjectSumW / tag2SubjectCreditSum, 2, MidpointRounding.AwayFromZero));
                                                rankStudents[key].Add(studentID);
                                            }
                                        }
                                    }
                                }
                                progressCount++;
                                bkw.ReportProgress(40 + progressCount * 30 / total);
                            }
                        }
                        foreach (var k in ranks.Keys)
                        {
                            var rankscores = ranks[k];
                            //排序
                            rankscores.Sort();
                            rankscores.Reverse();
                            //高均標、組距
                            if (rankscores.Count > 0)
                            {
                                #region 算高標的中點
                                int middleIndex = 0;
                                int count = 1;
                                var score = rankscores[0];
                                while (rankscores.Count > middleIndex)
                                {
                                    if (score != rankscores[middleIndex])
                                    {
                                        if (count * 2 >= rankscores.Count) break;
                                        score = rankscores[middleIndex];
                                    }
                                    middleIndex++;
                                    count++;
                                }
                                if (rankscores.Count == middleIndex)
                                {
                                    middleIndex--;
                                    count--;
                                }
                                #endregion
                                analytics.Add(k + "^^^高標", Math.Round(rankscores.GetRange(0, count).Average(), 2, MidpointRounding.AwayFromZero));
                                analytics.Add(k + "^^^均標", Math.Round(rankscores.Average(), 2, MidpointRounding.AwayFromZero));
                                #region 算低標的中點
                                middleIndex = rankscores.Count - 1;
                                count = 1;
                                score = rankscores[middleIndex];
                                while (middleIndex >= 0)
                                {
                                    if (score != rankscores[middleIndex])
                                    {
                                        if (count * 2 >= rankscores.Count) break;
                                        score = rankscores[middleIndex];
                                    }
                                    middleIndex--;
                                    count++;
                                }
                                if (middleIndex < 0)
                                {
                                    middleIndex++;
                                    count--;
                                }
                                #endregion
                                analytics.Add(k + "^^^低標", Math.Round(rankscores.GetRange(middleIndex, count).Average(), 2, MidpointRounding.AwayFromZero));
                                //Compute the Average      
                                var avg = (double)rankscores.Average();
                                //Perform the Sum of (value-avg)_2_2      
                                var sum = (double)rankscores.Sum(d => Math.Pow((double)d - avg, 2));
                                //Put it all together      
                                analytics.Add(k + "^^^標準差", Math.Round((decimal)Math.Sqrt((sum) / rankscores.Count()), 2, MidpointRounding.AwayFromZero));
                            }
                            #region 計算級距
                            int count90 = 0, count80 = 0, count70 = 0, count60 = 0, count50 = 0, count40 = 0, count30 = 0, count20 = 0, count10 = 0;
                            int count100Up = 0, count90Up = 0, count80Up = 0, count70Up = 0, count60Up = 0, count50Up = 0, count40Up = 0, count30Up = 0, count20Up = 0, count10Up = 0;
                            int count90Down = 0, count80Down = 0, count70Down = 0, count60Down = 0, count50Down = 0, count40Down = 0, count30Down = 0, count20Down = 0, count10Down = 0;
                            foreach (var score in rankscores)
                            {
                                if (score >= 100)
                                    count100Up++;
                                else if (score >= 90)
                                    count90++;
                                else if (score >= 80)
                                    count80++;
                                else if (score >= 70)
                                    count70++;
                                else if (score >= 60)
                                    count60++;
                                else if (score >= 50)
                                    count50++;
                                else if (score >= 40)
                                    count40++;
                                else if (score >= 30)
                                    count30++;
                                else if (score >= 20)
                                    count20++;
                                else if (score >= 10)
                                    count10++;
                                else
                                    count10Down++;
                            }
                            count90Up = count100Up + count90;
                            count80Up = count90Up + count80;
                            count70Up = count80Up + count70;
                            count60Up = count70Up + count60;
                            count50Up = count60Up + count50;
                            count40Up = count50Up + count40;
                            count30Up = count40Up + count30;
                            count20Up = count30Up + count20;
                            count10Up = count20Up + count10;

                            count20Down = count10Down + count10;
                            count30Down = count20Down + count20;
                            count40Down = count30Down + count30;
                            count50Down = count40Down + count40;
                            count60Down = count50Down + count50;
                            count70Down = count60Down + count60;
                            count80Down = count70Down + count70;
                            count90Down = count80Down + count80;

                            analytics.Add(k + "^^^count90", count90);
                            analytics.Add(k + "^^^count80", count80);
                            analytics.Add(k + "^^^count70", count70);
                            analytics.Add(k + "^^^count60", count60);
                            analytics.Add(k + "^^^count50", count50);
                            analytics.Add(k + "^^^count40", count40);
                            analytics.Add(k + "^^^count30", count30);
                            analytics.Add(k + "^^^count20", count20);
                            analytics.Add(k + "^^^count10", count10);
                            analytics.Add(k + "^^^count100Up", count100Up);
                            analytics.Add(k + "^^^count90Up", count90Up);
                            analytics.Add(k + "^^^count80Up", count80Up);
                            analytics.Add(k + "^^^count70Up", count70Up);
                            analytics.Add(k + "^^^count60Up", count60Up);
                            analytics.Add(k + "^^^count50Up", count50Up);
                            analytics.Add(k + "^^^count40Up", count40Up);
                            analytics.Add(k + "^^^count30Up", count30Up);
                            analytics.Add(k + "^^^count20Up", count20Up);
                            analytics.Add(k + "^^^count10Up", count10Up);
                            analytics.Add(k + "^^^count90Down", count90Down);
                            analytics.Add(k + "^^^count80Down", count80Down);
                            analytics.Add(k + "^^^count70Down", count70Down);
                            analytics.Add(k + "^^^count60Down", count60Down);
                            analytics.Add(k + "^^^count50Down", count50Down);
                            analytics.Add(k + "^^^count40Down", count40Down);
                            analytics.Add(k + "^^^count30Down", count30Down);
                            analytics.Add(k + "^^^count20Down", count20Down);
                            analytics.Add(k + "^^^count10Down", count10Down);
                            #endregion
                        }
                        #endregion

                        // 先取得 K12 StudentRec,因為後面透過 k12.data 取資料有的傳入ID,有的傳入 Record 有點亂
                        List<K12.Data.StudentRecord> StudRecList = new List<K12.Data.StudentRecord>();
                        List<string> StudIDList = (from data in studentRecords select data.StudentID).ToList();
                        StudRecList = K12.Data.Student.SelectByIDs(StudIDList);

                        int SchoolYear, Semester;
                        int.TryParse(conf.SchoolYear, out SchoolYear);
                        int.TryParse(conf.Semester, out Semester);

                        Dictionary<string, decimal> ServiceLearningByDateDict2 = new Dictionary<string, decimal>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict2 = new Dictionary<string, Dictionary<string, int>>();
                        Dictionary<string, decimal> ServiceLearningByDateDict1 = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> ServiceLearningByDateDict = new Dictionary<string, decimal>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict = new Dictionary<string, Dictionary<string, int>>();
                        Dictionary<string, Dictionary<string, int>> AttendanceCountDict1 = new Dictionary<string, Dictionary<string, int>>();

                        // 取得暫存資料 學習服務區間時數                       
                        // 本學期
                        ServiceLearningByDateDict2 = Utility.GetServiceLearningBySchoolYearSemester(StudIDList, SchoolYear, Semester);

                        // 取得缺曠本學期
                        AttendanceCountDict2 = Utility.GetAttendanceCountBySchoolYearSemester(StudRecList, SchoolYear, Semester);

                        if (Semester == 2)
                        {
                            // 前學期
                            ServiceLearningByDateDict1 = Utility.GetServiceLearningBySchoolYearSemester(StudIDList, SchoolYear, 1);
                            // 學年
                            ServiceLearningByDateDict = Utility.GetServiceLearningBySchoolYear(StudIDList, SchoolYear);

                            // 取得學年缺曠
                            AttendanceCountDict = Utility.GetAttendanceCountBySchoolYear(StudRecList, SchoolYear);

                            // 取得缺曠前學期
                            AttendanceCountDict1 = Utility.GetAttendanceCountBySchoolYearSemester(StudRecList, SchoolYear, 1);
                        }

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

                        _studSumCreditDict1.Clear();
                        _studPassSumCreditDict1.Clear();
                        _studPassSumCreditDictAll.Clear();

                        progressCount = 0;
                        #region 填入資料表
                        foreach (var stuRec in studentRecords)
                        {
                            // 本學期修習學分
                            if (!_studSumCreditDict1.ContainsKey(stuRec.StudentID))
                                _studSumCreditDict1.Add(stuRec.StudentID, 0);

                            
                            // 本學期取得學分數
                            if (!_studPassSumCreditDict1.ContainsKey(stuRec.StudentID))
                                _studPassSumCreditDict1.Add(stuRec.StudentID, 0);

                            // 累計取得學分數
                            if (!_studPassSumCreditDictAll.ContainsKey(stuRec.StudentID))
                                _studPassSumCreditDictAll.Add(stuRec.StudentID, 0);

                            string studentID = stuRec.StudentID;
                            string gradeYear = (stuRec.RefClass == null ? "" : "" + stuRec.RefClass.GradeYear);
                            DataRow row = table.NewRow();
                            #region 基本資料

                            // 服務學習時數
                            if (ServiceLearningByDateDict1.ContainsKey(studentID))
                            {
                                // 處理學生上學習服務時數	
                                row["前學期服務學習時數"] = ServiceLearningByDateDict1[studentID];
                            }

                            if (ServiceLearningByDateDict2.ContainsKey(studentID))
                            {
                                // 處理學生下學習服務時數	
                                row["本學期服務學習時數"] = ServiceLearningByDateDict2[studentID];
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
                                    string keyS = "前學期" + data.Key;

                                    if (table.Columns.Contains(keyS))

                                        row[keyS] = data.Value;
                                }
                            }

                            if (AttendanceCountDict2.ContainsKey(studentID))
                            {
                                foreach (KeyValuePair<string, int> data in AttendanceCountDict2[studentID])
                                {
                                    string keyS = "本學期" + data.Key;

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
                            row["試別"] = conf.ExamRecord.Name;

                            row["學年度"] = conf.SchoolYear;
                            row["學期"] = conf.Semester;
                            row["班級科別名稱"] = stuRec.RefClass == null ? "" : stuRec.RefClass.Department;
                            row["班級"] = stuRec.RefClass == null ? "" : stuRec.RefClass.ClassName;
                            row["學生班級年級"] = stuRec.RefClass == null ? "" : stuRec.RefClass.GradeYear;
                            row["班導師"] = (stuRec.RefClass == null || stuRec.RefClass.RefTeacher == null) ? "" : stuRec.RefClass.RefTeacher.TeacherName;
                            row["座號"] = stuRec.SeatNo;
                            row["學號"] = stuRec.StudentNumber;
                            row["姓名"] = stuRec.StudentName;
                            row["定期評量"] = conf.ExamRecord.Name;
                            #endregion
                            #region 成績資料
                            #region 各科成績資料
                            #region 分項成績
                            int currentGradeYear = -1;
                            foreach (var semesterEntryScore in stuRec.SemesterEntryScoreList)
                            {
                                if (("" + semesterEntryScore.SchoolYear) == conf.SchoolYear && ("" + semesterEntryScore.Semester) == conf.Semester)
                                {
                                    row["學期" + semesterEntryScore.Entry + "成績"] = semesterEntryScore.Score;
                                    currentGradeYear = semesterEntryScore.GradeYear;
                                }
                            }
                            #region 學期學業成績排名
                            //if (stuRec.Fields.ContainsKey("SemesterEntryClassRating"))
                            //{
                            //    System.Xml.XmlElement _sems_ratings = stuRec.Fields["SemesterEntryClassRating"] as System.Xml.XmlElement;
                            //    string path = string.Format("SemesterEntryScore[SchoolYear='{0}' and Semester='{1}']/ClassRating/Rating/Item[@分項='學業']/@排名", conf.SchoolYear, conf.Semester);
                            //    System.Xml.XmlNode result = _sems_ratings.SelectSingleNode(path);
                            //    if (result != null)
                            //    {
                            //        row["學期學業成績班排名"] = result.InnerText;
                            //    }
                            //}
                            foreach (var k in new string[] { "班", "科", "校" })
                            {
                                if (stuRec.Fields.ContainsKey("學期學業成績" + k + "排名")) row["學期學業成績" + k + "排名"] = "" + stuRec.Fields["學期學業成績" + k + "排名"];
                                if (stuRec.Fields.ContainsKey("學期學業成績" + k + "排名母數")) row["學期學業成績" + k + "排名母數"] = "" + stuRec.Fields["學期學業成績" + k + "排名母數"];
                            }
                            //類別1
                            if (studentTag1Group.ContainsKey(studentID))
                            {
                                foreach (var tag in studentTags[studentID])
                                {
                                    if (tag.RefTagID == studentTag1Group[studentID])
                                    {
                                        key = "學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["學期學業成績類別1排名"] = "" + stuRec.Fields[key];
                                        key = "學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["學期學業成績類別1排名母數"] = "" + stuRec.Fields[key];
                                        break;
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
                                        key = "學期學業成績" + tag.Name + "排名";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["學期學業成績類別2排名"] = "" + stuRec.Fields[key];
                                        key = "學期學業成績" + tag.Name + "排名母數";
                                        if (stuRec.Fields.ContainsKey(key))
                                            row["學期學業成績類別2排名母數"] = "" + stuRec.Fields[key];
                                        break;
                                    }
                                }
                            }
                            #endregion

                            if (conf.Semester == "2")
                            {
                                #region 學年學業成績及排名
                                if (conf.WithSchoolYearScore)
                                {
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
                                }
                                #endregion
                                if (conf.WithPrevSemesterScore)
                                {
                                    foreach (var semesterEntryScore in stuRec.SemesterEntryScoreList)
                                    {
                                        if (semesterEntryScore.Semester == 1 && semesterEntryScore.GradeYear == currentGradeYear)
                                        {
                                            row["上學期" + semesterEntryScore.Entry + "成績"] = semesterEntryScore.Score;
                                        }
                                    }
                                }
                            }






                            #endregion
                            #region 整理科目順序
                            List<string> subjects1 = new List<string>();//本學期
                            List<string> subjects2 = new List<string>();//上學期
                            List<string> subjects3 = new List<string>();//學年
                            foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                            {
                                if (("" + semesterSubjectScore.SchoolYear) == conf.SchoolYear && ("" + semesterSubjectScore.Semester) == conf.Semester)
                                {
                                    if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是")
                                    {
                                        subjects1.Add(semesterSubjectScore.Subject);
                                        currentGradeYear = semesterSubjectScore.GradeYear;
                                    }
                                }
                            }
                            if (studentExamSores.ContainsKey(stuRec.StudentID))
                            {
                                foreach (var subjectName in studentExamSores[studentID].Keys)
                                {
                                    foreach (var courseID in studentExamSores[studentID][subjectName].Keys)
                                    {
                                        if (conf.PrintSubjectList.Contains(subjectName))
                                        {
                                            #region 跟學期成績做差異新增
                                            bool match = false;
                                            foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                                            {
                                                if (("" + semesterSubjectScore.SchoolYear) == conf.SchoolYear
                                                    && ("" + semesterSubjectScore.Semester) == conf.Semester
                                                    && semesterSubjectScore.Subject == subjectName
                                                    && semesterSubjectScore.Level == accessHelper.CourseHelper.GetCourse(courseID)[0].SubjectLevel)
                                                {
                                                    match = true;
                                                    break;
                                                }
                                            }
                                            if (!match)
                                            {
                                                subjects1.Add(subjectName);
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            if (conf.Semester == "2")
                            {
                                if (conf.WithPrevSemesterScore)
                                {
                                    foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                                    {
                                        if (semesterSubjectScore.Semester == 1 && semesterSubjectScore.GradeYear == currentGradeYear)
                                        {
                                            if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是")
                                                subjects2.Add(semesterSubjectScore.Subject);
                                        }
                                    }
                                }
                                if (conf.WithSchoolYearScore)
                                {
                                    foreach (var schoolYearSubjectScore in stuRec.SchoolYearSubjectScoreList)
                                    {
                                        if (("" + schoolYearSubjectScore.SchoolYear) == conf.SchoolYear)
                                        {
                                            subjects3.Add(schoolYearSubjectScore.Subject);
                                        }
                                    }
                                }
                            }
                            var subjectNameList = new List<string>();
                            subjectNameList.AddRange(subjects1);
                            foreach (var subject in subjects1)
                            {
                                if (subjects2.Contains(subject)) subjects2.Remove(subject);
                                if (subjects3.Contains(subject)) subjects3.Remove(subject);
                            }
                            subjectNameList.AddRange(subjects2);
                            foreach (var subject in subjects2)
                            {
                                if (subjects3.Contains(subject)) subjects3.Remove(subject);
                            }
                            subjectNameList.AddRange(subjects3);
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


                            // 處理本學期取得學分與累計取得學分
                            foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                            {
                                if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是")
                                {

                                    // 本學期取得修習
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == conf.Semester)
                                        _studSumCreditDict1[stuRec.StudentID] += semesterSubjectScore.CreditDec();


                                    // 本學期取得
                                    if (semesterSubjectScore.SchoolYear.ToString() == conf.SchoolYear && semesterSubjectScore.Semester.ToString() == conf.Semester && semesterSubjectScore.Pass)
                                        _studPassSumCreditDict1[stuRec.StudentID] += semesterSubjectScore.CreditDec();

                                    // 累計取得
                                    if (semesterSubjectScore.Pass)
                                        _studPassSumCreditDictAll[stuRec.StudentID] += semesterSubjectScore.CreditDec();
                                }
                            }

                            row["本學期修習學分數"] = _studSumCreditDict1[stuRec.StudentID];
                            row["本學期取得學分數"] = _studPassSumCreditDict1[stuRec.StudentID];
                            row["累計取得學分數"] = _studPassSumCreditDictAll[stuRec.StudentID];

                            int subjectIndex = 1;
                            // 學期科目與定期評量
                            foreach (string subjectName in subjectNameList)
                            {
                                if (subjectIndex <= conf.SubjectLimit)
                                {
                                    decimal? subjectNumber = null;
                                    bool findInSemesterSubjectScore = false;
                                    bool findInSemester1SubjectScore = false;
                                    bool findInExamScores = false;
                                    #region 本學期學期成績
                                    foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                                    {
                                        if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是"
                                            && semesterSubjectScore.Subject == subjectName
                                            && ("" + semesterSubjectScore.SchoolYear) == conf.SchoolYear
                                            && ("" + semesterSubjectScore.Semester) == conf.Semester)
                                        {
                                            findInSemesterSubjectScore = true;


                                            decimal level;
                                            subjectNumber = decimal.TryParse(semesterSubjectScore.Level, out level) ? (decimal?)level : null;
                                            row["科目名稱" + subjectIndex] = semesterSubjectScore.Subject + GetNumber(subjectNumber);
                                            row["學分數" + subjectIndex] = semesterSubjectScore.CreditDec();
                                            row["科目必選修" + subjectIndex] = semesterSubjectScore.Require ? "必修" : "選修";
                                            row["科目校部定" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("修課校部訂");
                                            row["科目註記" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("註記");
                                            row["科目取得學分" + subjectIndex] = semesterSubjectScore.Pass ? "是" : "否";
                                            row["科目未取得學分註記" + subjectIndex] = semesterSubjectScore.Pass ? "" : "\f";

                                            //"原始成績", "學年調整成績", "擇優採計成績", "補考成績", "重修成績"
                                            if (semesterSubjectScore.Detail.GetAttribute("不需評分") != "是")
                                            {
                                                row["學期科目原始成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("原始成績");
                                                row["學期科目補考成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("補考成績");
                                                row["學期科目重修成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("重修成績");
                                                row["學期科目手動調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("擇優採計成績");
                                                row["學期科目學年調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("學年調整成績");
                                                row["學期科目成績" + subjectIndex] = semesterSubjectScore.Score;

                                                if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("原始成績"))
                                                    row["學期科目原始成績註記" + subjectIndex] = "\f";
                                                if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("補考成績"))
                                                    row["學期科目補考成績註記" + subjectIndex] = "\f";
                                                if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("重修成績"))
                                                    row["學期科目重修成績註記" + subjectIndex] = "\f";
                                                if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("擇優採計成績"))
                                                    row["學期科目手動成績註記" + subjectIndex] = "\f";
                                                if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("學年調整成績"))
                                                    row["學期科目學年成績註記" + subjectIndex] = "\f";
                                            }
                                            #region 學期科目班、科、校、類別1、類別2排名
                                            key = "學期科目排名成績" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目排名成績" + subjectIndex] = "" + stuRec.Fields[key];
                                            //班
                                            key = "學期科目班排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目班排名" + subjectIndex] = "" + stuRec.Fields[key];
                                            key = "學期科目班排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目班排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                            //科
                                            key = "學期科目科排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目科排名" + subjectIndex] = "" + stuRec.Fields[key];
                                            key = "學期科目班科名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目科排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                            //校
                                            key = "學期科目校排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目全校排名" + subjectIndex] = "" + stuRec.Fields[key];
                                            key = "學期科目科校名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                            if (stuRec.Fields.ContainsKey(key))
                                                row["學期科目全校排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                            //類別1
                                            if (studentTag1Group.ContainsKey(studentID))
                                            {
                                                foreach (var tag in studentTags[studentID])
                                                {
                                                    if (tag.RefTagID == studentTag1Group[studentID])
                                                    {
                                                        key = "學期科目成績" + tag.Name + "排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                        if (stuRec.Fields.ContainsKey(key))
                                                            row["學期科目類別1排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                        key = "學期科目成績" + tag.Name + "排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                        if (stuRec.Fields.ContainsKey(key))
                                                            row["學期科目類別1排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                                        break;
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
                                                        key = "學期科目成績" + tag.Name + "排名" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                        if (stuRec.Fields.ContainsKey(key))
                                                            row["學期科目類別2排名" + subjectIndex] = "" + stuRec.Fields[key];
                                                        key = "學期科目成績" + tag.Name + "排名母數" + semesterSubjectScore.Subject + "^^^" + semesterSubjectScore.Level;
                                                        if (stuRec.Fields.ContainsKey(key))
                                                            row["學期科目類別2排名母數" + subjectIndex] = "" + stuRec.Fields[key];
                                                        break;
                                                    }
                                                }
                                            }
                                            #endregion
                                            stuRec.SemesterSubjectScoreList.Remove(semesterSubjectScore);
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region 定期評量成績
                                    // 檢查畫面上定期評量列印科目
                                    if (conf.PrintSubjectList.Contains(subjectName))
                                    {
                                        if (studentExamSores.ContainsKey(studentID))
                                        {
                                            if (studentExamSores[studentID].ContainsKey(subjectName))
                                            {
                                                foreach (var courseID in studentExamSores[studentID][subjectName].Keys)
                                                {
                                                    var sceTakeRecord = studentExamSores[studentID][subjectName][courseID];
                                                    if (sceTakeRecord != null)
                                                    {//有輸入
                                                        if (findInSemesterSubjectScore)
                                                        {
                                                            if (sceTakeRecord.SubjectLevel != "" + subjectNumber)
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        findInExamScores = true;
                                                        if (!findInSemesterSubjectScore)
                                                        {
                                                            decimal level;
                                                            subjectNumber = decimal.TryParse(sceTakeRecord.SubjectLevel, out level) ? (decimal?)level : null;
                                                            row["科目名稱" + subjectIndex] = sceTakeRecord.Subject + GetNumber(subjectNumber);
                                                            row["學分數" + subjectIndex] = sceTakeRecord.CreditDec();
                                                        }
                                                        row["科目成績" + subjectIndex] = sceTakeRecord.SpecialCase == "" ? ("" + sceTakeRecord.ExamScore) : sceTakeRecord.SpecialCase;
                                                        #region 班排名及落點分析
                                                        if (stuRec.RefClass != null)
                                                        {
                                                            key = "班排名" + stuRec.RefClass.ClassID + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                                            {
                                                                row["班排名" + subjectIndex] = ranks[key].IndexOf(sceTakeRecord.ExamScore) + 1;
                                                                row["班排名母數" + subjectIndex] = ranks[key].Count;
                                                            }
                                                            if (rankStudents.ContainsKey(key))
                                                            {
                                                                row["班高標" + subjectIndex] = analytics[key + "^^^高標"];
                                                                row["班均標" + subjectIndex] = analytics[key + "^^^均標"];
                                                                row["班低標" + subjectIndex] = analytics[key + "^^^低標"];
                                                                row["班標準差" + subjectIndex] = analytics[key + "^^^標準差"];
                                                                row["班組距" + subjectIndex + "count90"] = analytics[key + "^^^count90"];
                                                                row["班組距" + subjectIndex + "count80"] = analytics[key + "^^^count80"];
                                                                row["班組距" + subjectIndex + "count70"] = analytics[key + "^^^count70"];
                                                                row["班組距" + subjectIndex + "count60"] = analytics[key + "^^^count60"];
                                                                row["班組距" + subjectIndex + "count50"] = analytics[key + "^^^count50"];
                                                                row["班組距" + subjectIndex + "count40"] = analytics[key + "^^^count40"];
                                                                row["班組距" + subjectIndex + "count30"] = analytics[key + "^^^count30"];
                                                                row["班組距" + subjectIndex + "count20"] = analytics[key + "^^^count20"];
                                                                row["班組距" + subjectIndex + "count10"] = analytics[key + "^^^count10"];
                                                                row["班組距" + subjectIndex + "count100Up"] = analytics[key + "^^^count100Up"];
                                                                row["班組距" + subjectIndex + "count90Up"] = analytics[key + "^^^count90Up"];
                                                                row["班組距" + subjectIndex + "count80Up"] = analytics[key + "^^^count80Up"];
                                                                row["班組距" + subjectIndex + "count70Up"] = analytics[key + "^^^count70Up"];
                                                                row["班組距" + subjectIndex + "count60Up"] = analytics[key + "^^^count60Up"];
                                                                row["班組距" + subjectIndex + "count50Up"] = analytics[key + "^^^count50Up"];
                                                                row["班組距" + subjectIndex + "count40Up"] = analytics[key + "^^^count40Up"];
                                                                row["班組距" + subjectIndex + "count30Up"] = analytics[key + "^^^count30Up"];
                                                                row["班組距" + subjectIndex + "count20Up"] = analytics[key + "^^^count20Up"];
                                                                row["班組距" + subjectIndex + "count10Up"] = analytics[key + "^^^count10Up"];
                                                                row["班組距" + subjectIndex + "count90Down"] = analytics[key + "^^^count90Down"];
                                                                row["班組距" + subjectIndex + "count80Down"] = analytics[key + "^^^count80Down"];
                                                                row["班組距" + subjectIndex + "count70Down"] = analytics[key + "^^^count70Down"];
                                                                row["班組距" + subjectIndex + "count60Down"] = analytics[key + "^^^count60Down"];
                                                                row["班組距" + subjectIndex + "count50Down"] = analytics[key + "^^^count50Down"];
                                                                row["班組距" + subjectIndex + "count40Down"] = analytics[key + "^^^count40Down"];
                                                                row["班組距" + subjectIndex + "count30Down"] = analytics[key + "^^^count30Down"];
                                                                row["班組距" + subjectIndex + "count20Down"] = analytics[key + "^^^count20Down"];
                                                                row["班組距" + subjectIndex + "count10Down"] = analytics[key + "^^^count10Down"];
                                                            }
                                                        }
                                                        #endregion
                                                        #region 科排名及落點分析
                                                        if (stuRec.Department != "")
                                                        {
                                                            key = "科排名" + stuRec.Department + "^^^" + gradeYear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                                            {
                                                                row["科排名" + subjectIndex] = ranks[key].IndexOf(sceTakeRecord.ExamScore) + 1;
                                                                row["科排名母數" + subjectIndex] = ranks[key].Count;
                                                            }
                                                            if (rankStudents.ContainsKey(key))
                                                            {
                                                                row["科高標" + subjectIndex] = analytics[key + "^^^高標"];
                                                                row["科均標" + subjectIndex] = analytics[key + "^^^均標"];
                                                                row["科低標" + subjectIndex] = analytics[key + "^^^低標"];
                                                                row["科標準差" + subjectIndex] = analytics[key + "^^^標準差"];
                                                                row["科組距" + subjectIndex + "count90"] = analytics[key + "^^^count90"];
                                                                row["科組距" + subjectIndex + "count80"] = analytics[key + "^^^count80"];
                                                                row["科組距" + subjectIndex + "count70"] = analytics[key + "^^^count70"];
                                                                row["科組距" + subjectIndex + "count60"] = analytics[key + "^^^count60"];
                                                                row["科組距" + subjectIndex + "count50"] = analytics[key + "^^^count50"];
                                                                row["科組距" + subjectIndex + "count40"] = analytics[key + "^^^count40"];
                                                                row["科組距" + subjectIndex + "count30"] = analytics[key + "^^^count30"];
                                                                row["科組距" + subjectIndex + "count20"] = analytics[key + "^^^count20"];
                                                                row["科組距" + subjectIndex + "count10"] = analytics[key + "^^^count10"];
                                                                row["科組距" + subjectIndex + "count100Up"] = analytics[key + "^^^count100Up"];
                                                                row["科組距" + subjectIndex + "count90Up"] = analytics[key + "^^^count90Up"];
                                                                row["科組距" + subjectIndex + "count80Up"] = analytics[key + "^^^count80Up"];
                                                                row["科組距" + subjectIndex + "count70Up"] = analytics[key + "^^^count70Up"];
                                                                row["科組距" + subjectIndex + "count60Up"] = analytics[key + "^^^count60Up"];
                                                                row["科組距" + subjectIndex + "count50Up"] = analytics[key + "^^^count50Up"];
                                                                row["科組距" + subjectIndex + "count40Up"] = analytics[key + "^^^count40Up"];
                                                                row["科組距" + subjectIndex + "count30Up"] = analytics[key + "^^^count30Up"];
                                                                row["科組距" + subjectIndex + "count20Up"] = analytics[key + "^^^count20Up"];
                                                                row["科組距" + subjectIndex + "count10Up"] = analytics[key + "^^^count10Up"];
                                                                row["科組距" + subjectIndex + "count90Down"] = analytics[key + "^^^count90Down"];
                                                                row["科組距" + subjectIndex + "count80Down"] = analytics[key + "^^^count80Down"];
                                                                row["科組距" + subjectIndex + "count70Down"] = analytics[key + "^^^count70Down"];
                                                                row["科組距" + subjectIndex + "count60Down"] = analytics[key + "^^^count60Down"];
                                                                row["科組距" + subjectIndex + "count50Down"] = analytics[key + "^^^count50Down"];
                                                                row["科組距" + subjectIndex + "count40Down"] = analytics[key + "^^^count40Down"];
                                                                row["科組距" + subjectIndex + "count30Down"] = analytics[key + "^^^count30Down"];
                                                                row["科組距" + subjectIndex + "count20Down"] = analytics[key + "^^^count20Down"];
                                                                row["科組距" + subjectIndex + "count10Down"] = analytics[key + "^^^count10Down"];
                                                            }
                                                        }
                                                        #endregion
                                                        #region 全校排名及落點分析
                                                        key = "全校排名" + gradeYear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                        if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                                        {
                                                            row["全校排名" + subjectIndex] = ranks[key].IndexOf(sceTakeRecord.ExamScore) + 1;
                                                            row["全校排名母數" + subjectIndex] = ranks[key].Count;
                                                        }
                                                        if (rankStudents.ContainsKey(key))
                                                        {
                                                            row["校高標" + subjectIndex] = analytics[key + "^^^高標"];
                                                            row["校均標" + subjectIndex] = analytics[key + "^^^均標"];
                                                            row["校低標" + subjectIndex] = analytics[key + "^^^低標"];
                                                            row["校標準差" + subjectIndex] = analytics[key + "^^^標準差"];
                                                            row["校組距" + subjectIndex + "count90"] = analytics[key + "^^^count90"];
                                                            row["校組距" + subjectIndex + "count80"] = analytics[key + "^^^count80"];
                                                            row["校組距" + subjectIndex + "count70"] = analytics[key + "^^^count70"];
                                                            row["校組距" + subjectIndex + "count60"] = analytics[key + "^^^count60"];
                                                            row["校組距" + subjectIndex + "count50"] = analytics[key + "^^^count50"];
                                                            row["校組距" + subjectIndex + "count40"] = analytics[key + "^^^count40"];
                                                            row["校組距" + subjectIndex + "count30"] = analytics[key + "^^^count30"];
                                                            row["校組距" + subjectIndex + "count20"] = analytics[key + "^^^count20"];
                                                            row["校組距" + subjectIndex + "count10"] = analytics[key + "^^^count10"];
                                                            row["校組距" + subjectIndex + "count100Up"] = analytics[key + "^^^count100Up"];
                                                            row["校組距" + subjectIndex + "count90Up"] = analytics[key + "^^^count90Up"];
                                                            row["校組距" + subjectIndex + "count80Up"] = analytics[key + "^^^count80Up"];
                                                            row["校組距" + subjectIndex + "count70Up"] = analytics[key + "^^^count70Up"];
                                                            row["校組距" + subjectIndex + "count60Up"] = analytics[key + "^^^count60Up"];
                                                            row["校組距" + subjectIndex + "count50Up"] = analytics[key + "^^^count50Up"];
                                                            row["校組距" + subjectIndex + "count40Up"] = analytics[key + "^^^count40Up"];
                                                            row["校組距" + subjectIndex + "count30Up"] = analytics[key + "^^^count30Up"];
                                                            row["校組距" + subjectIndex + "count20Up"] = analytics[key + "^^^count20Up"];
                                                            row["校組距" + subjectIndex + "count10Up"] = analytics[key + "^^^count10Up"];
                                                            row["校組距" + subjectIndex + "count90Down"] = analytics[key + "^^^count90Down"];
                                                            row["校組距" + subjectIndex + "count80Down"] = analytics[key + "^^^count80Down"];
                                                            row["校組距" + subjectIndex + "count70Down"] = analytics[key + "^^^count70Down"];
                                                            row["校組距" + subjectIndex + "count60Down"] = analytics[key + "^^^count60Down"];
                                                            row["校組距" + subjectIndex + "count50Down"] = analytics[key + "^^^count50Down"];
                                                            row["校組距" + subjectIndex + "count40Down"] = analytics[key + "^^^count40Down"];
                                                            row["校組距" + subjectIndex + "count30Down"] = analytics[key + "^^^count30Down"];
                                                            row["校組距" + subjectIndex + "count20Down"] = analytics[key + "^^^count20Down"];
                                                            row["校組距" + subjectIndex + "count10Down"] = analytics[key + "^^^count10Down"];
                                                        }
                                                        #endregion
                                                        #region 類別1排名及落點分析
                                                        if (studentTag1Group.ContainsKey(studentID) && conf.TagRank1SubjectList.Contains(subjectName))
                                                        {
                                                            key = "類別1排名" + studentTag1Group[studentID] + "^^^" + gradeYear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                                            {
                                                                row["類別1排名" + subjectIndex] = ranks[key].IndexOf(sceTakeRecord.ExamScore) + 1;
                                                                row["類別1排名母數" + subjectIndex] = ranks[key].Count;
                                                            }
                                                            if (rankStudents.ContainsKey(key))
                                                            {
                                                                row["類1高標" + subjectIndex] = analytics[key + "^^^高標"];
                                                                row["類1均標" + subjectIndex] = analytics[key + "^^^均標"];
                                                                row["類1低標" + subjectIndex] = analytics[key + "^^^低標"];
                                                                row["類1標準差" + subjectIndex] = analytics[key + "^^^標準差"];
                                                                row["類1組距" + subjectIndex + "count90"] = analytics[key + "^^^count90"];
                                                                row["類1組距" + subjectIndex + "count80"] = analytics[key + "^^^count80"];
                                                                row["類1組距" + subjectIndex + "count70"] = analytics[key + "^^^count70"];
                                                                row["類1組距" + subjectIndex + "count60"] = analytics[key + "^^^count60"];
                                                                row["類1組距" + subjectIndex + "count50"] = analytics[key + "^^^count50"];
                                                                row["類1組距" + subjectIndex + "count40"] = analytics[key + "^^^count40"];
                                                                row["類1組距" + subjectIndex + "count30"] = analytics[key + "^^^count30"];
                                                                row["類1組距" + subjectIndex + "count20"] = analytics[key + "^^^count20"];
                                                                row["類1組距" + subjectIndex + "count10"] = analytics[key + "^^^count10"];
                                                                row["類1組距" + subjectIndex + "count100Up"] = analytics[key + "^^^count100Up"];
                                                                row["類1組距" + subjectIndex + "count90Up"] = analytics[key + "^^^count90Up"];
                                                                row["類1組距" + subjectIndex + "count80Up"] = analytics[key + "^^^count80Up"];
                                                                row["類1組距" + subjectIndex + "count70Up"] = analytics[key + "^^^count70Up"];
                                                                row["類1組距" + subjectIndex + "count60Up"] = analytics[key + "^^^count60Up"];
                                                                row["類1組距" + subjectIndex + "count50Up"] = analytics[key + "^^^count50Up"];
                                                                row["類1組距" + subjectIndex + "count40Up"] = analytics[key + "^^^count40Up"];
                                                                row["類1組距" + subjectIndex + "count30Up"] = analytics[key + "^^^count30Up"];
                                                                row["類1組距" + subjectIndex + "count20Up"] = analytics[key + "^^^count20Up"];
                                                                row["類1組距" + subjectIndex + "count10Up"] = analytics[key + "^^^count10Up"];
                                                                row["類1組距" + subjectIndex + "count90Down"] = analytics[key + "^^^count90Down"];
                                                                row["類1組距" + subjectIndex + "count80Down"] = analytics[key + "^^^count80Down"];
                                                                row["類1組距" + subjectIndex + "count70Down"] = analytics[key + "^^^count70Down"];
                                                                row["類1組距" + subjectIndex + "count60Down"] = analytics[key + "^^^count60Down"];
                                                                row["類1組距" + subjectIndex + "count50Down"] = analytics[key + "^^^count50Down"];
                                                                row["類1組距" + subjectIndex + "count40Down"] = analytics[key + "^^^count40Down"];
                                                                row["類1組距" + subjectIndex + "count30Down"] = analytics[key + "^^^count30Down"];
                                                                row["類1組距" + subjectIndex + "count20Down"] = analytics[key + "^^^count20Down"];
                                                                row["類1組距" + subjectIndex + "count10Down"] = analytics[key + "^^^count10Down"];
                                                            }
                                                        }
                                                        #endregion
                                                        #region 類別2排名及落點分析
                                                        if (studentTag2Group.ContainsKey(studentID) && conf.TagRank2SubjectList.Contains(subjectName))
                                                        {
                                                            key = "類別2排名" + studentTag2Group[studentID] + "^^^" + gradeYear + "^^^" + sceTakeRecord.Subject + "^^^" + sceTakeRecord.SubjectLevel;
                                                            if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                                            {
                                                                row["類別2排名" + subjectIndex] = ranks[key].IndexOf(sceTakeRecord.ExamScore) + 1;
                                                                row["類別2排名母數" + subjectIndex] = ranks[key].Count;
                                                            }
                                                            if (rankStudents.ContainsKey(key))
                                                            {
                                                                row["類2高標" + subjectIndex] = analytics[key + "^^^高標"];
                                                                row["類2均標" + subjectIndex] = analytics[key + "^^^均標"];
                                                                row["類2低標" + subjectIndex] = analytics[key + "^^^低標"];
                                                                row["類2標準差" + subjectIndex] = analytics[key + "^^^標準差"];
                                                                row["類2組距" + subjectIndex + "count90"] = analytics[key + "^^^count90"];
                                                                row["類2組距" + subjectIndex + "count80"] = analytics[key + "^^^count80"];
                                                                row["類2組距" + subjectIndex + "count70"] = analytics[key + "^^^count70"];
                                                                row["類2組距" + subjectIndex + "count60"] = analytics[key + "^^^count60"];
                                                                row["類2組距" + subjectIndex + "count50"] = analytics[key + "^^^count50"];
                                                                row["類2組距" + subjectIndex + "count40"] = analytics[key + "^^^count40"];
                                                                row["類2組距" + subjectIndex + "count30"] = analytics[key + "^^^count30"];
                                                                row["類2組距" + subjectIndex + "count20"] = analytics[key + "^^^count20"];
                                                                row["類2組距" + subjectIndex + "count10"] = analytics[key + "^^^count10"];
                                                                row["類2組距" + subjectIndex + "count100Up"] = analytics[key + "^^^count100Up"];
                                                                row["類2組距" + subjectIndex + "count90Up"] = analytics[key + "^^^count90Up"];
                                                                row["類2組距" + subjectIndex + "count80Up"] = analytics[key + "^^^count80Up"];
                                                                row["類2組距" + subjectIndex + "count70Up"] = analytics[key + "^^^count70Up"];
                                                                row["類2組距" + subjectIndex + "count60Up"] = analytics[key + "^^^count60Up"];
                                                                row["類2組距" + subjectIndex + "count50Up"] = analytics[key + "^^^count50Up"];
                                                                row["類2組距" + subjectIndex + "count40Up"] = analytics[key + "^^^count40Up"];
                                                                row["類2組距" + subjectIndex + "count30Up"] = analytics[key + "^^^count30Up"];
                                                                row["類2組距" + subjectIndex + "count20Up"] = analytics[key + "^^^count20Up"];
                                                                row["類2組距" + subjectIndex + "count10Up"] = analytics[key + "^^^count10Up"];
                                                                row["類2組距" + subjectIndex + "count90Down"] = analytics[key + "^^^count90Down"];
                                                                row["類2組距" + subjectIndex + "count80Down"] = analytics[key + "^^^count80Down"];
                                                                row["類2組距" + subjectIndex + "count70Down"] = analytics[key + "^^^count70Down"];
                                                                row["類2組距" + subjectIndex + "count60Down"] = analytics[key + "^^^count60Down"];
                                                                row["類2組距" + subjectIndex + "count50Down"] = analytics[key + "^^^count50Down"];
                                                                row["類2組距" + subjectIndex + "count40Down"] = analytics[key + "^^^count40Down"];
                                                                row["類2組距" + subjectIndex + "count30Down"] = analytics[key + "^^^count30Down"];
                                                                row["類2組距" + subjectIndex + "count20Down"] = analytics[key + "^^^count20Down"];
                                                                row["類2組距" + subjectIndex + "count10Down"] = analytics[key + "^^^count10Down"];
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {//修課有該考試但沒有成績資料
                                                        var courseRecs = accessHelper.CourseHelper.GetCourse(courseID);
                                                        if (courseRecs.Count > 0)
                                                        {
                                                            var courseRec = courseRecs[0];
                                                            if (findInSemesterSubjectScore)
                                                            {
                                                                if (courseRec.SubjectLevel != "" + subjectNumber)
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            findInExamScores = true;
                                                            if (!findInSemesterSubjectScore)
                                                            {
                                                                decimal level;
                                                                subjectNumber = decimal.TryParse(courseRec.SubjectLevel, out level) ? (decimal?)level : null;
                                                                row["科目名稱" + subjectIndex] = courseRec.Subject + GetNumber(subjectNumber);
                                                                row["學分數" + subjectIndex] = courseRec.CreditDec();
                                                            }
                                                            row["科目成績" + subjectIndex] = "未輸入";
                                                        }
                                                    }
                                                    if (studentRefExamSores.ContainsKey(studentID) && studentRefExamSores[studentID].ContainsKey(courseID))
                                                    {
                                                        row["前次成績" + subjectIndex] =
                                                            studentRefExamSores[studentID][courseID].SpecialCase == ""
                                                            ? ("" + studentRefExamSores[studentID][courseID].ExamScore)
                                                            : studentRefExamSores[studentID][courseID].SpecialCase;
                                                    }
                                                    studentExamSores[studentID][subjectName].Remove(courseID);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 上學期學期成績
                                    if (conf.Semester == "2" && conf.WithPrevSemesterScore)
                                    {
                                        foreach (var semesterSubjectScore in stuRec.SemesterSubjectScoreList)
                                        {
                                            if (semesterSubjectScore.Detail.GetAttribute("不計學分") != "是"
                                                && semesterSubjectScore.Subject == subjectName
                                                && semesterSubjectScore.Semester == 1
                                                && semesterSubjectScore.GradeYear == currentGradeYear)
                                            {
                                                findInSemester1SubjectScore = true;
                                                if (!findInSemesterSubjectScore
                                                    && !findInExamScores)
                                                {
                                                    decimal level;
                                                    subjectNumber = decimal.TryParse(semesterSubjectScore.Level, out level) ? (decimal?)level : null;
                                                    row["科目名稱" + subjectIndex] = semesterSubjectScore.Subject + GetNumber(subjectNumber);
                                                    row["學分數" + subjectIndex] = semesterSubjectScore.CreditDec();
                                                    row["科目必選修" + subjectIndex] = semesterSubjectScore.Require ? "必修" : "選修";
                                                    row["科目校部定" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("修課校部訂");
                                                    row["科目註記" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("註記");
                                                }
                                                row["上學期科目取得學分" + subjectIndex] = semesterSubjectScore.Pass ? "是" : "否";
                                                row["上學期科目未取得學分註記" + subjectIndex] = semesterSubjectScore.Pass ? "" : "\f";

                                                //"原始成績", "學年調整成績", "擇優採計成績", "補考成績", "重修成績"
                                                if (semesterSubjectScore.Detail.GetAttribute("不需評分") != "是")
                                                {
                                                    row["上學期科目原始成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("原始成績");
                                                    row["上學期科目補考成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("補考成績");
                                                    row["上學期科目重修成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("重修成績");
                                                    row["上學期科目手動調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("擇優採計成績");
                                                    row["上學期科目學年調整成績" + subjectIndex] = semesterSubjectScore.Detail.GetAttribute("學年調整成績");
                                                    row["上學期科目成績" + subjectIndex] = semesterSubjectScore.Score;

                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("原始成績"))
                                                        row["上學期科目原始成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("補考成績"))
                                                        row["上學期科目補考成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("重修成績"))
                                                        row["上學期科目重修成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("擇優採計成績"))
                                                        row["上學期科目手動成績註記" + subjectIndex] = "\f";
                                                    if ("" + semesterSubjectScore.Score == semesterSubjectScore.Detail.GetAttribute("學年調整成績"))
                                                        row["上學期科目學年成績註記" + subjectIndex] = "\f";
                                                }
                                                stuRec.SemesterSubjectScoreList.Remove(semesterSubjectScore);
                                                break;
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 學年成績
                                    if (conf.Semester == "2" && conf.WithSchoolYearScore)
                                    {
                                        foreach (var schoolYearSubjectScore in stuRec.SchoolYearSubjectScoreList)
                                        {
                                            if (("" + schoolYearSubjectScore.SchoolYear) == conf.SchoolYear
                                                && schoolYearSubjectScore.Subject == subjectName)
                                            {
                                                if (!findInSemesterSubjectScore
                                                    && !findInSemester1SubjectScore
                                                    && !findInExamScores)
                                                {
                                                    row["科目名稱" + subjectIndex] = schoolYearSubjectScore.Subject;
                                                }
                                                row["學年科目成績" + subjectIndex] = schoolYearSubjectScore.Score;
                                                stuRec.SchoolYearSubjectScoreList.Remove(schoolYearSubjectScore);
                                                break;
                                            }
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
                            #endregion
                            #region 總分
                            if (studentPrintSubjectSum.ContainsKey(studentID))
                            {
                                row["總分"] = studentPrintSubjectSum[studentID];
                                //總分班排名                                
                                key = "總分班排名" + stuRec.RefClass.ClassID;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["總分班排名"] = ranks[key].IndexOf(studentPrintSubjectSum[studentID]) + 1;
                                    row["總分班排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["總分班高標"] = analytics[key + "^^^高標"];
                                    row["總分班均標"] = analytics[key + "^^^均標"];
                                    row["總分班低標"] = analytics[key + "^^^低標"];
                                    row["總分班標準差"] = analytics[key + "^^^標準差"];
                                    row["總分班組距count90"] = analytics[key + "^^^count90"];
                                    row["總分班組距count80"] = analytics[key + "^^^count80"];
                                    row["總分班組距count70"] = analytics[key + "^^^count70"];
                                    row["總分班組距count60"] = analytics[key + "^^^count60"];
                                    row["總分班組距count50"] = analytics[key + "^^^count50"];
                                    row["總分班組距count40"] = analytics[key + "^^^count40"];
                                    row["總分班組距count30"] = analytics[key + "^^^count30"];
                                    row["總分班組距count20"] = analytics[key + "^^^count20"];
                                    row["總分班組距count10"] = analytics[key + "^^^count10"];
                                    row["總分班組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["總分班組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["總分班組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["總分班組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["總分班組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["總分班組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["總分班組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["總分班組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["總分班組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["總分班組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["總分班組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["總分班組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["總分班組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["總分班組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["總分班組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["總分班組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["總分班組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["總分班組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["總分班組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                //總分科排名
                                key = "總分科排名" + stuRec.Department + "^^^" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["總分科排名"] = ranks[key].IndexOf(studentPrintSubjectSum[studentID]) + 1;
                                    row["總分科排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["總分科高標"] = analytics[key + "^^^高標"];
                                    row["總分科均標"] = analytics[key + "^^^均標"];
                                    row["總分科低標"] = analytics[key + "^^^低標"];
                                    row["總分科標準差"] = analytics[key + "^^^標準差"];
                                    row["總分科組距count90"] = analytics[key + "^^^count90"];
                                    row["總分科組距count80"] = analytics[key + "^^^count80"];
                                    row["總分科組距count70"] = analytics[key + "^^^count70"];
                                    row["總分科組距count60"] = analytics[key + "^^^count60"];
                                    row["總分科組距count50"] = analytics[key + "^^^count50"];
                                    row["總分科組距count40"] = analytics[key + "^^^count40"];
                                    row["總分科組距count30"] = analytics[key + "^^^count30"];
                                    row["總分科組距count20"] = analytics[key + "^^^count20"];
                                    row["總分科組距count10"] = analytics[key + "^^^count10"];
                                    row["總分科組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["總分科組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["總分科組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["總分科組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["總分科組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["總分科組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["總分科組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["總分科組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["總分科組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["總分科組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["總分科組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["總分科組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["總分科組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["總分科組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["總分科組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["總分科組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["總分科組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["總分科組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["總分科組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                //總分全校排名
                                key = "總分全校排名" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["總分全校排名"] = ranks[key].IndexOf(studentPrintSubjectSum[studentID]) + 1;
                                    row["總分全校排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["總分校高標"] = analytics[key + "^^^高標"];
                                    row["總分校均標"] = analytics[key + "^^^均標"];
                                    row["總分校低標"] = analytics[key + "^^^低標"];
                                    row["總分校標準差"] = analytics[key + "^^^標準差"];
                                    row["總分校組距count90"] = analytics[key + "^^^count90"];
                                    row["總分校組距count80"] = analytics[key + "^^^count80"];
                                    row["總分校組距count70"] = analytics[key + "^^^count70"];
                                    row["總分校組距count60"] = analytics[key + "^^^count60"];
                                    row["總分校組距count50"] = analytics[key + "^^^count50"];
                                    row["總分校組距count40"] = analytics[key + "^^^count40"];
                                    row["總分校組距count30"] = analytics[key + "^^^count30"];
                                    row["總分校組距count20"] = analytics[key + "^^^count20"];
                                    row["總分校組距count10"] = analytics[key + "^^^count10"];
                                    row["總分校組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["總分校組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["總分校組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["總分校組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["總分校組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["總分校組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["總分校組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["總分校組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["總分校組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["總分校組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["總分校組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["總分校組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["總分校組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["總分校組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["總分校組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["總分校組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["總分校組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["總分校組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["總分校組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                            }
                            #endregion
                            #region 平均
                            if (studentPrintSubjectAvg.ContainsKey(studentID))
                            {
                                row["平均"] = studentPrintSubjectAvg[studentID];
                                key = "平均班排名" + stuRec.RefClass.ClassID;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["平均班排名"] = ranks[key].IndexOf(studentPrintSubjectAvg[studentID]) + 1;
                                    row["平均班排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["平均班高標"] = analytics[key + "^^^高標"];
                                    row["平均班均標"] = analytics[key + "^^^均標"];
                                    row["平均班低標"] = analytics[key + "^^^低標"];
                                    row["平均班標準差"] = analytics[key + "^^^標準差"];
                                    row["平均班組距count90"] = analytics[key + "^^^count90"];
                                    row["平均班組距count80"] = analytics[key + "^^^count80"];
                                    row["平均班組距count70"] = analytics[key + "^^^count70"];
                                    row["平均班組距count60"] = analytics[key + "^^^count60"];
                                    row["平均班組距count50"] = analytics[key + "^^^count50"];
                                    row["平均班組距count40"] = analytics[key + "^^^count40"];
                                    row["平均班組距count30"] = analytics[key + "^^^count30"];
                                    row["平均班組距count20"] = analytics[key + "^^^count20"];
                                    row["平均班組距count10"] = analytics[key + "^^^count10"];
                                    row["平均班組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["平均班組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["平均班組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["平均班組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["平均班組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["平均班組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["平均班組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["平均班組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["平均班組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["平均班組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["平均班組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["平均班組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["平均班組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["平均班組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["平均班組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["平均班組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["平均班組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["平均班組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["平均班組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "平均科排名" + stuRec.Department + "^^^" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["平均科排名"] = ranks[key].IndexOf(studentPrintSubjectAvg[studentID]) + 1;
                                    row["平均科排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["平均科高標"] = analytics[key + "^^^高標"];
                                    row["平均科均標"] = analytics[key + "^^^均標"];
                                    row["平均科低標"] = analytics[key + "^^^低標"];
                                    row["平均科標準差"] = analytics[key + "^^^標準差"];
                                    row["平均科組距count90"] = analytics[key + "^^^count90"];
                                    row["平均科組距count80"] = analytics[key + "^^^count80"];
                                    row["平均科組距count70"] = analytics[key + "^^^count70"];
                                    row["平均科組距count60"] = analytics[key + "^^^count60"];
                                    row["平均科組距count50"] = analytics[key + "^^^count50"];
                                    row["平均科組距count40"] = analytics[key + "^^^count40"];
                                    row["平均科組距count30"] = analytics[key + "^^^count30"];
                                    row["平均科組距count20"] = analytics[key + "^^^count20"];
                                    row["平均科組距count10"] = analytics[key + "^^^count10"];
                                    row["平均科組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["平均科組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["平均科組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["平均科組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["平均科組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["平均科組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["平均科組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["平均科組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["平均科組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["平均科組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["平均科組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["平均科組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["平均科組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["平均科組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["平均科組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["平均科組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["平均科組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["平均科組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["平均科組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "平均全校排名" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["平均全校排名"] = ranks[key].IndexOf(studentPrintSubjectAvg[studentID]) + 1;
                                    row["平均全校排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["平均校高標"] = analytics[key + "^^^高標"];
                                    row["平均校均標"] = analytics[key + "^^^均標"];
                                    row["平均校低標"] = analytics[key + "^^^低標"];
                                    row["平均校標準差"] = analytics[key + "^^^標準差"];
                                    row["平均校組距count90"] = analytics[key + "^^^count90"];
                                    row["平均校組距count80"] = analytics[key + "^^^count80"];
                                    row["平均校組距count70"] = analytics[key + "^^^count70"];
                                    row["平均校組距count60"] = analytics[key + "^^^count60"];
                                    row["平均校組距count50"] = analytics[key + "^^^count50"];
                                    row["平均校組距count40"] = analytics[key + "^^^count40"];
                                    row["平均校組距count30"] = analytics[key + "^^^count30"];
                                    row["平均校組距count20"] = analytics[key + "^^^count20"];
                                    row["平均校組距count10"] = analytics[key + "^^^count10"];
                                    row["平均校組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["平均校組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["平均校組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["平均校組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["平均校組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["平均校組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["平均校組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["平均校組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["平均校組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["平均校組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["平均校組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["平均校組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["平均校組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["平均校組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["平均校組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["平均校組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["平均校組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["平均校組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["平均校組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                            }
                            #endregion
                            #region 加權總分
                            if (studentPrintSubjectSumW.ContainsKey(studentID))
                            {
                                row["加權總分"] = studentPrintSubjectSumW[studentID];
                                key = "加權總分班排名" + stuRec.RefClass.ClassID;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權總分班排名"] = ranks[key].IndexOf(studentPrintSubjectSumW[studentID]) + 1;
                                    row["加權總分班排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權總分班高標"] = analytics[key + "^^^高標"];
                                    row["加權總分班均標"] = analytics[key + "^^^均標"];
                                    row["加權總分班低標"] = analytics[key + "^^^低標"];
                                    row["加權總分班標準差"] = analytics[key + "^^^標準差"];
                                    row["加權總分班組距count90"] = analytics[key + "^^^count90"];
                                    row["加權總分班組距count80"] = analytics[key + "^^^count80"];
                                    row["加權總分班組距count70"] = analytics[key + "^^^count70"];
                                    row["加權總分班組距count60"] = analytics[key + "^^^count60"];
                                    row["加權總分班組距count50"] = analytics[key + "^^^count50"];
                                    row["加權總分班組距count40"] = analytics[key + "^^^count40"];
                                    row["加權總分班組距count30"] = analytics[key + "^^^count30"];
                                    row["加權總分班組距count20"] = analytics[key + "^^^count20"];
                                    row["加權總分班組距count10"] = analytics[key + "^^^count10"];
                                    row["加權總分班組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權總分班組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權總分班組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權總分班組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權總分班組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權總分班組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權總分班組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權總分班組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權總分班組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權總分班組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權總分班組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權總分班組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權總分班組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權總分班組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權總分班組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權總分班組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權總分班組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權總分班組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權總分班組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "加權總分科排名" + stuRec.Department + "^^^" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權總分科排名"] = ranks[key].IndexOf(studentPrintSubjectSumW[studentID]) + 1;
                                    row["加權總分科排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權總分科高標"] = analytics[key + "^^^高標"];
                                    row["加權總分科均標"] = analytics[key + "^^^均標"];
                                    row["加權總分科低標"] = analytics[key + "^^^低標"];
                                    row["加權總分科標準差"] = analytics[key + "^^^標準差"];
                                    row["加權總分科組距count90"] = analytics[key + "^^^count90"];
                                    row["加權總分科組距count80"] = analytics[key + "^^^count80"];
                                    row["加權總分科組距count70"] = analytics[key + "^^^count70"];
                                    row["加權總分科組距count60"] = analytics[key + "^^^count60"];
                                    row["加權總分科組距count50"] = analytics[key + "^^^count50"];
                                    row["加權總分科組距count40"] = analytics[key + "^^^count40"];
                                    row["加權總分科組距count30"] = analytics[key + "^^^count30"];
                                    row["加權總分科組距count20"] = analytics[key + "^^^count20"];
                                    row["加權總分科組距count10"] = analytics[key + "^^^count10"];
                                    row["加權總分科組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權總分科組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權總分科組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權總分科組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權總分科組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權總分科組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權總分科組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權總分科組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權總分科組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權總分科組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權總分科組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權總分科組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權總分科組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權總分科組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權總分科組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權總分科組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權總分科組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權總分科組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權總分科組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "加權總分全校排名" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權總分全校排名"] = ranks[key].IndexOf(studentPrintSubjectSumW[studentID]) + 1;
                                    row["加權總分全校排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權總分校高標"] = analytics[key + "^^^高標"];
                                    row["加權總分校均標"] = analytics[key + "^^^均標"];
                                    row["加權總分校低標"] = analytics[key + "^^^低標"];
                                    row["加權總分校標準差"] = analytics[key + "^^^標準差"];
                                    row["加權總分校組距count90"] = analytics[key + "^^^count90"];
                                    row["加權總分校組距count80"] = analytics[key + "^^^count80"];
                                    row["加權總分校組距count70"] = analytics[key + "^^^count70"];
                                    row["加權總分校組距count60"] = analytics[key + "^^^count60"];
                                    row["加權總分校組距count50"] = analytics[key + "^^^count50"];
                                    row["加權總分校組距count40"] = analytics[key + "^^^count40"];
                                    row["加權總分校組距count30"] = analytics[key + "^^^count30"];
                                    row["加權總分校組距count20"] = analytics[key + "^^^count20"];
                                    row["加權總分校組距count10"] = analytics[key + "^^^count10"];
                                    row["加權總分校組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權總分校組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權總分校組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權總分校組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權總分校組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權總分校組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權總分校組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權總分校組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權總分校組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權總分校組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權總分校組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權總分校組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權總分校組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權總分校組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權總分校組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權總分校組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權總分校組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權總分校組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權總分校組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                            }
                            #endregion
                            #region 加權平均
                            if (studentPrintSubjectAvgW.ContainsKey(studentID))
                            {
                                row["加權平均"] = studentPrintSubjectAvgW[studentID];
                                key = "加權平均班排名" + stuRec.RefClass.ClassID;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權平均班排名"] = ranks[key].IndexOf(studentPrintSubjectAvgW[studentID]) + 1;
                                    row["加權平均班排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權平均班高標"] = analytics[key + "^^^高標"];
                                    row["加權平均班均標"] = analytics[key + "^^^均標"];
                                    row["加權平均班低標"] = analytics[key + "^^^低標"];
                                    row["加權平均班標準差"] = analytics[key + "^^^標準差"];
                                    row["加權平均班組距count90"] = analytics[key + "^^^count90"];
                                    row["加權平均班組距count80"] = analytics[key + "^^^count80"];
                                    row["加權平均班組距count70"] = analytics[key + "^^^count70"];
                                    row["加權平均班組距count60"] = analytics[key + "^^^count60"];
                                    row["加權平均班組距count50"] = analytics[key + "^^^count50"];
                                    row["加權平均班組距count40"] = analytics[key + "^^^count40"];
                                    row["加權平均班組距count30"] = analytics[key + "^^^count30"];
                                    row["加權平均班組距count20"] = analytics[key + "^^^count20"];
                                    row["加權平均班組距count10"] = analytics[key + "^^^count10"];
                                    row["加權平均班組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權平均班組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權平均班組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權平均班組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權平均班組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權平均班組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權平均班組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權平均班組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權平均班組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權平均班組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權平均班組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權平均班組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權平均班組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權平均班組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權平均班組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權平均班組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權平均班組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權平均班組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權平均班組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "加權平均科排名" + stuRec.Department + "^^^" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權平均科排名"] = ranks[key].IndexOf(studentPrintSubjectAvgW[studentID]) + 1;
                                    row["加權平均科排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權平均科高標"] = analytics[key + "^^^高標"];
                                    row["加權平均科均標"] = analytics[key + "^^^均標"];
                                    row["加權平均科低標"] = analytics[key + "^^^低標"];
                                    row["加權平均科標準差"] = analytics[key + "^^^標準差"];
                                    row["加權平均科組距count90"] = analytics[key + "^^^count90"];
                                    row["加權平均科組距count80"] = analytics[key + "^^^count80"];
                                    row["加權平均科組距count70"] = analytics[key + "^^^count70"];
                                    row["加權平均科組距count60"] = analytics[key + "^^^count60"];
                                    row["加權平均科組距count50"] = analytics[key + "^^^count50"];
                                    row["加權平均科組距count40"] = analytics[key + "^^^count40"];
                                    row["加權平均科組距count30"] = analytics[key + "^^^count30"];
                                    row["加權平均科組距count20"] = analytics[key + "^^^count20"];
                                    row["加權平均科組距count10"] = analytics[key + "^^^count10"];
                                    row["加權平均科組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權平均科組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權平均科組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權平均科組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權平均科組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權平均科組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權平均科組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權平均科組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權平均科組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權平均科組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權平均科組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權平均科組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權平均科組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權平均科組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權平均科組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權平均科組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權平均科組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權平均科組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權平均科組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                                key = "加權平均全校排名" + gradeYear;
                                if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))//明確判斷學生是否參與排名
                                {
                                    row["加權平均全校排名"] = ranks[key].IndexOf(studentPrintSubjectAvgW[studentID]) + 1;
                                    row["加權平均全校排名母數"] = ranks[key].Count;
                                }
                                if (rankStudents.ContainsKey(key))
                                {
                                    row["加權平均校高標"] = analytics[key + "^^^高標"];
                                    row["加權平均校均標"] = analytics[key + "^^^均標"];
                                    row["加權平均校低標"] = analytics[key + "^^^低標"];
                                    row["加權平均校標準差"] = analytics[key + "^^^標準差"];
                                    row["加權平均校組距count90"] = analytics[key + "^^^count90"];
                                    row["加權平均校組距count80"] = analytics[key + "^^^count80"];
                                    row["加權平均校組距count70"] = analytics[key + "^^^count70"];
                                    row["加權平均校組距count60"] = analytics[key + "^^^count60"];
                                    row["加權平均校組距count50"] = analytics[key + "^^^count50"];
                                    row["加權平均校組距count40"] = analytics[key + "^^^count40"];
                                    row["加權平均校組距count30"] = analytics[key + "^^^count30"];
                                    row["加權平均校組距count20"] = analytics[key + "^^^count20"];
                                    row["加權平均校組距count10"] = analytics[key + "^^^count10"];
                                    row["加權平均校組距count100Up"] = analytics[key + "^^^count100Up"];
                                    row["加權平均校組距count90Up"] = analytics[key + "^^^count90Up"];
                                    row["加權平均校組距count80Up"] = analytics[key + "^^^count80Up"];
                                    row["加權平均校組距count70Up"] = analytics[key + "^^^count70Up"];
                                    row["加權平均校組距count60Up"] = analytics[key + "^^^count60Up"];
                                    row["加權平均校組距count50Up"] = analytics[key + "^^^count50Up"];
                                    row["加權平均校組距count40Up"] = analytics[key + "^^^count40Up"];
                                    row["加權平均校組距count30Up"] = analytics[key + "^^^count30Up"];
                                    row["加權平均校組距count20Up"] = analytics[key + "^^^count20Up"];
                                    row["加權平均校組距count10Up"] = analytics[key + "^^^count10Up"];
                                    row["加權平均校組距count90Down"] = analytics[key + "^^^count90Down"];
                                    row["加權平均校組距count80Down"] = analytics[key + "^^^count80Down"];
                                    row["加權平均校組距count70Down"] = analytics[key + "^^^count70Down"];
                                    row["加權平均校組距count60Down"] = analytics[key + "^^^count60Down"];
                                    row["加權平均校組距count50Down"] = analytics[key + "^^^count50Down"];
                                    row["加權平均校組距count40Down"] = analytics[key + "^^^count40Down"];
                                    row["加權平均校組距count30Down"] = analytics[key + "^^^count30Down"];
                                    row["加權平均校組距count20Down"] = analytics[key + "^^^count20Down"];
                                    row["加權平均校組距count10Down"] = analytics[key + "^^^count10Down"];
                                }
                            }
                            #endregion
                            #region 類別1綜合成績
                            if (studentTag1Group.ContainsKey(studentID))
                            {
                                foreach (var tag in studentTags[studentID])
                                {
                                    if (tag.RefTagID == studentTag1Group[studentID])
                                    {
                                        row["類別排名1"] = tag.Name;
                                    }
                                }
                                if (studentTag1SubjectSum.ContainsKey(studentID))
                                {
                                    row["類別1總分"] = studentTag1SubjectSum[studentID];
                                    key = "類別1總分排名" + "^^^" + gradeYear + "^^^" + studentTag1Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別1總分排名"] = ranks[key].IndexOf(studentTag1SubjectSum[studentID]) + 1;
                                        row["類別1總分排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類1總分高標"] = analytics[key + "^^^高標"];
                                        row["類1總分均標"] = analytics[key + "^^^均標"];
                                        row["類1總分低標"] = analytics[key + "^^^低標"];
                                        row["類1總分標準差"] = analytics[key + "^^^標準差"];
                                        row["類1總分組距count90"] = analytics[key + "^^^count90"];
                                        row["類1總分組距count80"] = analytics[key + "^^^count80"];
                                        row["類1總分組距count70"] = analytics[key + "^^^count70"];
                                        row["類1總分組距count60"] = analytics[key + "^^^count60"];
                                        row["類1總分組距count50"] = analytics[key + "^^^count50"];
                                        row["類1總分組距count40"] = analytics[key + "^^^count40"];
                                        row["類1總分組距count30"] = analytics[key + "^^^count30"];
                                        row["類1總分組距count20"] = analytics[key + "^^^count20"];
                                        row["類1總分組距count10"] = analytics[key + "^^^count10"];
                                        row["類1總分組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類1總分組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類1總分組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類1總分組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類1總分組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類1總分組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類1總分組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類1總分組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類1總分組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類1總分組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類1總分組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類1總分組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類1總分組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類1總分組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類1總分組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類1總分組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類1總分組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類1總分組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類1總分組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag1SubjectAvg.ContainsKey(studentID))
                                {
                                    row["類別1平均"] = studentTag1SubjectAvg[studentID];
                                    key = "類別1平均排名" + "^^^" + gradeYear + "^^^" + studentTag1Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別1平均排名"] = ranks[key].IndexOf(studentTag1SubjectAvg[studentID]) + 1; ;
                                        row["類別1平均排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類1平均高標"] = analytics[key + "^^^高標"];
                                        row["類1平均均標"] = analytics[key + "^^^均標"];
                                        row["類1平均低標"] = analytics[key + "^^^低標"];
                                        row["類1平均標準差"] = analytics[key + "^^^標準差"];
                                        row["類1平均組距count90"] = analytics[key + "^^^count90"];
                                        row["類1平均組距count80"] = analytics[key + "^^^count80"];
                                        row["類1平均組距count70"] = analytics[key + "^^^count70"];
                                        row["類1平均組距count60"] = analytics[key + "^^^count60"];
                                        row["類1平均組距count50"] = analytics[key + "^^^count50"];
                                        row["類1平均組距count40"] = analytics[key + "^^^count40"];
                                        row["類1平均組距count30"] = analytics[key + "^^^count30"];
                                        row["類1平均組距count20"] = analytics[key + "^^^count20"];
                                        row["類1平均組距count10"] = analytics[key + "^^^count10"];
                                        row["類1平均組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類1平均組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類1平均組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類1平均組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類1平均組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類1平均組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類1平均組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類1平均組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類1平均組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類1平均組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類1平均組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類1平均組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類1平均組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類1平均組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類1平均組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類1平均組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類1平均組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類1平均組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類1平均組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag1SubjectSumW.ContainsKey(studentID))
                                {
                                    row["類別1加權總分"] = studentTag1SubjectSumW[studentID];
                                    key = "類別1加權總分排名" + "^^^" + gradeYear + "^^^" + studentTag1Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別1加權總分排名"] = ranks[key].IndexOf(studentTag1SubjectSumW[studentID]) + 1; ;
                                        row["類別1加權總分排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類1加權總分高標"] = analytics[key + "^^^高標"];
                                        row["類1加權總分均標"] = analytics[key + "^^^均標"];
                                        row["類1加權總分低標"] = analytics[key + "^^^低標"];
                                        row["類1加權總分標準差"] = analytics[key + "^^^標準差"];
                                        row["類1加權總分組距count90"] = analytics[key + "^^^count90"];
                                        row["類1加權總分組距count80"] = analytics[key + "^^^count80"];
                                        row["類1加權總分組距count70"] = analytics[key + "^^^count70"];
                                        row["類1加權總分組距count60"] = analytics[key + "^^^count60"];
                                        row["類1加權總分組距count50"] = analytics[key + "^^^count50"];
                                        row["類1加權總分組距count40"] = analytics[key + "^^^count40"];
                                        row["類1加權總分組距count30"] = analytics[key + "^^^count30"];
                                        row["類1加權總分組距count20"] = analytics[key + "^^^count20"];
                                        row["類1加權總分組距count10"] = analytics[key + "^^^count10"];
                                        row["類1加權總分組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類1加權總分組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類1加權總分組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類1加權總分組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類1加權總分組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類1加權總分組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類1加權總分組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類1加權總分組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類1加權總分組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類1加權總分組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類1加權總分組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類1加權總分組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類1加權總分組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類1加權總分組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類1加權總分組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類1加權總分組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類1加權總分組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類1加權總分組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類1加權總分組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag1SubjectAvgW.ContainsKey(studentID))
                                {
                                    row["類別1加權平均"] = studentTag1SubjectAvgW[studentID];
                                    key = "類別1加權平均排名" + "^^^" + gradeYear + "^^^" + studentTag1Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別1加權平均排名"] = ranks[key].IndexOf(studentTag1SubjectAvgW[studentID]) + 1; ;
                                        row["類別1加權平均排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類1加權平均高標"] = analytics[key + "^^^高標"];
                                        row["類1加權平均均標"] = analytics[key + "^^^均標"];
                                        row["類1加權平均低標"] = analytics[key + "^^^低標"];
                                        row["類1加權平均標準差"] = analytics[key + "^^^標準差"];
                                        row["類1加權平均組距count90"] = analytics[key + "^^^count90"];
                                        row["類1加權平均組距count80"] = analytics[key + "^^^count80"];
                                        row["類1加權平均組距count70"] = analytics[key + "^^^count70"];
                                        row["類1加權平均組距count60"] = analytics[key + "^^^count60"];
                                        row["類1加權平均組距count50"] = analytics[key + "^^^count50"];
                                        row["類1加權平均組距count40"] = analytics[key + "^^^count40"];
                                        row["類1加權平均組距count30"] = analytics[key + "^^^count30"];
                                        row["類1加權平均組距count20"] = analytics[key + "^^^count20"];
                                        row["類1加權平均組距count10"] = analytics[key + "^^^count10"];
                                        row["類1加權平均組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類1加權平均組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類1加權平均組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類1加權平均組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類1加權平均組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類1加權平均組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類1加權平均組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類1加權平均組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類1加權平均組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類1加權平均組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類1加權平均組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類1加權平均組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類1加權平均組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類1加權平均組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類1加權平均組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類1加權平均組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類1加權平均組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類1加權平均組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類1加權平均組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                            }
                            #endregion
                            #region 類別2綜合成績
                            if (studentTag2Group.ContainsKey(studentID))
                            {
                                foreach (var tag in studentTags[studentID])
                                {
                                    if (tag.RefTagID == studentTag2Group[studentID])
                                    {
                                        row["類別排名2"] = tag.Name;
                                    }
                                }
                                if (studentTag2SubjectSum.ContainsKey(studentID))
                                {
                                    row["類別2總分"] = studentTag2SubjectSum[studentID];
                                    key = "類別2總分排名" + "^^^" + gradeYear + "^^^" + studentTag2Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別2總分排名"] = ranks[key].IndexOf(studentTag2SubjectSum[studentID]) + 1;
                                        row["類別2總分排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類2總分高標"] = analytics[key + "^^^高標"];
                                        row["類2總分均標"] = analytics[key + "^^^均標"];
                                        row["類2總分低標"] = analytics[key + "^^^低標"];
                                        row["類2總分標準差"] = analytics[key + "^^^標準差"];
                                        row["類2總分組距count90"] = analytics[key + "^^^count90"];
                                        row["類2總分組距count80"] = analytics[key + "^^^count80"];
                                        row["類2總分組距count70"] = analytics[key + "^^^count70"];
                                        row["類2總分組距count60"] = analytics[key + "^^^count60"];
                                        row["類2總分組距count50"] = analytics[key + "^^^count50"];
                                        row["類2總分組距count40"] = analytics[key + "^^^count40"];
                                        row["類2總分組距count30"] = analytics[key + "^^^count30"];
                                        row["類2總分組距count20"] = analytics[key + "^^^count20"];
                                        row["類2總分組距count10"] = analytics[key + "^^^count10"];
                                        row["類2總分組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類2總分組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類2總分組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類2總分組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類2總分組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類2總分組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類2總分組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類2總分組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類2總分組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類2總分組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類2總分組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類2總分組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類2總分組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類2總分組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類2總分組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類2總分組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類2總分組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類2總分組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類2總分組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag2SubjectAvg.ContainsKey(studentID))
                                {
                                    row["類別2平均"] = studentTag2SubjectAvg[studentID];
                                    key = "類別2平均排名" + "^^^" + gradeYear + "^^^" + studentTag2Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別2平均排名"] = ranks[key].IndexOf(studentTag2SubjectAvg[studentID]) + 1; ;
                                        row["類別2平均排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類2平均高標"] = analytics[key + "^^^高標"];
                                        row["類2平均均標"] = analytics[key + "^^^均標"];
                                        row["類2平均低標"] = analytics[key + "^^^低標"];
                                        row["類2平均標準差"] = analytics[key + "^^^標準差"];
                                        row["類2平均組距count90"] = analytics[key + "^^^count90"];
                                        row["類2平均組距count80"] = analytics[key + "^^^count80"];
                                        row["類2平均組距count70"] = analytics[key + "^^^count70"];
                                        row["類2平均組距count60"] = analytics[key + "^^^count60"];
                                        row["類2平均組距count50"] = analytics[key + "^^^count50"];
                                        row["類2平均組距count40"] = analytics[key + "^^^count40"];
                                        row["類2平均組距count30"] = analytics[key + "^^^count30"];
                                        row["類2平均組距count20"] = analytics[key + "^^^count20"];
                                        row["類2平均組距count10"] = analytics[key + "^^^count10"];
                                        row["類2平均組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類2平均組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類2平均組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類2平均組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類2平均組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類2平均組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類2平均組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類2平均組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類2平均組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類2平均組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類2平均組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類2平均組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類2平均組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類2平均組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類2平均組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類2平均組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類2平均組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類2平均組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類2平均組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag2SubjectSumW.ContainsKey(studentID))
                                {
                                    row["類別2加權總分"] = studentTag2SubjectSumW[studentID];
                                    key = "類別2加權總分排名" + "^^^" + gradeYear + "^^^" + studentTag2Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別2加權總分排名"] = ranks[key].IndexOf(studentTag2SubjectSumW[studentID]) + 1; ;
                                        row["類別2加權總分排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類2加權總分高標"] = analytics[key + "^^^高標"];
                                        row["類2加權總分均標"] = analytics[key + "^^^均標"];
                                        row["類2加權總分低標"] = analytics[key + "^^^低標"];
                                        row["類2加權總分標準差"] = analytics[key + "^^^標準差"];
                                        row["類2加權總分組距count90"] = analytics[key + "^^^count90"];
                                        row["類2加權總分組距count80"] = analytics[key + "^^^count80"];
                                        row["類2加權總分組距count70"] = analytics[key + "^^^count70"];
                                        row["類2加權總分組距count60"] = analytics[key + "^^^count60"];
                                        row["類2加權總分組距count50"] = analytics[key + "^^^count50"];
                                        row["類2加權總分組距count40"] = analytics[key + "^^^count40"];
                                        row["類2加權總分組距count30"] = analytics[key + "^^^count30"];
                                        row["類2加權總分組距count20"] = analytics[key + "^^^count20"];
                                        row["類2加權總分組距count10"] = analytics[key + "^^^count10"];
                                        row["類2加權總分組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類2加權總分組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類2加權總分組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類2加權總分組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類2加權總分組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類2加權總分組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類2加權總分組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類2加權總分組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類2加權總分組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類2加權總分組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類2加權總分組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類2加權總分組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類2加權總分組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類2加權總分組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類2加權總分組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類2加權總分組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類2加權總分組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類2加權總分組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類2加權總分組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                                if (studentTag2SubjectAvgW.ContainsKey(studentID))
                                {
                                    row["類別2加權平均"] = studentTag2SubjectAvgW[studentID];
                                    key = "類別2加權平均排名" + "^^^" + gradeYear + "^^^" + studentTag2Group[studentID];
                                    if (rankStudents.ContainsKey(key) && rankStudents[key].Contains(studentID))
                                    {
                                        row["類別2加權平均排名"] = ranks[key].IndexOf(studentTag2SubjectAvgW[studentID]) + 1; ;
                                        row["類別2加權平均排名母數"] = ranks[key].Count;
                                    }
                                    if (rankStudents.ContainsKey(key))
                                    {
                                        row["類2加權平均高標"] = analytics[key + "^^^高標"];
                                        row["類2加權平均均標"] = analytics[key + "^^^均標"];
                                        row["類2加權平均低標"] = analytics[key + "^^^低標"];
                                        row["類2加權平均標準差"] = analytics[key + "^^^標準差"];
                                        row["類2加權平均組距count90"] = analytics[key + "^^^count90"];
                                        row["類2加權平均組距count80"] = analytics[key + "^^^count80"];
                                        row["類2加權平均組距count70"] = analytics[key + "^^^count70"];
                                        row["類2加權平均組距count60"] = analytics[key + "^^^count60"];
                                        row["類2加權平均組距count50"] = analytics[key + "^^^count50"];
                                        row["類2加權平均組距count40"] = analytics[key + "^^^count40"];
                                        row["類2加權平均組距count30"] = analytics[key + "^^^count30"];
                                        row["類2加權平均組距count20"] = analytics[key + "^^^count20"];
                                        row["類2加權平均組距count10"] = analytics[key + "^^^count10"];
                                        row["類2加權平均組距count100Up"] = analytics[key + "^^^count100Up"];
                                        row["類2加權平均組距count90Up"] = analytics[key + "^^^count90Up"];
                                        row["類2加權平均組距count80Up"] = analytics[key + "^^^count80Up"];
                                        row["類2加權平均組距count70Up"] = analytics[key + "^^^count70Up"];
                                        row["類2加權平均組距count60Up"] = analytics[key + "^^^count60Up"];
                                        row["類2加權平均組距count50Up"] = analytics[key + "^^^count50Up"];
                                        row["類2加權平均組距count40Up"] = analytics[key + "^^^count40Up"];
                                        row["類2加權平均組距count30Up"] = analytics[key + "^^^count30Up"];
                                        row["類2加權平均組距count20Up"] = analytics[key + "^^^count20Up"];
                                        row["類2加權平均組距count10Up"] = analytics[key + "^^^count10Up"];
                                        row["類2加權平均組距count90Down"] = analytics[key + "^^^count90Down"];
                                        row["類2加權平均組距count80Down"] = analytics[key + "^^^count80Down"];
                                        row["類2加權平均組距count70Down"] = analytics[key + "^^^count70Down"];
                                        row["類2加權平均組距count60Down"] = analytics[key + "^^^count60Down"];
                                        row["類2加權平均組距count50Down"] = analytics[key + "^^^count50Down"];
                                        row["類2加權平均組距count40Down"] = analytics[key + "^^^count40Down"];
                                        row["類2加權平均組距count30Down"] = analytics[key + "^^^count30Down"];
                                        row["類2加權平均組距count20Down"] = analytics[key + "^^^count20Down"];
                                        row["類2加權平均組距count10Down"] = analytics[key + "^^^count10Down"];
                                    }
                                }
                            }
                            #endregion
                            #endregion
                            #region 學務資料
                            #region 綜合表現
                            foreach (SemesterMoralScoreInfo info in stuRec.SemesterMoralScoreList)
                            {
                                if (("" + info.Semester) == conf.Semester && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    row["導師評語"] = info.SupervisedByComment;
                                    System.Xml.XmlElement xml = info.Detail;
                                    foreach (System.Xml.XmlElement each in xml.SelectNodes("TextScore/Morality"))
                                    {
                                        string face = each.GetAttribute("Face");
                                        if ((SmartSchool.Customization.Data.SystemInformation.Fields["文字評量對照表"] as System.Xml.XmlElement).SelectSingleNode("Content/Morality[@Face='" + face + "']") != null)
                                        {
                                            string comment = each.InnerText;
                                            row["綜合表現：" + face] = each.InnerText;
                                        }
                                    }
                                    break;
                                }
                            }
                            #endregion
                            #region 獎懲統計
                            int 大功 = 0;
                            int 小功 = 0;
                            int 嘉獎 = 0;
                            int 大過 = 0;
                            int 小過 = 0;
                            int 警告 = 0;
                            bool 留校察看 = false;
                            foreach (RewardInfo info in stuRec.RewardList)
                            {
                                if (("" + info.Semester) == conf.Semester && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    大功 += info.AwardA;
                                    小功 += info.AwardB;
                                    嘉獎 += info.AwardC;
                                    if (!info.Cleared)
                                    {
                                        大過 += info.FaultA;
                                        小過 += info.FaultB;
                                        警告 += info.FaultC;
                                    }
                                    if (info.UltimateAdmonition)
                                        留校察看 = true;
                                }
                            }
                            row["大功統計"] = 大功 == 0 ? "" : ("" + 大功);
                            row["小功統計"] = 小功 == 0 ? "" : ("" + 小功);
                            row["嘉獎統計"] = 嘉獎 == 0 ? "" : ("" + 嘉獎);
                            row["大過統計"] = 大過 == 0 ? "" : ("" + 大過);
                            row["小過統計"] = 小過 == 0 ? "" : ("" + 小過);
                            row["警告統計"] = 警告 == 0 ? "" : ("" + 警告);
                            row["留校察看"] = 留校察看 ? "是" : "";
                            #endregion
                            #region 缺曠統計
                            Dictionary<string, int> 缺曠項目統計 = new Dictionary<string, int>();
                            foreach (AttendanceInfo info in stuRec.AttendanceList)
                            {
                                if (("" + info.Semester) == conf.Semester && ("" + info.SchoolYear) == conf.SchoolYear)
                                {
                                    string infoType = "";
                                    if (dicPeriodMappingType.ContainsKey(info.Period))
                                        infoType = dicPeriodMappingType[info.Period];
                                    else
                                        infoType = "";
                                    string attendanceKey = "" + infoType + "_" + info.Absence;
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
                            progressCount++;
                            bkw.ReportProgress(70 + progressCount * 20 / selectedStudents.Count);
                        }
                        #endregion
                        bkw.ReportProgress(90);

                        // 取欄位用
                        //StreamWriter sw = new StreamWriter(Application.StartupPath + "\\111.txt");
                        //foreach (DataColumn dc in table.Columns)
                        //{
                        //    sw.WriteLine(dc.Caption);
                        //}                            
                        //    sw.Flush();
                        //sw.Close();


                        // 處理 epost
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


                            //data["總成績名次"] = dr["學期學業成績班排名"].ToString() + "/" + dr["學期學業成績班排名母數"].ToString();
                            data["總成績名次"] = dr["學期學業成績班排名"].ToString();

                            // 處理固定對照
                            foreach (DataColumn dc in table.Columns)
                            {
                                if (eKeyValDict.ContainsKey(dc.Caption))
                                    data[eKeyValDict[dc.Caption]] = dr[dc.Caption];
                            }



                            // 處理科目成績
                            for (int subjectIndex = 1; subjectIndex <= conf.SubjectLimit; subjectIndex++)
                            {
                                if (dr["科目名稱" + subjectIndex].ToString() != "")
                                {
                                    data["科目" + subjectIndex] = dr["科目名稱" + subjectIndex];
                                    data["學分" + subjectIndex] = dr["學分數" + subjectIndex];
                                    data["成績" + subjectIndex] = dr["學期科目成績" + subjectIndex];

                                    // 學期科目成績
                                    decimal sc;
                                    if (decimal.TryParse(dr["學期科目成績" + subjectIndex].ToString(), out sc))
                                    {
                                        // 小於及格標準
                                        if (sc < scA)
                                        {
                                            // 可以補考,不可補考
                                            if (sc >= scB)
                                            {
                                                data["備註" + subjectIndex] = "*";
                                                // *
                                                dr["學期科目可補考註記" + subjectIndex] = "\f";
                                            }
                                            else
                                            {
                                                data["備註" + subjectIndex] = "#";
                                                // #
                                                dr["學期科目不可補考註記" + subjectIndex] = "\f";
                                            }
                                        }
                                    }

                                    //// 未取得學分
                                    //string strP = dr["科目取得學分"].ToString();
                                    //if (!string.IsNullOrEmpty(strP) && strP == "否")
                                    //    data["重修補考" + subjectIndex] = "#";


                                    data["名次" + subjectIndex] = dr["學期科目班排名" + subjectIndex].ToString() + "/" + dr["學期科目班排名母數" + subjectIndex].ToString();
                                    //data["名次" + subjectIndex] = dr["學期科目班排名" + subjectIndex].ToString();
                                }
                            }

                            // 處理綜合評語字串前加"
                            foreach (string str in CommList)
                            {
                                if (_dtEpost.Columns.Contains(str))
                                    data[str] = @"""" + data[str].ToString() + @"""";
                            }

                            data["導師評語"] = @"""" + data["導師評語"].ToString() + @"""";
                            _dtEpost.Rows.Add(data);
                        }
                        //document = conf.Template;
                        //document.MailMerge.Execute(table); 

                        #endregion
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


