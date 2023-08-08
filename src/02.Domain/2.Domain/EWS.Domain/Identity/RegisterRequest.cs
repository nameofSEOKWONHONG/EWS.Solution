namespace EWS.Domain.Identity;

public class RegisterRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; }

    public string Notes { get; set; }

    public bool ActivateUser { get; set; } = false;
    
    public bool AutoConfirmEmail { get; set; } = false;
        
    /// <summary>
    /// 부서명
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 직급명
    /// </summary>
    public string LvlName { get; set; }
        
    public string Memo { get; set; }
}