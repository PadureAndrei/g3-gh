﻿using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

using g3gh.Core;
using g3gh.Components.Params;

using g3;
using System.Windows.Forms;
using g3gh.Core.Goos;

using System.IO;
using GH_IO.Serialization;

namespace g3gh.Components.FileIO
{
    public class DMesh3ToFile : GH_Component
    {

        FileType type = FileType.obj;

        public DMesh3ToFile()
          : base("Write To File", "DMshFromObj",
            "Write a DMesh3 object to a file as either an obj, stl or off file.",
            g3ghUtil.pluginName, "9_FileIO")
        {
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (var item in Enum.GetValues(typeof(FileType)))
                Menu_AppendItem(menu, item.ToString(), Menu_PanelTypeChanged, true, item.ToString() == type.ToString()).Tag = "FileMode";

        }

        private void Menu_PanelTypeChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is "FileMode")
                type = (FileType)Enum.Parse(typeof(FileType), item.Text, true);
            this.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param(), "Mesh", "dm3", "Mesh to write as a file", GH_ParamAccess.item);
            pManager.AddTextParameter("Folder Path", "path", "Path to folder where file should be saved.", GH_ParamAccess.item);
            pManager.AddTextParameter("File Name", "name", "File name without extension", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "path", "The file path to which the file was written.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = "." + type.ToString();

            DMesh3_goo goo = null;
            string path = "";
            string file = "";

            DA.GetData(0, ref goo);
            DA.GetData(1, ref path);
            DA.GetData(2, ref file);
            DMesh3 mesh = new DMesh3(goo.Value);


            IOWriteResult result = StandardMeshWriter.WriteFile(Path.Combine(path, file) + "." + type.ToString(), new List<WriteMesh>() { new WriteMesh(mesh) }, WriteOptions.Defaults);
           
            DA.SetData(0, Path.Combine(path,file) + "." + type.ToString());
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("Type", (int)type);

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this.type = (FileType)reader.GetInt32("Type");

            return base.Read(reader);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {

                return Resource1.g3_gh_icons_31;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("eee63ee6-901a-4f0d-a9e3-c34d42ad7778"); }
        }

        public enum FileType
        {
            obj = 0,
            stl = 1,
            off = 2,

        }
    }
}
