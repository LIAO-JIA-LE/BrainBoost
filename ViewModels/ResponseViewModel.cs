using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.ViewModels
{
    public class ResponseViewModel<T>
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public T data {get;set;}
    }
    public class ResponseViewModel
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public object data {get;set;}
    }
}