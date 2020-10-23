using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_5_StairFlightReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение доступа к UI
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

#region Старт блока выбора форм арматурных стержней
            //Выбор формы прямых стержней
            List<RebarShape> rebarStraightShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarStraightShapeList.Count == 0)
            {
                rebarStraightShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_1")
                .Cast<RebarShape>()
                .ToList();
                if (rebarStraightShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarStraightShape = rebarStraightShapeList.First();

            //Выбор формы Г-образных стержней
            List<RebarShape> rebarLShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (rebarLShapeList.Count == 0)
            {
                rebarLShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (rebarLShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarLShape = rebarLShapeList.First();
#endregion

            //Выбор лестничного марша
            Reference selStairFlightRef = sel.PickObject(ObjectType.Element, "Выберите лестничный марш");
            Element stairFlight = doc.GetElement(selStairFlightRef);

            //Выбор грани для первых двух точек
            Reference selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань");
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
            //Выбор точки 2
            XYZ secondPoint = sel.PickPoint(snapTypes, "Выберите точку 2");

            //Выбор грани для третьей точки
            selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань");
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
            
            //Получение основных векторов
            XYZ horisontalСrossVector = (secondPoint - firstPoint).Normalize();
            XYZ horisontalLongitudinalVector = (new XYZ(thirdPoint.X, thirdPoint.Y, 0) - new XYZ(firstPoint.X, firstPoint.Y, 0)).Normalize();
            XYZ mainLongitudinalVector = (thirdPoint - firstPoint).Normalize();

            //Список типов для выбора арматуры ступеней
            List<RebarBarType> stepRebarTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Вызов формы
            CIT_04_5_StairFlightReinforcementForm stairFlightReinforcementForm = new CIT_04_5_StairFlightReinforcementForm(stepRebarTypeList);
            stairFlightReinforcementForm.ShowDialog();
            if (stairFlightReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Получение типа арматуры ступеней
            RebarBarType stepRebarType = stairFlightReinforcementForm.mySelectionStepRebarType;
            //Диаметр стержня арматуры ступеней
            Parameter stepRebarTypeDiamParam = stepRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stepRebarDiam = stepRebarTypeDiamParam.AsDouble();


            //Защитный слой арматуры ступеней
            double stepRebarCoverLayer = stairFlightReinforcementForm.StepRebarCoverLayer/304.8;

            //Гипотенуза длинны и высоты ступени
            double stepsHypotenuse = Math.Sqrt(Math.Pow(300/304.8, 2) + Math.Pow(150 / 304.8, 2));

            //Расчет длины вертикального участка стержня ступени
            //Угол наклона лестницы к вертикальной оси
            double angle = mainLongitudinalVector.AngleTo(XYZ.BasisZ.Negate()) * (180 / Math.PI);
            //Толщина плиты лестничного марша по вертикали
            double verticalStairThickness = (200 / Math.Cos((90 - angle) * (Math.PI / 180)))/304.8;
            //Толщина защитного слоя плиты лестничного марша по вертикали
            double verticalStairCoverLayer = (25 / Math.Cos((90 - angle) * (Math.PI / 180))) / 304.8;
            //Дополнительное смещение
            double verticalAdditionalOffset = (stepRebarCoverLayer + stepRebarDiam / 2) * Math.Tan((90 - angle) * (Math.PI / 180));
            //Длина вертикального участка арматуры ступени
            double stepRebarVerticalLength = RoundUpToFive(((150 / 304.8 - stepRebarCoverLayer - stepRebarDiam) + (verticalStairThickness - verticalAdditionalOffset - verticalStairCoverLayer))*304.8)/304.8 - stepRebarDiam/2;

            //Расчет длины горизонтального участка стержня ступени
            //Толщина плиты лестничного марша по горизонтали
            double horizontalStairThickness = (200 / Math.Sin((90 - angle) * (Math.PI / 180))) / 304.8;
            //Толщина защитного слоя плиты лестничного марша по горизонтали
            double horizontalStairCoverLayer = (25 / Math.Sin((90 - angle) * (Math.PI / 180))) / 304.8;
            //Дополнительное смещение
            double horizontalAdditionalOffset = (stepRebarCoverLayer + stepRebarDiam / 2) / Math.Tan((90 - angle) * (Math.PI / 180));
            double stepRebarHorizontalLength = RoundUpToFive(((300/304.8 - stepRebarCoverLayer - stepRebarDiam) + (horizontalStairThickness - horizontalStairCoverLayer - horizontalAdditionalOffset))*304.8)/304.8 - stepRebarDiam/2;

            // Ширина лестницы
            double stairFlightWidth = firstPoint.DistanceTo(secondPoint);
            //Ширина установки арматуры ступеней за вычетом защитных слоев и диаметров арматуры ступеней
            double stepRebarPlacementWidth = (stairFlightWidth - stepRebarCoverLayer*2 - stepRebarDiam) * 304.8;
            //Колличество стержней для ступени
            int stepBarsQuantity = (int)(stepRebarPlacementWidth / 100);
            //Остаток ширины при размещении стержней для ступени
            double remainderWidth = (stepRebarPlacementWidth - stepBarsQuantity * 100) / 304.8;
                      


            //Армирование ступеней стартовый стержень
            //Точки для построения кривых Г-стержня
            XYZ startStepLRebar_p1 = firstPoint 
                + stepsHypotenuse * mainLongitudinalVector 
                - (stepRebarCoverLayer + stepRebarDiam / 2) * XYZ.BasisZ 
                - (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalLongitudinalVector 
                + (stepRebarCoverLayer + stepRebarDiam/2) * horisontalСrossVector;

            XYZ startStepLRebar_p2 = startStepLRebar_p1 - stepRebarHorizontalLength * horisontalLongitudinalVector;
            XYZ startStepLRebar_p3 = startStepLRebar_p1 - stepRebarVerticalLength * XYZ.BasisZ;

            //Точки для построения кривых  прямого стержня
            XYZ startStepStraightRebar_p1 = firstPoint
                + stepsHypotenuse * mainLongitudinalVector
                - (stepRebarCoverLayer + stepRebarDiam) * XYZ.BasisZ
                - (stepRebarCoverLayer + stepRebarDiam / 2 + stepRebarDiam) * horisontalLongitudinalVector
                + (10/304.8) * horisontalСrossVector;

            XYZ startStepStraightRebar_p2 = startStepStraightRebar_p1 + ((stairFlightWidth - (20/304.8)) * horisontalСrossVector);

            //Кривые Г-стержня
            List<Curve> startStepLRebarCurves = new List<Curve>();

            Curve startStepLRebar_Line1 = Line.CreateBound(startStepLRebar_p2, startStepLRebar_p1) as Curve;
            startStepLRebarCurves.Add(startStepLRebar_Line1);
            Curve startStepLRebar_Line2 = Line.CreateBound(startStepLRebar_p1, startStepLRebar_p3) as Curve;
            startStepLRebarCurves.Add(startStepLRebar_Line2);

            //Кривые прямого стержня
            List<Curve> startStepStraightRebarCurves = new List<Curve>();

            Curve startStepStraightRebar_Line1 = Line.CreateBound(startStepStraightRebar_p1, startStepStraightRebar_p2) as Curve;
            startStepStraightRebarCurves.Add(startStepStraightRebar_Line1);

            using (Transaction t = new Transaction(doc))
            {
                //Универсальная коллекция для формирования группы ступени
                ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                t.Start("Создание стержней");
                //Г-стержень ступени
                Rebar startStepLRebar = Rebar.CreateFromCurvesAndShape(doc,
                rebarLShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                horisontalСrossVector,
                startStepLRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);
                startStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(1);
                rebarIdCollection.Add(startStepLRebar.Id);

                List<ElementId> endStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startStepLRebar.Id, (stairFlightWidth - (stepRebarCoverLayer*2 + stepRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                Element endStepLRebar = doc.GetElement(endStepLRebarIdList.First());
                endStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                rebarIdCollection.Add(endStepLRebar.Id);

                List<ElementId> middleStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startStepLRebar.Id, (remainderWidth / 2 + 100 / 304.8) * horisontalСrossVector) as List<ElementId>;
                Element middleStepLRebar = doc.GetElement(middleStepLRebarIdList.First());
                
                middleStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                middleStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stepBarsQuantity - 1);
                middleStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                middleStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                rebarIdCollection.Add(middleStepLRebar.Id);

                //Прямой стержень ступени по вертикали
                Rebar startStepStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                rebarStraightShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                XYZ.BasisZ,
                startStepStraightRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);

                ElementTransformUtils.MoveElement(doc, startStepStraightRebar_1.Id, (-10 / 304.8) * XYZ.BasisZ);
                startStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                startStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                startStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                startStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                rebarIdCollection.Add(startStepStraightRebar_1.Id);

                //Прямой стержень ступени по горизонтали
                Rebar startStepStraightRebar_2 = Rebar.CreateFromCurvesAndShape(doc,
                rebarStraightShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                horisontalLongitudinalVector,
                startStepStraightRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);

                ElementTransformUtils.MoveElement(doc, startStepStraightRebar_2.Id, (-90 / 304.8) * horisontalLongitudinalVector - (stepRebarDiam/2) * XYZ.BasisZ);
                startStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                startStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                startStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                startStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                rebarIdCollection.Add(startStepStraightRebar_2.Id);

                Group startStepRebarGroup = doc.Create.NewGroup(rebarIdCollection);
                List<ElementId> startStepRebarIdList = startStepRebarGroup.GetMemberIds().ToList();
                foreach (ElementId barId in startStepRebarIdList)
                {
                    doc.GetElement(barId).LookupParameter("Мрк.МаркаИзделия").Set("C-1");
                }

                uiDoc.ActiveView.HideActiveWorkPlane();
                t.Commit();
            }

            return Result.Succeeded;
        }
        private double RoundUpToFive(double toRound)
        {
            if (toRound % 5 == 0) return toRound;
            return (5 - toRound % 5) + toRound;
        }
    }
}
