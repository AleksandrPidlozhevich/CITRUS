using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;


namespace CITRUS
{
    class ColumnSelectionFilter : ISelectionFilter
    {
		
		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{

			if (elem is FamilyInstance)
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
