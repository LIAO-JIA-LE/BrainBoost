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
        public List<Subject> GetAllSubject(){
            Member member = MemberService.GetDataByAccount(User.Identity.Name);
            return SubjectService.GetAllSubject(member.Member_Id);
        }
        //查詢單個科目
        [HttpGet]
        [Route("Subject")]
        public SubjectViewModel GetSubject(int subject_id){
            Member member = MemberService.GetDataByAccount(User.Identity.Name);
            return SubjectService.GetSubject(member.Member_Id,subject_id);
        }

        // 新增科目
        [HttpPost]
        public JsonResult InsertSubject([FromBody]InsertSubject insertData){
            insertData.teacher_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            Response result;
            try{
                SubjectService.InsertSubject(insertData);
                result = new(){
                    status_code = 200,
                    message = "新增成功"
                };
            }
            catch (Exception e){
                result = new(){
                    status_code = 500,
                    message = e.Message
                };
            }
            return Json(result);
        }

        //修改科目名稱
        [HttpPut]
        public IActionResult UpdateSubject([FromQuery]int subject_id,[FromBody]string subject_name){
            Subject subject = new(){
                                    subject_id = subject_id,
                                    subject_name = subject_name,
                                    member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id
                                };
            SubjectService.UpdateSubject(subject);
            return Ok();
        }

        //刪除科目
        [HttpDelete]
        public IActionResult DeleteSubject([FromQuery]int subject_id){
            Member member = MemberService.GetDataByAccount(User.Identity.Name);
            if(SubjectService.GetSubject(member.Member_Id,subject_id) != null){
                SubjectService.DeleteSubject(member.Member_Id,subject_id);
                return Ok();
            }
            else return BadRequest();
        }

        #endregion

        #region 針對科目的學生
        // 新增學生
        [HttpPost]
        [Route("Student")]
        public IActionResult InsertStudent([FromQuery]int subject_id,[FromQuery]int student_id){
            SubjectStudent data = new(){
                subjecct_id = subject_id,
                student_id = student_id
            };
            SubjectService.InsertStudent(data);
            return Ok();
        }

        // 刪除學生
        [HttpDelete]
        [Route("Student")]
        public IActionResult DeleteStudent([FromQuery]int subject_id,[FromQuery]int student_id){
            SubjectStudent data = new(){
                subjecct_id = subject_id,
                student_id = student_id
            };
            SubjectService.DeleteStudent(data);
            return Ok();
        }
        #endregion
    }
}