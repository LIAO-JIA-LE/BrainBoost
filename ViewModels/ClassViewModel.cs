using BrainBoost.Models;

namespace BrainBoost.ViewModels
{
    public class ClassViewModel
    {
        public Class @class {get;set;} = new Class();

        public List<Class_Member> class_member {get;set;} = [];
    }
}