using Volo.Abp.Emailing;
using Volo.Abp.Settings;

namespace PWD.Audit.Settings
{
    public class AttendanceSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AttendanceSettings.MySetting1));

            context.Add(
                new SettingDefinition("EmailSettings.Smtp.Host", "mail1.pwd.gov.bd"),
                new SettingDefinition("EmailSettings.Smtp.Port", "465"),
                new SettingDefinition("EmailSettings.Smtp.UserName", "recruitment@pwd.gov.bd"),
                new SettingDefinition("EmailSettings.Smtp.Password", "pwd@042022"),
                new SettingDefinition("EmailSettings.Smtp.EnableSsl", "false"),
                new SettingDefinition("EmailSettings.Smtp.UseDefaultCredentials", "false"),
                new SettingDefinition("EmailSettings.Smtp.DefaultFromAddress", "recruitment@pwd.gov.bd"),
                new SettingDefinition("EmailSettings.Smtp.DefaultFromDisplayName", "PWD Attendance")
            );


        }
    }
}
