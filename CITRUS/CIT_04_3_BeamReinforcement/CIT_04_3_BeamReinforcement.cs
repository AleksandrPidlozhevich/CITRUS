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

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры балок");
                foreach (FamilyInstance beam in beamsList)
                {
                    //Ширина балки
                    double beamWidth = beam.Symbol.LookupParameter("Рзм.Ширина").AsDouble();

                    //Получаем основную кривую балки
                    LocationCurve myMainBeamLocationCurves = beam.Location as LocationCurve;
                    Curve myMainBeamCurve = myMainBeamLocationCurves.Curve;
                    //Начальная и конечная точки кривой
                    XYZ beamStartPoint = myMainBeamCurve.GetEndPoint(0);
                    XYZ beamEndPoint = myMainBeamCurve.GetEndPoint(1);

                    //Получение вектора основной кривой балки
                    Line beamMainLine = myMainBeamLocationCurves.Curve as Line;
                    XYZ beamMainLineDirectionVector = beamMainLine.Direction;

                    //Реальная длина балки по верхней грани
                    List<Solid> beamSolidList = new List<Solid>();
                    GeometryElement beamGeomElement = beam.get_Geometry(new Options());
                    foreach (GeometryObject geoObject in beamGeomElement)
                    {
                        Solid beamSolidForList = geoObject as Solid;
                        if (beamSolidForList != null)
                        {
                            if (beamSolidForList.Volume != 0)
                            {
                                beamSolidList.Add(beamSolidForList);
                            }
                        }
                    }

                    List<Line> linesInMainDirectionList = new List<Line>();
                    Solid beamSolid = beamSolidList.First();
                    EdgeArray beamEdgesArray = beamSolid.Edges;
                    foreach (Edge edge in beamEdgesArray)
                    {
                        Line edgeLine = edge.AsCurve() as Line;
                        XYZ edgeLineDirectionVector = edgeLine.Direction;
                        if (Math.Round(edgeLineDirectionVector.X, 6) == Math.Round(beamMainLineDirectionVector.X, 6) & Math.Round(edgeLineDirectionVector.Y, 6) == Math.Round(beamMainLineDirectionVector.Y, 6) & Math.Round(edgeLineDirectionVector.Z, 6) == Math.Round(beamMainLineDirectionVector.Z, 6))
                        {
                            linesInMainDirectionList.Add(edgeLine);
                        }
                    }

                    //Грань с максимальной отметкой Z
                    double lineMaxZ = -10000;
                    Line requiredLine = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0)) as Line;
                    foreach (Line line in linesInMainDirectionList)
                    {
                        //Начальная и конечная точки грани
                        XYZ lineStartPoint = line.GetEndPoint(0);
                        XYZ lineEndPoint = line.GetEndPoint(1);

                        if (lineMaxZ < lineStartPoint.Z || lineMaxZ < lineEndPoint.Z)
                        {
                            if (lineStartPoint.Z > lineEndPoint.Z)
                            {
                                lineMaxZ = lineStartPoint.Z;
                            }
                            else
                            {
                                lineMaxZ = lineEndPoint.Z;
                            }
                            requiredLine = line;
                        }
                    }
                    //Начальная и конечная точки грани
                    XYZ requiredLineStartPoint = requiredLine.GetEndPoint(0);
                    XYZ requiredLineEndPoint = requiredLine.GetEndPoint(1);

                    //Нормаль для построения основной арматуры
                    XYZ normal = beam.FacingOrientation;

                    //Точки для построения стержней основной арматуры
                    XYZ requiredLineStartPointToCenter = requiredLineStartPoint /*+ (beamWidth / 2 * normal)*/;
                    XYZ requiredLineEndPointToCenter = requiredLineEndPoint /*+ (beamWidth / 2 * normal)*/;

                    //Точки для построения стержней основной арматуры
                    XYZ p1 = new XYZ(requiredLineStartPointToCenter.X, requiredLineStartPointToCenter.Y, beamStartPoint.Z) - (400 / 304.8 * beamMainLineDirectionVector);
                    XYZ p2 = new XYZ(requiredLineEndPointToCenter.X, requiredLineEndPointToCenter.Y, beamEndPoint.Z) + (400 / 304.8 * beamMainLineDirectionVector);

                    //Кривые стержня основной арматуры
                    List<Curve> myMainRebarCurves = new List<Curve>();
                    Curve line1 = Line.CreateBound(p1, p2) as Curve;
                    myMainRebarCurves.Add(line1);


                    //Тестовый стержень
                    Rebar mainRebar = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShape
                        , mainRebarTapesList.First()
                        , null
                        , null
                        , beam
                        , normal
                        , myMainRebarCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
