using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_7_ElementsTransfer
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_7_ElementsTransfer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Опции копирования
            CopyPasteOptions copyOptions = new CopyPasteOptions();
            copyOptions.SetDuplicateTypeNamesHandler(new CopyUseDestination());

            //Вызов формы с выбором типа копирования проемов
            CIT_04_7_ElementsTransferForm elementsTransferForm = new CIT_04_7_ElementsTransferForm();
            elementsTransferForm.ShowDialog();
            if (elementsTransferForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            bool floorTransferCheck = elementsTransferForm.FloorTransferCheck;
            bool columnTransferCheck = elementsTransferForm.СolumnTransferCheck;
            bool wallTransferCheck = elementsTransferForm.WallTransferCheck;
            bool beamTransferCheck = elementsTransferForm.BeamTransferCheck;
            bool foundatioTransferCheck = elementsTransferForm.FoundatioTransferCheck;

            bool replaceFloorType = elementsTransferForm.ReplaceFloorType;

            Document doc2 = null;
            Transform linkOrigin = null;
            List <RevitLinkInstance> myRevitLinkInstanceList = new List<RevitLinkInstance>();
            if (floorTransferCheck || columnTransferCheck || wallTransferCheck || beamTransferCheck || foundatioTransferCheck)
            {
                //Выбор связанного файла
                RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
                List<Reference> selRevitLinkInstanceList = sel.PickObjects(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанные файлы!").ToList();//Получение ссылки на выбранную группу

                foreach (Reference refElem in selRevitLinkInstanceList)
                {
                    RevitLinkInstance myRevitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Id == refElem.ElementId)
                    .Cast<RevitLinkInstance>()
                    .First();
                    myRevitLinkInstanceList.Add(myRevitLinkInstance);
                }
            }

            foreach (RevitLinkInstance rli in myRevitLinkInstanceList)
            {
                linkOrigin = rli.GetTotalTransform();
                doc2 = rli.GetLinkDocument();

                ICollection<ElementId> floorIdList = new List<ElementId>();
                if (floorTransferCheck)
                {
                    List<Floor> floorList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(Floor))
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .Cast<Floor>()
                    .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                    .ToList();

                    foreach (Floor fl in floorList)
                    {
                        floorIdList.Add(fl.Id);
                    }
                }

                ICollection<ElementId> columnIdList = new List<ElementId>();
                if (columnTransferCheck)
                {
                    List<FamilyInstance> columnList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralColumns)
                    .Cast<FamilyInstance>()
                    .ToList();

                    foreach (FamilyInstance c in columnList)
                    {
                        columnIdList.Add(c.Id);
                    }
                }

                ICollection<ElementId> wallIdList = new List<ElementId>();
                if (wallTransferCheck)
                {
                    List<Wall> wallList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                    .ToList();

                    foreach (Wall w in wallList)
                    {
                        wallIdList.Add(w.Id);
                    }
                }

                ICollection<ElementId> beamIdList = new List<ElementId>();
                if (beamTransferCheck)
                {
                    List<FamilyInstance> beamList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .Cast<FamilyInstance>()
                    .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM) != null)
                    .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString().Contains("Бетон"))
                    .ToList();

                    foreach (FamilyInstance fi in beamList)
                    {
                        beamIdList.Add(fi.Id);
                    }
                }

                ICollection<ElementId> foundationIdList = new List<ElementId>();
                if (foundatioTransferCheck)
                {
                    List<FamilyInstance> foundationList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                    .Cast<FamilyInstance>()
                    .ToList();

                    foreach (FamilyInstance f in foundationList)
                    {
                        foundationIdList.Add(f.Id);
                    }
                }

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Копирование элементов");
                    if (floorTransferCheck)
                    {
                        ElementTransformUtils.CopyElements(doc2, floorIdList, doc, linkOrigin, copyOptions);
                    }

                    if (columnTransferCheck)
                    {
                        ElementTransformUtils.CopyElements(doc2, columnIdList, doc, linkOrigin, copyOptions);
                    }

                    if (wallTransferCheck)
                    {
                        ElementTransformUtils.CopyElements(doc2, wallIdList, doc, linkOrigin, copyOptions);
                    }

                    if (beamTransferCheck)
                    {
                        ElementTransformUtils.CopyElements(doc2, beamIdList, doc, linkOrigin, copyOptions);
                    }

                    if (foundatioTransferCheck)
                    {
                        ElementTransformUtils.CopyElements(doc2, foundationIdList, doc, linkOrigin, copyOptions);
                    }

                    doc.Regenerate();
                    t.Commit();
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Замена элементов");
                if (replaceFloorType)
                {
                    List<Floor> floorListForReplacement = new FilteredElementCollector(doc)
                        .OfClass(typeof(Floor))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Cast<Floor>()
                        .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                        .ToList();

                    List<FloorType> floorTypesList = new FilteredElementCollector(doc)
                        .OfClass(typeof(FloorType))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Where(ft => ft.Name == "В25 200мм")
                        .Cast<FloorType>().ToList();

                    if (floorTypesList.Count() == 0)
                    {
                        TaskDialog.Show("Revit", "Тип перекрытия \"В25 200мм\" не найден");
                        return Result.Cancelled;
                    }
                    FloorType floorTypeB20200mm = floorTypesList.First();

                    List<double> floorThicknessList = new List<double>();
                    foreach (Floor floor in floorListForReplacement)
                    {
                        double floorThickness = floor.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
                        if (floorThicknessList.Contains(floorThickness)) continue;
                        else floorThicknessList.Add(floorThickness);
                    }

                    foreach (double thickness in floorThicknessList)
                    {
                        List<Floor> floorListForReplacementTemp = new FilteredElementCollector(doc)
                        .OfClass(typeof(Floor))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Cast<Floor>()
                        .Where(f => f.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble() == thickness)
                        .ToList();

                        List<FloorType> floorTypesListTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(FloorType))
                            .OfCategory(BuiltInCategory.OST_Floors)
                            .Where(ft => ft.Name == "В25 " + thickness * 304.8 + "мм")
                            .Cast<FloorType>()
                            .ToList();

                        FloorType newFloorType = null;
                        if (floorTypesListTemp.Count() == 0)
                        {
                            newFloorType = floorTypeB20200mm.Duplicate("В25 " + thickness * 304.8 + "мм") as FloorType;

                            CompoundStructure compound = newFloorType.GetCompoundStructure();
                            compound.SetLayerWidth(0, thickness);
                            newFloorType.SetCompoundStructure(compound);
                            newFloorType.LookupParameter("О_Наименование").Set("Перекрытие t = " + thickness * 304.8 + "мм");
                        }
                        else
                        {
                            newFloorType = floorTypesListTemp.First();
                        }

                        foreach (Floor fl in floorListForReplacementTemp)
                        {
                            if (fl.FloorType != newFloorType)
                            {
                                fl.FloorType = newFloorType;
                            }
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }

    internal class CopyUseDestination : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}
