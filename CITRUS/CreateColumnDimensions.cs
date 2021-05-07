using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreateColumnDimensions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Активный вид
            View activeView = doc.ActiveView;

            List<FamilyInstance> columnsList = new FilteredElementCollector(doc, activeView.Id)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Образмерить колонны");

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
