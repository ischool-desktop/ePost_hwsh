using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SH_YearScoreReport_hwsh
{
    [FISCA.UDT.TableName("ischool.StudentSH_hwsh_YearScoreReport.Configure")]
    public class Configure : FISCA.UDT.ActiveRecord
    {

        public Configure()
        {
            PrintSubjectList = new List<string>();
            TagRank1TagList = new List<string>();
            TagRank1SubjectList = new List<string>();
            TagRank2TagList = new List<string>();
            TagRank2SubjectList = new List<string>();
            RankFilterTagList = new List<string>();
        }
        /// <summary>
        /// 設定檔名稱
        /// </summary>
        [FISCA.UDT.Field]
        public string Name { get; set; }
        /// <summary>
        /// 學年度
        /// </summary>
        [FISCA.UDT.Field]
        public string SchoolYear { get; set; }
        /// <summary>
        /// 學期
        /// </summary>
        [FISCA.UDT.Field]
        public string Semester { get; set; }
        /// <summary>
        /// 樣板包含學年成績
        /// </summary>
        [FISCA.UDT.Field]
        public bool WithSchoolYearScore { get; set; }
        /// <summary>
        /// 樣板包含上學期成績
        /// </summary>
        [FISCA.UDT.Field]
        public bool WithPrevSemesterScore { get; set; }
        /// <summary>
        /// 列印樣板
        /// </summary>
        [FISCA.UDT.Field]
        private string TemplateStream { get; set; }
        public Aspose.Words.Document Template { get; set; }
        /// <summary>
        /// 樣板中支援列印科目的最大數
        /// </summary>
        [FISCA.UDT.Field]
        public int SubjectLimit { get; set; }

        /// <summary>
        /// 列印科別
        /// </summary>
        [FISCA.UDT.Field]
        private string PrintSubjectListString { get; set; }
        public List<string> PrintSubjectList { get; private set; }
        /// <summary>
        /// 類別排名1
        /// </summary>
        [FISCA.UDT.Field]
        public string TagRank1TagName { get; set; }
        public List<string> TagRank1TagList { get; private set; }
        /// <summary>
        /// 類別排名1，排名科目
        /// </summary>
        [FISCA.UDT.Field]
        private string TagRank1SubjectListString { get; set; }
        public List<string> TagRank1SubjectList { get; private set; }
        /// <summary>
        /// 類別排名2
        /// </summary>
        [FISCA.UDT.Field]
        public string TagRank2TagName { get; set; }
        public List<string> TagRank2TagList { get; private set; }
        /// <summary>
        /// 類別排名2，排名科目
        /// </summary>
        [FISCA.UDT.Field]
        private string TagRank2SubjectListString { get; set; }
        public List<string> TagRank2SubjectList { get; private set; }
        /// <summary>
        /// 不參與排名學生類別
        /// </summary>
        [FISCA.UDT.Field]
        public string RankFilterTagName { get; set; }
        public List<string> RankFilterTagList { get; private set; }

        /// <summary>
        /// 是否產生epost檔案，是:產生epost
        /// </summary>
        [FISCA.UDT.Field]
        public bool ExportEpost { get; set; }

        /// <summary>
        /// 在儲存前，把資料填入儲存欄位中
        /// </summary>
        public void Encode()
        {
            this.PrintSubjectListString = "";
            foreach (var item in this.PrintSubjectList)
            {
                this.PrintSubjectListString += (this.PrintSubjectListString == "" ? "" : "^^^") + item;
            }


            this.TagRank1SubjectListString = "";
            foreach (var item in this.TagRank1SubjectList)
            {
                this.TagRank1SubjectListString += (this.TagRank1SubjectListString == "" ? "" : "^^^") + item;
            }
            this.TagRank2SubjectListString = "";
            foreach (var item in this.TagRank2SubjectList)
            {
                this.TagRank2SubjectListString += (this.TagRank2SubjectListString == "" ? "" : "^^^") + item;
            }
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.Template.Save(stream, Aspose.Words.SaveFormat.Doc);
            this.TemplateStream = Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 在資料取出後，把資料從儲存欄位轉換至資料欄位
        /// </summary>
        public void Decode()
        {
            if (!string.IsNullOrEmpty(this.PrintSubjectListString))
                this.PrintSubjectList = new List<string>(this.PrintSubjectListString.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries));
            else
                this.PrintSubjectList = new List<string>();

            if (!string.IsNullOrEmpty(this.TagRank1SubjectListString))
                this.TagRank1SubjectList = new List<string>(this.TagRank1SubjectListString.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries));
            else
                this.TagRank1SubjectList = new List<string>();

            if (!string.IsNullOrEmpty(this.TagRank2SubjectListString))
                this.TagRank2SubjectList = new List<string>(this.TagRank2SubjectListString.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries));
            else
                this.TagRank2SubjectList = new List<string>();

            if (this.TemplateStream != null)
                this.Template = new Aspose.Words.Document(new MemoryStream(Convert.FromBase64String(this.TemplateStream)));
            else
                this.Template = new Aspose.Words.Document(new MemoryStream(Properties.Resources.惠文高中學年成績單樣版));
        }
    }
}
