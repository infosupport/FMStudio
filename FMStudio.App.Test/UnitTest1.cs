﻿using FMStudio.App.Test.Screens;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestStack.White;
using TestStack.White.UIItems.Finders;

namespace FMStudio.App.Test
{
    
    public class UnitTest1
    {
        public MainWindowScreen MainWindow { get; private set; }
        
        public UnitTest1()
        {
            MainWindow = new MainWindowScreen();
        }

        //[Fact]
        public void Cleanup()
        {
            MainWindow.Dispose();
        }

        //[Fact]
        public void AddProject()
        {
            MainWindow
                .ClickAddProject()
                .ProjectTreeScreen()
                    .WithProject("New project")
                    .VerifyExists()
            ;
        }

        //#region Proto

        //[Fact]
        //public void TestMethod1()
        //{
        //    var path = Path.GetFullPath("../../FMStudio.App/bin/FMStudio.exe");
        //    using (_app = Application.Launch(path))
        //    {
        //        var window = _app.GetWindows().FirstOrDefault();

        //        var p1 = GetProjects();

        //        var btnAddProject = window.Get(SearchCriteria.ByAutomationId("btnAddProject"));
        //        btnAddProject.Click();

        //        var p2 = GetProjects();
        //    }
        //}

        //private List<ProjectVM> GetProjects()
        //{
        //    var window = _app.GetWindows().FirstOrDefault();
        //    var projects = window.GetMultiple(SearchCriteria.ByClassName("ProjectViewer"));

        //    var result = new List<ProjectVM>();

        //    foreach(var p in projects)
        //    {
        //        var lblProjectName = p.GetElement(SearchCriteria.ByAutomationId("lblProjectName"));
                
        //        if (lblProjectName != null)
        //        {
        //            var currentLabel = lblProjectName.Current;

        //            var vm = new ProjectVM()
        //            {
        //                Id = p.Id,
        //                Name = currentLabel.Name
        //            };

        //            result.Add(vm);
        //        }
        //    }

        //    return result;
        //}

        //#endregion
    }

    //public class ProjectVM
    //{
    //    public string Id { get; set; }

    //    public string Name { get; set; }
    //}
}