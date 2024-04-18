using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using BrainBoost.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    public class SubjectController(SubjectService _subjectService, MemberService _memberService) : Controller
    {
        #region 呼叫函式
        readonly SubjectService SubjectService = _subjectService;
        readonly MemberService MemberService = _memberService;
        #endregion

        #region 科目
        // 查看該老師所有的科目
        [HttpGet]
        [Route("AllSubject")]
        public IActionResult GetAllSubject(){
            Response result;
            try{
                Member member = MemberService.GetDataByAccount(User.Identity.Name);
                result = new(){
                    status_code = 200,
                    data = SubjectService.GetAllSubject(member.Member_Id)
                };
            }
            catch (Exception ex) {
                result = new(){
                    status_code = 400,
                    message = ex.Message
                };
            }
            return Ok(result);
        }
        //查詢單個科目
        [HttpGet]
        [Route("Subject")]
        public IActionResult GetSubject(int subject_id){
            Response result;
            try
            {
                Member member = MemberService.GetDataByAccount(User.Identity.Name);
                result = new(){
                    status_code = 200,
                    data = SubjectService.GetSubject(member.Member_Id,subject_id)
                };
            }
            catch (Exception ex)
            {
                result = new(){
                    status_code = 400,
                    message = ex.Message
                };
            }
            return Ok(result);
        }

        // 新增科目
        [HttpPost]
        public IActionResult InsertSubject([FromBody]InsertSubject insertData){
            insertData.teacher_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            Response result;
            try{
                
                result = new(){
                    status_code = 200,
                    message = "新增成功",
                    data = SubjectService.InsertSubject(insertData)
                };
            }
            catch (Exception e){
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        //修改科目名稱
        [HttpPut]
        public IActionResult UpdateSubject([FromQuery]int subject_id,[FromBody]string subject_name){
            Response result;
            try
            {
                Subject subject = new(){
                                    subject_id = subject_id,
                                    subject_name = subject_name,
                                    member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id
                                };
                SubjectService.UpdateSubject(subject);
                result = new(){
                    status_code = 200,
                    message = "修改成功",
                    data = SubjectService.GetSubject(subject.member_id,subject.subject_id)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        //刪除科目
        [HttpDelete]
        public IActionResult DeleteSubject([FromQuery]int subject_id){
            Response result;
            try
            {
                Member member = MemberService.GetDataByAccount(User.Identity.Name);
                if(SubjectService.GetSubject(member.Member_Id,subject_id) != null){
                    SubjectService.DeleteSubject(member.Member_Id,subject_id);
                    result = new(){
                        status_code = 200,
                        message = "刪除成功"
                    };
                }
                else result = new(){
                        status_code = 204,
                        message = "無此資料"
                    };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        #endregion

        #region 針對科目的學生
        // 新增學生
        [HttpPost]
        [Route("Student")]
        public IActionResult InsertStudent([FromQuery]int subject_id,[FromQuery]int student_id){
            Response result;
            try
            {
                SubjectStudent data = new(){
                    subject_id = subject_id,
                    student_id = student_id
                };
                SubjectService.InsertStudent(data);
                result = new(){
                    status_code = 200,
                    message = "新增成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        // 刪除學生
        [HttpDelete]
        [Route("Student")]
        public IActionResult DeleteStudent([FromQuery]int subject_id,[FromQuery]int student_id){
            Response result;
            try
            {
                SubjectStudent data = new(){
                    subject_id = subject_id,
                    student_id = student_id
                };
                SubjectService.DeleteStudent(data);
                result = new(){
                    status_code = 200,
                    message = "刪除成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }
        #endregion
    }
}