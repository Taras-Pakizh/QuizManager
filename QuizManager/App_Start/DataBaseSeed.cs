using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QuizManager.DBModels;
using QuizManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace QuizManager.App_Start
{
    public class DataBaseSeed : CreateDatabaseIfNotExists<QuizContext>
    {
        protected override void Seed(QuizContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var roleList = new List<IdentityRole>()
            {
                new IdentityRole { Name = Role.Admin.ToString() },
                new IdentityRole { Name = Role.Student.ToString() },
            };

            foreach(var role in roleList)
            {
                roleManager.Create(role);
            }

            base.Seed(context);
        }
    }
}