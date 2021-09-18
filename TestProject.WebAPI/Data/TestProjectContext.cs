using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TestProject.WebAPI.Data
{
    public class TestProjectContext : IdentityDbContext<User>
    {
        public TestProjectContext(DbContextOptions options)
            : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Seed();
            //builder.ApplyConfiguration(new RoleConfiguration());
            //builder.Entity("AspNetUsers", b =>
            //{
            //    b.Property<int>("UserId")
            //            .HasColumnType("int")
            //            .ValueGeneratedOnAdd();
            //});
                
            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; }
    }

    public class User : IdentityUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public override string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public uint Age { get; set; }
    }

    public class UserDTO //CreateUserForm
    {
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(8, ErrorMessage = "Id cannot be more that 8")]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Required integer value"), MaxLength(100, ErrorMessage = "Email cannot be more that 100")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(15, ErrorMessage = "Password cannot be more that 15")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(30, ErrorMessage = "First Name cannot be more that 15")]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(30, ErrorMessage = "Last Name cannot be more that 30")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(3, ErrorMessage = "Age cannot be more that 3 digit"), Range(1, 120, ErrorMessage = "age should be in between 1 to 120")]
        public int Age { get; set; }
    }

    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class Register
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
