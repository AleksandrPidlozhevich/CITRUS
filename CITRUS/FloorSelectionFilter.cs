using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    class FloorSelectionFilter : ISelectionFilter
    {
		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{

			if (elem is Floor)
			{
				return true;
			}
			return false;
		}

		public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
		{
			return false;
		}
	}
}
