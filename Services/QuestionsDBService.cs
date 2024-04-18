using BrainBoost.Models;
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
using NPOI.SS.Formula.Functions;

namespace BrainBoost.Services
{
    public class QuestionsDBService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        private readonly ImportRepository _importRepository;
        public QuestionsDBService(IConfiguration configuration, ImportRepository importRepository)
        {
            cnstr = configuration.GetConnectionString("ConnectionStrings"); 
            _importRepository = importRepository;
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
        public int InsertQuestion(QuestionList questionList)
        {
            string sql = $@"INSERT INTO Question(type_id,subject_id, member_id, question_level, question_content, question_picture, create_time)
                            VALUES('{questionList.QuestionData.type_id}',{questionList.QuestionData.subject_id},{questionList.QuestionData.member_id},
                            '{questionList.QuestionData.question_level}','{questionList.QuestionData.question_content}',
                            '{questionList.QuestionData.question_picture}', '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}')
                            
                            DECLARE @question_id int;
                            SET @question_id = SCOPE_IDENTITY();
                            SELECT @question_id
                            ";
            
            // 先執行當前題目內容
            using var conn = new SqlConnection(cnstr);
            int question_id = conn.QueryFirst<int>(sql);
            InsertOption(questionList);
            return question_id;
        }

        // 獲得沒有重複的Tag
        public string NotRepeatQuestionTag(int memberid, string tagname){
            string sql = $@" SELECT tag_name FROM Tag WHERE member_id = '{memberid}' AND tag_name = '{tagname}' ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<string>(sql);
        }

        // 新增Tag
        public void InsertTag(int member_id, string tag_name){
            string sql = $@" INSERT Tag(member_id, tag_name)VALUES('{member_id}', '{tag_name}') ";
            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }

        // 獲得Tagid
        public int GetTagId(string tagname){
            string sql = $@" SELECT tag_id FROM Tag WHERE tag_name = '{tagname}'";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql);
        }
        
        // 儲存選項
        public void InsertOption(QuestionList questionList)
        {
            int question_id = GetQuestionId(questionList);
            StringBuilder stringBuilder = new StringBuilder();
            // 題目答案
            stringBuilder.Append($@"INSERT INTO Answer(question_id, question_answer, question_parse)
                                    VALUES({question_id},'{questionList.AnswerData.question_answer}',
                                    '{questionList.AnswerData.question_parse}')");
                            
            // 題目標籤
            // 看有沒有Tag的資訊
            if(!String.IsNullOrEmpty(questionList.TagData.tag_name)){
                // 獲得沒有重複的Tag
                if(String.IsNullOrEmpty(NotRepeatQuestionTag(questionList.QuestionData.member_id, questionList.TagData.tag_name)))
                    InsertTag(questionList.QuestionData.member_id, questionList.TagData.tag_name);
                int tag_id = GetTagId(questionList.TagData.tag_name);
                stringBuilder.Append($@" INSERT INTO Question_Tag (question_id, tag_id) VALUES ('{question_id}', '{tag_id}') ");
            }
            
            // 是非題 
            // 只有是跟否因此儲存在Answer的Table
            // 後續正式搶答邏輯直接與Answer的Table做關聯比對答案
            // if(questionList.QuestionData.type_id == 1)
            // {
            //     stringBuilder.Append($@"INSERT INTO ""Option""(question_id, option_content, is_answer)
            //                             VALUES('{question_id}', '{questionList.AnswerData.question_answer}', {(questionList.AnswerData.question_answer == "是" ? 1 : 0)})");
            // }
            
            // 選擇題
            if(questionList.QuestionData.type_id == 2)
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
            conn.Execute(stringBuilder.ToString());
        }
        #endregion

        #region 獲得id
        // 獲得題目id
        public int GetQuestionId(QuestionList question){
            string sql = $@" SELECT question_id FROM Question WHERE question_content = @question_content ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql,new{ question_content  = question.QuestionData.question_content});
        }
        #endregion

        #region 題目列表（只顯示題目內容，不包含選項）
        // 選擇題
        public List<Question> GetQuestionList(int type, string Search,Forpaging forpaging){
            string sql = $@" SELECT * FROM Question WHERE is_delete=0 AND 1=1";
            if(!String.IsNullOrEmpty(Search))
                sql = sql.Replace("1=1", $@"question_content LIKE '%{Search}%' AND type_id = '{type}' AND 1=1");
            else if(type != 0)
                sql = sql.Replace("1=1", $@"AND type_id = '{type}'");
            using var conn = new SqlConnection(cnstr);
            //指定時間格式
            return new List<Question>(conn.Query<Question>(sql));
        }
        #endregion

        //根據Id獲取題目內容
        public Question GetQuestionById(int id){
            string sql = $@" SELECT * FROM Question WHERE question_id = @question_id AND is_delete = 0";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Question>(sql, new{question_id = id});
        }
    }
}