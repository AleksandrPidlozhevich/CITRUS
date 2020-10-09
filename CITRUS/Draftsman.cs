using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Draftsman : IExternalCommand
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

            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .Where(vt => vt.Name == "01.Разрез без номера листа.")
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Создание разрезов");

                foreach (FamilyInstance column in columnsList)
                {
                    //Марка колонны
                    string columnMark = column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();

                    //Получение нижней точки геометрии колонны
                    LocationPoint columnOriginLocationPoint = column.Location as LocationPoint;
                    XYZ columnOriginBase = columnOriginLocationPoint.Point;
                    //Ширина сечения колонны
                    double columnSectionWidth = column.Symbol.LookupParameter("Рзм.Ширина").AsDouble();
                    //Высота сечения колонны
                    double columnSectionHeight = column.Symbol.LookupParameter("Рзм.Высота").AsDouble();

                    //Ось вращения
                    XYZ rotationPoint1 = new XYZ(columnOriginBase.X, columnOriginBase.Y, columnOriginBase.Z);
                    XYZ rotationPoint2 = new XYZ(columnOriginBase.X, columnOriginBase.Y, columnOriginBase.Z + 1);
                    Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);
                    //Угол поворота колонны
                    double columnRotation = columnOriginLocationPoint.Rotation;

                    // BoundingBox для колонны
                    BoundingBoxXYZ columnBoundingBox = column.get_BoundingBox(null);
                    double columnBoundingBoxMinZ = columnBoundingBox.Min.Z;
                    double columnBoundingBoxMaxZ = columnBoundingBox.Max.Z;
                    double columnHeight = columnBoundingBoxMaxZ - columnBoundingBoxMinZ;

                    XYZ min = new XYZ(-columnSectionHeight/2 - 300/304.8, -300 / 304.8, -columnSectionWidth-50/304.8);
                    XYZ max = new XYZ(columnSectionHeight / 2 + 300 / 304.8, columnHeight + 300/304.8, columnSectionWidth + 50/304.8);

                    Transform transform = Transform.Identity;
                    transform.Origin = columnOriginBase;
                    transform.BasisX = XYZ.BasisY;
                    transform.BasisY = XYZ.BasisZ;
                    transform.BasisZ = XYZ.BasisX;

                    BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
                    sectionBox.Transform = transform;
                    sectionBox.Min = min;
                    sectionBox.Max = max;
                    ViewSection viewSection = ViewSection.CreateSection(doc, vft.Id, sectionBox);
                    viewSection.Name = "Колонна " + columnMark;

                    if (columnOriginLocationPoint.Rotation != 0)
                    {
                        ElementTransformUtils.RotateElement(doc, viewSection.Id, rotationAxis, columnRotation);
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
