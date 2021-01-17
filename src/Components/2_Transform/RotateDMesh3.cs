﻿using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

using gh3sharp.Core;
using gh3sharp.Core.Goos;
using gh3sharp.Components.Params;

using g3;

namespace gh3sharp.Components.Transform
{
    public class RotateDMesh3 : GH_Component
    {
        public RotateDMesh3()
          : base("Rotate", "Nickname",
            "Rotate DMesh3 description",
            gh3sharpUtil.pluginName, "2_Transform")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param());
            pManager.AddNumberParameter("Rotation Angle", "r", "Rotation angle for DMesh3", GH_ParamAccess.item, 0);
            pManager.AddPlaneParameter("Plane", "p", "Plane to rotate in", GH_ParamAccess.item, Plane.WorldXY);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param());
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DMesh3_goo dMsh_goo = null;
            double rot = 0;
            Plane plane = Plane.WorldXY;

            DA.GetData(0, ref dMsh_goo);
            DA.GetData(1, ref rot);
            DA.GetData(2, ref plane);

            DMesh3 dMsh_copy = new DMesh3(dMsh_goo.Value);

            Quaterniond quat = new Quaterniond(plane.ZAxis.ToVec3d(), rot);
            MeshTransforms.Rotate(dMsh_copy,plane.Origin.ToVec3d(),quat);

            DA.SetData(0, dMsh_copy);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Resource1.rotate;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("2d78a98b-4940-49d7-b1c9-7335e40e9ab8"); }
        }
    }
}
