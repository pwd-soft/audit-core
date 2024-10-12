using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Pic { get; set; }
        public int[] Roles { get; set; }
        public string Occupation { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public AddressModel Address { get; set; }
        public SocialNetworksModel SocialNetworks { get; set; }

        // Personal information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Website { get; set; }

        // Account information
        public string Language { get; set; }
        public string TimeZone { get; set; }

        // email settings
        public EmailSettings EmailSettings { get; set; }

        //public AuthModel AuthModel { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }

        public bool IsActive { get; set; }

        public int UserStatus { get; set; }

        public int UserType { get; set; }

        public UserModel()
        {
            this.Roles = new int[1];
            //this.AuthModel = new AuthModel();
            this.EmailSettings = new EmailSettings();
            this.Address = new AddressModel();
            this.SocialNetworks = new SocialNetworksModel();
            this.FullName = $"{ this.FirstName} { this.LastName}";

            this.AuthToken = "auth-token-8f3ae836da744329a6f93bf20594b5cc";
            this.RefreshToken = "auth-token-f8c137a2c98743f48b643e71161d90aa";
            this.ExpiresIn = DateTime.Now.AddHours(23.0);
            this.Gender = GenderType.Male.ToText();
            this.IsActive = true;
            this.UserStatus = 1;
            this.UserType = 1;
        }
    }
}