﻿using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

using g3gh.Core;
using g3gh.Core.Goos;
using g3gh.Components.Params;

using g3;

namespace g3gh.Components.Transform
{
    public class MoveDMesh3 : GH_Component
    {

        public MoveDMesh3()
          : base("Translate", "transDMesh3",
            "Translate a DMesh3 along a translation vector",
            g3ghUtil.pluginName, "3_Transform")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param(), "Mesh", "dm3", "Mesh to translate", GH_ParamAccess.item);
            pManager.AddVectorParameter("Translation Vector", "v", "Translation vector for DMesh3", GH_ParamAccess.item, new Rhino.Geometry.Vector3d(0, 0, 0));
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new DMesh3_Param(), "Mesh", "dm3", "Translated mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DMesh3_goo dMsh_goo = null;
            Rhino.Geometry.Vector3d vec = new Rhino.Geometry.Vector3d(0,0,0);

            DA.GetData(0, ref dMsh_goo);
            DA.GetData(1, ref vec);

            DMesh3 dMsh_copy = new DMesh3(dMsh_goo.Value);
            MeshTransforms.Translate(dMsh_copy, vec.ToVec3d());

            DA.SetData(0, dMsh_copy);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Resource1.g3_gh_icons_10;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f1968b8e-de03-4e61-9695-7782372ba100"); }
        }
    }
}
