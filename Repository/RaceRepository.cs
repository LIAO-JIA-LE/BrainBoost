using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;
using NPOI.SS.Formula.Functions;

namespace BrainBoost.Services
{
    public class RaceRepository
    {
        #region 呼叫函式
        private readonly string? cnstr;

        public RaceRepository(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion
        
        #region 新增搶答室資訊
        public void InsertRoom(string Code, InsertRoom raceData){
            string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            // 新增搶答室資訊
            string sql = $@"INSERT INTO RaceRoom(member_id,race_name, race_date, race_code, race_function, time_limit)
                            VALUES(@member_id,@race_name, @race_date, @race_code, @race_function, @time_limit) ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {raceData.member_id,raceData.race_name, race_date = currentTime, race_code = Code,raceData.race_function,raceData.time_limit});
            RoomQuestion(raceData);
        }
        #endregion

        #region 搶答室id
        public int GetRoomId(InsertRoom raceData){
            // 跟member_id和race_name尋找raceroom_id
            string sql = $@" SELECT raceroom_id FROM RaceRoom WHERE race_name = @race_name AND is_delete = @is_delete";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql, new {race_name = raceData.race_name, is_delete = 0});
        }
        #endregion

        #region 匯入搶答室問題
        public void RoomQuestion(InsertRoom raceData){
            int Raceroom_id = GetRoomId(raceData);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < raceData.question_id.Count; i++)
                stringBuilder.AppendLine($@"INSERT INTO Race_Question (raceroom_id, question_id, is_delete) 
                                            VALUES({Raceroom_id}, {raceData.question_id[i]}, 0) ");
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
        }
        #endregion

        #region 搶答室列表
        public List<RaceRooms> GetList(){
            string sql = $@" SELECT	* FROM RaceRoom WHERE is_delete = 0 ORDER BY race_public DESC, race_date DESC ";
            using (var conn = new SqlConnection(cnstr))
            return (List<RaceRooms>)conn.Query<RaceRooms>(sql);
        }
        #endregion

        #region 單一搶答室資訊
        public RaceRooms GetInformation(int Raceroom_id){
            string sql = $@" SELECT	* FROM RaceRoom WHERE raceroom_id = @raceroom_id AND is_delete = 0 ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<RaceRooms>(sql, new { raceroom_id = Raceroom_id });
        }
        #endregion

        #region  邀請碼取得搶答室
        public RaceRooms GetRaceRoomByCode(string race_code){
            string sql = " SELECT * FROM RaceRoom WHERE race_code = @race_code";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<RaceRooms>(sql,new{race_code});
        }
        #endregion

