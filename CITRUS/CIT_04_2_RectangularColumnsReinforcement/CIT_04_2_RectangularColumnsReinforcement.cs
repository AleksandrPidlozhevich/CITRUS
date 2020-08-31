﻿using Autodesk.Revit.DB;
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
#region Выбор колонн
            ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

            List<FamilyInstance> columnsList = new List<FamilyInstance>();//Получение списка выбранных колонн
            foreach (Reference columnRef in selColumns)
            {
                columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
            }
            //Завершение блока Получение списка колонн
#endregion

#region Выбор форм арматурных стержней и загибов
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

            //Выбор формы шпильки
            List<RebarShape> rebarPinShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "02")
                .Cast<RebarShape>()
                .ToList();
            if (rebarPinShapeList == null)
            {
                TaskDialog.Show("Revit", "Форма 02 не найдена");
                return Result.Failed;
            }
            RebarShape myPinRebarShape = rebarPinShapeList.First();

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

            //Выбор формы загиба шпильки
            List<RebarHookType> rebarPinHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Стандартный - 180 градусов")
                .Cast<RebarHookType>()
                .ToList();
            if (rebarPinHookTypeList == null)
            {
                TaskDialog.Show("Revit", "Форма загиба Стандартный - 180 градусов не найдена");
                return Result.Failed;
            }
            RebarHookType myRebarPinHookType = rebarPinHookTypeList.First();
            //Завершение блока выбора форм арматурных стержней
#endregion

#region Создание списков типов для формы
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

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> pinRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            //Завершение блока создания списков типов для формы
            #endregion
            
#region Вызов и обработка результатов формы
            //Вызов формы
            CIT_04_2_RectangularColumnsReinforcementForm rectangularColumnsReinforcementForm 
                = new CIT_04_2_RectangularColumnsReinforcementForm(mainRebarOneTapesList
                , mainRebarTwoTapesList
                , mainRebarThreeTapesList
                , stirrupRebarTapesList
                , pinRebarTapesList
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
            RebarBarType myPinBarTape = rectangularColumnsReinforcementForm.mySelectionPinBarTape;
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
            //Диаметр шпильки
            Parameter pinRebarTypeDiamParam = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double pinRebarDiam = pinRebarTypeDiamParam.AsDouble();
            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;

            //Кол-во стержней по левой и правой граням
            int numberOfBarsLRFaces = rectangularColumnsReinforcementForm.NumberOfBarsLRFaces;
            int numberOfBarsTBFaces = rectangularColumnsReinforcementForm.NumberOfBarsTBFaces;

            //Длины выпусков
            double rebarOutletsLengthLong = rectangularColumnsReinforcementForm.RebarOutletsLengthLong / 304.8;
            double rebarOutletsLengthShort = rectangularColumnsReinforcementForm.RebarOutletsLengthShort / 304.8;

            double floorThicknessAboveColumn = rectangularColumnsReinforcementForm.FloorThicknessAboveColumn / 304.8;
            double standardStirrupStep = rectangularColumnsReinforcementForm.StandardStirrupStep / 304.8;
            double increasedStirrupStep = rectangularColumnsReinforcementForm.IncreasedStirrupStep / 304.8;
            double firstStirrupOffset = rectangularColumnsReinforcementForm.FirstStirrupOffset / 304.8;
            double stirrupIncreasedPlacementHeight = rectangularColumnsReinforcementForm.StirrupIncreasedPlacementHeight / 304.8;
            int StirrupBarElemFrequentQuantity = (int)(stirrupIncreasedPlacementHeight / increasedStirrupStep) + 1;

            string checkedRebarOutletsButtonName = rectangularColumnsReinforcementForm.CheckedRebarOutletsButtonName;

#endregion

#region Старт блока Получение типа элемента CIT_04_ВаннаДляСварки
            //Список семейств с именем CIT_04_ВаннаДляСварки
            List<Family> familiesTubWelding = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(f => f.Name == "CIT_04_ВаннаДляСварки").ToList();
            if (familiesTubWelding.Count != 1) return Result.Failed;
            Family familieTubWelding = familiesTubWelding.First();

            //CIT_04_ВаннаДляСварки
            List<ElementId> symbolsTubWeldingIds = familieTubWelding.GetFamilySymbolIds().ToList();
            ElementId firstSymbolTubWeldingId = symbolsTubWeldingIds.First();

            //Тип элемента(FamilySymbol) CIT_04_ВаннаДляСварки
            FamilySymbol myTubWeldingSymbol = doc.GetElement(firstSymbolTubWeldingId) as FamilySymbol;
            if (myTubWeldingSymbol == null) return Result.Failed;

            //Завершение блока Получение типа элемента CIT_04_ВаннаДляСварки
#endregion

            //Нормаль для построения стержней основной арматуры
            XYZ mainRebarNormalMain = new XYZ(0, 1, 0);
            XYZ mainRebarNormalAdditional = new XYZ(1, 0, 0);

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");
                foreach (FamilyInstance myColumn in columnsList)
                {
#region Сбор параметров колонны
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
#endregion

                    //Старт блока задания параметра защитного слоя боковых граней колонны
                    //Защитный слой арматуры боковых граней
                    Parameter clearCoverOther = myColumn.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER);
                    clearCoverOther.Set(myRebarCoverType.Id);
                    //Завершение блока сбора параметров колонны

                    int numberOfSpacesLRFacesForStirrup = numberOfBarsLRFaces - 3;
                    double stepBarsLRFacesForStirrup = 0;
                    double residueForOffsetForStirrup = 0;

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods")
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + columnLength, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + columnLength, 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                        }
#endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength, 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffset = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrup = residueForOffset;

                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffset
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffset
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffset
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffset
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня три длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeLong_p4 = new XYZ(Math.Round(mainRebarTypeThreeLong_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня три короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeShort_p4 = new XYZ(Math.Round(mainRebarTypeThreeShort_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);
                            Curve myMainRebarTypeThreeLong_line2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line2);
                            Curve myMainRebarTypeThreeLong_line3 = Line.CreateBound(mainRebarTypeThreeLong_p3, mainRebarTypeThreeLong_p4) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line3);

                            //Кривые стержня короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);
                            Curve myMainRebarTypeThreeShort_line3 = Line.CreateBound(mainRebarTypeThreeShort_p3, mainRebarTypeThreeShort_p4) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line3);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            double residueForOffset = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffset
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffset
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffset
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffset
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                        }

