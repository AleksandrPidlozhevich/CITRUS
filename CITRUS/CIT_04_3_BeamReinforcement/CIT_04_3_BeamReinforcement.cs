using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_3_BeamReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_3_BeamReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор балок
            StructuralFramingSelectionFilter structuralFramingSelFilter = new StructuralFramingSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selBeams = sel.PickObjects(ObjectType.Element, structuralFramingSelFilter, "Выберите балки!");//Получение списка ссылок на выбранные балки
            //Получение списка выбранных балок
            List<FamilyInstance> beamsList = new List<FamilyInstance>();
            foreach (Reference beamRef in selBeams)
            {
                beamsList.Add(doc.GetElement(beamRef) as FamilyInstance);
            }
            //Завершение блока Получение списка балок

            //Выбор формы основной арматуры прямой стержень
            List<RebarShape> rebarShapeMainRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeMainRodsList == null)
            {
                TaskDialog.Show("Revit", "Форма 01 не найдена");
                return Result.Failed;
            }
            RebarShape myMainRebarShape = rebarShapeMainRodsList.First();

            //Список типов для выбора основной арматуры
            List<RebarBarType> mainRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            
            LocationCurve myMainRebarTypeLocationCurves = beamsList.First().Location as LocationCurve;
            Curve myMainRebarTypeCurve = myMainRebarTypeLocationCurves.Curve;
            XYZ startPoint = myMainRebarTypeCurve.GetEndPoint(0);
            XYZ endPoint = myMainRebarTypeCurve.GetEndPoint(1);
            XYZ vector = endPoint - startPoint;
 
            List<Curve> myMainRebarTypeCurves = new List<Curve>();
            myMainRebarTypeCurves.Add(myMainRebarTypeCurve);

            XYZ normal = new XYZ(0, 1, 0);

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");
                //Верхний правый угол
                Rebar mainRebar = Rebar.CreateFromCurvesAndShape(doc
                , myMainRebarShape
                , mainRebarTapesList.First()
                , null
                , null
                , beamsList.First()
                , normal
                , myMainRebarTypeCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
