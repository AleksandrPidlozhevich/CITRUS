using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class StairFlightReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

#region Старт блока выбора форм арматурных стержней
            //Выбор формы основной арматуры если стыковка на сварке
            List<RebarShape> rebarStraightShapeMainRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarStraightShapeMainRodsList.Count == 0)
            {
                rebarStraightShapeMainRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_1")
                .Cast<RebarShape>()
                .ToList();
                if (rebarStraightShapeMainRodsList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarStraightShapeMainRods = rebarStraightShapeMainRodsList.First();

            //Выбор формы основной арматуры если загиб в плиту 
            List<RebarShape> rebarLShapeMainRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (rebarLShapeMainRodsList.Count == 0)
            {
                rebarLShapeMainRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (rebarLShapeMainRodsList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarLShapeMainRods = rebarLShapeMainRodsList.First();
#endregion

            //Выбор лестничного марша
            Reference selStairFlightRef = sel.PickObject(ObjectType.Element, "Выберите лестничный марш!");
            Element selStairFlight = doc.GetElement(selStairFlightRef);

            //Выбор грани для первых двух точек
            Reference selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань!");
            GeometryObject faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            PlanarFace planarFace = faceAsGeoObject as PlanarFace;

            //Задание рабочей плоскости для выбора точек 1 и 2
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точек 1 и 2");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }

            //Выбор точки 1
            ObjectSnapTypes snapTypes = ObjectSnapTypes.Endpoints | ObjectSnapTypes.Intersections;
            XYZ firstPoint = sel.PickPoint(snapTypes, "Выберите точку 1");
            string strCoords = "Выбранная точка " + firstPoint.ToString();
            TaskDialog.Show("Revit", strCoords);
            //Выбор точки 2
            XYZ secondPoint = sel.PickPoint(snapTypes, "Выберите точку 2");
            strCoords = "Выбранная точка " + secondPoint.ToString();
            TaskDialog.Show("Revit", strCoords);

            //Выбор грани для третьей точки
            selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань!");
            faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            planarFace = faceAsGeoObject as PlanarFace;
            //Задание рабочей плоскости для выбора точки 3
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точки 3");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }
            //Выбор точки 3
            XYZ thirdPoint = sel.PickPoint(snapTypes, "Выберите точку 3");
            strCoords = "Выбранная точка " + thirdPoint.ToString();
            TaskDialog.Show("Revit", strCoords);

            XYZ horisontalСcrossVector = (secondPoint - firstPoint).Normalize();
            XYZ horisontalLongitudinalVector = (new XYZ (thirdPoint.X, thirdPoint.Y, 0) - new XYZ (firstPoint.X, firstPoint.Y, 0)).Normalize();
            XYZ mainLongitudinalVector = (thirdPoint - firstPoint).Normalize();

            //Список типов для выбора основной арматуры
            RebarBarType stepMainRebarType = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList().First();


            //Армирование ступеней
            //Точки для построения кривфх стержня
            XYZ rebar_p1 = firstPoint - (25/308.4)*XYZ.BasisZ - (25/308.4) * horisontalLongitudinalVector;
            XYZ rebar_p2 = rebar_p1 -  (300/308.4) * horisontalLongitudinalVector;
            XYZ rebar_p3 = rebar_p1 - (300/304.8) * XYZ.BasisZ;

            //Кривые стержня
            List<Curve> stepRebarCurves = new List<Curve>();

            Curve line1 = Line.CreateBound(rebar_p2, rebar_p1) as Curve;
            stepRebarCurves.Add(line1);
            Curve line2 = Line.CreateBound(rebar_p1, rebar_p3) as Curve;
            stepRebarCurves.Add(line2);
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Создание стержней");
                //Стержень ступени
                Rebar stepMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                rebarLShapeMainRods,
                stepMainRebarType,
                null,
                null,
                selStairFlight,
                horisontalСcrossVector,
                stepRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);




                uiDoc.ActiveView.HideActiveWorkPlane();
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
