using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Services;
using BrainBoost.Parameter;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Teacher,Manager,Admin")]
        public IActionResult InsertClass([FromBody]InsertClass insertClass){
            try
            {
                //是否有登入
                if(User.Identity.Name == null)
                    return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                //是否有班級存在
                if(classService.CheckClass(insertClass))
                    return BadRequest(new Response(){status_code = 400, message = "該班級已存在"});
                //學生都有在資料庫中
                if(classService.CheckStudent(insertClass.List_student_id) == insertClass.List_student_id.Count){
                    insertClass.teacher_id = memberService.GetDataByAccount(User.Identity.Name).Member_Id;
                    int class_id = classService.InsertClass(insertClass);
                    return Ok(new Response(){
                        status_code = Response.StatusCode,
                        message = "新增成功",
                        data = classService.GetClassViewModel(class_id)
                    });
                }
                return BadRequest(new Response(){status_code = 400, message = "有學生不存在，請重新輸入"});
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
        public IActionResult GetClassList([FromQuery]int class_id){
            try
            {
                // 取得班級詳細學生
                if(class_id>0){
                    return Ok(new Response(){
                        status_code = 200,
                        message = @"讀取成功,班級詳細資訊",
                        data = classService.GetClassViewModel(class_id)
                    });
                }
                // 取得所有班級
                else{
                    List<Class> Class_List = classService.GetClassList();
                    return Ok(new Response(){
                        status_code = 200,
                        message = @"讀取成功,所有班級",
                        data = Class_List
                    });
                }
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        //刪除班級
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
        //修改班級資訊
        [HttpPut]
        [Route("")]
        public IActionResult UpdateClass([FromBody]UpdateClass updateData){
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
        public IActionResult InsertStudent([FromBody]ClassStudent insertData){
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