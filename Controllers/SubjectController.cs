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
        readonly SubjectService subjectService = _subjectService;
        readonly MemberService memberService = _memberService;

        //查看該老師所有的科目
        [HttpGet]
        [Route("AllSubject")]
        public List<Subject> GetAllSubject(){
            Member member = memberService.GetDataByAccount(User.Identity.Name);
            return subjectService.GetAllSubject(member.Member_Id);
        }

        //查詢單個科目
        [HttpGet]
        [Route("Subject/{subject_id}")]
        public SubjectViewModel GetSubject(int subject_id){
            Member member = memberService.GetDataByAccount(User.Identity.Name);
            return subjectService.GetSubject(member.Member_Id,subject_id);
        }

        //修改科目  
        [HttpPut]
        [Route("Subject/{subject_id}")]
        public IActionResult UpdateSubject(int subject_id,[FromBody]string subject_name){
            Subject subject = new(){
                                    subject_id = subject_id,
                                    subject_name = subject_name,
                                    member_id = memberService.GetDataByAccount(User.Identity.Name).Member_Id
                                };
            subjectService.UpdateSubject(subject);
            return Ok();
        }

        //新增科目
        [HttpPost]
        [Route("Subject")]
        public IActionResult InsertSubject([FromBody]InsertSubject insertData){
            insertData.teacher_id = memberService.GetDataByAccount(User.Identity.Name).Member_Id;
            subjectService.InsertSubject(insertData);
            return Ok();
        }
        [HttpDelete]
        [Route("Subject")]
        public IActionResult DeleteSubject([FromQuery]int subject_id){
            Member member = memberService.GetDataByAccount(User.Identity.Name);
            if(subjectService.GetSubject(member.Member_Id,subject_id) != null){
                subjectService.DeleteSubject(member.Member_Id,subject_id);
                return Ok();
            }
            else return BadRequest();
        }
    }
}