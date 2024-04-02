using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost.Controllers
{
    [Route("[controller]")]
    public class SubjectController(SubjectService _subjectService,MemberService _memberService) : Controller
    {
        readonly SubjectService subjectService = _subjectService;
        readonly MemberService memberService = _memberService;

        //查看該老師所有的科目
        [HttpGet]
        [Route("Subject")]
        public List<Subject> GetAllSubject(){
            Member member = memberService.GetDataByAccount(User.Identity.Name);
            return subjectService.GetAllSubject(member.Member_Id);
        }

        //修改科目
        [HttpPut]
        [Route("Subjcet")]
        public IActionResult UpdateSubject(){
            return Ok();
        }

        //新增科目
        // [HttpPost]
        // [Route("Subject")]
        // public IActionResult InsertSubject([FromBody]InsertSubject Data){

        // }
    }
}