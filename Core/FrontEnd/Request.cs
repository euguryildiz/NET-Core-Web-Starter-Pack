using System;
using System.Collections.Generic;

namespace Core.FrontEnd
{
    public class Request
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public string FileName { get; set; }
        public List<RequestColumn> Columns { get; set; }
        public List<RequestFilter> Filters { get; set; }
    }

    public class RequestColumn
    {
        public string Field { get; set; }
        public string Order { get; set; }
    }

    public class RequestFilter
    {
        public string Field { get; set; }
        public string Operant { get; set; }
        public string Value { get; set; }
    }
}

