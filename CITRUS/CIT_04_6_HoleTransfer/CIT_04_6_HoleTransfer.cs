using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_6_HoleTransfer
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_6_HoleTransfer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Список типов для поиска семейства отверстия
            List<FamilySymbol> familySymbolList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Where(f => f.Name == "231_Проем прямоуг (Окно_Стена)")
                .Cast<FamilySymbol>()
                .ToList();
            if(familySymbolList.Count == 0)
            {
                TaskDialog.Show("Revit", "Семейство \"231_Проем прямоуг (Окно_Стена)\" не найдено");
                return Result.Cancelled;
            }

            //Получение типа 231_Проем прямоуг (Окно_Стена)
            FamilySymbol doorwayFamilySymbol = familySymbolList.First();

            //Вызов формы с выбором типа копирования проемов
            CIT_04_6_HoleTransferForm holeTransferForm = new CIT_04_6_HoleTransferForm();
            holeTransferForm.ShowDialog();
            if (holeTransferForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            string checkedButtonNameTransferOption = holeTransferForm.CheckedButtonNameTransferOption;

            //Если выбран перенос выборочный
            if (checkedButtonNameTransferOption == "radioButton_TransferSelected")
            {
                //Выбор связанного файла
                RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
                Reference selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");//Получение ссылки на выбранную группу
                IEnumerable<RevitLinkInstance> myRevitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Id == selRevitLinkInstance.ElementId)
                    .Cast<RevitLinkInstance>();
                XYZ linkOrigin = myRevitLinkInstance.First().GetTransform().Origin;
                Document doc2 = myRevitLinkInstance.First().GetLinkDocument();

                //Выбор окон в связанном файле
                IList<Reference> selElementsList = sel.PickObjects(ObjectType.LinkedElement, "Выберите окна и двери"); //Получение списка ссылок на выбранные окна и двери
                
                //Список окон из связанного файла
                List<FamilyInstance> windowsList = new List<FamilyInstance>();
                foreach (Reference refWindow in selElementsList)
                {
                    if (doc2.GetElement(refWindow.LinkedElementId).Category.Id.ToString() == "-2000014")
                    {
                        windowsList.Add((doc2.GetElement(refWindow.LinkedElementId)) as FamilyInstance);
                    }
                }

                //Список дверей из связанного файла
                List<FamilyInstance> doorsList = new List<FamilyInstance>();
                foreach (Reference refDoor in selElementsList)
                {
                    if (doc2.GetElement(refDoor.LinkedElementId).Category.Id.ToString() == "-2000023")
                    {
                        doorsList.Add((doc2.GetElement(refDoor.LinkedElementId)) as FamilyInstance);
                    }
                }

                List<string> elementsId = new List<string>();

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Создание проемов");

                    //Перенос дверных проемов
                    foreach (FamilyInstance door in doorsList)
                    {
                        //Примерная высота и ширина двери
                        double windowHeight = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                        double furnitureWidth = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        
                        //Точка размещения двери
                        LocationPoint doorLocationPoint = door.Location as LocationPoint;
                        XYZ doorPoint = doorLocationPoint.Point;
                        XYZ doorSecondPoint = doorPoint + 0.05 * XYZ.BasisX + 0.05 * XYZ.BasisY + 0.05 * XYZ.BasisZ;

                        //Создание объема для поиска пересечения со стеной
                        double minX = Math.Min(doorPoint.X, doorSecondPoint.X);
                        double maxX = Math.Max(doorPoint.X, doorSecondPoint.X);

                        double minY = Math.Min(doorPoint.Y, doorSecondPoint.Y);
                        double maxY = Math.Max(doorPoint.Y, doorSecondPoint.Y);

                        double minZ = Math.Min(doorPoint.Z, doorSecondPoint.Z);
                        double maxZ = Math.Max(doorPoint.Z, doorSecondPoint.Z);

                        Outline myOutLn = new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
                        BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(myOutLn, false);

                        //Поиск стен для вставки проема
                        List<Wall> wallList = new FilteredElementCollector(doc)
                            .OfClass(typeof(Wall))
                            .WherePasses(intersectFilter)
                            .Cast<Wall>()
                            .ToList();
                        if (wallList.Count == 0)
                        {
                            elementsId.Add(door.Id.ToString());
                            continue;
                        }

                        //Вставка проема
                        FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, wallList.First(), StructuralType.NonStructural);
                        newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                        newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    }

                    //Перенос оконных проемов
                    foreach (FamilyInstance window in windowsList)
                    {
                        //Примерная высота и ширина двери
                        double windowHeight = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                        double furnitureWidth = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();

                        //Точка размещения двери
                        LocationPoint doorLocationPoint = window.Location as LocationPoint;
                        XYZ doorPoint = doorLocationPoint.Point;
                        XYZ doorSecondPoint = doorPoint + 0.05 * XYZ.BasisX + 0.05 * XYZ.BasisY + 0.05 * XYZ.BasisZ;

                        //Создание объема для поиска пересечения со стеной
                        double minX = Math.Min(doorPoint.X, doorSecondPoint.X);
                        double maxX = Math.Max(doorPoint.X, doorSecondPoint.X);

                        double minY = Math.Min(doorPoint.Y, doorSecondPoint.Y);
                        double maxY = Math.Max(doorPoint.Y, doorSecondPoint.Y);

                        double minZ = Math.Min(doorPoint.Z, doorSecondPoint.Z);
                        double maxZ = Math.Max(doorPoint.Z, doorSecondPoint.Z);

                        Outline myOutLn = new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
                        BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(myOutLn, false);

                        //Поиск стен для вставки проема
                        List<Wall> wallList = new FilteredElementCollector(doc)
                            .OfClass(typeof(Wall))
                            .WherePasses(intersectFilter)
                            .Cast<Wall>()
                            .ToList();
                        if (wallList.Count == 0)
                        {
                            elementsId.Add(window.Id.ToString());
                            continue;
                        }

                        //Вставка проема
                        FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, wallList.First(), StructuralType.NonStructural);
                        newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                        newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    }
                    t.Commit();
                }

                if (elementsId.Count != 0)
                {
                    string messageStr = "";
                    foreach (string str in elementsId)
                    {
                        if (messageStr == "")
                        {
                            messageStr += str;
                        }
                        else
                        {
                            messageStr = messageStr + ", " + str;
                        }
                    }

                    CIT_04_6_HoleTransferMessageBox holeTransferMessageBox = new CIT_04_6_HoleTransferMessageBox(messageStr);
                    holeTransferMessageBox.ShowDialog();
                }
            }

            //Если выбран перенос всех проемов
            if (checkedButtonNameTransferOption == "radioButton_TransferAll")
            {
                //Выбор связанного файла
                RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
                Reference selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");//Получение ссылки на выбранную группу
                IEnumerable<RevitLinkInstance> myRevitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Id == selRevitLinkInstance.ElementId)
                    .Cast<RevitLinkInstance>();
                XYZ linkOrigin = myRevitLinkInstance.First().GetTransform().Origin;
                Document doc2 = myRevitLinkInstance.First().GetLinkDocument();

                //Получение всех дверей в проекте
                List<FamilyInstance> doorsList = new FilteredElementCollector(doc2)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(d => d.Host != null)
                    .Where(d => d.Host.Category.Id.ToString() == "-2000011")
                    .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT) != null)
                    .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                    .ToList();

                List<FamilyInstance> windowsList = new FilteredElementCollector(doc2)
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(d => d.Host != null)
                    .Where(d => d.Host.Category.Id.ToString() == "-2000011")
                    .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT) != null)
                    .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                    .ToList();

                List<string> elementsId = new List<string>();

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Создание проемов");
                    //Перенос дверных проемов
                    foreach (FamilyInstance door in doorsList)
                    {
                        //Примерная высота и ширина двери
                        double windowHeight = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                        double furnitureWidth = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();

                        //Точка размещения двери
                        LocationPoint doorLocationPoint = door.Location as LocationPoint;
                        XYZ doorPoint = doorLocationPoint.Point;
                        XYZ doorSecondPoint = doorPoint + 0.05 * XYZ.BasisX + 0.05 * XYZ.BasisY + 0.05 * XYZ.BasisZ;

                        //Создание объема для поиска пересечения со стеной
                        double minX = Math.Min(doorPoint.X, doorSecondPoint.X);
                        double maxX = Math.Max(doorPoint.X, doorSecondPoint.X);

                        double minY = Math.Min(doorPoint.Y, doorSecondPoint.Y);
                        double maxY = Math.Max(doorPoint.Y, doorSecondPoint.Y);

                        double minZ = Math.Min(doorPoint.Z, doorSecondPoint.Z);
                        double maxZ = Math.Max(doorPoint.Z, doorSecondPoint.Z);

                        Outline myOutLn = new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
                        BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(myOutLn, false);

                        //Поиск стен для вставки проема
                        List<Wall> wallList = new FilteredElementCollector(doc)
                            .OfClass(typeof(Wall))
                            .WherePasses(intersectFilter)
                            .Cast<Wall>()
                            .ToList();
                        if (wallList.Count == 0)
                        {
                            elementsId.Add(door.Id.ToString());
                            continue;
                        }

                        //Получить уровень для размещения проема
                        Level lv = doc.GetElement(wallList.First().LevelId) as Level;

                        //Вставка проема
                        FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, wallList.First(), lv, StructuralType.NonStructural);
                        newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                        newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    }

                    //Перенос оконных проемов
                    foreach (FamilyInstance window in windowsList)
                    {
                        //Примерная высота и ширина двери
                        double windowHeight = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                        double furnitureWidth = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();

                        //Точка размещения двери
                        LocationPoint doorLocationPoint = window.Location as LocationPoint;
                        XYZ doorPoint = doorLocationPoint.Point;
                        XYZ doorSecondPoint = doorPoint + 0.05 * XYZ.BasisX + 0.05 * XYZ.BasisY + 0.05 * XYZ.BasisZ;

                        //Создание объема для поиска пересечения со стеной
                        double minX = Math.Min(doorPoint.X, doorSecondPoint.X);
                        double maxX = Math.Max(doorPoint.X, doorSecondPoint.X);

                        double minY = Math.Min(doorPoint.Y, doorSecondPoint.Y);
                        double maxY = Math.Max(doorPoint.Y, doorSecondPoint.Y);

                        double minZ = Math.Min(doorPoint.Z, doorSecondPoint.Z);
                        double maxZ = Math.Max(doorPoint.Z, doorSecondPoint.Z);

                        Outline myOutLn = new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
                        BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(myOutLn, false);

                        //Поиск стен для вставки проема
                        List<Wall> wallList = new FilteredElementCollector(doc)
                            .OfClass(typeof(Wall))
                            .WherePasses(intersectFilter)
                            .Cast<Wall>()
                            .ToList();
                        if (wallList.Count == 0)
                        {
                            elementsId.Add(window.Id.ToString());
                            continue;
                        }

                        //Получить уровень для размещения проема
                        Level lv = doc.GetElement(wallList.First().LevelId) as Level;

                        //Вставка проема
                        FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, wallList.First(), lv, StructuralType.NonStructural);
                        newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                        newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    }
                    t.Commit();
                }

                if (elementsId.Count != 0)
                {
                    string messageStr = "";
                    foreach (string str in elementsId)
                    {
                        if (messageStr == "")
                        {
                            messageStr += str;
                        }
                        else
                        {
                            messageStr = messageStr + ", " + str;
                        }
                    }

                    CIT_04_6_HoleTransferMessageBox holeTransferMessageBox = new CIT_04_6_HoleTransferMessageBox(messageStr);
                    holeTransferMessageBox.ShowDialog();
                }
                
            }

            

            return Result.Succeeded;
        }
    }
}
