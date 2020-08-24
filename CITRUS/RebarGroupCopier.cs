﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class RebarGroupCopier : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			//Получение текущего документа
			Document doc = commandData.Application.ActiveUIDocument.Document;

			//Получение доступа к Selection
			Selection sel = commandData.Application.ActiveUIDocument.Selection;

			//Выбор группы арматуры
			GroupSelectionFilter selFilter = new GroupSelectionFilter(); //Вызов фильтра выбора
			List<Reference> selGroupList = sel.PickObjects(ObjectType.Element, selFilter, "Выберите группу!").ToList();//Получение ссылки на выбранную группу
			List<Group> myGroupList = new List<Group>();
			foreach (Reference refer in selGroupList)
            {
				myGroupList.Add(doc.GetElement(refer) as Group);
			}
			using (Transaction t = new Transaction(doc))
			{
				t.Start("Копирование групп арматуры");

				foreach (Group myGroup in myGroupList)
				{
				List<ElementId> myElementsInGroup = myGroup.GetDependentElements(null).ToList();
				Element myFirstElementInGroup = doc.GetElement(myElementsInGroup.First()) as Element;
				string rebarElemHostMarkParam = myFirstElementInGroup.get_Parameter(BuiltInParameter.REBAR_ELEM_HOST_MARK).AsString();

				Rebar myFirstElementInGroupAsRebar = doc.GetElement(myElementsInGroup.First()) as Rebar;
				ElementId myRebarHostElementId = myFirstElementInGroupAsRebar.GetHostId();
				FamilyInstance myRebarHostElement = doc.GetElement(myRebarHostElementId) as FamilyInstance;
				LocationPoint rebarHostElementLocation = myRebarHostElement.Location as LocationPoint;
				XYZ rebarHostElementLocationXYZ = rebarHostElementLocation.Point;

				List<FamilyInstance> columnsOfMyRebarHostMark = new FilteredElementCollector(doc)
					.OfClass(typeof(FamilyInstance))
					.OfCategory(BuiltInCategory.OST_StructuralColumns)
					.Where(fi => fi.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == rebarElemHostMarkParam & fi.Id != myRebarHostElementId)
					.Cast<FamilyInstance>()
					.ToList();
					foreach (FamilyInstance column in columnsOfMyRebarHostMark)
					{
						LocationPoint columnLocation = column.Location as LocationPoint;
						XYZ columnLocationXYZ = columnLocation.Point;
						XYZ vectorForGroupCopy = new XYZ(columnLocationXYZ.X - rebarHostElementLocationXYZ.X, columnLocationXYZ.Y - rebarHostElementLocationXYZ.Y, columnLocationXYZ.Z - rebarHostElementLocationXYZ.Z);

						List<ElementId> newGroupElementIdList = ElementTransformUtils.CopyElement(doc, myGroup.Id, vectorForGroupCopy) as List<ElementId>;
						ElementId newGroupElementId = newGroupElementIdList.First();

						//Ось вращения
						XYZ rotationPoint1 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z);
						XYZ rotationPoint2 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z + 1);
						Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);

						if (columnLocation.Rotation != 0)
						{
							ElementTransformUtils.RotateElement(doc, newGroupElementId, rotationAxis, columnLocation.Rotation);
						}
					}
				}
				t.Commit();
			}

			return Result.Succeeded;
        }
    }
}