#endregion
                    }

                    //Если стыковка стержней на сварке без изменения сечения колонны выше
                    if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods")
                    {
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6), Math.Round(mainRebarTypeOneLong_p1.Y, 6), Math.Round(mainRebarTypeOneLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        // Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6), Math.Round(mainRebarTypeOneShort_p1.Y, 6), Math.Round(mainRebarTypeOneShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        //Точки для установки ванночки
                        XYZ longTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong);
                        XYZ shortTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort);

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();
                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                       
                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();
                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeWeldingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        FamilyInstance tubWeldingLowerLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingLowerLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeWeldingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        FamilyInstance tubWeldingUpperRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingUpperRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                        }

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривых стержня один длинного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6), Math.Round(mainRebarTypeTwoLong_p1.Y, 6), Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            // Точки для построения кривых стержня один короткого
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6), Math.Round(mainRebarTypeTwoShort_p1.Y, 6), Math.Round(mainRebarTypeTwoShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong);
                            XYZ shortTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort);

                            //Кривые стержня один длинного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();
                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();
                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffset = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrup = residueForOffset;

                            XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffset
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                            columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                for (int i =1; i< (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                }
                            }
                            if (columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffset
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                    }

                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                    }
                                }
                                if (columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffset
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);

                                for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                }
                            }
                            if (columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffset
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                    }
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                    }
                                }
                                if (columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня один длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6), Math.Round(mainRebarTypeThreeLong_p1.Y, 6), Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            // Точки для построения кривых стержня один короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6), Math.Round(mainRebarTypeThreeShort_p1.Y, 6), Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong);
                            XYZ shortTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort);

                            //Кривые стержня один длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();
                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();
                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            double residueForOffset = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffset
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                }
                            }

                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                }
                            }
                            if (columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffset
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i,0 , 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                    }
                                }
                                if (columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffset
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                }
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                }
                            }
                            if (columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffset
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                    
                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                    }
                                }
                                if (columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }

                            }
                        }

