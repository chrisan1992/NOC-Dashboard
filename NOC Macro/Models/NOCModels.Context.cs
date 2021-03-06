﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NOC_Macro.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    
    public partial class NOCEntities : DbContext
    {
        public NOCEntities()
            : base("name=NOCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersPermissions> UsersPermissions { get; set; }
        public DbSet<MajorIncidents> MajorIncidents { get; set; }
        public DbSet<Timeline> Timeline { get; set; }
    
        public virtual ObjectResult<Nullable<int>> AutenticateUser(string username, string pass)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passParameter = pass != null ?
                new ObjectParameter("pass", pass) :
                new ObjectParameter("pass", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AutenticateUser", usernameParameter, passParameter);
        }
    
        public virtual int UpdateUserPassword(Nullable<int> userID, string newPassword)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var newPasswordParameter = newPassword != null ?
                new ObjectParameter("NewPassword", newPassword) :
                new ObjectParameter("NewPassword", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateUserPassword", userIDParameter, newPasswordParameter);
        }
    }
}
