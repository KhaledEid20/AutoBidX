using System;

namespace IdentityService.Pages.Account.Register;

public class ReturnViewModel
{
    public string Email { get; set; }
    public string password { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string returnUrl { get; set; }
    public string Button { get; set; }
}
