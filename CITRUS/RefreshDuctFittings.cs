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
    class RefreshDuctFittings : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<FamilyInstance> ductFittingList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctFitting)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Обновить фитинги");
                foreach (FamilyInstance ductFitting in ductFittingList)
                {
                    ConnectorSet connectorSet = ductFitting.MEPModel.ConnectorManager.Connectors;
                    DuctType ductType = null;
                    bool flag = false;
                    foreach (Connector pConn in connectorSet)
                    {
                        ConnectorSet connectorSetRefs = pConn.AllRefs;
                        foreach (Connector c in connectorSetRefs)
                        {
                            Element connectionElement = c.Owner;
                            if (connectionElement.Category.Id.IntegerValue != (int)BuiltInCategory.OST_DuctCurves) continue;
                            if (ductType == null)
                            {
                                ductType = (connectionElement as Duct).DuctType;
                            }
                            else
                            {
                                if (ductType.Id != (connectionElement as Duct).DuctType.Id)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (flag) break;
                    }
                    if (!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Elbow)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Elbows);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule elbowsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Elbows, 0);
                            FamilySymbol elbowsFamilySymbol = doc.GetElement(elbowsRule.MEPPartId) as FamilySymbol;
                            if (elbowsFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = elbowsFamilySymbol;
                            }
                        }
                    }
                    else if(!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.TapAdjustable)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule transitionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, 0);
                            FamilySymbol transitionsFamilySymbol = doc.GetElement(transitionsRule.MEPPartId) as FamilySymbol;
                            if (transitionsFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = transitionsFamilySymbol;
                            }
                        }
                    }
                    else if (!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Transition)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Transitions);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule transitionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Transitions, 0);
                            FamilySymbol transitionsFamilySymbol = doc.GetElement(transitionsRule.MEPPartId) as FamilySymbol;
                            if (transitionsFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = transitionsFamilySymbol;
                            }
                        }
                    }
                    else if (!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Cap)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Caps);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule capsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Caps, 0);
                            FamilySymbol capsRuleFamilySymbol = doc.GetElement(capsRule.MEPPartId) as FamilySymbol;
                            if (capsRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = capsRuleFamilySymbol;
                            }
                        }
                    }
                    else if (!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Tee)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule junctionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, 0);
                            FamilySymbol junctionsRuleFamilySymbol = doc.GetElement(junctionsRule.MEPPartId) as FamilySymbol;
                            if (junctionsRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = junctionsRuleFamilySymbol;
                            }
                        }
                    }
                    else if (!flag && (ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Cross)
                    {
                        RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                        int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Crosses);
                        if (initRuleCount != 0)
                        {
                            RoutingPreferenceRule crossRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Crosses, 0);
                            FamilySymbol crossRuleFamilySymbol = doc.GetElement(crossRule.MEPPartId) as FamilySymbol;
                            if (crossRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                            {
                                ductFitting.Symbol = crossRuleFamilySymbol;
                            }
                        }
                    }
                }
                t.Commit();
            }
            TaskDialog.Show("Revit","Обработка завершена!");
            return Result.Succeeded;
        }
    }
}
