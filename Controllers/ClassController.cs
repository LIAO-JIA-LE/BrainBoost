using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Services;
using BrainBoost.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost.Controllers
{
    public class ClassController(ClassService _classService) : ControllerBase
    {
        private readonly ClassService classService = _classService;
        [HttpPost]
        [Route("Class")]
        public IActionResult InsertClass(InsertClass insertClass){
            classService.InsertClass(insertClass);
            return Ok();
        }
    }
}