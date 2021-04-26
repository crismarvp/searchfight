using System;
using System.Collections.Generic;
using System.Text;

namespace searchfight2.Entities
{
    public class GoogleData
    {
        public Queries queries { get; set; }
    }
    public class Request
    {
        public string totalResults { get; set; }
    }

    public class Queries
    {
        public List<Request> request { get; set; }
    }




}
