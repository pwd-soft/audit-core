using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class AuthModel
    {
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }

        public AuthModel()
        {
            this.AuthToken = "auth-token-8f3ae836da744329a6f93bf20594b5cc";
            this.RefreshToken = "auth-token-f8c137a2c98743f48b643e71161d90aa";
            this.ExpiresIn = DateTime.Now.AddHours(23.0);
        }
    }
}
