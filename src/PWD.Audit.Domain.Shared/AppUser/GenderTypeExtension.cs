using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public static class GenderTypeExtension
    {
        public static string ToText(this GenderType genderType)
        {
            switch (genderType)
            {
                case GenderType.Male:
                    return "Male";
                case GenderType.Female:
                    return "Female";
                case GenderType.TransGender:
                    return "Transgender";
                default:
                    return "";
            }
        }
    }
}