        #region 修改搶答室資訊
        public void RoomInformation(RaceInformation raceData){
            // 新增搶答室資訊
            string sql = $@"UPDATE RaceRoom SET race_name = @race_name, race_function = @race_function,
                            time_limit = @time_limit WHERE raceroom_id = @raceroom_id ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {race_name = raceData.race_name, race_function = raceData.race_function , time_limit = raceData.time_limit, raceroom_id = raceData.raceroom_id});
        }
        #endregion

        #region 新增搶答室題目
        public void InsertQuestion(RoomQuestionList roomQuestionList){
            StringBuilder stringBuilder = new StringBuilder();
            // 新增題目
            for(int i = 0; i < roomQuestionList.question_id_list.Count; i++){
                stringBuilder.Append($@"INSERT INTO Race_Question(raceroom_id, question_id)
                                        VALUES('{roomQuestionList.raceroom_id}', '{roomQuestionList.question_id_list[i]}') ");
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
        }
        public void InsertQuestion(int id, int question_id){
            string sql = $@"INSERT INTO Race_Question(raceroom_id, question_id)
                                        VALUES('{id}', '{question_id}') ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql.ToString());
        }
        #endregion

        #region 刪除搶答室題目
        public void DeleteQuestion(RaceroomQuestion raceroomQuestion){
            string sql = $@"UPDATE Race_Question SET is_delete = 1 
                            WHERE raceroom_id = @raceroom_id AND question_id = @question_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ raceroomQuestion.raceroom_id, raceroomQuestion.question_id});
        }
        #endregion

        #region 刪除搶答室
        public void DeleteRoom(int id){
            string sql = $@" UPDATE RaceRoom SET is_delete = 1 WHERE raceroom_id = @raceroom_id ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new{raceroom_id = id});
        }
        #endregion

        #region 搶答室題目列表
        public List<SimpleQuestion> QuestionList(int id){
            string sql = $@"SELECT R.question_id, question_content FROM Race_Question AS R
                            INNER JOIN Question AS Q ON R.question_id = Q.question_id WHERE raceroom_id = {id} AND R.is_delete = 0";
            using (var conn = new SqlConnection(cnstr))
            return (List<SimpleQuestion>)conn.Query<SimpleQuestion>(sql);
        }
        #endregion

        #region 搶答室題目單一
        public List<RaceQuestionAnswer> Question(RaceroomQuestion raceroomQuestion){
            string sql = $@"SELECT
                                A.question_id,
                                A.question_level,
                                A.question_content,
                                A.question_picture,
                                A.option_content,
                                A.option_picture,
                                A.is_delete
                            FROM Race_Question AS R
                            INNER JOIN(
                                SELECT
                                    Q.question_id,
                                    Q.question_level,
                                    Q.question_content,
                                    Q.question_picture,
                                    O.option_content,
                                    O.option_picture,
                                    O.is_answer,
                                    Q.is_delete
                                FROM Question AS Q
                                INNER JOIN ""Option"" AS O
                                ON Q.question_id = O.question_id
                                WHERE Q.question_id = 2 AND Q.is_delete = 0 AND O.is_delete = 0
                            )A
                            ON R.question_id = A.question_id
                            WHERE raceroom_id = {raceroomQuestion.raceroom_id} AND A.question_id = {raceroomQuestion.question_id} AND R.is_delete = 0";
            using (var conn = new SqlConnection(cnstr))
            return (List<RaceQuestionAnswer>)conn.Query<RaceQuestionAnswer>(sql);
        }
        #endregion

        #region 設定最大頁數
        public void SetMaxPaging(int member_id, Forpaging paging){
            string sql = $@"SELECT
                                COUNT(*)
                            FROM Question Q
                            WHERE member_id = @member_id AND 1=1";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql, new{member_id = member_id});
            paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / paging.Item));
            paging.SetRightPage();
        }
        public void SetMaxPaging(Forpaging paging, QuestionFiltering Search){
            StringBuilder stringBuilder = new StringBuilder();
            string sql = $@"SELECT
                                COUNT(*)
                            FROM Question Q
                            WHERE member_id = @member_id AND 1=1";
            #region 判斷
            if(Search.subject_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.subject_id = {Search.subject_id}");
            if(Search.type_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.type_id = {Search.type_id}");
            if(Search.tag_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND T.tag_id = {Search.tag_id}");
            if(Search.question_level != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_level = {Search.question_level}");
            if(Search.search != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_content LIKE '%{Search.search}%'");
            #endregion
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql, new{member_id = Search.member_id});
            paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / paging.Item));
            paging.SetRightPage();
        }
        #endregion

        #region 顯示題庫List
        public List<SimpleQuestion> GetAllQuestionList(int member_id, Forpaging paging){
            string sql = $@"SELECT
                             *
                            FROM (
                                SELECT
                                    ROW_NUMBER() OVER(ORDER BY Q.question_id) AS number,
                                    Q.question_id,
                                    question_content
                                FROM Question Q
                                INNER JOIN Question_Tag T ON Q.question_id = T.question_id
                                WHERE member_id = @member_id
                            ) AS numbered_rows
                            WHERE number BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item }";
            using (var conn = new SqlConnection(cnstr))
            return (List<SimpleQuestion>)conn.Query<SimpleQuestion>(sql, new{member_id = member_id});
        }
        public List<SimpleQuestion> GetAllQuestionList(Forpaging paging, QuestionFiltering Search){
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($@"SELECT
                                        *
                                    FROM (
                                        SELECT
                                            ROW_NUMBER() OVER(ORDER BY Q.question_id) AS number,
                                            Q.question_id,
                                            question_content
                                        FROM Question Q
                                        INNER JOIN Question_Tag T ON Q.question_id = T.question_id
                                        WHERE member_id = @member_id AND 1=1
                                    ) AS numbered_rows
                                    WHERE number BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item }");
            #region 判斷
            if(Search.subject_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.subject_id = {Search.subject_id} ");
            if(Search.type_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.type_id = {Search.type_id} ");
            if(Search.tag_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND T.tag_id = {Search.tag_id} ");
            if(Search.question_level != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_level = {Search.question_level} ");
            if(Search.search != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_content LIKE '%{Search.search}%' ");
            #endregion

            using (var conn = new SqlConnection(cnstr))
            return (List<SimpleQuestion>)conn.Query<SimpleQuestion>(stringBuilder.ToString(), new{member_id = Search.member_id});
        }
        #endregion

        #region 刪除隨機亂碼
        public void DeleteCode(int id){
            string sql = $@"DELETE FROM RaceRoom WHERE raceroom_id = @raceroom_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new{raceroom_id = id});
        }
        #endregion

        #region 標籤列表
        public List<Tag> TagList(int member_id){
            string sql = $@" SELECT
                                Q.question_id,
                                Q.question_content
                            FROM Tag T
                            INNER JOIN Question Q
                            ON T.member_id = Q.member_id
                            WHERE Q.subject_id = 1 AND Q.is_delete = 0";
            using var conn = new SqlConnection(cnstr);
            return (List<Tag>)conn.Query<Tag>(sql, new{member_id = member_id});
        }
        #endregion

        #region 隨機出題
        // 取得搶答室題目id題型
        public List<RaceQuestionListType> GetRaceRoomQuestionType(int id){
            string sql = $@"SELECT
                                R.raceroom_id,
                                Q.question_id,
                                type_id
                            FROM Question Q
                            INNER JOIN Race_Question R
                            ON Q.question_id = R.question_id
                            WHERE raceroom_id = @raceroom_id AND is_output = 0 AND q.is_delete = 0
                            ORDER BY type_id";
            using var conn = new SqlConnection(cnstr);
            return (List<RaceQuestionListType>)conn.Query<RaceQuestionListType>(sql, new{raceroom_id = id});
        }

        public RaceQuestionViewModel GetRandomQuestion(RaceQuestionListType Question){
            Random rd = new();
            string sql2 = String.Empty;
            // 顯示題目和答案 
            // 將出過的題目設定已出現
            string sql = $@"SELECT
                                question_id,
                                question_content,
                                question_picture
                            FROM Question
                            WHERE question_id = @question_id AND is_delete = 0
                            
                            UPDATE Race_Question
                            SET is_output = 1
                            WHERE raceroom_id = @raceroom_id AND question_id = @question_id";
            // 選項
            if(Question.type_id == 2){
                sql2 = $@"SELECT
                            O.question_id,
                            O.option_content,
                            O.option_picture
                        FROM ""Option"" O
                        INNER JOIN Question Q
                        ON O.question_id = Q.question_id
                        WHERE q.is_delete = 0 AND Q.question_id = @question_id";
            }
            using var conn = new SqlConnection(cnstr);
            // 宣告raceQuestionList
            RaceQuestionViewModel raceQuestionList = new();
            
            // 執行顯示題目和答案
            raceQuestionList.question = conn.QueryFirstOrDefault<Question>(sql, new {Question.question_id, Question.raceroom_id});
            
            // 如果是選擇題的話顯示選項
            if(!String.IsNullOrEmpty(sql2)){
                List<Option> options = new List<Option>(conn.Query<Option>(sql2, new {Question.question_id }));
                raceQuestionList.options = options.OrderBy(x => rd.Next()).ToList();
            }
            return raceQuestionList;
                
        }

        //重製RaceQuestion is_output欄位
        public void ResetRaceRoomQuestion(int raceroom_id){
            string sql = $@"
                            UPDATE Race_Question
                            SET is_output = 0
                            WHERE raceroom_id = @raceroom_id
                        ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ raceroom_id });
        }
        #endregion

        #region Level統計
        public List<int> Level(int raceroom_id){
            List<int> QuestionLevel = new List<int>();
            // 總題數
            string sql = $@"SELECT
                                COUNT(R.question_id) Level
                            FROM Race_Question R
                            INNER JOIN Question Q
                            ON R.question_id = Q.question_id
                            WHERE R.raceroom_id = @raceroom_id";
            using var conn = new SqlConnection(cnstr);
            QuestionLevel.Add(conn.QueryFirstOrDefault<int>(sql, new { raceroom_id }));
            // 防呆
            if(QuestionLevel[0] == null)
                QuestionLevel[0] = 0;

            // 其他難度統計
            string sql2 = String.Empty;
            for(int i = 1 ; i <= 10 ; i++){
                sql2 = $@"SELECT
                                COUNT(R.question_id) Level
                            FROM Race_Question R
                            INNER JOIN Question Q
                            ON R.question_id = Q.question_id
                            WHERE R.raceroom_id = @raceroom_id AND Q.question_level = @level";
                QuestionLevel.Add(conn.QueryFirstOrDefault<int>(sql2, new { raceroom_id, level = i }));
                if(QuestionLevel[i] == null)
                    QuestionLevel[i] = 0;
            }
            return QuestionLevel;
        }
        #endregion

        #region 記錄學生搶答室答案
        public void StudentReseponse(StudentReseponse studentReseponse){
            // 新增學生回答道資料庫
            string sql = $@"INSERT INTO Race_Response(raceroom_id, question_id, member_id, race_answer, race_time)
                            VALUES(@raceroom_id, @question_id, @member_id, @race_answer, @race_time)";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new { raceroom_id = studentReseponse.raceroom_id, question_id = studentReseponse.question_id, member_id = studentReseponse.member_id, race_answer = studentReseponse.race_answer, race_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }
        #endregion

        // #region 獲得race_response_id
        // public int GetRaceReseponseId(StudentReseponse studentReseponse){
        //     string sql = $@"SELECT
        //                         race_response_id
        //                     FROM Race_Response
        //                     WHERE raceroom_id = @raceroom_id AND question_id = @question_id AND member_id = @member_id";
        //     using var conn = new SqlConnection(cnstr);
        //     return conn.QueryFirstOrDefault<int>(sql, new {raceroom_id = studentReseponse.raceroom_id, question_id = studentReseponse.question_id, member_id = studentReseponse.member_id});
        // }
        // #endregion

        #region 確定答案
        public string QuestionAnswer(StudentReseponse studentReseponse){
            // 正確答案
            string sql = $@"SELECT
                                question_answer
                            FROM Answer
                            WHERE question_id = @question_id AND is_delete = 0 ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<string>(sql, new{ question_id = studentReseponse.question_id });
        }
        #endregion

        #region 學生答案
        public string StudentAnswer(StudentReseponse studentReseponse){
            // 學生答案
            string sql = $@"SELECT
                                race_answer
                            FROM Race_Response
                            WHERE raceroom_id = @raceroom_id AND question_id = @question_id AND member_id = @member_id ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<string>(sql, new{ raceroom_id = studentReseponse.raceroom_id, question_id = studentReseponse.question_id, member_id = studentReseponse.member_id });
        }
        #endregion

        #region 確定答案
        public void CheckAnswer(StudentReseponse studentReseponse){
            bool check = false;
            if(QuestionAnswer(studentReseponse) == StudentAnswer(studentReseponse))
                check = true;
            string sql = $@"UPDATE Race_Reseponse
                        SET check_answer = @check_answer WHERE raceroom_id = @raceroom_id AND question_id = @question_id AND member_id = @member_id ";
            using var conn = new SqlConnection(cnstr);
            conn.QueryFirstOrDefault<string>(sql, new{ check_answer = check , raceroom_id = studentReseponse.raceroom_id, question_id = studentReseponse.question_id, member_id = studentReseponse.member_id });
        }
        #endregion
    }
}
