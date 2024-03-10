using BrainBoost.Models;
using BrainBoost.Parameter;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using System.Drawing.Printing;
using BrainBoost.Services;

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
        
    }
}