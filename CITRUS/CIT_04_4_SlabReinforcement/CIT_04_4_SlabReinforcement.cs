using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_4_SlabReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_4_SlabReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            FloorSelectionFilter selFilter = new FloorSelectionFilter();
            IList<Reference> selSlabs = sel.PickObjects(ObjectType.Element, selFilter, "Выберите плиту!");
            if (selSlabs.Count == 0)
            {
                return Result.Succeeded;
            }
            List<Floor> floorList = new List<Floor>();

            foreach (Reference floorRef in selSlabs)
            {
                floorList.Add(doc.GetElement(floorRef) as Floor);
            }
            if (floorList.Count == 0)
            {
                TaskDialog.Show("Revit", "Плита не выбрана!");
                return Result.Cancelled;
            }

            List<AreaReinforcementType> areaReinforcementTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(AreaReinforcementType))
                .Cast<AreaReinforcementType>()
                .ToList();
            if (areaReinforcementTypeList.Count == 0)
            {
                TaskDialog.Show("Revit", "Тип армирования по площади не найден!");
                return Result.Cancelled;
            }
            AreaReinforcementType areaReinforcementType = areaReinforcementTypeList.First();
            List<RebarBarType> bottomXDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование плиты");
                foreach (Floor floor in floorList)
                {
                    double spanDirectionAngle = floor.SpanDirectionAngle;
                    XYZ zeroPoint = new XYZ(0, 0, 0);
                    XYZ directionPointStart = new XYZ(1, 0, 0);
                    Transform rot = Transform.CreateRotationAtPoint(XYZ.BasisZ, spanDirectionAngle, zeroPoint);
                    XYZ directionPoint = rot.OfPoint(directionPointStart);
                    XYZ directionVector = (directionPoint - zeroPoint).Normalize();

                    GeometryElement geomFloorElement = floor.get_Geometry(new Options());
                    Solid floorSolid = null;
                    foreach (GeometryObject geomObj in geomFloorElement)
                    {
                        floorSolid = geomObj as Solid;
                        if (floorSolid != null) break;
                    }
                    FaceArray faceArray = floorSolid.Faces;
                    PlanarFace myFace = null;
                    foreach (PlanarFace pf in faceArray)
                    {
                        if (pf.FaceNormal.Normalize() == XYZ.BasisZ)
                        {
                            myFace = pf;
                            break;
                        }
                    }

                    AreaReinforcement.Create(doc, floor, directionVector, areaReinforcementType.Id, bottomXDirectionRebarTapesList.First().Id, ElementId.InvalidElementId);
                }
                t.Commit();
            }


            return Result.Succeeded;
        }
    }
}
