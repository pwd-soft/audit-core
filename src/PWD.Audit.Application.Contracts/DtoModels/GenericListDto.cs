using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Attendance_Swagger.DtoModels
{
    public class GenericListDto<T> where T : class
    {
        public List<T> ListData { get; set; }
        public int CountData { get; set; }

    }
}
