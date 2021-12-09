using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SystemsToSpace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Space> spaceList = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_MEPSpaces)
                .Cast<Space>()
                .ToList();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Системы в пространства");
                foreach (Space space in spaceList)
                {
                    List<FamilyInstance> ductTerminalList = new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .OfCategory(BuiltInCategory.OST_DuctTerminal)
                        .Cast<FamilyInstance>()
                        .Where(dt => dt.Space != null)
                        .Where(dt => dt.Space.Id == space.Id)
                        .ToList();

                    List<string> systemNameList = new List<string>();
                    foreach (FamilyInstance ductTerminal in ductTerminalList)
                    {
                        if (ductTerminal.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM) != null)
                        {
                            string systemName = ductTerminal.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString();
                            if (!systemNameList.Contains(systemName) 
                                && (systemName.StartsWith("В") 
                                || systemName.StartsWith("П")))
                            {
                                systemNameList.Add(systemName);
                            }
                        }
                    }
                    systemNameList.Sort(new AlphanumComparatorFastString());
                    string systemNames = "";
                    foreach (string st in systemNameList)
                    {
                        if (systemNames == "")
                        {
                            systemNames = st;
                        }
                        else
                        {
                            systemNames += ", " + st;
                        }
                    }
                    space.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(systemNames);
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
