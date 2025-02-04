﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

using g3gh.Core;
using g3gh.Core.Goos;
using g3gh.Components.Params;

using g3;
using Grasshopper.GUI;
using GH_IO.Serialization;

namespace g3gh.Components.MarchingCubes
{
    

    public class MarchingCubesIso : GH_Component
    {
        g3.MarchingCubes.RootfindingModes modes = g3.MarchingCubes.RootfindingModes.LerpSteps;
        int steps = 5;

        public MarchingCubesIso()
          : base("Marching Cubes Iso Surface", "isoSrf",
            "Construct marching cubes iso surface from a grid by interpolating through a specific value.",
            g3ghUtil.pluginName, "5_MarchingCubes")
        {
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (var item in Enum.GetValues(typeof(g3.MarchingCubes.RootfindingModes)))
                Menu_AppendItem(menu, item.ToString(), Menu_PanelTypeChanged, true, item.ToString() == modes.ToString()).Tag = "RootMode";

            Menu_AppendSeparator(menu);

            GH_DocumentObject.Menu_AppendTextItem(menu, "Root Mode Steps", null, null, false, 0, false);
            var maxAngleTST = GH_DocumentObject.Menu_AppendTextItem(menu, steps.ToString(), null, Menu_SetSteps, false); ;
            maxAngleTST.ToolTipText = "Root Mode Steps";

            Menu_AppendSeparator(menu);

            menu.Closed += contextMenuStrip_Closing;
        }

        private void Menu_SetSteps(GH_MenuTextBox sender, string text)
        {
            try
            {
                steps = int.Parse(text);
            }
            catch (Exception)
            {
                steps = 5;
            }
        }

        private void Menu_PanelTypeChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is "RootMode")
                modes = (g3.MarchingCubes.RootfindingModes)Enum.Parse(typeof(g3.MarchingCubes.RootfindingModes), item.Text, true);
            this.ExpireSolution(true);
        }

        private void contextMenuStrip_Closing(object sender, EventArgs e)
        {
            this.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Grid3f_Param(), "Grid", "g3f", "Basis grid for interpolation.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Value", "val", "Value for iso surface", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Expansion", "e", "Expansion of grid beyond size of mesh", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param(),"Iso Mesh","dm3","Resulting Iso mesh from the Marching Cubes process",GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = modes.ToString() + " : " + steps.ToString() + " Steps";

            Grid3f_goo goo = null;
            double val = 0;
            double exp = 0;

            DA.GetData(0, ref goo);
            DA.GetData(1, ref val);
            DA.GetData(2, ref exp);

            var iso = goo.Value;

            g3.MarchingCubes c = new g3.MarchingCubes();
            c.Implicit = iso;
            c.Bounds = iso.Bounds();
            c.RootMode = modes;
            c.RootModeSteps = steps;
            c.CubeSize = iso.CellSize;
            c.Bounds.Expand(3 * c.CubeSize);
            c.IsoValue = val;
            c.Generate();

            
            DMesh3 outputMesh = c.Mesh;
            bool isValid = outputMesh.CheckValidity();

            if(!isValid)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Mesh seems to be corrupted. Please check...");

            DA.SetData(0,outputMesh);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("Modes", (int)modes);
            writer.SetInt32("steps", this.steps);

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this.modes = (g3.MarchingCubes.RootfindingModes)reader.GetInt32("Modes");
            this.steps = reader.GetInt32("steps");

            return base.Read(reader);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Resource1.g3_gh_icons_15;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0a86bc8e-8671-451d-ab32-721184b90bf0"); }
        }
    }
}
