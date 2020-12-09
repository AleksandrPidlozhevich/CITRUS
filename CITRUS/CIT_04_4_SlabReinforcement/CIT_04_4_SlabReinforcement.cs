using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

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

            List<RebarBarType> bottomYDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> topXDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> topYDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры верх
            List<RebarCoverType> rebarCoverTypesListForTop = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            //Список типов защитных слоев арматуры низ
            List<RebarCoverType> rebarCoverTypesListForBottom = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            CIT_04_4_SlabReinforcementForm slabReinforcementForm = new CIT_04_4_SlabReinforcementForm(bottomXDirectionRebarTapesList
                , bottomYDirectionRebarTapesList
                , topXDirectionRebarTapesList
                , topYDirectionRebarTapesList
                , rebarCoverTypesListForTop
                , rebarCoverTypesListForBottom);

            slabReinforcementForm.ShowDialog();
            if (slabReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            RebarBarType bottomXDirectionRebarTape = slabReinforcementForm.mySelectionBottomXDirectionRebarTape;
            Parameter bottomXDirectionRebarTapeDiamParam = bottomXDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double bottomXDirectionRebarDiam = bottomXDirectionRebarTapeDiamParam.AsDouble();

            RebarBarType bottomYDirectionRebarTape = slabReinforcementForm.mySelectionBottomYDirectionRebarTape;
            Parameter bottomYDirectionRebarTapeDiamParam = bottomYDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double bottomYDirectionRebarDiam = bottomYDirectionRebarTapeDiamParam.AsDouble();

            RebarBarType topXDirectionRebarTape = slabReinforcementForm.mySelectionTopXDirectionRebarTape;
            Parameter topXDirectionRebarTapeDiamParam = topXDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double topXDirectionRebarDiam = topXDirectionRebarTapeDiamParam.AsDouble();

            RebarBarType topYDirectionRebarTape = slabReinforcementForm.mySelectionTopYDirectionRebarTape;
            Parameter topYDirectionRebarTapeDiamParam = topYDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double topYDirectionRebarDiam = topYDirectionRebarTapeDiamParam.AsDouble();

            double bottomXDirectionRebarSpacing = slabReinforcementForm.BottomXDirectionRebarSpacing / 304.8;
            double bottomYDirectionRebarSpacing = slabReinforcementForm.BottomYDirectionRebarSpacing / 304.8;
            double topXDirectionRebarSpacing = slabReinforcementForm.TopXDirectionRebarSpacing / 304.8;
            double topYDirectionRebarSpacing = slabReinforcementForm.TopYDirectionRebarSpacing / 304.8;

            double perimeterFramingDiam = slabReinforcementForm.PerimeterFramingDiam / 304.8;
            double perimeterFramingAnchoring = slabReinforcementForm.PerimeterFramingAnchoring / 304.8;
            double perimeterFramingEndCoverLayer = slabReinforcementForm.PerimeterFramingEndCoverLayer / 304.8 + perimeterFramingDiam/2;
            double perimeterFramingStep = slabReinforcementForm.PerimeterFramingStep / 304.8;

            //Выбор типа защитного слоя сверху
            RebarCoverType rebarCoverTypeForTop = slabReinforcementForm.mySelectionRebarCoverTypeForTop;
            double rebarCoverTop = rebarCoverTypeForTop.CoverDistance;
            //Выбор типа защитного слоя снизу
            RebarCoverType rebarCoverTypeForBottom = slabReinforcementForm.mySelectionRebarCoverTypeForBottom;
            double rebarCoverBottom = rebarCoverTypeForBottom.CoverDistance;

            double perimeterFramingTopCoverLayer = rebarCoverTop + topYDirectionRebarDiam + perimeterFramingDiam / 2;
            double perimeterFramingBottomCoverLayer = rebarCoverBottom + perimeterFramingDiam / 2;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование плиты");
                foreach (Floor floor in floorList)
                {
                    floor.get_Parameter(BuiltInParameter.CLEAR_COVER_TOP).Set(rebarCoverTypeForTop.Id);
                    floor.get_Parameter(BuiltInParameter.CLEAR_COVER_BOTTOM).Set(rebarCoverTypeForBottom.Id);
                    double floorThickness = floor.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
                    FamilySymbol targetPerimeterFramingFamilySymbol = null;

                    //Семейство обрамления проема
                    List<FamilySymbol> perimeterFramingFamilySymbolList = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .Where(fs => fs.FamilyName == "264_Обрамление периметра Пшки (ОбщМод_Линия)")
                        .Where(fs => fs.LookupParameter("Диаметр стержня").AsDouble() == perimeterFramingDiam)
                        .Where(fs => fs.LookupParameter("Анкеровка Пшки").AsDouble() == perimeterFramingAnchoring)
                        .Where(fs => fs.LookupParameter("Защитный слой верх").AsDouble() == perimeterFramingTopCoverLayer)
                        .Where(fs => fs.LookupParameter("Защитный слой низ").AsDouble() == perimeterFramingBottomCoverLayer)
                        .Where(fs => fs.LookupParameter("Защитный слой торец").AsDouble() == perimeterFramingEndCoverLayer)
                        .Where(fs => fs.LookupParameter("Толщина плиты").AsDouble() == floorThickness)
                        .ToList();

                    if (perimeterFramingFamilySymbolList.Count == 0)
                    {
                        //Семейство обрамления проема
                        perimeterFramingFamilySymbolList = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .Cast<FamilySymbol>()
                            .Where(fs => fs.FamilyName == "264_Обрамление периметра Пшки (ОбщМод_Линия)")
                            .ToList();
                        if (perimeterFramingFamilySymbolList.Count == 0)
                        {
                            TaskDialog.Show("Revit", "Семейство \"264_Обрамление периметра Пшки (ОбщМод_Линия)\" не найдено!");
                            return Result.Cancelled;
                        }
                        FamilySymbol typeForCopy = perimeterFramingFamilySymbolList.First();

                        targetPerimeterFramingFamilySymbol = typeForCopy.Duplicate("Плита=" + floorThickness*304.8 +"мм" 
                            + ", D="+ perimeterFramingDiam * 304.8 + "мм" 
                            + " ,Анкеровка ="+ perimeterFramingAnchoring * 304.8 + "мм" 
                            + " , ЗС_Торец="+ (perimeterFramingEndCoverLayer - perimeterFramingDiam / 2) * 304.8 + "мм" 
                            + " , ЗС_Верх=" + (perimeterFramingTopCoverLayer - perimeterFramingDiam / 2) * 304.8 + "мм" 
                            + " , ЗС_Низ=" + (perimeterFramingBottomCoverLayer - perimeterFramingDiam / 2) * 304.8 + "мм") as FamilySymbol;

                        targetPerimeterFramingFamilySymbol.LookupParameter("Диаметр стержня").Set(perimeterFramingDiam);
                        targetPerimeterFramingFamilySymbol.LookupParameter("Анкеровка Пшки").Set(perimeterFramingAnchoring);
                        targetPerimeterFramingFamilySymbol.LookupParameter("Защитный слой верх").Set(perimeterFramingTopCoverLayer);
                        targetPerimeterFramingFamilySymbol.LookupParameter("Защитный слой низ").Set(perimeterFramingBottomCoverLayer);
                        targetPerimeterFramingFamilySymbol.LookupParameter("Защитный слой торец").Set(perimeterFramingEndCoverLayer);
                        targetPerimeterFramingFamilySymbol.LookupParameter("Толщина плиты").Set(floorThickness);
                    }
                    else
                    {
                        targetPerimeterFramingFamilySymbol = perimeterFramingFamilySymbolList.First();
                    }

                    double spanDirectionAngle = floor.SpanDirectionAngle;
                    XYZ zeroPoint = new XYZ(0, 0, 0);
                    XYZ directionPointStart = new XYZ(1, 0, 0);
                    Transform rot = Transform.CreateRotationAtPoint(XYZ.BasisZ, spanDirectionAngle, zeroPoint);
                    XYZ directionPoint = rot.OfPoint(directionPointStart);
                    XYZ directionVector = (directionPoint - zeroPoint).Normalize();

                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    GeometryElement geomFloorElement = floor.get_Geometry(opt);
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
                        if (Math.Round(pf.FaceNormal.Normalize().Z) == XYZ.BasisZ.Z)
                        {
                            myFace = pf;
                            break;
                        }
                    }

                    IList<CurveLoop> curveLoopList = myFace.GetEdgesAsCurveLoops();
                    IList<Curve> curveList = new List<Curve>();
                    foreach (Curve c in curveLoopList.First())
                    {
                        curveList.Add(c);
                    }

                    AreaReinforcement areaReinforcementBottomXDirection = AreaReinforcement.Create(doc
                        , floor
                        , curveList
                        , directionVector
                        , areaReinforcementType.Id
                        , bottomXDirectionRebarTape.Id
                        , ElementId.InvalidElementId);

                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(1);
                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_1).Set(bottomXDirectionRebarSpacing);
                    areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("низ X фон");

                    AreaReinforcement areaReinforcementBottomYDirection = AreaReinforcement.Create(doc
                        , floor
                        , curveList
                        , directionVector
                        , areaReinforcementType.Id
                        , bottomYDirectionRebarTape.Id
                        , ElementId.InvalidElementId);

                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(1);
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_2).Set(bottomYDirectionRebarSpacing);
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("низ Y фон");
                    areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_BOTTOM_OFFSET).Set(bottomXDirectionRebarDiam);

                    AreaReinforcement areaReinforcemenTopXDirection = AreaReinforcement.Create(doc
                        , floor
                        , curveList
                        , directionVector
                        , areaReinforcementType.Id
                        , topXDirectionRebarTape.Id
                        , ElementId.InvalidElementId);

                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(1);
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_1).Set(topXDirectionRebarSpacing);
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("верх X фон");
                    areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_TOP_OFFSET).Set(topYDirectionRebarDiam);

                    AreaReinforcement areaReinforcemenTopYDirection = AreaReinforcement.Create(doc
                        , floor
                        , curveList
                        , directionVector
                        , areaReinforcementType.Id
                        , topYDirectionRebarTape.Id
                        , ElementId.InvalidElementId);

                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(1);
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_2).Set(topYDirectionRebarSpacing);
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("верх Y фон");
                    areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_TOP_OFFSET).Set(0);

                    foreach (CurveLoop cl in curveLoopList)
                    {
                        foreach(Line ln in cl)
                        {
                            FamilyInstance fi = doc.Create.NewFamilyInstance(myFace, ln, targetPerimeterFramingFamilySymbol);
                            fi.LookupParameter("Мсв.Шаг").Set(perimeterFramingStep);
                            fi.LookupParameter("Мрк.МаркаКонструкции").Set(floor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString());
                        }
                    }
                    
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