#endregion

                    }


#region Хомуты и стяжки
                    //Хомут
                    //Нормаль для построения хомута
                    XYZ narmalStirrup = new XYZ(0, 0, 1);

                    if (numberOfBarsLRFaces > 5)
                    {
                        //Точки для построения кривых стержня хомута 1
                        XYZ rebarStirrupFirst_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

                        XYZ rebarStirrupFirst_p2 = new XYZ(Math.Round(rebarStirrupFirst_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrupFirst_p1.Y, 6)
                            , Math.Round(rebarStirrupFirst_p1.Z, 6));

                        XYZ rebarStirrupFirst_p3 = new XYZ(Math.Round(rebarStirrupFirst_p2.X, 6)
                            , Math.Round(rebarStirrupFirst_p2.Y
                            - stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
                            - residueForOffsetForStirrup
                            - mainRebarDiamTypeOne / 2
                            - mainRebarDiamTypeTwo / 2
                            - stirrupRebarDiam
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

                        columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                        columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                        //Копирование хомута 1
                        XYZ pointFirstTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                        List<ElementId> columnRebarFirstTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarFirstDownStirrup.Id, pointFirstTopStirrupInstallation) as List<ElementId>;
                        Element columnRebarFirstTopStirrup = doc.GetElement(columnRebarFirstTopStirrupIdList.First());

                        //Высота размещения хомутов со стандартным шагом
                        double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset;
                        int stirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);

                        columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                        columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);

                        //Точки для построения кривых стержня хомута 2
                        XYZ rebarStirrupSecond_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam, 6));

                        XYZ rebarStirrupSecond_p2 = new XYZ(Math.Round(rebarStirrupSecond_p1.X, 6)
                            , Math.Round(rebarStirrupSecond_p1.Y
                            + stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
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
                            - stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
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

                        columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                        columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                        //Копирование хомута 2
                        XYZ pointSecondTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                        List<ElementId> columnRebarSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarSecondDownStirrup.Id, pointSecondTopStirrupInstallation) as List<ElementId>;
                        Element columnRebarSecondTopStirrup = doc.GetElement(columnRebarSecondTopStirrupIdList.First());

                        columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                        columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                    }

                    if (numberOfBarsLRFaces > 7)
                    {
                        //Шпилька
                        //Точки для построения кривых стержня шпильки
                        double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                        XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                            , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFacesForStirrup * 4 + residueForOffsetForStirrup + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2
                            , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam + pinRebarDiam, 6));

                        XYZ rebarDownPin_p2 = new XYZ(Math.Round(columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                            , rebarDownPin_p1.Y
                            , Math.Round(rebarDownPin_p1.Z, 6));

                        //Кривые шпильки
                        List<Curve> myDownPinCurves = new List<Curve>();
                        Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                        myDownPinCurves.Add(downPin_line1);

                        Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                            , myPinRebarShape
                            , myPinBarTape
                            , myRebarPinHookType
                            , myRebarPinHookType
                            , myColumn
                            , narmalStirrup
                            , myDownPinCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                        columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                        //Высота размещения хомутов со стандартным шагом
                        double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset;
                        int stirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);

                        //Копирование шпильки вверх
                        XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                        List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                        Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                        columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                        columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);

                        if (numberOfBarsLRFaces > 9)
                        {
                            int n = (numberOfBarsLRFaces - 8) / 2; // Необходимое кол-во копий шпильки
                            for (int i =1; i<=n; i++)
                            {
                                XYZ pointPinInstallation = new XYZ(0, (stepBarsLRFacesForStirrup*2)*i, 0);
                                List<ElementId> newColumnRebarDownPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointPinInstallation) as List<ElementId>;
                                Element newColumnRebarDownPin = doc.GetElement(newColumnRebarDownPinIdList.First());

                                List<ElementId> newColumnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarTopPin.Id, pointPinInstallation) as List<ElementId>;
                                Element newColumnRebarTopPin = doc.GetElement(newColumnRebarDownPinIdList.First());
                            }
                        }
                    }

                    #endregion
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
