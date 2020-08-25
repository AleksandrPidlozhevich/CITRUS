using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_2_RectangularColumnsReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор колонн
            ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

            List<FamilyInstance> columnsList = new List<FamilyInstance>();//Получение списка выбранных колонн
            foreach (Reference columnRef in selColumns)
            {
                columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
            }
            //Завершение блока Получение списка колонн

            // Выбор формы основной арматуры если стыковка стержней в нахлест
            List<RebarShape> rebarShapeMainOverlappingRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "26")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeMainOverlappingRodsList == null)
            {
                TaskDialog.Show("Revit", "Форма 26 не найдена");
                return Result.Failed;
            }
            RebarShape myMainRebarShapeOverlappingRods = rebarShapeMainOverlappingRodsList.First();

            //Выбор формы основной арматуры если стыковка на сварке
            List<RebarShape> rebarShapeMainWeldingRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeMainWeldingRodsList == null)
            {
                TaskDialog.Show("Revit", "Форма 01 не найдена");
                return Result.Failed;
            }
            RebarShape myMainRebarShapeWeldingRods = rebarShapeMainWeldingRodsList.First();

            //Выбор формы хомута
            List<RebarShape> rebarStirrupShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "51")
                .Cast<RebarShape>()
                .ToList();
            if (rebarStirrupShapeList == null)
            {
                TaskDialog.Show("Revit", "Форма 51 не найдена");
                return Result.Failed;
            }
            RebarShape myStirrupRebarShape = rebarStirrupShapeList.First();

            //Выбор формы загиба хомута
            List<RebarHookType> rebarHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Сейсмическая поперечная арматура - 135 градусов")
                .Cast<RebarHookType>()
                .ToList();
            if (rebarHookTypeList == null)
            {
                TaskDialog.Show("Revit", "Форма загиба Сейсмическая поперечная арматура - 135 градусов не найдена");
                return Result.Failed;
            }
            RebarHookType myRebarHookType = rebarHookTypeList.First();
            //Завершение блока выбора форм арматурных стержней

            //Список типов для выбора основной арматуры
            List<RebarBarType> mainRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> stirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            //Завершение блока создания списков типов для 

            //Вызов формы
            CIT_04_2_RectangularColumnsReinforcementForm rectangularColumnsReinforcementForm = new CIT_04_2_RectangularColumnsReinforcementForm(mainRebarTapesList, stirrupRebarTapesList, rebarCoverTypesList);
            rectangularColumnsReinforcementForm.ShowDialog();
            if (rectangularColumnsReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Выбор типа основной арматуры
            RebarBarType myMainRebarType = rectangularColumnsReinforcementForm.mySelectionMainBarTape;
            //Выбор типа арматуры хомутов
            RebarBarType myStirrupBarTape = rectangularColumnsReinforcementForm.mySelectionStirrupBarTape;
            //Выбор типа защитного слоя основной арматуры
            RebarCoverType myRebarCoverType = rectangularColumnsReinforcementForm.mySelectionRebarCoverType;

            //Диаметр стержня основной арматуры
            Parameter mainRebarTypeDiamParam = myMainRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiam = mainRebarTypeDiamParam.AsDouble();
            //Диаметр хомута
            Parameter stirrupRebarTypeDiamParam = myStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stirrupRebarDiam = stirrupRebarTypeDiamParam.AsDouble();
            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;

            //Нормаль для построения стержней основной арматуры
            XYZ mainRebarNormal = new XYZ(0, 1, 0);


            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");
                foreach (FamilyInstance myColumn in columnsList)
                {
                    // Базовый уровень
                     ElementId baseLevelId = myColumn.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                    Level baseLevel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Where(lv => lv.Id == baseLevelId)
                        .Cast<Level>()
                        .ToList()
                        .First();
                    //Отметка базового уровня
                    double baseLevelElevation = Math.Round(baseLevel.Elevation, 6);

                    //Верхний уровень
                    ElementId topLevelId = myColumn.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
                    Level topLevel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Where(lv => lv.Id == topLevelId)
                        .Cast<Level>()
                        .ToList()
                        .First();
                    //Отметка верхнего уровня
                    double topLevelElevation = Math.Round(topLevel.Elevation, 6);

                    //Смещение снизу
                    Parameter baseLevelOffsetParam = myColumn.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM);
                    double baseLevelOffset = Math.Round(baseLevelOffsetParam.AsDouble(), 6);

                    //Смещение сверху
                    Parameter topLevelOffsetParam = myColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
                    double topLevelOffset = Math.Round(topLevelOffsetParam.AsDouble(), 6);

                    //Длина колонны
                    double columnLength = ((topLevelElevation + topLevelOffset) - (baseLevelElevation + baseLevelOffset));

                    //Ширина сечения колонны
                    double columnSectionWidth = myColumn.Symbol.LookupParameter("Рзм.Ширина").AsDouble();

                    //Высота сечения колонны
                    double columnSectionHeight = myColumn.Symbol.LookupParameter("Рзм.Высота").AsDouble();

                    if (columnSectionWidth == columnSectionHeight)
                    {
                        continue;
                    }

                    //Получение нижней точки геометрии колонны
                    LocationPoint columnOriginLocationPoint = myColumn.Location as LocationPoint;
                    XYZ columnOriginBase = columnOriginLocationPoint.Point;
                    XYZ columnOrigin = new XYZ(columnOriginBase.X, columnOriginBase.Y, baseLevelElevation + baseLevelOffset);

                    //Угол поворота колонны
                    double columnRotation = columnOriginLocationPoint.Rotation;
                    //Ось вращения
                    XYZ rotationPoint1 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z);
                    XYZ rotationPoint2 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z + 1);
                    Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);
                    //Завершение блока сбора параметров колонны

                    //Старт блока задания параметра защитного слоя боковых граней колонны
                    //Защитный слой арматуры боковых граней
                    Parameter clearCoverOther = myColumn.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER);
                    clearCoverOther.Set(myRebarCoverType.Id);
                    //Завершение блока сбора параметров колонны

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    //Точки для построения кривфх стержня
                    XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X- columnSectionWidth/2 + mainRebarCoverLayer + mainRebarDiam/2, 6)
                        , Math.Round(columnOrigin.Y - columnSectionHeight/2 + mainRebarCoverLayer + mainRebarDiam/2, 6)
                        , Math.Round(columnOrigin.Z, 6));
                    XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                        , Math.Round(rebar_p1.Y, 6)
                        , Math.Round(rebar_p1.Z + columnLength, 6));
                    XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + mainRebarDiam, 6)
                        , Math.Round(rebar_p2.Y, 6)
                        , Math.Round(rebar_p2.Z + 200/304.8, 6));
                    XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6)
                        , Math.Round(rebar_p3.Y, 6)
                        , Math.Round(rebar_p3.Z + 1200/304.8, 6));

                    //Кривые стержня
                    List<Curve> myMainRebarCurves = new List<Curve>();

                    Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                    myMainRebarCurves.Add(line1);
                    Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                    myMainRebarCurves.Add(line2);
                    Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                    myMainRebarCurves.Add(line3);


                    //Нижний левый угол
                    Rebar columnMainRebar = Rebar.CreateFromCurvesAndShape(doc
                    , myMainRebarShapeOverlappingRods
                    , myMainRebarType
                    , null
                    , null
                    , myColumn
                    , mainRebarNormal
                    , myMainRebarCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                    columnMainRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnMainRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(9);
                }
                t.Commit();
            }

                return Result.Succeeded;
        }
    }
}
