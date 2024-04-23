using BrainBoost.Models;

namespace BrainBoost.ViewModels
{
    public class ClassViewModel
    {
        public Class @class {get;set;} = new Class();

        public List<Student> students {get;set;} = [];
    }
    public class Student {
        public Member member{get;set;}
        public Class_Member class_member{get;set;}

        public Student(Member member,Class_Member class_member){
            this.member = member;
            this.class_member = class_member;
        }
    }
}