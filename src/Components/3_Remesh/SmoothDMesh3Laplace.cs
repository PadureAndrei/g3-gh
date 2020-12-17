﻿using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

using gh3sharp.Core;
using gh3sharp.Components.Params;
using gh3sharp.Core.Goos;

using g3;

namespace gh3sharp.Components.Remesh
{
    public class SmoothDMesh3Laplace : GH_Component
    {

        public SmoothDMesh3Laplace()
          : base("Smooth DMesh3 [Laplace]", "laplaceSmooth",
            "SmoothDMesh3Laplace description",
            gh3sharpUtil.pluginName, "3_Operations")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param());
            pManager.AddNumberParameter("Interior Weight", "w", "Weight for interior constraints", GH_ParamAccess.item,1);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param());
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DMesh3_goo dMsh_goo = null;
            double w = 1;

            DA.GetData(0, ref dMsh_goo);
            DA.GetData(1, ref w);

            DMesh3 dMsh_copy = new DMesh3(dMsh_goo.Value, true);
            LaplacianMeshSmoother smoother = new LaplacianMeshSmoother(dMsh_copy);

            foreach (int vid in dMsh_copy.VertexIndices())
            {
                if (smoother.IsConstrained(vid) == false)
                    smoother.SetConstraint(vid, dMsh_copy.GetVertex(vid), w);
            }

            bool success = smoother.SolveAndUpdateMesh();
            bool isValid = dMsh_copy.CheckValidity();

            if (!success)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Laplacian smooth seems to have failed. Please check...");

            if (!isValid)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Mesh seems to have been corrupted during smoothing. Please check...");

            DA.SetData(0, dMsh_copy);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("69fa6a7a-29a4-4288-bae7-cbbf0b03b7dd"); }
        }
    }
}