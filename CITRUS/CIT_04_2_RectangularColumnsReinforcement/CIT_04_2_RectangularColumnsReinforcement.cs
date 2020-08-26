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
            List<RebarBarType> mainRebarOneTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> mainRebarTwoTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> mainRebarThreeTapesList = new FilteredElementCollector(doc)
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
            CIT_04_2_RectangularColumnsReinforcementForm rectangularColumnsReinforcementForm 
                = new CIT_04_2_RectangularColumnsReinforcementForm(mainRebarOneTapesList
                , mainRebarTwoTapesList
                , mainRebarThreeTapesList
                , stirrupRebarTapesList
                , rebarCoverTypesList);
            rectangularColumnsReinforcementForm.ShowDialog();
            if (rectangularColumnsReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Выбор типа основной арматуры
            RebarBarType myMainRebarTypeOne = rectangularColumnsReinforcementForm.mySelectionMainBarTapeOne;
            RebarBarType myMainRebarTypeTwo = rectangularColumnsReinforcementForm.mySelectionMainBarTapeTwo;
            RebarBarType myMainRebarTypeThree = rectangularColumnsReinforcementForm.mySelectionMainBarTapeThree;
            //Выбор типа арматуры хомутов
            RebarBarType myStirrupBarTape = rectangularColumnsReinforcementForm.mySelectionStirrupBarTape;
            //Выбор типа защитного слоя основной арматуры
            RebarCoverType myRebarCoverType = rectangularColumnsReinforcementForm.mySelectionRebarCoverType;

            //Диаметр стержня основной арматуры
            Parameter mainRebarTypeOneDiamParam = myMainRebarTypeOne.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeOne = mainRebarTypeOneDiamParam.AsDouble();
            Parameter mainRebarTypeTwoDiamParam = myMainRebarTypeTwo.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeTwo = mainRebarTypeTwoDiamParam.AsDouble();
            Parameter mainRebarTypeThreeDiamParam = myMainRebarTypeThree.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeThree = mainRebarTypeThreeDiamParam.AsDouble();

            //Диаметр хомута
            Parameter stirrupRebarTypeDiamParam = myStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stirrupRebarDiam = stirrupRebarTypeDiamParam.AsDouble();
            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;

            //Кол-во стержней по левой и правой граням
            int numberOfBarsLRFaces = rectangularColumnsReinforcementForm.NumberOfBarsLRFaces;
            int numberOfBarsTBFaces = rectangularColumnsReinforcementForm.NumberOfBarsTBFaces;
            //Нормаль для построения стержней основной арматуры
            XYZ mainRebarNormalMain = new XYZ(0, 1, 0);
            XYZ mainRebarNormalAdditional = new XYZ(1, 0, 0);
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


                    //Сделать тут IF


                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    //Точки для построения кривфх стержня один
                    XYZ mainRebarTypeOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z, 6));
                    XYZ mainRebarTypeOne_p2 = new XYZ(Math.Round(mainRebarTypeOne_p1.X, 6)
                        , Math.Round(mainRebarTypeOne_p1.Y, 6)
                        , Math.Round(mainRebarTypeOne_p1.Z + columnLength, 6));
                    XYZ mainRebarTypeOne_p3 = new XYZ(Math.Round(mainRebarTypeOne_p2.X + mainRebarDiamTypeOne, 6)
                        , Math.Round(mainRebarTypeOne_p2.Y, 6)
                        , Math.Round(mainRebarTypeOne_p2.Z + 200/304.8, 6));
                    XYZ mainRebarTypeOne_p4 = new XYZ(Math.Round(mainRebarTypeOne_p3.X, 6)
                        , Math.Round(mainRebarTypeOne_p3.Y, 6)
                        , Math.Round(mainRebarTypeOne_p3.Z + 1200/304.8, 6));

                    //Кривые стержня один
                    List<Curve> myMainRebarTypeOneCurves = new List<Curve>();

                    Curve myMainRebarTypeOne_line1 = Line.CreateBound(mainRebarTypeOne_p1, mainRebarTypeOne_p2) as Curve;
                    myMainRebarTypeOneCurves.Add(myMainRebarTypeOne_line1);
                    Curve myMainRebarTypeOne_line2 = Line.CreateBound(mainRebarTypeOne_p2, mainRebarTypeOne_p3) as Curve;
                    myMainRebarTypeOneCurves.Add(myMainRebarTypeOne_line2);
                    Curve myMainRebarTypeOne_line3 = Line.CreateBound(mainRebarTypeOne_p3, mainRebarTypeOne_p4) as Curve;
                    myMainRebarTypeOneCurves.Add(myMainRebarTypeOne_line3);

                    //Нижний левый угол
                    Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                    , myMainRebarShapeOverlappingRods
                    , myMainRebarTypeOne
                    , null
                    , null
                    , myColumn
                    , mainRebarNormalMain
                    , myMainRebarTypeOneCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                    XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                    ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                    //Верхний левый угол
                    Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                    , myMainRebarShapeOverlappingRods
                    , myMainRebarTypeOne
                    , null
                    , null
                    , myColumn
                    , mainRebarNormalMain
                    , myMainRebarTypeOneCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                    XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                    ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                    //Верхний правый угол
                    Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                    , myMainRebarShapeOverlappingRods
                    , myMainRebarTypeOne
                    , null
                    , null
                    , myColumn
                    , mainRebarNormalMain
                    , myMainRebarTypeOneCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                    XYZ rotate_p1 = new XYZ(mainRebarTypeOne_p1.X, mainRebarTypeOne_p1.Y, mainRebarTypeOne_p1.Z);
                    XYZ rotate_p2 = new XYZ(mainRebarTypeOne_p1.X, mainRebarTypeOne_p1.Y, mainRebarTypeOne_p1.Z + 1);
                    Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                    ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                    XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                    ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                    //Нижний правый угол
                    Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                    , myMainRebarShapeOverlappingRods
                    , myMainRebarTypeOne
                    , null
                    , null
                    , myColumn
                    , mainRebarNormalMain
                    , myMainRebarTypeOneCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                    ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                    XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                    ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                    int numberOfSpacesTBFacesForStirrup = numberOfBarsLRFaces-3;
                    double stepBarsLRFacesForStirrup = 0;
                    double residueForOffsetForStirrup = 0;


                    if (numberOfBarsLRFaces >= 3)
                    {
                        //Точки для построения кривфх стержня два
                        XYZ mainRebarTypeTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z, 6));
                        XYZ mainRebarTypeTwo_p2 = new XYZ(Math.Round(mainRebarTypeTwo_p1.X, 6)
                            , Math.Round(mainRebarTypeTwo_p1.Y, 6)
                            , Math.Round(mainRebarTypeTwo_p1.Z + columnLength, 6));
                        XYZ mainRebarTypeTwo_p3 = new XYZ(Math.Round(mainRebarTypeTwo_p2.X + mainRebarDiamTypeTwo, 6)
                            , Math.Round(mainRebarTypeTwo_p2.Y, 6)
                            , Math.Round(mainRebarTypeTwo_p2.Z + 200 / 304.8, 6));
                        XYZ mainRebarTypeTwo_p4 = new XYZ(Math.Round(mainRebarTypeTwo_p3.X, 6)
                            , Math.Round(mainRebarTypeTwo_p3.Y, 6)
                            , Math.Round(mainRebarTypeTwo_p3.Z + 1200 / 304.8, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarTypeTwoCurves = new List<Curve>();

                        Curve myMainRebarTypeTwo_line1 = Line.CreateBound(mainRebarTypeTwo_p1, mainRebarTypeTwo_p2) as Curve;
                        myMainRebarTypeTwoCurves.Add(myMainRebarTypeTwo_line1);
                        Curve myMainRebarTypeTwo_line2 = Line.CreateBound(mainRebarTypeTwo_p2, mainRebarTypeTwo_p3) as Curve;
                        myMainRebarTypeTwoCurves.Add(myMainRebarTypeTwo_line2);
                        Curve myMainRebarTypeTwo_line3 = Line.CreateBound(mainRebarTypeTwo_p3, mainRebarTypeTwo_p4) as Curve;
                        myMainRebarTypeTwoCurves.Add(myMainRebarTypeTwo_line3);

                        //Левая грань
                        Rebar columnMainRebarLeftFace = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeTwo
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeTwoCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        //Cтержни левая и правая грани
                        int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                        double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                        double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                        stepBarsLRFacesForStirrup = stepBarsLRFaces;
                        double residueForOffset = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                        residueForOffsetForStirrup = residueForOffset;

                        XYZ newPlaсeСolumnMainRebarLeftFace = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                            , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffset
                            , 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFace.Id, newPlaсeСolumnMainRebarLeftFace);
                        columnMainRebarLeftFace.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnMainRebarLeftFace.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsLRFaces - 2);
                        columnMainRebarLeftFace.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces);

                        //Правая грань
                        Rebar columnMainRebarRightFace = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeTwo
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeTwoCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebarRightFace.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarRightFace = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                            , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffset
                            , 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarRightFace.Id, newPlaсeColumnMainRebarRightFace);
                        columnMainRebarRightFace.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnMainRebarRightFace.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsLRFaces - 2);
                        columnMainRebarRightFace.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces);
                    }

                    if (numberOfBarsTBFaces >= 3)
                    {
                        //Точки для построения кривфх стержня три
                        XYZ mainRebarTypeThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z, 6));
                        XYZ mainRebarTypeThree_p2 = new XYZ(Math.Round(mainRebarTypeThree_p1.X, 6)
                            , Math.Round(mainRebarTypeThree_p1.Y, 6)
                            , Math.Round(mainRebarTypeThree_p1.Z + columnLength, 6));
                        XYZ mainRebarTypeThree_p3 = new XYZ(Math.Round(mainRebarTypeThree_p2.X, 6)
                            , Math.Round(mainRebarTypeThree_p2.Y + mainRebarDiamTypeThree, 6)
                            , Math.Round(mainRebarTypeThree_p2.Z + 200 / 304.8, 6));
                        XYZ mainRebarTypeThree_p4 = new XYZ(Math.Round(mainRebarTypeThree_p3.X, 6)
                            , Math.Round(mainRebarTypeThree_p3.Y, 6)
                            , Math.Round(mainRebarTypeThree_p3.Z + 1200 / 304.8, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarTypeThreeCurves = new List<Curve>();

                        Curve myMainRebarTypeThree_line1 = Line.CreateBound(mainRebarTypeThree_p1, mainRebarTypeThree_p2) as Curve;
                        myMainRebarTypeThreeCurves.Add(myMainRebarTypeThree_line1);
                        Curve myMainRebarTypeThree_line2 = Line.CreateBound(mainRebarTypeThree_p2, mainRebarTypeThree_p3) as Curve;
                        myMainRebarTypeThreeCurves.Add(myMainRebarTypeThree_line2);
                        Curve myMainRebarTypeThree_line3 = Line.CreateBound(mainRebarTypeThree_p3, mainRebarTypeThree_p4) as Curve;
                        myMainRebarTypeThreeCurves.Add(myMainRebarTypeThree_line3);

                        //Нижняя грань
                        Rebar columnMainRebarBottomFace = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeThree
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalAdditional
                        , myMainRebarTypeThreeCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        //Cтержни нижняя и верхняя грани
                        int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                        double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                        double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                        double residueForOffset = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;

                        XYZ newPlaсeСolumnMainRebarBottomFace = new XYZ(- columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffset
                            , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                            , 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFace.Id, newPlaсeСolumnMainRebarBottomFace);
                        columnMainRebarBottomFace.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnMainRebarBottomFace.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTBFaces - 2);
                        columnMainRebarBottomFace.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces);

                        //Верхняя грань
                        Rebar columnMainRebarTopFace = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeThree
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalAdditional
                        , myMainRebarTypeThreeCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebarTopFace.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebarTopFace = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffset
                            , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                            , 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarTopFace.Id, newPlaсeСolumnMainRebarTopFace);
                        columnMainRebarTopFace.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnMainRebarTopFace.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTBFaces - 2);
                        columnMainRebarTopFace.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces);
                    }

                    //Хомут
                    //Нормаль для построения хомута
                    XYZ narmalStirrup = new XYZ(0, 0, 1);

                    //Точки для построения кривых стержня хомута 1
                    XYZ rebarStirrupFirst_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Z + 50 / 304.8, 6));

                    XYZ rebarStirrupFirst_p2 = new XYZ(Math.Round(rebarStirrupFirst_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                        , Math.Round(rebarStirrupFirst_p1.Y, 6)
                        , Math.Round(rebarStirrupFirst_p1.Z, 6));

                    XYZ rebarStirrupFirst_p3 = new XYZ(Math.Round(rebarStirrupFirst_p2.X, 6)
                        , Math.Round(rebarStirrupFirst_p2.Y
                        - stepBarsLRFacesForStirrup * numberOfSpacesTBFacesForStirrup
                        - residueForOffsetForStirrup - mainRebarDiamTypeOne / 2
                        - mainRebarDiamTypeTwo / 2 - stirrupRebarDiam
                        - stirrupRebarDiam / 2, 6)
                        , Math.Round(rebarStirrupFirst_p2.Z, 6));

                    XYZ rebarStirrupFirst_p4 = new XYZ(Math.Round(rebarStirrupFirst_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                        , Math.Round(rebarStirrupFirst_p3.Y, 6)
                        , Math.Round(rebarStirrupFirst_p3.Z, 6));

                    //Кривые хомута 1
                    List<Curve> myStirrupFirstCurves = new List<Curve>();

                    Curve firstStirrup_line1 = Line.CreateBound(rebarStirrupFirst_p1, rebarStirrupFirst_p2) as Curve;
                    myStirrupFirstCurves.Add(firstStirrup_line1);
                    Curve firstStirrup_line2 = Line.CreateBound(rebarStirrupFirst_p2, rebarStirrupFirst_p3) as Curve;
                    myStirrupFirstCurves.Add(firstStirrup_line2);
                    Curve firstStirrup_line3 = Line.CreateBound(rebarStirrupFirst_p3, rebarStirrupFirst_p4) as Curve;
                    myStirrupFirstCurves.Add(firstStirrup_line3);
                    Curve firstStirrup_line4 = Line.CreateBound(rebarStirrupFirst_p4, rebarStirrupFirst_p1) as Curve;
                    myStirrupFirstCurves.Add(firstStirrup_line4);

                    //Построение нижнего хомута 1
                    Rebar columnRebarFirstDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupBarTape
                        , myRebarHookType
                        , myRebarHookType
                        , myColumn
                        , narmalStirrup
                        , myStirrupFirstCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                    //Точки для построения кривых стержня хомута 2
                    XYZ rebarStirrupSecond_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Z + 50 / 304.8 + stirrupRebarDiam, 6));

                    XYZ rebarStirrupSecond_p2 = new XYZ(Math.Round(rebarStirrupSecond_p1.X, 6)
                        , Math.Round(rebarStirrupSecond_p1.Y
                        + stepBarsLRFacesForStirrup * numberOfSpacesTBFacesForStirrup
                        + residueForOffsetForStirrup 
                        + mainRebarDiamTypeOne / 2
                        + mainRebarDiamTypeTwo / 2 
                        + stirrupRebarDiam / 2, 6)
                        , Math.Round(rebarStirrupSecond_p1.Z, 6));

                    XYZ rebarStirrupSecond_p3 = new XYZ(Math.Round(rebarStirrupSecond_p2.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                        , Math.Round(rebarStirrupSecond_p2.Y, 6)
                        , Math.Round(rebarStirrupSecond_p2.Z, 6));

                    XYZ rebarStirrupSecond_p4 = new XYZ(Math.Round(rebarStirrupSecond_p3.X, 6)
                        , Math.Round(rebarStirrupSecond_p3.Y
                        - stepBarsLRFacesForStirrup * numberOfSpacesTBFacesForStirrup
                        - residueForOffsetForStirrup
                        - mainRebarDiamTypeOne / 2
                        - mainRebarDiamTypeTwo / 2
                        - stirrupRebarDiam / 2, 6)
                        , Math.Round(rebarStirrupSecond_p3.Z, 6));

                    //Кривые хомута 2
                    List<Curve> myStirrupSecondCurves = new List<Curve>();

                    Curve secondStirrup_line1 = Line.CreateBound(rebarStirrupSecond_p1, rebarStirrupSecond_p2) as Curve;
                    myStirrupSecondCurves.Add(secondStirrup_line1);
                    Curve secondStirrup_line2 = Line.CreateBound(rebarStirrupSecond_p2, rebarStirrupSecond_p3) as Curve;
                    myStirrupSecondCurves.Add(secondStirrup_line2);
                    Curve secondStirrup_line3 = Line.CreateBound(rebarStirrupSecond_p3, rebarStirrupSecond_p4) as Curve;
                    myStirrupSecondCurves.Add(secondStirrup_line3);
                    Curve secondStirrup_line4 = Line.CreateBound(rebarStirrupSecond_p4, rebarStirrupSecond_p1) as Curve;
                    myStirrupSecondCurves.Add(secondStirrup_line4);

                    //Построение нижнего хомута 2
                    Rebar columnRebarSecondDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupBarTape
                        , myRebarHookType
                        , myRebarHookType
                        , myColumn
                        , narmalStirrup
                        , myStirrupSecondCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                }
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
