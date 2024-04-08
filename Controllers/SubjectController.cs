using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
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
        [Route("Subject")]
        public List<Subject> GetAllSubject(){
            Member member = MemberService.GetDataByAccount(User.Identity.Name);
            return SubjectService.GetAllSubject(member.Member_Id);
        }

        // 修改科目名字
        [HttpPut]
        [Route("Subjcet")]
        public IActionResult UpdateSubjectName([FromBody]InsertSubject updateData){
            updateData.teacher_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            SubjectService.UpdateSubjectName(updateData);
            return Ok();
        }

        // 新增學生
        [HttpPost]
        [Route("Student")]
        public IActionResult InsertStudent([FromBody] int subject_id, [FromBody]int student_id){
            SubjectService.InsertStudent(subject_id,student_id);
            return Ok();
        }

        // 刪除學生
        [HttpDelete]
        [Route("Student")]
        public IActionResult DeleteStudent([FromBody] int subject_id, [FromBody]int student_id){
            SubjectService.DeleteStudent(subject_id, student_id);
            return Ok();
        }

        // 新增科目
        [HttpPost]
        [Route("Subject")]
        public IActionResult InsertSubject([FromBody]InsertSubject insertData){
            insertData.teacher_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            SubjectService.InsertSubject(insertData);
            return Ok();
        }
        #endregion
    }
}