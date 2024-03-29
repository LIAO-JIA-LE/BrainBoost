using System.Data.SqlClient;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;

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
        public void Room(string Code, InsertRoom raceData){
            string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            // 新增搶答室資訊
            string sql = $@"INSERT INTO RaceRoom(member_id,race_name, race_date, race_code, race_function, time_limit)
                            VALUES(@member_id,@race_name, @race_date, @race_code, @race_function, @time_limit) ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {member_id = raceData.member_id,race_name = raceData.race_name, race_date = currentTime, race_code = Code, race_function = raceData.race_function, time_limit = raceData.time_limit});
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

        #region 修改搶答室資訊
        public void RoomInformation(int id, RaceInformation raceData){
            // 新增搶答室資訊
            string sql = $@"UPDATE RaceRoom SET race_name = @race_name, race_function = @race_function,
                            time_limit = @time_limit WHERE raceroom_id = {id} ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {race_name = raceData.race_name, race_function = raceData.race_function , time_limit = raceData.time_limit});
        }
        #endregion

        #region 新增搶答室題目
        public void InsertQuestion(int id, List<int> question_id_list){
            StringBuilder stringBuilder = new StringBuilder();
            // 新增題目
            for(int i = 0; i < question_id_list.Count; i++){
                stringBuilder.Append($@"INSERT INTO Race_Question(raceroom_id, question_id)
                                        VALUES('{id}', '{question_id_list[i]}') ");
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
        }
        #endregion

        #region 刪除搶答室題目
        public void DeleteQuestion(int id, List<int> question_id_list){
            StringBuilder stringBuilder = new StringBuilder();
            // 刪除題目
            for(int i = 0; i < question_id_list.Count; i++){
                stringBuilder.Append($@"UPDATE Race_Question SET is_delete = 1 
                                        WHERE raceroom_id = {id} AND question_id = {question_id_list[i]}");
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
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
        public List<RaceQuestionAnswer> Question(int id,int question_id){
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
                            WHERE raceroom_id = {id} AND A.question_id = {question_id} AND R.is_delete = 0";
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
                stringBuilder.Replace("1=1", $"1=1 AND Q.subject_id = @subject_id");
            if(Search.type_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.type_id = @type_id");
            if(Search.tag_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND T.tag_id = @tag_id");
            if(Search.question_level != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_level = @question_level");
            if(Search.search != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_content = '@question_content'");
            #endregion
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql, new{subject_id = Search.subject_id, type_id = Search.type_id, tag_id = Search.tag_id, question_level = Search.question_level, question_content = Search.search, member_id = Search.member_id});
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
                stringBuilder.Replace("1=1", $"1=1 AND Q.subject_id = @subject_id");
            if(Search.type_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.type_id = @type_id");
            if(Search.tag_id != null)
                stringBuilder.Replace("1=1", $"1=1 AND T.tag_id = @tag_id");
            if(Search.question_level != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_level = @question_level");
            if(Search.search != null)
                stringBuilder.Replace("1=1", $"1=1 AND Q.question_content = '@question_content'");
            #endregion

            using (var conn = new SqlConnection(cnstr))
            return (List<SimpleQuestion>)conn.Query<SimpleQuestion>(stringBuilder.ToString(), new{subject_id = Search.subject_id, type_id = Search.type_id, tag_id = Search.tag_id, question_level = Search.question_level, question_content = Search.search, member_id = Search.member_id});
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
    }
}
