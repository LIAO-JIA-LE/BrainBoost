using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Services;
using BrainBoost.Parameter;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    public class ClassController(ClassService _classService,MemberService _memberService) : ControllerBase
    {
        private readonly ClassService classService = _classService;
        private readonly MemberService memberService = _memberService;
        #region 班級
        //新增班級
        [HttpPost]
        [Route("")]
        public IActionResult InsertClass([FromBody]InsertClass insertClass){
            try
            {
                insertClass.teacher_id = memberService.GetDataByAccount(User.Identity.Name).Member_Id;
                int class_id = classService.InsertClass(insertClass);
                return Ok(new Response(){
                    status_code = Response.StatusCode,
                    message = "新增成功",
                    data = classService.GetClassViewModel(class_id)
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        //取得班級列表
        [HttpGet]
        [Route("")]
        public IActionResult GetClassList(){
            try
            {
                List<Class> Class_List = classService.GetClassList();
                return Ok(new Response(){
                    status_code = 200,
                    message = "讀取成功",
                    data = Class_List
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        [HttpDelete]
        [Route("")]
        public IActionResult DeleteClass(DeleteClass deleteData){
            try
            {
                classService.DeleteClass(deleteData);
                return Ok(new Response(){
                    status_code = 200,
                    message = "刪除成功"
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        [HttpPut]
        [Route("")]
        public IActionResult UpdateClass(UpdateClass updateData){
            try
            {
                classService.UpdateClass(updateData);
                return Ok(new Response(){
                    status_code = 200,
                    message = "修改成功",
                    data = classService.GetClassViewModel(updateData.class_id)
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }    
        }
        #endregion
        #region 班級學生
        [HttpPost]
        [Route("Student")]
        public IActionResult InsertStudent(ClassStudent insertData){
            try
            {
                classService.InsertStudent(insertData);
                return Ok(new Response(){
                    status_code = 200,
                    message = "新增成功",
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        [HttpDelete]
        [Route("Student")]
        public IActionResult DeleteStudent(ClassStudent deleteData){try
            {
                classService.DeleteStudent(deleteData);
                return Ok(new Response(){
                    status_code = 200,
                    message = "刪除成功",
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion
    }
}