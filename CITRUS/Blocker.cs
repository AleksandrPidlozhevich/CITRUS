using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
    class Blocker : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор связанного файла
            RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
            Reference selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");//Получение ссылки на выбранную группу
            IEnumerable<RevitLinkInstance> myRevitLinkInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Where(li => li.Id == selRevitLinkInstance.ElementId)
                .Cast<RevitLinkInstance>();
            XYZ linkOrigin = myRevitLinkInstance.First().GetTransform().Origin;
            Document doc2 = myRevitLinkInstance.First().GetLinkDocument();


            //Выбор форм в связанном файле
            List<FamilyInstance> massList = new FilteredElementCollector(doc2).OfCategory(BuiltInCategory.OST_Mass).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            massList.OrderBy(keySelector: column => column.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString());

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Заполнение свойств");

                foreach (FamilyInstance mass in massList)
                {
                    string comment = mass.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                    string mark = mass.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                    BoundingBoxXYZ massBB = mass.get_BoundingBox(null);
                    Outline massOutLn = new Outline(massBB.Min, massBB.Max);

                    List<Rebar> allRebarList = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).WherePasses(new BoundingBoxIntersectsFilter(massOutLn)).Cast<Rebar>().ToList();
                    foreach (Rebar reb in allRebarList)
                    {
                        string rebComment = reb.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                        if (rebComment == "" || rebComment == null)
                        {
                            reb.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(comment);
                        }
                    }

                    List<FamilyInstance> allFamilyInstanceList = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WherePasses(new BoundingBoxIntersectsFilter(massOutLn)).Cast<FamilyInstance>().ToList();
                    foreach (FamilyInstance fi in allFamilyInstanceList)
                    {
                        string fiComment = fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                        if (fiComment == "" || fiComment == null)
                        {
                            fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(comment);
                        }
                    }

                    List<Floor> allFloorList = new FilteredElementCollector(doc).OfClass(typeof(Floor)).WherePasses(new BoundingBoxIntersectsFilter(massOutLn)).Cast<Floor>().ToList();
                    foreach (Floor fl in allFloorList)
                    {
                        string flComment = fl.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                        if (flComment == "" || flComment == null)
                        {
                            fl.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(comment);
                        }
                    }

                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
