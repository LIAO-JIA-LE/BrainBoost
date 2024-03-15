﻿using BrainBoost.Models;
using BrainBoost.Parameter;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using System.Drawing.Printing;
using BrainBoost.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace BrainBoost.Services
{
    public class QuestionsDBService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        readonly MemberService MemberService;

        public QuestionsDBService(IConfiguration configuration,MemberService _memberService)
        {
            cnstr = configuration.GetConnectionString("ConnectionStrings"); 
            MemberService = _memberService;
        }
        #endregion


        #region 檔案匯入
        public DataTable FileDataPrecess(IFormFile file){
            // 上傳的文件
            Stream stream = file.OpenReadStream();

            // 儲存Excel的資料
            DataTable dataTable = new DataTable();

            // 讀取or處理Excel文件
            IWorkbook wb;

            // 工作表
            ISheet sheet;

            // 標頭
            IRow headerRow;

            // 欄數
            int cellCount;

            try
            {
                // excel版本(.xlsx)
                if (file.FileName.ToUpper().EndsWith("XLSX"))
                    wb = new XSSFWorkbook(stream);
                // excel版本(.xls)
                else
                    wb = new HSSFWorkbook(stream);

                // 取第一個工作表
                sheet = wb.GetSheetAt(0);

                // 此工作表的第一列
                headerRow = sheet.GetRow(0);

                // 計算欄位數
                cellCount = headerRow.LastCellNum;

                // 讀取標題列，將抓到的值放進DataTable做完欄位名稱
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    dataTable.Columns.Add(new DataColumn(headerRow.GetCell(i).StringCellValue));

                int column = 1; //計算每一列讀到第幾個欄位

                // 略過標題列，處理到最後一列
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    // 取目前的列
                    IRow row = sheet.GetRow(i);

                    // 若該列的第一個欄位無資料，break跳出
                    if (string.IsNullOrEmpty(row.Cells[0].ToString().Trim())) break;
                    
                    // 宣告DataRow
                    DataRow dataRow = dataTable.NewRow();

                    // 宣告ICell，獲取單元格的資訊
                    ICell cell;

                    try
                    {
                        // 依先前取得，依每一列的欄位數，逐一設定欄位內容
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            // 計算每一列讀到第幾個欄位（For錯誤訊息）
                            column = j + 1;

                            // 設定cell為目前第j欄位
                            cell = row.GetCell(j);

                            // 若cell有值
                            if (cell != null)
                            {
                                // 判斷資料格式
                                switch (cell.CellType)
                                {
                                    // 字串
                                    case NPOI.SS.UserModel.CellType.String:
                                        if (cell.StringCellValue != null)
                                            // 設定dataRow第j欄位的值，cell以字串型態取值
                                            dataRow[j] = cell.StringCellValue;
                                        else
                                            dataRow[j] = "";
                                        break;

                                    // 數字
                                    case NPOI.SS.UserModel.CellType.Numeric:
                                        // 日期
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))
                                            // 設定dataRow第j欄位的值，cell以日期格式取值
                                            dataRow[j] = DateTime.FromOADate(cell.NumericCellValue).ToString("yyyy/MM/dd HH:mm");
                                        else
                                            // 非日期格式
                                            dataRow[j] = cell.NumericCellValue;
                                        break;

                                    // 布林值
                                    case NPOI.SS.UserModel.CellType.Boolean:
                                        // 設定dataRow第j欄位的值，cell以布林型態取值
                                        dataRow[j] = cell.BooleanCellValue;
                                        break;

                                    //空值
                                    case NPOI.SS.UserModel.CellType.Blank:
                                        dataRow[j] = "";
                                        break;

                                    // 預設
                                    default:
                                        dataRow[j] = cell.StringCellValue;
                                        break;
                                }
                            }
                        }
                        // DataTable加入dataRow
                        dataTable.Rows.Add(dataRow);
                    }
                    catch (Exception e)
                    {
                        //錯誤訊息
                        throw new Exception("第 " + i + "列，資料格式有誤:\r\r" + e.ToString());
                    }
                }


            }
            finally
            {
                stream.Dispose();
                stream.Close();
            }
            return dataTable;
        }
            
        #endregion
        #region 匯入題目、選項和答案
        // 儲存題目
        public void InsertQuestion(QuestionList questionList)
        {
            string sql = $@"INSERT INTO Question(type_id, question_content, question_picture,member_id)
                            VALUES('{questionList.QuestionData.type_id}', '{questionList.QuestionData.question_content}',
                            '{questionList.QuestionData.question_picture}',{questionList.QuestionData.member_id})";
            // 先執行當前題目內容
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);

            InsertOption(questionList);
        }

        // 儲存選項
        public void InsertOption(QuestionList questionList)
        {
            int question_id = GetQuestionId(questionList);
            StringBuilder stringBuilder = new StringBuilder();
            string sql = $@"INSERT INTO Answer(question_id, question_answer, question_parse)
                            VALUES({question_id},'{questionList.AnswerData.question_answer}',
                            '{questionList.AnswerData.question_parse}')";
            
            // 是非題
            if(questionList.QuestionData.type_id == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    stringBuilder.Append($@"INSERT INTO ""Option""(question_id, option_content)
                                            VALUES('{question_id}', '{questionList.Options[i]}')");
                }
            }
            
            // 選擇題
            else if(questionList.QuestionData.type_id == 2)
            {
                for(int i = 0; i < 4; i++)
                {
                    //新增判斷是否為答案
                    stringBuilder.Append($@"INSERT INTO ""Option""(question_id, option_content, is_answer)      
                                            VALUES('{question_id}', '{questionList.Options[i]}','{questionList.Options[i] == questionList.AnswerData.question_answer}')");
                }
            }
            
            // 問答題
            else if (questionList.QuestionData.type_id == 3)
            {
                stringBuilder.Append($@"INSERT INTO ""Option""(question_id, option_content)
                                        VALUES('{question_id}', '{questionList.Options}')");
            }

            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
            conn.Execute(stringBuilder.ToString());
        }
        #endregion

        #region 獲得id
        // 獲得題目id
        public int GetQuestionId(QuestionList question){
            string sql = $@" SELECT question_id FROM Question WHERE question_content = '{question.QuestionData.question_content}' ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql);
        }
        #endregion

        #region 題目列表
        // 選擇題（只顯示題目內容，不包含選項）
        public List<QuestionList> GetQuestionList(int type, string Search){
            string sql = String.Empty;
            if(!String.IsNullOrEmpty(Search))
                sql = $@" SELECT * FROM Question WHERE question_content LIKE '%{Search}%' AND type_id = '{type}' ";
            using (var conn = new SqlConnection(cnstr))
            return new List<QuestionList>(conn.Query<QuestionList>(sql));
        }
        #endregion
        
    }
}