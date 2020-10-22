using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class StairsReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            Reference selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань!");
            GeometryObject faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            PlanarFace planarFace = faceAsGeoObject as PlanarFace;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точек 1 и 2");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }


            ObjectSnapTypes snapTypes = ObjectSnapTypes.Endpoints | ObjectSnapTypes.Intersections;
            XYZ firstPoint = sel.PickPoint(snapTypes, "Выберите точку 1");
            string strCoords = "Выбранная точка " + firstPoint.ToString();
            TaskDialog.Show("Revit", strCoords);

            XYZ secondPoint = sel.PickPoint(snapTypes, "Выберите точку 2");
            strCoords = "Выбранная точка " + secondPoint.ToString();
            TaskDialog.Show("Revit", strCoords);


            selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань!");
            faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            planarFace = faceAsGeoObject as PlanarFace;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точки 3");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }

            XYZ thirdPoint = sel.PickPoint(snapTypes, "Выберите точку 3");
            strCoords = "Выбранная точка " + thirdPoint.ToString();
            TaskDialog.Show("Revit", strCoords);

            return Result.Succeeded;
        }
    }
}
